using UnityEngine;
using UnityEngine.Events;

public class Silo : MonoBehaviour
{

    public int maxAmount = int.MaxValue;
    public int totalAmount;
    public WorldResource.ResourceType type;

    private ModalPanel modalPanel;
    private UnityAction RestAction;
    private UnityAction ReleaseAction;
    private UnityAction PleaseGods;
    private UnityAction refuteAction;


    public void OnEnable()
    {

        modalPanel = ModalPanel.GetInstance();

        RestAction = new UnityAction(DropOff);

        refuteAction = new UnityAction(Cancel);
    }

    public void Cancel()
    {
        modalPanel.ClosePanel();
    }

    public void DropOff()
    {
        GameDecisionEffects.PlayConfirmSound();
        //move selected units there to farm
        Tile targetTile = GetComponent<Tile>();

        foreach (Villager villager in VillagerSelectionController.GetInstance().activeVillagers)
        {
            if (villager.currentState == Villager.VillagerState.DELIVERING && villager.carrying > 0 && villager.getResourceToGather() != null && villager.getResourceToGather().type == type)
            {
                villager.SetTargetTile(targetTile);
            }
        }
        VillagerSelectionController.GetInstance().Clear();
        modalPanel.ClosePanel();
    }

    public void LateUpdate()
    {
        if (GameStats.INSTANCE != null)
        {
            GameStats.INSTANCE.UpdateNumber(type, totalAmount);
        }
    }

    public static Silo FindTargetSilo(WorldResource.ResourceType type)
    {
        Silo[] silos = GameObject.FindObjectsOfType<Silo>();
        foreach (Silo silo in silos)
        {
            if (silo.type == type)
            {
                return silo;
            }
        }
        return null;
    }

    public void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1))
        {
            GodScript godScript = GodScript.INSTANCE;
            if (godScript.state == GodScript.GodState.CALLING | godScript.state == GodScript.GodState.PUNISH)
            {
                if (godScript.condition != null && godScript.condition.type == type)
                {
                    if (totalAmount > godScript.condition.amount)
                    {
                        totalAmount -= godScript.condition.amount;
                        godScript.MatchCondition();
                    }
                }
            }


        }
    }


}
