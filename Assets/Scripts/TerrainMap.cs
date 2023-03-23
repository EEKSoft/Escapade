using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
//TEST FOR COMMITING TO GITHUB!!!!! 
public class TerrainMap
{
    //Distance between tile centers
    public const int TILE_GAP = 1;
    //General map size, can be altered later to be changeable
    public const int MAP_WIDTH = 40;
    public const int MAP_HEIGHT = 40;
    //Width / Height of player start, key, and goal locations
    public const int SAFE_TILE_ZONE_WH = 3;
    //How far in from the corner does the player and exit spawn, and what is the min key distance from edges
    public const int SPAWN_OFFSET = SAFE_TILE_ZONE_WH / 2;
    //About how far from the center of the path from the start to exit can the start of the path to the key generate
    public const int KEYPATH_DEVIATION = 10;

    //A dictionary of tile locations, x y point keys to the tile at that coordinate
    public Dictionary<Point, MapTile> TileLocations;

    //List of all coordinates that have not yet collapsed yet
    public List<Point> UnrealizedCoordinates;

    //Reference to spawn position
    public Point spawnPoint;
    //Reference to exit position
    public Point exitPoint;
    //Reference to point to for placing the key
    public Point keyPoint;

    public TerrainMap(int seed, GameObject parent)
    {
        //Initiate the random state with the given seed
        UnityEngine.Random.InitState(seed);
        //Initialize the tilelocations dictionary
        TileLocations = new Dictionary<Point, MapTile>();
        //First generate the edges of the map
        GenerateEdges();
        //Next, generate the spawn and exit zones
        GenerateSpawnExit();
        //Afterwards, find a location for the key and setup the area for it
        GenerateKeyZone();
        //Generate the paths from start to exit, and from random point on the path to the key
        GenerateLegalPaths();
        //Finally, fully generate all tiles with actual objects
        foreach(KeyValuePair<Point, MapTile> kvp in TileLocations)
        {
            TileLocations[kvp.Key].Generate(parent);
        }
    }
    #region Preset tiles, need to be certain tiles so there is no WFC going on here
    /// <summary>
    /// Generates the predefined edges of the map
    /// </summary>
    private void GenerateEdges()
    {
        //First get the top edge tiles
        for (int t = 0; t <= MAP_WIDTH; t++) AddPredefinedTile(TileIndex.Edge, t, 1);

        //Next right tiles
        for (int r = 0; r >= -MAP_HEIGHT; r--) AddPredefinedTile(TileIndex.Edge, MAP_WIDTH, r);

        //Then bottom tiles
        for (int b = MAP_WIDTH -1; b >= -1; b--) AddPredefinedTile(TileIndex.Edge, b, -MAP_HEIGHT);

        //Finally left tiles
        for (int l = -MAP_HEIGHT + 1; l <= 1; l++) AddPredefinedTile(TileIndex.Edge, -1, l);
    }

    /// <summary>
    /// Generates predefined spawn and exit zones of the map
    /// </summary>
    private void GenerateSpawnExit()
    {
        //Mark the spawn and exit points
        spawnPoint = new Point(SPAWN_OFFSET, -SPAWN_OFFSET);
        exitPoint = new Point(MAP_WIDTH - SPAWN_OFFSET - 1, -MAP_HEIGHT + SPAWN_OFFSET + 1);
        //First the top left for spawn zone
        for(int x = spawnPoint.X - 1; x <= spawnPoint.X + 1; x++)
        {
            for(int y = spawnPoint.Y + 1; y >= spawnPoint.Y - 1; y--)
            {
                AddPredefinedTile(TileIndex.Basic, x, y);
            }
        }
        //Then the bottom right for exit zone
        for (int x = exitPoint.X + 1; x >= exitPoint.X - 1; x--)
        {
            for (int y = exitPoint.Y - 1; y <= exitPoint.Y + 1; y++)
            {
                AddPredefinedTile(TileIndex.Basic, x, y);
            }
        }
    }

    /// <summary>
    /// Generates the area where the key will spawn and finds a spawn point for it, this is where the first hint
    /// of randomness starts to appear
    /// </summary>
    private void GenerateKeyZone()
    {
        //Used later in the calculation
        int halfX = MAP_WIDTH / 2;
        int halfY = MAP_HEIGHT / 2;
        //Randomly generate the x location with a few rules
        //First determine an x location, between the map edges (0, map width) accounting for offset
        int randX = (int)MathF.Round(UnityEngine.Random.Range(SPAWN_OFFSET, MAP_WIDTH - SPAWN_OFFSET - 1));
        //Generate the Y offset based on x value
        int yOffset = randX > halfX - 1 ? 0 : halfY - 1;
        //Ok, now we generate the y value, half -> full if x is < half, otherwise it is 0 -> half
        int randY = (int)-MathF.Round(UnityEngine.Random.Range(SPAWN_OFFSET, halfY - 1)) - yOffset;
        keyPoint = new Point(randX, randY); 
        //Next we will start generating the square for the key to exist on
        for(int x = randX - 1; x <= randX + 1; x++)
        {
            for(int y = randY + 1; y >= randY - 1; y--)
            {
                AddPredefinedTile(TileIndex.Basic, x, y);
            }
        }
    }

