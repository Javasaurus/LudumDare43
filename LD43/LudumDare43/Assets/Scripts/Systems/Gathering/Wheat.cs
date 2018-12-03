using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Wheat : WorldResource
{
    //Wheat is a world food resource that takes water as a condition to grow ...

    public float growthStage;
    public int waterCost = 2;
    public int water;

    public Animator soilAnimator;


    public new void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1) && villagerSelectionController != null && villagerSelectionController.activeVillagers.Count > 0)
        {
            modalPanel.PromptUser(new string[] { "Water", "Harvest " + type, "Cancel" }, new UnityAction[] { Water, confirmAction, refuteAction });
        }
    }

    public void Water()
    {
        //move selected units there to farm
        Tile targetTile = GetComponent<Tile>();

        foreach (Villager villager in villagerSelectionController.activeVillagers)
        {
            //     villager.SetState(Villager.VillagerState.EXECUTING);
            villager.SetTargetTile(targetTile);
            villager.onArrivedAtLocation += CheckIfCanExecute;
            villager.onStateChange += CancelWatering;
        }
        villagerSelectionController.Clear();
        modalPanel.ClosePanel();
    }

    public void CancelWatering(Villager villager)
    {
        Debug.Log("Watering cancelled");
        villager.onStateChange -= CheckIfCanExecute;
        villager.onArrivedAtLocation -= CancelWatering;
    }

    public void CheckIfCanExecute(Villager villager)
    {
        //check if the villager has arrived, he will not have changed target tile nor state as that would have triggered the cancel
        if (villager.pathLocomotor.state == PathLocomotor.State.ARRIVED)
        {

            Silo waterSilo = Silo.FindTargetSilo(ResourceType.WATER);
            int amount = (int)(Mathf.Min(waterSilo.totalAmount, Mathf.Min(Random.Range(waterCost, 3 * waterCost))));
            if (amount > 0)
            {
                villager.villagerAnimator.SetBool("harvesting", true);
                waterSilo.totalAmount -= amount;
                water += amount;
                StartCoroutine(WaitToStopAnimation(villager));
            }
            CancelWatering(villager);
        }
    }

    private IEnumerator WaitToStopAnimation(Villager villager)
    {
        yield return new WaitForSeconds(2f);
        villager.villagerAnimator.SetBool("harvesting", false);
    }

    private void LateUpdate()
    {
        /* if (amount <= 0)
         {
             transform.position = new Vector3(9999, 9999, 9999);
         }
         if (amount > maxAmountHarvest)
         {
             transform.position = initialPosition;
         }*/
        growthStage = (amount) / 10.0f;

        soilAnimator.SetFloat("growth", growthStage);

        if (Time.time > refillTimer)
        {
            //check if there is enough water
            if (water > waterCost)
            {
                amount++;
                if (amount > maxAmount)
                {
                    amount = maxAmount;
                }
                water -= waterCost;
            }
            refillTimer = Time.time + (60f / refillRatePerMinute);
        }


    }

}
