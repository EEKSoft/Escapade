using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Level 
{
    //Current level instance in use
    public static Level currentLevel { get; private set; }

    //Terrainmap used by the level
    public TerrainMap tileMap;

    public static void InstantiateNewLevel(int seed, GameObject generator)
    {
        //Instantiate the level
        Level level = new Level();
        //Get the tile sprites ready
        MapTile.LoadTileSprites();
        //Instantiate the tilemap
        level.tileMap = new TerrainMap(seed, generator);
        //Assign the currentlevel the created level
        currentLevel = level;
        //Place the player onto the level
        GameObject player = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerPrefab"));
        player.GetComponentInChildren<CharacterManager>().location = new Point(-TerrainMap.halfWidth + 2, TerrainMap.halfHeight - 2);
        player.transform.position = new Vector3(-TerrainMap.halfWidth + 2, TerrainMap.halfHeight - 2) * TerrainMap.TILE_GAP;
    }

    public static void DestroyLevel()
    {
        currentLevel = null;
    }

    public TileIndex QueryTile(Point position)
    {
        return tileMap.TileLocations.ContainsKey(position) ? tileMap.TileLocations[position].tileType : TileIndex.Basic;
    }
}
