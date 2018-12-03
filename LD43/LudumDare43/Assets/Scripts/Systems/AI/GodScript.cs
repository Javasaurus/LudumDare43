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
    // Use this for initialization
    private void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void OnEnable()
    {

        modalPanel = ModalPanel.GetInstance();

    }

    // Update is called once per frame
    private void Update()
    {
        if (state == GodState.IDLE)
        {
            //avoid doing it from the get-go
            if (Time.time > chanceTimer)
            {
                if (chanceTimer != 0)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < chanceForSacrificeCall)
                    {
                        state = GodState.CALLING;
                        sacrificeTimer = timeToSacrifice;
                        _source.PlayOneShot(SacrificeCall);
                        if (Random.Range(0f, 1f) < chanceForIdolHunt)
                        {
                            ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.REWARD);
                            currentIdolHunt = Instantiate(idolHuntPrefab);
                            modalPanel.DialogUser(godDialogIcon, "The gods have sent an idol into the forest. Show your faith and find it for them! ", new string[] { "OK" }, new UnityAction[] { Cancel });
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
                        ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.AMBIENT);
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
        ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.PUNISH);
        timeSincePunishCall = 0;
        state = GodState.PUNISH;
        punishmentEffects[UnityEngine.Random.Range(0, punishmentEffects.Length)].SetActive(true);
    }

    public void CancelCountdown()
    {
        if (currentPunishmentEffect != null)
        {
            currentPunishmentEffect.SetActive(false);
            currentPunishmentEffect = null;
        }
        killTimer = 0;
        state = GodState.IDLE;
        ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.AMBIENT);
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
        modalPanel.DialogUser("Are you sure you want to sacrifice " + condition.amount + " " + condition.type + " ?",
        new string[] { "Yes", "No" }, new UnityAction[] { DoSacrifice, Cancel });
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
