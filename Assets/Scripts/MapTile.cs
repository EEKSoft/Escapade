using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
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
}

public class MapTile
{
    public static readonly TileIndex MovementBlocking = TileIndex.Solid | TileIndex.Impassable | TileIndex.Edge;
    public static readonly TileIndex VisionBlocking = TileIndex.Solid | TileIndex.Edge;
    public static readonly TileIndex Unrealized = TileIndex.Basic | TileIndex.Solid | TileIndex.Rough | TileIndex.Impassable;

    public Dictionary<TileIndex, int> weights = new Dictionary<TileIndex, int>()
    {
        {TileIndex.Basic, 175 },
        {TileIndex.Solid, 100 },
        {TileIndex.Impassable, 10 },
        {TileIndex.Rough, 100 }
    };

    public static Sprite[] tileSprites;
    //Object itself
    public GameObject self;
    //Position of the tile
    public Point position;
    //Type of tile
    public TileIndex tileType = Unrealized;
    //Entropy value, IE how many potential states, lower is better, and is mostly used for sorting the next tile to collapse
    public float entropy = 1f;
    //Determines whether or not the tile has collapsed
    public bool decided = false;
    //Adjacent tiles
    MapTile[] adjacent = new MapTile[4];

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
    /// Recalculates the entropy of the tile
    /// </summary>
    private void RecalculateEntropy()
    {
        //Basic entropy calculation based on the number of flags, less flags = lower number
        entropy = (float)Enum.GetValues(typeof(TileIndex)).Cast<Enum>().Count(tileType.HasFlag) / 4f;
    }

    /// <summary>
    /// Gets the adjacent tiles to be checked for the evaluation method
    /// </summary>
    /// <returns></returns>
    public void GetAdjacent(TerrainMap map)
    {
        //X and Y to more easily get the points
        int X = position.X;
        int Y = position.Y;
        //Fill it with the adjacent points
        adjacent[0] = map.TileLocations[new Point(X + 1, Y)];
        adjacent[1] = map.TileLocations[new Point(X - 1, Y)];
        adjacent[2] = map.TileLocations[new Point(X, Y + 1)];
        adjacent[3] = map.TileLocations[new Point(X, Y - 1)];
    }

    /// <summary>
    /// Looks at the surrounding tiles and updates the current tile rules and entropy based on the nearby rules
    /// </summary>
    public void Evaluate()
    {
        //Perform operations based on nearby tiles
        int wallCount = 0;
        int floorCount = 0;
        //Flags for rough and impassable tiles
        bool flag = false;
        //Loop through and count based on nearby
        foreach(MapTile tile in adjacent)
        {
            if (tile.decided)
            {
                //Tick up the wall and floor 
                if ((tile.tileType & (TileIndex.Basic | TileIndex.Rough)) != 0) floorCount++;
                if ((tile.tileType & MovementBlocking) != 0) wallCount++;
                //If any of the nearby tiles are the appropriate tile, trip flag
                if ((tile.tileType & (TileIndex.Rough | TileIndex.Impassable)) != 0) flag = true;
            }
        }
        //Apply rules based on these values
        //No rough or impassables near eachother
        if (flag) tileType &= (TileIndex.Basic | TileIndex.Solid);
        //Floor must always be adjacent to more floor
        if (floorCount == 0) tileType &= (TileIndex.Solid);
        //No rough tiles by wall, and vice versa
        if (wallCount > 0) tileType &= (TileIndex.Basic | TileIndex.Solid);
        //Lastly do this for safety
        RecalculateEntropy();
    }

    /// <summary>
    /// Using the existing rules, decides on a final tile type to settle on, with some weighted probablities based on the given options
    /// </summary>
    public void Collapse()
    {
        //First Evaluate the tile one last time
        Evaluate();
        //This lets use do weighted randomness
        Dictionary<int, TileIndex> randomSelectionDictionary = new Dictionary<int, TileIndex>();
        //Used in the random dictionary above
        int sum = 0;
        //Loop through all the current flags that are active
        foreach (Enum e in Enum.GetValues(typeof(TileIndex)).Cast<Enum>().ToList().Where(tileType.HasFlag))
        {
            //Add the weight to the sum, and the sum to the dictionary with the enum
            sum += weights[(TileIndex)e];
            randomSelectionDictionary.Add(sum, (TileIndex)e);
        }
        //Now that we've done that, generate a random number between 0 and sum
        int choice = UnityEngine.Random.Range(0, sum);
        //Now linq grab from the dictionary the next number above or equal to choice
        tileType = randomSelectionDictionary.Where(i => i.Key >= choice).OrderBy(s => s.Key).First().Value;
        //Set these to indicate the tile is finished
        decided = true;
        entropy = 0f;
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
