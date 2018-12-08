using UnityEngine;

public class Tile : MonoBehaviour
{

    public TileMetaData metaData;
    public bool walkable;
    private VillageGrid grid;
    // Use this for initialization
    public void init()
    {
        if (metaData != null)
        {
            {
                walkable = metaData.walkable;
                if (metaData.prefab != null)
                {
                    GameObject tmp = Instantiate(metaData.prefab);
                    tmp.transform.SetParent(transform);
                    tmp.transform.localPosition = Vector3.zero;
                }

            }
        }
    }

    public void OnMouseDown()
    {
        if (GameStateManager.INSTANCE.currentState == GameStateManager.GameState.WAIT_INPUT)
        {
            foreach (Villager villager in VillagerSelectionController.GetInstance().activeVillagers)
            {
                villager.RequestMovement(this);
            }
            VillagerSelectionController.GetInstance().Clear();
        }
    }
}
