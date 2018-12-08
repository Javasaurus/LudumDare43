using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillageGrid : MonoBehaviour
{
    public static Vector2 GAME_SIZE;
    public TileMetaData[] possibleTiles;
    public TileMetaData[] sorted;
    public Vector2 size = new Vector2(100, 100);
    public Dictionary<Vector2, Tile> grid;
    public Tile TilePrefab;

    public TileMetaData forestMetaData;
    public bool generatePerlinNoiseBased;
    // Use this for initialization
    private void Start()
    {
        GAME_SIZE = size;
        sorted = possibleTiles.OrderBy(c => c.rarity).ToArray();

        grid = new Dictionary<Vector2, Tile>();

        LoadExistingTiles();

        if (generatePerlinNoiseBased)
        {
            placeForest();

            placeNodes();
        }

        //remove the unwalkable tiles from the grid, we never used them after placement
        grid = grid.Where(pair => pair.Value.walkable && pair.Value.metaData.walkable).ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private void placeForest()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {

                Vector2 coordinates = new Vector2(i, j);
                if (!grid.ContainsKey(coordinates))
                {
                    Tile tile = Instantiate(TilePrefab, coordinates, Quaternion.identity, transform) as Tile;
                    tile.name = "Tile (" + i + "," + j + ")";
                    //generate the node metadata first and see if there's anything
                    TileMetaData mData = getRandomForestPerlinNoice(i, j, 10);
                    if (mData != null && mData.prefab != null)
                    {
                        tile.metaData = mData;
                        grid.Add(coordinates, tile);
                        tile.init();
                        tile.gameObject.name = "ForestTree";
                    }

                }
            }
        }
    }

    private void placeNodes()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                // we go over the empty tiles 
                Vector2 coordinates = new Vector2(i, j);
                if (!grid.ContainsKey(coordinates))
                {
                    Tile tile = Instantiate(TilePrefab, coordinates, Quaternion.identity, transform) as Tile;
                    tile.name = "Tile (" + i + "," + j + ")";
                    if (Random.Range(0f, 100f) < 5f)
                    {
                        TileMetaData data = possibleTiles[Random.Range(0, possibleTiles.Length)];
                        tile.metaData = data;
                    }
                    grid.Add(coordinates, tile);
                    tile.init();
                }
            }
        }
    }

    private void LoadExistingTiles()
    {
        Tile[] existingTiles = GameObject.FindObjectsOfType<Tile>();
        List<Tile> spawnerTiles = new List<Tile>();
        foreach (Tile existingTile in existingTiles)
        {

            Vector2 gridLocation = new Vector2(existingTile.transform.position.x, existingTile.transform.position.y);
            if (grid.ContainsKey(gridLocation))
            {
                Tile currentTile = grid[gridLocation];
                //check which one is placeholder and destroy that one, keep other one
                if (currentTile.metaData.prefab == null && currentTile.metaData.type != TileMetaData.TileType.DECORATIVE)
                {
                    grid[gridLocation] = existingTile;
                    GameObject.Destroy(currentTile.gameObject);
                }
                else
                {
                    GameObject.Destroy(existingTile.gameObject);
                }
            }
            else
            {
                existingTile.name = "Tile (" + gridLocation.x + "," + gridLocation.y + ")";
                grid.Add(gridLocation, existingTile);
            }
        }
        foreach (Tile existingTile in grid.Values)
        {
            if (existingTile.metaData != null && existingTile.metaData.prefab != null)
            {
                Transform spawned = GameObject.Instantiate(existingTile.metaData.prefab).transform;
                spawned.SetParent(existingTile.transform);
                spawned.localPosition = Vector3.zero;
            }
        }
    }

    private TileMetaData getRandomForestPerlinNoice(int x, int y, float frequency)
    {
        //we need float values for perlin to work
        float check = Mathf.PerlinNoise(x / frequency, y / frequency) * 100f;
        //shuffle the possible tile array ---> defies point of using perlin ...
        return check < forestMetaData.rarity ? forestMetaData : null;
    }

    private TileMetaData getRandomNodesNoise(int x, int y, int seed, float frequency)
    {
        //we need float values for perlin to work
        float check = Mathf.PerlinNoise((x + seed) / frequency, (y + seed) / frequency) * 100f;
        //shuffle the possible tile array ---> defies point of using perlin ...
        Debug.Log(check);
        return check < forestMetaData.rarity ? forestMetaData : null;
    }

    private TileMetaData getRandomNodes()
    {
        float check = Random.Range(0f, 500f);
        //shuffle the possible tile array ---> defies point of using perlin ...
        for (int i = 0; i < sorted.Length; i++)
        {
            if (sorted[i].type != TileMetaData.TileType.PLACEHOLDER && sorted[i].rarity > check)
            {
                return sorted[i];
            }
        }
        return null;
    }


}
