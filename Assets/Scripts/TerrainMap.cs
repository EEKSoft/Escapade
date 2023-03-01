using System.Collections.Generic;
using UnityEngine;

public class TerrainMap
{
    const int MAX_WIDTH = 32;
    const int MAX_HEIGHT = 32;

    Dictionary<int[][], TileIndex> TileLocations;
    public TerrainMap()
    {

    }
}

public enum TileIndex
{
    //Simple land, can be walked on
    Basic = 0,
    //Solid, used for walls
    Solid = 1,
    //
    Rough = 2,
    //
    Impassable = 3,
    //Map Edge
    Edge = 4
}
