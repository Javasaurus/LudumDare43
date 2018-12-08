using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStats : MonoBehaviour
{

    public int villagers;
    public int wood;
    public int food;
    public int gold;
    public int water;

    public float godTimer;

    public TMPro.TextMeshProUGUI villagersTxt;
    public TMPro.TextMeshProUGUI woodTxt;
    public TMPro.TextMeshProUGUI foodTxt;
    public TMPro.TextMeshProUGUI goldTxt;
    public TMPro.TextMeshProUGUI waterTxt;

    public Image GodLoadingBar;

    public static GameStats INSTANCE;
    private ModalPanel modalPanel;
    public void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            modalPanel = ModalPanel.GetInstance();
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void UpdateNumber(WorldResource.ResourceType type, int amount)
    {
        switch (type)
        {
            case WorldResource.ResourceType.FOOD:
                food = amount;
                foodTxt.text = "X" + food;
                break;
            case WorldResource.ResourceType.WOOD:
                wood = amount;
                woodTxt.text = "X" + wood;
                break;
            case WorldResource.ResourceType.GOLD:
                gold = amount;
                goldTxt.text = "X" + gold;
                break;
            case WorldResource.ResourceType.WATER:
                water = amount;
                waterTxt.text = "X" + water;
                break;
        }
    }

    public void UpdateGodTimer(float amount)
    {
        GodLoadingBar.fillAmount = Mathf.Clamp(amount, 0f, 1f);
    }

    public void UpdateVillagers(int count)
    {
        villagers = count;
        villagersTxt.text = "X " + count;
    }

    public void LateUpdate()
    {
        if (!checkVillagers() && GameStateManager.INSTANCE.currentState != GameStateManager.GameState.GAME_OVER)
        {
            if (ProceduralMusicChanger.INSTANCE != null)
            {
                ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.GAME_OVER);
            }
            GameStateManager.INSTANCE.currentState = GameStateManager.GameState.GAME_OVER;
            modalPanel.DialogUser("The last of the villagers have perished, per the will of the gods. Sacrifices have been made in vain.",
                new string[] { "End" }, new UnityAction[] { BackToMain });
        }
    }

    private bool checkVillagers()
    {
        return GameObject.FindObjectsOfType<Villager>().Length > 0;
    }

    private void BackToMain()
    {
        modalPanel.ClosePanel();
        if (ProceduralMusicChanger.INSTANCE != null)
        {
            ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.MAIN_MENU);
        }
        SceneManager.LoadScene(0);
    }
}
