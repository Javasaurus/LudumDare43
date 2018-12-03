using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{

    public List<GameObject> currentIndicators;

    public void Awake()
    {
        currentIndicators = new List<GameObject>();
    }

    public void ShowPath(List<Tile> tiles, Color color)
    {
        foreach (GameObject go in currentIndicators)
        {
            GameObject.Destroy(go);
        }

        foreach (Tile tile in tiles)
        {
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.transform.localScale = Vector3.one * 0.1f;
            indicator.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, 1);
            indicator.GetComponent<Renderer>().material.color = color;
            currentIndicators.Add(indicator);
        }
    }



}
