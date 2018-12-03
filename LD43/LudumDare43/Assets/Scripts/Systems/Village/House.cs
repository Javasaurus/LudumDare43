using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class House : MonoBehaviour
{
    public Villager prefab;
    private float currentPricePerVillager = 50;
    public float spawnChance = 0.1f;
    public float spawnRollDelay = 60;
    public float spawnTimer;

    public List<Villager> villagersToMonitor;
    public int maxVillagers = 5;
    private ModalPanel modalPanel;
    private UnityAction RestAction;
    private UnityAction ReleaseAction;
    private UnityAction refuteAction;
    private Animator _animator;
    // Use this for initialization
    private void Start()
    {
        villagersToMonitor = new List<Villager>();
        _animator = GetComponentInChildren<Animator>();
    }

    public void OnEnable()
    {

        modalPanel = ModalPanel.GetInstance();

        RestAction = new UnityAction(Rest);
        ReleaseAction = new UnityAction(Release);
        refuteAction = new UnityAction(Cancel);
    }

    public void Rest()
    {
        GameDecisionEffects.PlayConfirmSound();
        //move selected units there to farm
        Tile targetTile = GetComponent<Tile>();

        foreach (Villager villager in VillagerSelectionController.GetInstance().activeVillagers)
        {
            if (villagersToMonitor.Count <= maxVillagers)
            {
                villager.SetTargetTile(targetTile);
                villagersToMonitor.Add(villager);
                villager.SetState(Villager.VillagerState.IN_HOUSE);
            }
        }
        VillagerSelectionController.GetInstance().Clear();
        modalPanel.ClosePanel();
    }

    public void Update()
    {
        List<Villager> readyVillagers = new List<Villager>();
        foreach (Villager villager in villagersToMonitor)
        {
            float manhattanDistance = Mathf.Abs(transform.position.x - villager.transform.position.x) + Mathf.Abs(transform.position.y - villager.transform.position.y);

            if (manhattanDistance < 2f)
            {
                villager.transform.position = new Vector3(9999, 9999);
                villager.gameObject.SetActive(false);
            }
        }

        if (Time.time > spawnTimer && villagersToMonitor.Count > 2)
        {
            float tmpSpawnChance = spawnChance * Mathf.Min(5, villagersToMonitor.Count);
            if (UnityEngine.Random.Range(0f, 1f) < tmpSpawnChance)
            {
                SpawnNewVillager();
            }
            spawnTimer = Time.time + spawnRollDelay;
        }


    }

    private void SpawnNewVillager()
    {
        if (GodScript.INSTANCE.state != GodScript.GodState.PUNISH)
        {
            Silo[] storages = GameObject.FindObjectsOfType<Silo>();

            // check silo
            foreach (Silo silo in storages)
            {
                if (silo.totalAmount < (int)currentPricePerVillager)
                {
                    return;
                }
            }
            //pay silo
            foreach (Silo silo in storages)
            {
                silo.totalAmount -= (int)currentPricePerVillager;
            }

            Villager instance = Instantiate(prefab) as Villager;
            instance.transform.position = new Vector2(transform.position.x + 0, transform.position.x - 1);
            //notify birth
            modalPanel.DialogUser("A new villager was born ! Say hello to " + instance.villagerName, new string[] { "OK" }, new UnityAction[] { Cancel });
        }
    }

    public void LateUpdate()
    {
    //    currentPricePerVillager = 100 + VillagerSelectionController.GetInstance().allVillagers.Count * 2;
        _animator.SetInteger("occupants", villagersToMonitor.Count);
    }

    public void Release()
    {
        foreach (Villager villager in villagersToMonitor)
        {
            villager.transform.position = new Vector3(transform.position.x, transform.position.y - 1, villager.transform.position.z);
            villager.gameObject.SetActive(true);
            villager.SetState(Villager.VillagerState.IDLE);
        }
        villagersToMonitor.Clear();
        VillagerSelectionController.GetInstance().Clear();
        modalPanel.ClosePanel();
    }


    public void Cancel()
    {
        modalPanel.ClosePanel();
    }

    public void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1))
        {
            if (VillagerSelectionController.GetInstance() != null && VillagerSelectionController.GetInstance().activeVillagers.Count > 0)
            {
                modalPanel.PromptUser(new string[] { "Rest " + VillagerSelectionController.GetInstance().activeVillagers.Count, "Cancel" }, new UnityAction[] { RestAction, refuteAction });
            }
            else if (villagersToMonitor.Count > 0 && villagersToMonitor.Count < maxVillagers)
            {
                modalPanel.PromptUser(new string[] { "Release " + villagersToMonitor.Count, "Cancel" }, new UnityAction[] { ReleaseAction, refuteAction });
            }


        }
    }





}
