using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
//TEST FOR COMMITING TO GITHUB!!!!! 
public class TerrainMap
{
    //Pixel size / distance between tile centers
    public const int TILE_GAP = 5;

    //Max map sizes, for now just standard map size
    private const int MAX_WIDTH = 32;
    private const int MAX_HEIGHT = 32;

    //A dictionary of tile locations, x y point keys to the tileindex representing the specific tile type at that coordinate
    Dictionary<Point, TileIndex> TileLocations;
    public TerrainMap(int seed)
    {
        //Initiate the random state with the given seed
        UnityEngine.Random.InitState(seed);
        //Initialize the tilelocations dictionary with the max w / h values
        TileLocations = new Dictionary<Point, TileIndex>();
    }
}

public enum TileIndex
{
    //Simple land, can be walked on
    Basic = 0,
    //Cannot be moved through or seen through
    Solid = 1,
    //Basic, but slower move speed through it
    Rough = 2,
    //Solid, but can be seen through
    Impassable = 3,
    //Map Edge
    Edge = 4
}
