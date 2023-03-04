using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
//TEST FOR COMMITING TO GITHUB!!!!! 
public class TerrainMap
{
    //Pixel size / distance between tile centers
    public const int TILE_GAP = 1;
    //Width / Height of player start and goal locations
    public const int SAFE_TILE_ZONE_WH = 3;
    //General map size, can be altered later to be changeable
    public const int MAP_WIDTH = 20;
    public const int MAP_HEIGHT = 20;
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

        //Finally fully, generate all tiles
        foreach(KeyValuePair<Point, MapTile> kvp in TileLocations)
        {
            TileLocations[kvp.Key].Generate(parent);
        }
    }

    private void GenerateEdges()
    {
        //First get the top edge tiles
        for (int t = -halfWidth; t < halfWidth; t++) Collapse(TileIndex.Edge, t, halfHeight);

        //Next right tiles
        for (int r = halfHeight; r > -halfHeight; r--) Collapse(TileIndex.Edge, halfWidth, r);

        //Then bottom tiles
        for (int b = halfWidth; b > -halfWidth; b--) Collapse(TileIndex.Edge, b, -halfHeight);

        //Finally left tiles
        for (int l = -halfHeight; l < halfHeight; l++) Collapse(TileIndex.Edge, -halfWidth, l);
    }

    public void Collapse(TileIndex type, int x, int y)
    {
        //Makes the tile and sets it at it's position
        MapTile tile = new MapTile();
        tile.tileType = type;
        tile.position = new Point(x, y);
        TileLocations.Add(tile.position, tile);
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

public class MapTile
{
    public static Sprite[] tileSprites;
    //Object itself
    public GameObject self;
    //Position of the tile
    public Point position;
    //Type of tile
    public TileIndex tileType;
    
    public static void LoadTileSprites()
    {
        //Establish the array
        tileSprites = new Sprite[5];
        //Loop through the array and assign the tile sprites to it
        for(int i = 0; i < tileSprites.Length; i++)
        {
            //Like this means we can do levels with slightly differently designed tiles if we want, just a tad more changes
            tileSprites[i] = Resources.Load<Sprite>($"Sprites/sprite_tiles_{i}");
        }
    }

    //Places the tile with everything input
    public void Generate(GameObject parent)
    {
        //Create the gameobject
        self = new GameObject($"Tile{tileType}");
        self.transform.parent = parent.transform;
        //Set the position appropriately
        self.transform.position = new Vector3(position.X, position.Y) * TerrainMap.TILE_GAP;
        //Add the sprite renderer component
        SpriteRenderer renderer = self.AddComponent<SpriteRenderer>();
        renderer.sprite = tileSprites[(int)tileType];
    }
}
