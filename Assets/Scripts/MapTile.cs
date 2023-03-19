using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Expected behavior of player on tiles as an enum to reference
/// </summary>
[Flags]
public enum TileIndex
{
    //Simple land, can be walked on
    Basic = 1,
    //Cannot be moved through or seen through
    Solid = 2,
    //Basic, but slower move speed through it
    Rough = 4,
    //Solid, but can be seen through
    Impassable = 8,
    //Map Edge
    Edge = 16,
    //Before it is officially 'realized' or decided, it is every tile except edge
    Unrealized = ~0 << 1
}

public class MapTile
{
    public static readonly TileIndex MovementBlocking = TileIndex.Solid | TileIndex.Impassable | TileIndex.Edge;
    public static readonly TileIndex VisionBlocking = TileIndex.Solid | TileIndex.Edge;

    public static Sprite[] tileSprites;
    //Object itself
    public GameObject self;
    //Position of the tile
    public Point position;
    //Type of tile
    public TileIndex tileType = TileIndex.Unrealized;

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
    /// Will likely change to use a prefab to spawn, and the prefab will likely contain a script
    /// to help the wfc algorithm
    /// </summary>
    /// <param name="parent"></param>
    public void Generate(GameObject parent)
    {
        //Create the gameobject
        self = new GameObject($"Tile_{tileType}");
        //Assign it to the parent object
        self.transform.parent = parent.transform;
        self.transform.localScale = self.transform.localScale * TerrainMap.TILE_GAP;
        //Set the position appropriately
        self.transform.position = new Vector3(position.X, position.Y) * TerrainMap.TILE_GAP;
        //Add the sprite renderer component
        SpriteRenderer renderer = self.AddComponent<SpriteRenderer>();
        //Calculate the sprite index by getting the base 2 log of the enumerable tile type
        int spriteIndex = (int)Mathf.Log((int)tileType, 2);
        //Get the sprite at the given index
        renderer.sprite = tileSprites[spriteIndex];
    }
    
    
}
