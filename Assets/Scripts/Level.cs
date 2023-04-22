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
        GameObject player = Object.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerPrefab"));
        player.GetComponentInChildren<CharacterManager>().location = new Point(TerrainMap.SPAWN_OFFSET, -TerrainMap.SPAWN_OFFSET);
        player.transform.position = new Vector3(TerrainMap.SPAWN_OFFSET, -TerrainMap.SPAWN_OFFSET) * TerrainMap.TILE_GAP;
        player.transform.localScale = player.transform.localScale * TerrainMap.TILE_GAP;
        //Places the exit into the level
        GameObject exit = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ExitPrefab"));
        exit.transform.position = new Vector3(level.tileMap.exitPoint.X, level.tileMap.exitPoint.Y);
        exit.transform.localScale = exit.transform.localScale * TerrainMap.TILE_GAP;
        //Places Key on the level
        GameObject key = Object.Instantiate(Resources.Load<GameObject>("Prefabs/KeyPrefab"));
        key.transform.position = new Vector3(level.tileMap.keyPoint.X, level.tileMap.keyPoint.Y) * TerrainMap.TILE_GAP;
        key.transform.localScale = key.transform.localScale * TerrainMap.TILE_GAP;
    }

    /// <summary>
    /// Gets rid of the current level in use
    /// </summary>
    public static void DestroyLevel()
    {
        currentLevel = null;
    }

    /// <summary>
    /// Requests the tile type from the level's terrainmap, returns basic if there is no tile there
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileIndex QueryTile(Point position)
    {
        return tileMap.TileLocations.ContainsKey(position) ? tileMap.TileLocations[position].tileType : TileIndex.Basic;
    }
}
