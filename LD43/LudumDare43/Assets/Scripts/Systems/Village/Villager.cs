using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Villager : MonoBehaviour
{

    public enum VillagerState
    {
        HARVESTING, TARGET_HARVEST_REQUESTED, GENERIC_HARVEST_REQUESTED, MOVEMENT_REQUESTED, IDLE, SACRIFICED, SACRIFICED_REQUESTED, DELIVERING, DELIVERED, IN_HOUSE
    }

    public enum VillagerMovementState
    {
        PATH_REQUEST, PATH_CALCULATING, PATH_FOLLOW, PATH_COMPLETED, IDLE
    }

    public string villagerName;

    public VillagerState currentState;
    public VillagerState previousState;

    public VillagerMovementState movementState;

    private SpriteRenderer _renderer;
    private AStarActor aStarActor;
    public PathLocomotor pathLocomotor;
    public bool active;

    private ModalPanel modalPanel;

    private UnityAction confirmAction;
    private UnityAction sacrificeAction;
    private UnityAction refuteAction;
    private UnityAction dropOffAction;
    private UnityAction dropAction;

    public SpriteRenderer clothesRenderer;

    public WorldResource resourceToGather;

    public int carrying;

    public int attemptsToFind = 0;

    public Animator villagerAnimator;
    private float harvestingTimer;

    private float idleStart;

    private ScanForResources resourceScanner;
    public WorldResource.ResourceType resourceToFind;
    private StateIcon icon;


    public delegate void OnStateChange(Villager villager);
    public OnStateChange onStateChange;

    public delegate void OnTargetChange(Villager villager);
    public OnTargetChange onArrivedAtLocation;
    internal House house;


    //Delegates on arive
    public void HandleArrived()
    {
        movementState = VillagerMovementState.PATH_COMPLETED;
        onArrivedAtLocation(this);
        GameDecisionEffects.PlayConfirmSound();
    }


    private void NotifyChange(Villager villager)
    {
        {
            onArrivedAtLocation -= Altar.INSTANCE.SacrificeVillager;
        }
    }

    private void NotifyTarget(Villager villager)
    {

    }

    public void OnDisable()
    {
        onStateChange -= NotifyChange;
        onArrivedAtLocation -= NotifyTarget;
    }

    public void OnEnable()
    {
        onStateChange += NotifyChange;
        onArrivedAtLocation += NotifyTarget;
        if (villagerName == "")
        {
            villagerName = GameObject.FindObjectOfType<NameGenerator>().GenerateName();
        }
        modalPanel = ModalPanel.GetInstance();
        resourceScanner = GetComponent<ScanForResources>();
        confirmAction = new UnityAction(PromptSacrifice);
        refuteAction = new UnityAction(Cancel);
        sacrificeAction = new UnityAction(HandleSacrificePrompt);
        dropOffAction = new UnityAction(DropOffLoad);
        dropAction = new UnityAction(DropLoad);

        icon = GetComponentInChildren<StateIcon>();


        villagerAnimator = GetComponent<Animator>();
    }

    public void OnDestroy()
    {
        VillagerSelectionController.GetInstance().allVillagers.Remove(this);
    }

    public void Awake()
    {
        aStarActor = GetComponent<AStarActor>();
        pathLocomotor = GetComponent<PathLocomotor>();
        pathLocomotor.onArrive += HandleArrived;
        //      pathLocomotor.onConfusion += HandleConfused;
        _renderer = GetComponent<SpriteRenderer>();
        VillagerSelectionController.GetInstance().allVillagers.Add(this);
    }

    public void Update()
    {
        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.PAUSED)
        {
            return;
        }

        //Check end of movement
        switch (movementState)
        {
            case VillagerMovementState.PATH_REQUEST:
                HandleAStar();
                break;
            case VillagerMovementState.PATH_COMPLETED:
                switch (currentState)
                {
                    case VillagerState.TARGET_HARVEST_REQUESTED:
                        HandleHarvestStart();
                        break;
                    case VillagerState.SACRIFICED:
                        Altar.INSTANCE.SacrificeVillager(this);
                        break;
                    case VillagerState.DELIVERING:
                        HandleDelivery();
                        break;
                }
                movementState = VillagerMovementState.IDLE;
                break;
        }

        //Chek AI requests
        switch (currentState)
        {
            case VillagerState.DELIVERING:
                icon.SetLooking(resourceToFind);
                break;
            case VillagerState.IDLE:
                HandleIdle();
                break;
            case VillagerState.DELIVERED:
                HandleHarvestRequest();
                break;
            case VillagerState.IN_HOUSE:
                HandleHouse();
                break;
            case VillagerState.MOVEMENT_REQUESTED:
                HandleMovementRequest();
                break;
            case VillagerState.HARVESTING:
                HandleHarvesting();
                break;
            case VillagerState.GENERIC_HARVEST_REQUESTED:
                HandleHarvestRequest();
                break;
            case VillagerState.SACRIFICED_REQUESTED:
                HandleSacrificeRequest();
                break;
        }

    }


    public void SetResourceToGather(WorldResource worldResource)
    {
        resourceToGather = worldResource;
        VillageGrid grid = GameObject.FindObjectOfType<VillageGrid>();
        Tile resourceTile = grid.grid[new Vector2((int)worldResource.transform.position.x, (int)worldResource.transform.position.y)];
        SetState(VillagerState.TARGET_HARVEST_REQUESTED);
        RequestPath(resourceTile);
    }

    public WorldResource getResourceToGather()
    {
        return resourceToGather;
    }

    public void SetState(VillagerState newState)
    {
        if (newState == VillagerState.IDLE)
        {
            idleStart = Time.time + Random.Range(5f, 15f);
        }
        previousState = currentState;
        currentState = newState;
        onStateChange(this);
    }

    public void RequestMovement(Tile tile)
    {
        if (currentState == VillagerState.SACRIFICED)
        {
            return;
        }
        currentState = VillagerState.MOVEMENT_REQUESTED;
        RequestPath(tile);
    }

    private void RequestPath(Tile tile)
    {
        movementState = (VillagerMovementState.PATH_REQUEST);
        aStarActor.state = AStarActor.State.READY;
        aStarActor.RunPathfinding(tile);
    }

    private void AcceptPathSuggestion()
    {
        pathLocomotor.setPath(aStarActor.currentPath);
        GameDecisionEffects.PlayConfirmSound();
        icon.SetGitty();
        aStarActor.state = AStarActor.State.READY;
        movementState = (VillagerMovementState.PATH_FOLLOW);
    }

    public void OnMouseOver()
    {
        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.PAUSED)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (carrying > 0)
            {
                GameDecisionEffects.PlayConfirmSound();
                modalPanel.PromptUser(new string[] { "Deliver", "Drop", "Cancel" }, new UnityAction[] { dropOffAction, dropAction, refuteAction });
            }
            else
            {
                GameDecisionEffects.PlayConfirmSound();
                modalPanel.PromptUser(new string[] { "Scavenge", "Sacrifice", "Cancel" }, new UnityAction[] { Scavenge, confirmAction, refuteAction });
            }

        }
    }

    private void Scavenge()
    {
        GameDecisionEffects.PlayDeclineSound();
        modalPanel.PromptUser(new string[] { "Look for food", "Look for wood", "Look for water", "Look for rock", "Cancel" }, new UnityAction[] { LookForFood, LookForWood, LookForWater, LookForRock, refuteAction });
        VillagerSelectionController.GetInstance().Clear();
        attemptsToFind = 0;
    }

    private void LookForRock()
    {
        Cancel();
        resourceToGather = null;
        resourceToFind = WorldResource.ResourceType.GOLD;
        SetState(VillagerState.GENERIC_HARVEST_REQUESTED);
    }

    private void LookForWater()
    {
        Cancel();
        resourceToGather = null;
        resourceToFind = WorldResource.ResourceType.WATER;
        SetState(VillagerState.GENERIC_HARVEST_REQUESTED);
    }

    private void LookForWood()
    {
        Cancel();
        resourceToGather = null;
        resourceToFind = WorldResource.ResourceType.WOOD;
        SetState(VillagerState.GENERIC_HARVEST_REQUESTED);
    }

    private void LookForFood()
    {
        Cancel();
        resourceToGather = null;
        resourceToFind = WorldResource.ResourceType.FOOD;
        SetState(VillagerState.GENERIC_HARVEST_REQUESTED);
    }


    public void OnMouseDown()
    {
        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.PAUSED)
        {
            return;
        }
        transform.position = new Vector3(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y),
            Mathf.RoundToInt(transform.position.z));

        if (Input.GetMouseButtonDown(0) & GameStateManager.INSTANCE.currentState != GameStateManager.GameState.DIALOG)
        {
            GameDecisionEffects.PlayConfirmSound();
            VillagerSelectionController.GetInstance().Clear();
            if (!VillagerSelectionController.GetInstance().activeVillagers.Contains(this))
            {
                VillagerSelectionController.GetInstance().activeVillagers.Add(this);
            }
            pathLocomotor.state = PathLocomotor.State.PAUSED;

            //clear what the NPC was doing
            icon.setConfused();
            resourceToGather = null;
            SetState(VillagerState.IDLE);

            StartCoroutine(resumeWalking());
        }
    }

    public void PromptSacrifice()
    {
        modalPanel.ClosePanel();
        modalPanel.DialogUser("Are you sure you want to sacrifice " + villagerName + " ?", new string[] { "Yes", "No" }, new UnityAction[] { sacrificeAction, refuteAction });
    }

    public void Cancel()
    {
        modalPanel.ClosePanel();
    }


    public void DropOffLoad()
    {
        Cancel();
        if (carrying > 0 && resourceToGather != null)
        {
            Silo silo = Silo.FindTargetSilo(resourceToGather.type);
            if (silo != null)
            {
                RequestMovement(silo.GetComponent<Tile>());
            }
        }
    }

    public void DropLoad()
    {
        Cancel();
        if (carrying > 0)
        {
            carrying = 0;
            resourceToGather = null;
            pathLocomotor.state = PathLocomotor.State.WAITING;
        }
    }

    private IEnumerator resumeWalking()
    {
        yield return new WaitForSeconds(5f);
        pathLocomotor.state = PathLocomotor.State.MOVING;
    }



    private void ReturnHome()
    {
        resourceToGather = null;
        SetState(VillagerState.IDLE);
        icon.SetSick();
        Vector2 altarPosition = Altar.INSTANCE.transform.position;
        Vector2 targetPosition = new Vector2((int)altarPosition.x + Random.Range(1, 5), (int)altarPosition.y + Random.Range(1, 5));
        Dictionary<Vector2, Tile> targetTiles = GameObject.FindObjectOfType<VillageGrid>().grid;
        int attempts = 0;
        while (!targetTiles.ContainsKey(targetPosition))
        {
            attempts++;
            if (attempts > 10)
            {
                targetPosition = altarPosition;
                break;
            }
            targetPosition = new Vector2((int)targetPosition.x + Random.Range(1, 5), (int)targetPosition.y + Random.Range(1, 5));
        }
        RequestMovement(targetTiles[targetPosition]);
    }



    private void HandleAStar()
    {
        icon.SetThinking();
        if (aStarActor.state != AStarActor.State.RUNNING)
        {
            if (aStarActor.success)
            {
                AcceptPathSuggestion();
                if (currentState == VillagerState.GENERIC_HARVEST_REQUESTED)
                {
                    SetState(VillagerState.HARVESTING);
                }
                if (currentState == VillagerState.SACRIFICED_REQUESTED)
                {
                    SetState(VillagerState.SACRIFICED);
                }
            }
            else
            {
                SetState(VillagerState.IDLE);
            }
            aStarActor.state = AStarActor.State.READY;
        }
    }

    private void HandleIdle()
    {
        if ((Time.time > idleStart))
        {
            //get a random neighbour tile?
            VillageGrid grid = GameObject.FindObjectOfType<VillageGrid>();
            Vector2 position = new Vector2(Mathf.RoundToInt(transform.position.x + Random.Range(-2, 2)), Mathf.RoundToInt(transform.position.y + Random.Range(-2, 2)));
            if (grid.grid.ContainsKey(position))
            {
                RequestMovement(grid.grid[position]);
            }
            idleStart = Time.time + Random.Range(5f, 15f);
        }
    }

    private void HandleHouse()
    {
        //
    }

    private void HandleHarvestRequest()
    {
        attemptsToFind++;
        if (attemptsToFind > 10)
        {
            ReturnHome();
        }
        // find resource type
        if (resourceScanner != null)
        {
            List<WorldResource> potentialTargets = resourceScanner.Scan(resourceToFind, 50f);
            icon.SetLooking(resourceToFind);
            if (potentialTargets.Count > 0)
            {
                for (int i = 0; i < potentialTargets.Count; i++)
                {
                    resourceToGather = potentialTargets[i];
                    //we request a path to this tile
                    SetResourceToGather(resourceToGather);
                    return;
                }
            }
            if (resourceToGather == null)
            {
                Debug.Log("Did not find other targets");
                modalPanel.DialogUser("The villager can not find " + resourceToFind + " nearby", new string[] { "Ok" }, new UnityAction[] { Cancel });
                icon.SetSick();
                SetState(VillagerState.IDLE);
            }
        }
    }

    private void HandleHarvestStart()
    {
        if (Mathf.Abs(resourceToGather.transform.position.x - transform.position.x) < 2 && Mathf.Abs(resourceToGather.transform.position.y - transform.position.y) < 2)
        {
            villagerAnimator.SetBool("harvesting", true);
            harvestingTimer = Time.time + resourceToGather.harvestDuration;
            resourceToGather.setShake();
            SetState(VillagerState.HARVESTING);
        }
        else
        {
            SetState(VillagerState.GENERIC_HARVEST_REQUESTED);
        }
    }

    private void HandleHarvesting()
    {
        if (resourceToGather == null)
        {
            SetState(VillagerState.IDLE);
        }
        else if (Time.time > harvestingTimer)
        {
            villagerAnimator.SetBool("harvesting", false);
            Silo targetSilo = Silo.FindTargetSilo(resourceToGather.type);
            carrying = resourceToGather.FarmResource();
            RequestPath(targetSilo.GetComponent<Tile>());
            SetState(VillagerState.DELIVERING);
        }

    }

    private void HandleDelivery()
    {
        Silo targetSilo = Silo.FindTargetSilo(resourceToGather.type);
        targetSilo.totalAmount += carrying;
        carrying = 0;
        //try to find a new target
        icon.SetHappy();
        targetSilo = null;
        resourceToGather = null;
        SetState(VillagerState.DELIVERED);
    }

    private void HandleSacrificeRequest()
    {
        RequestPath(Altar.INSTANCE.GetComponentInParent<Tile>());
        onArrivedAtLocation += Altar.INSTANCE.SacrificeVillager;
    }

    private void HandleMovementRequest()
    {

    }

    private void HandleSacrificePrompt()
    {
        Cancel();
        GameDecisionEffects.PlaySacrificeConfirm();
        modalPanel.ClosePanel();
        SetState(VillagerState.SACRIFICED_REQUESTED);
    }


    public void Kill()
    {
        villagerAnimator.SetBool("dead", true);
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        VillagerSelectionController.GetInstance().allVillagers.Remove(this);
        VillagerSelectionController.GetInstance().activeVillagers.Remove(this);
        GameObject.Destroy(pathLocomotor);
        GameObject.Destroy(aStarActor);
        GameObject.Destroy(this);
    }

}
