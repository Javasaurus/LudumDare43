using UnityEngine;
using UnityEngine.Events;

public class IdolHunt : MonoBehaviour
{

    protected ModalPanel modalPanel;
    public UnityAction confirmAction;
    protected UnityAction refuteAction;
    private VillagerSelectionController villagerSelectionController;
    private VillageGrid grid;
    protected bool hasLimitedAmount;
    protected Vector3 initialPosition;

    public void Start()
    {
        initialPosition = transform.position;
    }

    public void OnEnable()
    {
        villagerSelectionController = GameObject.FindObjectOfType<VillagerSelectionController>();
        grid = GameObject.FindObjectOfType<VillageGrid>();
        //set the location of this object to a certain tile as a child of said tile (mimic a resource)
        Tile parent = findRandomParentTile();
        transform.SetParent(parent.transform);
        transform.localPosition = Vector3.zero;
        modalPanel = ModalPanel.GetInstance();
        confirmAction = new UnityAction(CollectIdol);
        refuteAction = new UnityAction(Cancel);
    }


    private Tile findRandomParentTile()
    {
        Vector2 position = new Vector2(Random.Range(0, VillageGrid.GAME_SIZE.x), Random.Range(0, VillageGrid.GAME_SIZE.y));
        Tile tile = grid.grid[position];
        int attempts = 0;
        while (tile.metaData.type != TileMetaData.TileType.PLACEHOLDER)
        {
            position = new Vector2(Random.Range(0, VillageGrid.GAME_SIZE.x), Random.Range(0, VillageGrid.GAME_SIZE.y));
            tile = grid.grid[position];
            attempts++;
            if (attempts > 100)
            {
                break;
            }
        }
        return tile;
    }

    public void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1) && villagerSelectionController != null && villagerSelectionController.activeVillagers.Count > 0)
        {
            //           modalPanel.transform.position = new Vector2(Input.mousePosition.x - 64f, Input.mousePosition.y);
            modalPanel.PromptUser(new string[] { "Collect", "Cancel" }, new UnityAction[] { confirmAction, refuteAction });
        }
    }

    public void Cancel()
    {
        modalPanel.ClosePanel();
    }

    public void FinalizeHunt()
    {
        Cancel();
        GameObject.Destroy(gameObject);
    }

    public void CollectIdol()
    {
        GodScript.INSTANCE.CancelCountdown();
        int amount = Random.Range(1, 5) * 200;
        WorldResource.ResourceType type = (WorldResource.ResourceType)Random.Range(0, 3);
        Silo silo = Silo.FindTargetSilo(type);
        if (silo != null)
        {
            silo.totalAmount += amount;
        }
        modalPanel.DialogUser("The gods are pleased with you. As a reward they present you with " + amount + " " + type + " .", 
            new string[] { "OK" }, new UnityAction[] { FinalizeHunt });
    }


}
