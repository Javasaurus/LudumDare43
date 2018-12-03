using UnityEngine;


//turn into a scriptable object ?
[CreateAssetMenu(menuName = "Village/Tiles/TileMetaData")]
public class TileMetaData : ScriptableObject
{

    public enum TileType
    {
        GROUND, WATER, TREE, ROCK, WHEAT, ROAD, BUILDING, PLACEHOLDER,
        DECORATIVE
    }

    public TileType type;
    public GameObject prefab;
    [Range(0f, 100f)]
    public float rarity = 50f;
    public bool walkable = true;
    // calculate values for the A* in a bit

}
