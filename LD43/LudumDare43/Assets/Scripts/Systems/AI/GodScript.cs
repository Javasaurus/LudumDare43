using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GodScript : MonoBehaviour
{

    public enum GodState
    {
        IDLE, CALLING, PUNISH, REWARD, IDOL_HUNT
    }

    public GodState state;
    public Condition condition;
    public Sprite godDialogIcon;
    public GameObject idolHuntPrefab;
    private GameObject currentIdolHunt;

    public bool forceNextIdolHunt;
    public bool forceNextPunishment;

    public float time;
    public float UpperTimeLimit = 180;
    public float timeTillGuaranteedNext;
    [Header("IDLE")]
    [Range(0, 1)]

    //ADD TIME FOR SAFETY 
    public float chanceForSacrificeCall;
    [Range(0, 1)]
    public float chanceForIdolHunt = 0.5f;
    public float chanceDelay = 60;
    public float chanceTimer;

    [Header("CALLING")]
    public float timeToSacrifice = 180;
    public float sacrificeTimer;
    public AudioClip SacrificeCall;

    public ButtonPulsate villagerButton;
    public ButtonPulsate woodButton;
    public ButtonPulsate foodButton;
    public ButtonPulsate waterButton;
    public ButtonPulsate goldButton;


    [Header("PUNISH")]
    public float killsPerMinute;
    public float killTimer;
    public float timeSincePunishCall;
    public float punishDuration = 300;
    public GameObject[] punishmentEffects;
    public GameObject currentPunishmentEffect;

    private AudioSource _source;
    public static GodScript INSTANCE;
    private ModalPanel modalPanel;

    public void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisableButtons()
    {
        villagerButton.enabled = false;
        woodButton.enabled = false;
        foodButton.enabled = false;
        waterButton.enabled = false;
        goldButton.enabled = false;
    }

    // Use this for initialization
    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void OnEnable()
    {

        modalPanel = ModalPanel.GetInstance();
        timeTillGuaranteedNext = Time.time + UpperTimeLimit;
        DisableButtons();
    }

    // Update is called once per frame
    private void Update()
    {
        if (condition != null && condition.amount > 0)
        {
            switch (condition.type)
            {
                case WorldResource.ResourceType.VILLAGER:
                    villagerButton.enabled = true;
                    break;
                case WorldResource.ResourceType.FOOD:
                    foodButton.enabled = true;
                    break;
                case WorldResource.ResourceType.GOLD:
                    goldButton.enabled = true;
                    break;
                case WorldResource.ResourceType.WATER:
                    waterButton.enabled = true;
                    break;
                case WorldResource.ResourceType.WOOD:
                    woodButton.enabled = true;
                    break;
            }
        }


        time = Time.time;
        if (GameStateManager.INSTANCE != null && GameStateManager.INSTANCE.currentState == GameStateManager.GameState.PAUSED)
        {
            return;
        }

        bool forceEvent = Time.time >= timeTillGuaranteedNext;

        if (state == GodState.IDLE)
        {
            //avoid doing it from the get-go
            if (forceEvent | Time.time > chanceTimer)
            {
                if (forceEvent | chanceTimer != 0)
                {
                    if (forceEvent | (UnityEngine.Random.Range(0f, 1f) < chanceForSacrificeCall | (forceNextIdolHunt | forceNextPunishment)))
                    {
                        timeTillGuaranteedNext = Time.time + UpperTimeLimit;
                        forceNextPunishment = false;
                        state = GodState.CALLING;
                        sacrificeTimer = timeToSacrifice;
                        _source.PlayOneShot(SacrificeCall);
                        if (Random.Range(0f, 1f) < chanceForIdolHunt | forceNextIdolHunt)
                        {
                            forceNextIdolHunt = false;
                            if (ProceduralMusicChanger.INSTANCE != null)
                            {
                                ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.REWARD);
                            }

                            // need to move it to an empty position in the forest you dummy


                            VillageGrid grid = GameObject.FindObjectOfType<VillageGrid>();
                            int attempts = 0;
                            Tile targetTile = null;
                            while (targetTile == null)
                            {
                                //get a random position on the map 
                                Vector2 randomPosition = new Vector2((int)Random.Range(1, grid.size.x - 1), (int)Random.Range(1, grid.size.y - 1));
                                attempts++;
                                if (grid.grid.ContainsKey(randomPosition) && grid.grid[randomPosition].metaData.prefab == null)
                                {
                                    targetTile = grid.grid[randomPosition];
                                    currentIdolHunt = Instantiate(idolHuntPrefab);
                                    currentIdolHunt.transform.position = targetTile.transform.position;
                                }
                                if (attempts > 1000)
                                {
                                    break;
                                }
                            }

                            if (currentIdolHunt != null)
                            {
                                sacrificeTimer += 2 * timeToSacrifice;
                                modalPanel.DialogUser(godDialogIcon, "The gods have sent an idol into the forest. Show your faith and find it for them! ", new string[] { "OK" }, new UnityAction[] { Cancel });
                            }
                            else
                            {
                                //do a punishment instead
                                GenerateCondition();
                                modalPanel.DialogUser(godDialogIcon, "The gods require you to sacrifice " + condition.amount + " " + condition.type + " or face their wrath !", new string[] { "OK" }, new UnityAction[] { Cancel });
                            }
                        }
                        else
                        {
                            GenerateCondition();
                            modalPanel.DialogUser(godDialogIcon, "The gods require you to sacrifice " + condition.amount + " " + condition.type + " or face their wrath !", new string[] { "OK" }, new UnityAction[] { Cancel });
                        }
                    }
                }
                chanceTimer = Time.time + chanceDelay;
            }
        }

        if (state == GodState.CALLING)
        {
            if (GameStateManager.INSTANCE.currentState != GameStateManager.GameState.DIALOG && GameStateManager.INSTANCE.currentState != GameStateManager.GameState.PAUSED)
            {
                sacrificeTimer -= Time.deltaTime;
                float ratio = sacrificeTimer / timeToSacrifice;
                GameStats.INSTANCE.UpdateGodTimer(1.0f - ratio);
                if (sacrificeTimer <= 0)
                {
                    //THEN we need to punish or remove the idol hunt
                    if (currentIdolHunt != null)
                    {
                        modalPanel.DialogUser(godDialogIcon, "The idol has vanished. The gods are looking down on you and question your faith...", new string[] { "OK" }, new UnityAction[] { Cancel });
                        GameObject.Destroy(currentIdolHunt);
                        currentIdolHunt = null;
                        if (ProceduralMusicChanger.INSTANCE != null)
                        {
                            ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.AMBIENT);
                        }
                    }
                    else
                    {
                        ActivatePunishment();
                    }
                }
            }
        }

        if (state == GodState.PUNISH)
        {
            if (timeSincePunishCall >= punishDuration)
            {
                CancelCountdown();
            }

            if (Time.time > killTimer && killTimer > 0)
            {
                float delay = 60 / (timeSincePunishCall / 60f) + killsPerMinute;
                killTimer = Time.time + delay;
                foreach (Villager villager in VillagerSelectionController.GetInstance().allVillagers)
                {
                    if (villager.currentState != Villager.VillagerState.IN_HOUSE)
                    {
                        villager.Kill();
                        break;
                    }
                }

            }
        }

    }

    public void ActivatePunishment()
    {
        if (ProceduralMusicChanger.INSTANCE != null)
        {
            ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.PUNISH);
        }
        timeSincePunishCall = 0;
        state = GodState.PUNISH;
        punishmentEffects[UnityEngine.Random.Range(0, punishmentEffects.Length)].SetActive(true);
    }

    public void CancelCountdown()
    {
        condition = null;
        DisableButtons();
        if (currentPunishmentEffect != null)
        {
            currentPunishmentEffect.SetActive(false);
            currentPunishmentEffect = null;
        }
        killTimer = 0;
        state = GodState.IDLE;
        if (ProceduralMusicChanger.INSTANCE != null)
        {
            ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.AMBIENT);
        }
    }

    public void MatchCondition()
    {
        condition = null;
        CancelCountdown();
    }

    public void CheckCondition(int enumIndex)
    {
        WorldResource.ResourceType type = (WorldResource.ResourceType)enumIndex;
        Debug.Log(type);
        if ((state != GodState.CALLING && state != GodState.PUNISH) || condition == null || condition.type != type && condition.type != WorldResource.ResourceType.VILLAGER)
        {
            return;
        }
        if (type == WorldResource.ResourceType.VILLAGER)
        {
            modalPanel.DialogUser("Right click a villager of your choice and make the sacrifice", new string[] { "OK :(" }, new UnityAction[] { Cancel });
        }
        else
        {
            modalPanel.DialogUser("Are you sure you want to sacrifice " + condition.amount + " " + condition.type + " ?",
            new string[] { "Yes", "No" }, new UnityAction[] { DoSacrifice, Cancel });
        }

    }

    private void DoSacrifice()
    {
        if (condition.type != WorldResource.ResourceType.VILLAGER)
        {
            Silo silo = Silo.FindTargetSilo(condition.type);
            if (silo != null && silo.totalAmount > condition.amount)
            {
                silo.totalAmount -= condition.amount;
                MatchCondition();
            }
        }
        Cancel();
    }


    private void GenerateCondition()
    {
        //pick a random enum value
        WorldResource.ResourceType conditionType = (WorldResource.ResourceType)UnityEngine.Random.Range(0, 5);
        int amount = 1;

        if (conditionType != WorldResource.ResourceType.VILLAGER)
        {
            Silo silo = Silo.FindTargetSilo(conditionType);
            if (silo != null & silo.totalAmount > 0)
            {
                amount = (int)(silo.totalAmount * UnityEngine.Random.Range(0.65f, 1.08f));
            }
            else
            {
                conditionType = WorldResource.ResourceType.VILLAGER;
            }

        }
        condition = new Condition();
        condition.type = conditionType;
        condition.amount = amount;
    }


    public void Cancel()
    {
        modalPanel.ClosePanel();
    }

}

[System.Serializable]
public class Condition
{
    public WorldResource.ResourceType type;
    public int amount;
}
