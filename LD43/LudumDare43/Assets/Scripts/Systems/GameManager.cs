using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager INSTANCE;


    public void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void OpenConfigDialogWindow()
    {
        ModalPanel.GetInstance().modalConfigPanelObject.SetActive(true);
    }

    public void CloseConfigDialogWindow()
    {
        ModalPanel.GetInstance().modalConfigPanelObject.SetActive(false);
    }

    public void LoadGame()
    {
        GameObject.FindObjectOfType<LevelLoader>().RequestLoading();
    }

    public void ExitGame()
    {
        Application.Quit();
    }








}
