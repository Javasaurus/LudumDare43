using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Villager : MonoBehaviour
{

    public enum VillagerState
    {
        IDLE, SACRIFICED, MOVING, HARVEST_REQUESTED, HARVESTING, /*EXECUTING,*/ DELIVERING, IN_HOUSE
    }

    public string villagerName;

    public VillagerState currentState;
    public VillagerState previousState;

    public Tile targetTile;
    private SpriteRenderer _renderer;
    private AStarActor aStarActor;
    public PathLocomotor pathLocomotor;
    public bool active;

    private List<Tile> avoidanceTiles;

    private ModalPanel modalPanel;

    private UnityAction confirmAction;
    private UnityAction sacrificeAction;
    private UnityAction refuteAction;
    private UnityAction dropOffAction;
    private UnityAction dropAction;

    public SpriteRenderer clothesRenderer;

    public WorldResource resourceToGather;
    public WorldResource previousResource;


    public Silo targetSilo;

    public int carrying;

    public int attemptsToFind = 0;

    public Animator villagerAnimator;
    private float harvestingTimer;

    private float idleStart;

    private ScanForResources resourceScanner;
    private StateIcon icon;


    public delegate void OnStateChange(Villager villager);
    public OnStateChange onStateChange;

    public delegate void OnTargetChange(Villager villager);
    public OnTargetChange onArrivedAtLocation;

    private void NotifyChange(Villager villager)
    {

        //       Debug.Log(villager.villagerName + " is now " + villager.currentState);   //       if (previousState == VillagerState.SACRIFICED & state != VillagerState.SACRIFICED)
        {
            onArrivedAtLocation -= Altar.INSTANCE.SacrificeVillager;
            avoidanceTiles.Clear();
        }
    }

    private void NotifyTarget(Villager villager)
    {
        if (targetTile != null)
        {
            //     Debug.Log(villager.villagerName + " is now heading to" + targetTile.name);
        }
    }

    public void OnDisable()
    {
        onStateChange -= NotifyChange;
        onArrivedAtLocation -= NotifyTarget;
    }

    public void OnEnable()
    {
        avoidanceTiles = new List<Tile>();
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
        sacrificeAction = new UnityAction(ExecuteSacrifice);
        dropOffAction = new UnityAction(DropOffLoad);
        dropAction = new UnityAction(DropLoad);

        icon = GetComponentInChildren<StateIcon>();


        villagerAnimator = GetComponent<Animator>();
    }

    public void SetResourceToGather(WorldResource worldResource)
    {
        if (previousResource != null && worldResource.type == previousResource.type)
        {
            previousResource = resourceToGather;
        }
        else
        {
            previousResource = null;
        }
        SetState(VillagerState.HARVEST_REQUESTED);
        resourceToGather = worldResource;
    }

    public WorldResource getResourceToGather()
    {
        return resourceToGather;
    }

    public void OnDestroy()
    {
        VillagerSelectionController.GetInstance().allVillagers.Remove(this);
    }

    public void SetState(VillagerState newState)
    {
        if (newState == VillagerState.IDLE)
        {
            idleStart = Time.time;
        }
        previousState = currentState;
        currentState = newState;
        onStateChange(this);
    }

    public void HandleArrived()
    {
        onArrivedAtLocation(this);
        GameDecisionEffects.PlayConfirmSound();
    }

    private Coroutine pathFindingRoutine;

    public void Awake()
    {
        aStarActor = GetComponent<AStarActor>();
        pathLocomotor = GetComponent<PathLocomotor>();
        pathLocomotor.onArrive += HandleArrived;
        pathLocomotor.onConfusion += HandleConfused;
        _renderer = GetComponent<SpriteRenderer>();
        VillagerSelectionController.GetInstance().allVillagers.Add(this);
    }

    public void SetTargetTile(Tile tile)
    {

        if (currentState == VillagerState.SACRIFICED)
        {
            return;
        }

        if (pathFindingRoutine != null)
        {
            StopCoroutine(pathFindingRoutine);
            pathFindingRoutine = null;
        }
        targetTile = tile;
        pathFindingRoutine = StartCoroutine(DoPathfinding());
    }

    private IEnumerator DoPathfinding()
    {
        //    Debug.Log("Calculating to " + targetTile.transform.position);

        bool pathFound = aStarActor.CalculatePath(targetTile, true);
        if (pathFound)
        {
            pathLocomotor.setPath(aStarActor.currentPath);
            GameDecisionEffects.PlayConfirmSound();
            icon.SetGitty();
        }
        else
        {
            GameDecisionEffects.PlayDeclineSound();
            icon.SetSad();
        }
        pathFindingRoutine = null;

        yield return null;
    }

    public void OnMouseOver()
    {
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
        LookForResource(WorldResource.ResourceType.GOLD);
    }

    private void LookForWater()
    {
        Cancel();
        resourceToGather = null;
        LookForResource(WorldResource.ResourceType.WATER);
    }

    private void LookForWood()
    {
        Cancel();
        resourceToGather = null;
        LookForResource(WorldResource.ResourceType.WOOD);
    }

    private void LookForFood()
    {
        Cancel();
        resourceToGather = null;
        LookForResource(WorldResource.ResourceType.FOOD);
    }


    public void Cancel()
    {
        modalPanel.ClosePanel();
    }

    public void OnMouseDown()
    {

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

    public void ExecuteSacrifice()
    {
        Cancel();
        GameDecisionEffects.PlaySacrificeConfirm();
        modalPanel.ClosePanel();

        Tile altarTile = Altar.INSTANCE.GetComponentInParent<Tile>();
        SetTargetTile(altarTile);
        SetState(VillagerState.SACRIFICED);
        onArrivedAtLocation += Altar.INSTANCE.SacrificeVillager;
    }

    public void DropOffLoad()
    {
        Cancel();
        if (carrying > 0 && resourceToGather != null)
        {
            Silo silo = Silo.FindTargetSilo(resourceToGather.type);
            if (silo != null)
            {
                SetTargetTile(silo.GetComponent<Tile>());
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

    public void LookForResource(WorldResource.ResourceType type)
    {
        attemptsToFind++;
        if (attemptsToFind > 10)
        {
            ReturnHome();
        }
        //if the previous thing still exists, go there
        if (previousResource != null && previousResource.isReady() && previousResource.type == type)
        {
            SetResourceToGather(previousResource);
            SetTargetTile(previousResource.GetComponentInParent<Tile>());
        }
        else if (resourceScanner != null)
        {
            List<WorldResource> potentialTargets = resourceScanner.Scan(type, 50f);
            if (potentialTargets.Count > 0)
            {
                for (int i = 0; i < potentialTargets.Count; i++)
                {
                    resourceToGather = potentialTargets[i];
                    Tile targetTile = resourceToGather.GetComponentInParent<Tile>();
                    if (targetTile == null)
                    {
                        targetTile = resourceToGather.GetComponent<Tile>();
                    }
                    if (targetTile == null)
                    {
                        targetTile = resourceToGather.GetComponentInChildren<Tile>();
                    }
                    if (targetTile != null && !avoidanceTiles.Contains(targetTile))
                    {
                        //stop current pathfinding
                        if (pathFindingRoutine != null)
                        {
                            StopCoroutine(pathFindingRoutine);
                            pathFindingRoutine = null;
                        }
                        //don't estimate, must be exact
                        bool canAccess = aStarActor.CalculatePath(targetTile, false);
                        if (canAccess)
                        {
                            SetResourceToGather(resourceToGather);
                            pathLocomotor.setPath(aStarActor.currentPath);
                            icon.SetGitty();
                            return;
                        }
                    }
                }

                if (targetTile == null)
                {
                    Debug.Log("Did not find other targets");
                    modalPanel.DialogUser("The villager can not find " + type + " nearby", new string[] { "Ok" }, new UnityAction[] { Cancel });
                    icon.setConfused();
                }
            }
        }
        icon.SetSad();
        Debug.Log("Did not find other targets");
        resourceToGather = null;
        SetState(VillagerState.IDLE);
    }

    private void ReturnHome()
    {
        previousResource = null;
        resourceToGather = null;
        targetTile = null;
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
        SetTargetTile(targetTiles[targetPosition]);
    }

    public void Update()
    {

        if (pathLocomotor.state == PathLocomotor.State.CONFUSED)
        {
            icon.setConfused();
        }



        HandleHarvesting();

    }

    private void HandleIdle()
    {
        //make them move if idle for 10 seconds
        if (currentState == VillagerState.IDLE)
        {
            if ((Time.time - idleStart) > 10)
            {
                //get a random neighbour tile?
                VillageGrid grid = GameObject.FindObjectOfType<VillageGrid>();
                Vector2 position = new Vector2(Mathf.RoundToInt(transform.position.x + Random.Range(-3, 3)), Mathf.RoundToInt(transform.position.y + Random.Range(-3, 3)));
                if (grid.grid.ContainsKey(position))
                {
                    SetTargetTile(grid.grid[position]);
                }
                idleStart = Time.time;
            }
        }
    }

    private void HandleHarvesting()
    {
        //execture harvesting if we are 

        if (resourceToGather != null && currentState != VillagerState.HARVESTING && pathLocomotor.state == PathLocomotor.State.ARRIVED & carrying == 0)
        {
            //sometimes the AI messes up. Be sure to avoid harvesting from a distance...
            float manhattanDistance = Mathf.Abs(transform.position.x - resourceToGather.transform.position.x) + Mathf.Abs(transform.position.y - resourceToGather.transform.position.y);
            if (manhattanDistance <= 1)
            {
                SetState(VillagerState.HARVESTING);
                //do the harvesting...
                villagerAnimator.SetBool("harvesting", true);
                harvestingTimer = Time.time + resourceToGather.harvestDuration;
                carrying = resourceToGather.FarmResource();
            }
            else
            {
                //means something went really wrong with the AI, no time to figure it out so better to find the "next best thing"
                icon.setConfused();
                WorldResource.ResourceType tmp = resourceToGather.type;
                resourceToGather = null;
                LookForResource(tmp);
                carrying = 0;
            }
        }


        if (resourceToGather != null && currentState == VillagerState.HARVESTING & Time.time > harvestingTimer)
        {                //return to silo AFTER harvesting time
            targetSilo = Silo.FindTargetSilo(resourceToGather.type);
            SetTargetTile(targetSilo.GetComponent<Tile>());
            villagerAnimator.SetBool("harvesting", false);
            SetState(VillagerState.DELIVERING);
        }


        if (pathLocomotor.state == PathLocomotor.State.ARRIVED & currentState == VillagerState.DELIVERING)
        {
            if (targetSilo != null)
            {
                targetSilo.totalAmount += carrying;

                carrying = 0;
                //try to find a new target
                icon.SetHappy();
                StartCoroutine(WaitForNextTarget(targetSilo.type));
                targetSilo = null;
                resourceToGather = null;
                SetState(VillagerState.IDLE);
            }
        }
    }

    public void HandleConfused()
    {
        switch (currentState)
        {
            case VillagerState.DELIVERING:
                //means the path to the silo was lost ... we need to find it again !
                if (targetSilo != null)
                {
                    SetTargetTile(targetSilo.GetComponent<Tile>());
                }
                break;
            case VillagerState.HARVEST_REQUESTED:
                //try to find another resource nearby...
                if (resourceToGather != null)
                {
                    avoidTile(resourceToGather.transform);
                    LookForResource(resourceToGather.type);
                }
                else
                {
                    SetState(VillagerState.IDLE);
                }
                break;
            case VillagerState.SACRIFICED:
                // Try new pathfinding, avoid the current target tile in the locomotor?
                if (pathLocomotor.currentTile != null)
                {
                    //retry and avoid the next tile
                    avoidTile(pathLocomotor.currentTile.transform);
                    SetTargetTile(Altar.INSTANCE.GetComponentInParent<Tile>());
                }
                break;

        }
    }

    private void avoidTile(Transform transform)
    {
        VillageGrid grid = GameObject.FindObjectOfType<VillageGrid>();
        Vector2 position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        if (grid.grid.ContainsKey(position))
        {
            avoidanceTiles.Add(grid.grid[position]);
        }
    }



    private IEnumerator WaitForNextTarget(WorldResource.ResourceType type)
    {
        yield return new WaitForSeconds(3f);
        LookForResource(type);
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
