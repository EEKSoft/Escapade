using System;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
//TEST FOR COMMITING TO GITHUB!!!!! 
public class TerrainMap
{
    //Distance between tile centers
    public const int TILE_GAP = 1;
    //General map size, can be altered later to be changeable
    public const int MAP_WIDTH = 20;
    public const int MAP_HEIGHT = 20;
    //Width / Height of player start, key, and goal locations
    public const int SAFE_TILE_ZONE_WH = 3;
    //How far in from the corner does the player and exit spawn, and what is the min key distance from edges
    public const int SPAWN_OFFSET = SAFE_TILE_ZONE_WH / 2;

    //A dictionary of tile locations, x y point keys to the tile at that coordinate
    public Dictionary<Point, MapTile> TileLocations;

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
        //Finally, fully generate all tiles with actual objects
        foreach(KeyValuePair<Point, MapTile> kvp in TileLocations)
        {
            TileLocations[kvp.Key].Generate(parent);
        }
    }

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
        //First the top left for spawn zone
        for(int x = 0; x < SAFE_TILE_ZONE_WH; x++)
        {
            for(int y = 0; y > -SAFE_TILE_ZONE_WH; y--)
            {
                AddPredefinedTile(TileIndex.Basic, x, y);
            }
        }
        //Then the bottom right for exit zone
        for (int x = MAP_WIDTH - 1; x > MAP_WIDTH - 1 - SAFE_TILE_ZONE_WH; x--)
        {
            for (int y = -MAP_HEIGHT + 1; y < -MAP_HEIGHT + 1 + SAFE_TILE_ZONE_WH; y++)
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
        //Next we will start generating the square for the key to exist on and make the point to actually place it later
        //but for now I must study, make a notecard, and sleep for an exam
        AddPredefinedTile(TileIndex.Basic, randX, randY);
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

}
