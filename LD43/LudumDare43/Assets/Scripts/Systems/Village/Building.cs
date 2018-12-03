using UnityEngine;

public class Building : MonoBehaviour
{
    private VillageGrid grid;
    //DOORS MUST BE IN THE CENTER
    public Vector2 size;

    private bool placed;
    // Use this for initialization

    public void LateUpdate()
    {
        //dirty but script order is set so I can't change it ...
        if (!placed)
        {
            grid = GameObject.FindObjectOfType<VillageGrid>();
            if (grid != null)
            {
                if (grid.grid.ContainsKey(transform.position))
                {
                    Place(grid.grid[transform.position]);
                }
                placed = true;
            }
        }
    }

    public void Place(Tile parentTile)
    {
        VillageGrid grid = GameObject.FindObjectOfType<VillageGrid>();

        int leftBound = (int)(parentTile.transform.position.x - size.x / 2);
        int rightBound = (int)(parentTile.transform.position.x + size.x / 2);
        int lowerBound = (int)(parentTile.transform.position.y - size.y / 2);
        int UpperBound = (int)(parentTile.transform.position.y + size.y / 2);

        for (int i = leftBound; i <= rightBound; i++)
        {
            for (int j = lowerBound; j <= UpperBound; j++)
            {
                Vector2 coordinates = new Vector2(i, j);
                if (grid.grid.ContainsKey(coordinates))
                {
                    grid.grid[coordinates].walkable = false;
                    //remove stuff that's already there on the tile
                    foreach (Transform child in grid.grid[coordinates].transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
        //the bottom center part is the door ...
        Vector2 tmpPosition = new Vector2(parentTile.transform.position.x, lowerBound);
        if (grid.grid.ContainsKey(tmpPosition))
        {
            grid.grid[tmpPosition].walkable = true;
        }
    }


    // Update is called once per frame
    private void Update()
    {

    }
}
