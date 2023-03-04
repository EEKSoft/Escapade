using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/// <summary>
/// Expected behavior of player on tiles as an enum to reference
/// </summary>
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

    /// <summary>
    /// Used to load tiles sprites for the level, can be used to
    /// </summary>
    public static void LoadTileSprites()
    {
        //Establish the array
        tileSprites = new Sprite[5];
        //Loop through the array and assign the tile sprites to it
        for (int i = 0; i < tileSprites.Length; i++)
        {
            //Like this means we can do levels with slightly differently designed tiles if we want, just a tad more changes
            tileSprites[i] = Resources.Load<Sprite>($"Sprites/sprite_tiles_{i}");
        }
    }

    /// <summary>
    /// Should only be used at the end of building the tile map to place all the given tiles
    /// </summary>
    /// <param name="parent"></param>
    public void Generate(GameObject parent)
    {
        //Create the gameobject
        self = new GameObject($"Tile{tileType}");
        //Assign it to the parent object
        self.transform.parent = parent.transform;
        //Set the position appropriately
        self.transform.position = new Vector3(position.X, position.Y) * TerrainMap.TILE_GAP;
        //Add the sprite renderer component
        SpriteRenderer renderer = self.AddComponent<SpriteRenderer>();
        renderer.sprite = tileSprites[(int)tileType];
    }
}