    /// <summary>
    /// Generates paths from start to finish and from path to key so that the level is indeed possible
    /// </summary>
    private void GenerateLegalPaths()
    {
        //Add onto this 
        List<Point> queuedNodes = new List<Point>();
        //First generate a path between spawn and exit
        GeneratePath(spawnPoint, exitPoint, ref queuedNodes, true);
        //Grab one of the points at around the half point of the current queued nodes for the key, using the offset
        int offset = UnityEngine.Random.Range(-KEYPATH_DEVIATION, KEYPATH_DEVIATION);
        int nodePosition = queuedNodes.Count() / 2 - 1 + offset;
        Point keyStart = queuedNodes[nodePosition];
        //Next generate path from the start of the key path to the key
        GeneratePath(keyStart, keyPoint, ref queuedNodes, false);
        //Cull the queued nodes of all existing nodes (In the tilelocation dictionary) and duplicate nodes
        queuedNodes = queuedNodes.Where(p => !TileLocations.ContainsKey(p)).Distinct().ToList();
        //Finally, for each queued node, add them as predefined basic tiles
        foreach(Point p in queuedNodes)
        {
            AddPredefinedTile(TileIndex.Basic, p.X, p.Y);
        }
    }

    /// <summary>
    /// Helper function for the GenerateLegalPaths method
    /// Out for the path generated
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void GeneratePath(Point a, Point b, ref List<Point> path, bool useKeyWeight)
    {
        //Current point we are looking at, for iterating through the path.  Start at the beginning
        Point current = a;
        //Basic get distance from start to end point for x, y, and a total distance for both.  We gotta use floats or bad things happen
        float xDist = MathF.Abs(b.X - a.X);
        int xDir = MathF.Sign(b.X - a.X);
        float yDist = MathF.Abs(b.Y - a.Y);
        int yDir = MathF.Sign(b.Y - a.Y);
        float totalDist = xDist + yDist;
        //Establish vector2 for direction choosing weight.  We use a vector2 because it has inbuilt normalization
        Vector2 directionWeight;
        //Establish bias variables
        float yBias = 1f;
        float xBias = 1f;
        //If key weight, then calculate it, otherwise make it slightly randomized
        if (useKeyWeight)
        {
            //Finally before looping, find a key-based bias for X and Y
            yBias = keyPoint.X / ((float)exitPoint.X / 2) + 0.5f;
            xBias = keyPoint.Y / ((float)exitPoint.Y / 2) + 0.5f;
            //Used to prevent a middle-key forcing the path to cut through it.  If below a certain number, we
            //force the path to take a slight detour in either direction
            float biasDifferential = MathF.Abs(xBias - yBias);
            if(biasDifferential <= 0.35f)
            {
                //Randomly add to Y or X
                float bonusBias = (1f - biasDifferential) * 3f;
                if (UnityEngine.Random.value <= 0.5f) yBias += bonusBias;
                else xBias += bonusBias;
            }
        }
        Vector2 bias = new Vector2(xBias, yBias).normalized;

        //Now we start the loop for pathmaking, as long as we aren't at the end
        while (totalDist > 0)
        {
            //Add the current node to the path
            path.Add(current);
            //Now to decide the next node by finding a weight usually starts at 0.5 to 0.5, avoiding div by 0 ofc
            directionWeight = new Vector2(xDist / (totalDist + 1), yDist / (totalDist + 1));
            //Now we get a little funky with it and multiply a little bit of random bias
            directionWeight *= bias;
            //We only really need the x for the rng calculation, so take it and turn it into a fraction of 1
            float xPercent = directionWeight.x / (directionWeight.x + directionWeight.y);
            //Now use the X value as a percentage chance for it to move horizontally, otherwise move vertically
            if(UnityEngine.Random.value < xPercent)
            {
                current = new Point(current.X + xDir, current.Y);
                xDist--;
            } 
            else
            {
                current = new Point(current.X, current.Y + yDir);
                yDist--;
            }
            //Lastly, decrement the total distance between points by 1
            totalDist--;
        }
    }

    /// <summary>
    /// Used for tiles that have to exist in a certain spot, start / end / key zone tiles
    /// </summary>
    /// <param name="type">Tile type</param>
    /// <param name="x">X location of tiles</param>
    /// <param name="y">Y location of tile</param>
    private void AddPredefinedTile(TileIndex type, int x, int y)
    {
        //Makes the tile and sets it at it's position
        MapTile tile = new MapTile();
        tile.tileType = type;
        tile.position = new Point(x, y);
        TileLocations.Add(tile.position, tile);
    }
    #endregion

    /// <summary>
    /// Prepares the UnrealizedTiles variable with all tiles not currently in use and initially evaluates them
    /// </summary>
    private void PrepareUnestablishedTiles()
    {
        //Establish this first
        Point point;
        //First generate all points sequentially as long as they do not exist in TileLocations
        for (int x = 0; x < MAP_WIDTH - 1; x++)
        {
            for (int y = 0; y > -MAP_HEIGHT + 1; y--)
            {
                //Generate the point
                point = new Point(x, y);
                //Check if it is in the dictionary, if so skip this iteration
                if (TileLocations.ContainsKey(point)) continue;
                //If it is not in the dictionary add it there and to the unrealized coordinate list
                MapTile tile = new MapTile();
                tile.position = point;
                TileLocations.Add(point, tile);
                UnrealizedCoordinates.Add(point);
            }
        }
    }


}
