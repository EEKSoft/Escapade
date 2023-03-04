using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
//TEST FOR COMMITING TO GITHUB!!!!! 
public class TerrainMap
{
    //Distance between tile centers
    public const int TILE_GAP = 1;
    //Width / Height of player start, key, and goal locations
    public const int SAFE_TILE_ZONE_WH = 3;
    //General map size, can be altered later to be changeable
    public const int MAP_WIDTH = 40;
    public const int MAP_HEIGHT = 40;
    //How far in from the corner does the player and exit spawn
    public const int SPAWN_OFFSET = 1;
    //Get the half sizes, useful for distances and positioning
    public static int halfWidth = MAP_WIDTH / 2;
    public static int halfHeight = MAP_HEIGHT / 2;

    //A dictionary of tile locations, x y point keys to the tileindex representing the specific tile type at that coordinate
    public Dictionary<Point, MapTile> TileLocations;

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
        //Finally fully, generate all tiles
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
        for (int t = -halfWidth - 1; t < halfWidth + 1; t++) AddPredefinedTile(TileIndex.Edge, t, halfHeight + 1);

        //Next right tiles
        for (int r = halfHeight + 1; r > -halfHeight - 1; r--) AddPredefinedTile(TileIndex.Edge, halfWidth + 1, r);

        //Then bottom tiles
        for (int b = halfWidth + 1; b > -halfWidth - 1; b--) AddPredefinedTile(TileIndex.Edge, b, -halfHeight - 1);

        //Finally left tiles
        for (int l = -halfHeight - 1; l < halfHeight + 1; l++) AddPredefinedTile(TileIndex.Edge, -halfWidth - 1, l);
    }

    /// <summary>
    /// Generates predefined spawn and exit zones of the map
    /// </summary>
    private void GenerateSpawnExit()
    {
        //First the top left for spawn zone
        for(int x = -halfWidth; x < -halfWidth + SAFE_TILE_ZONE_WH; x++)
        {
            for(int y = halfHeight; y > halfHeight - SAFE_TILE_ZONE_WH; y--)
            {
                AddPredefinedTile(TileIndex.Basic, x, y);
            }
        }
        //Then the bottom right for exit zone
        for (int x = halfWidth; x > halfWidth - SAFE_TILE_ZONE_WH; x--)
        {
            for (int y = -halfHeight; y < -halfHeight + SAFE_TILE_ZONE_WH; y++)
            {
                AddPredefinedTile(TileIndex.Basic, x, y);
            }
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

}
