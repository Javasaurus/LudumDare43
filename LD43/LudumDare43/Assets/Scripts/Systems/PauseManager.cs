using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public bool paused;
    private GameStateManager.GameState previousState;

    public void TogglePause()
    {
        paused = !paused;
        //   Time.timeScale = paused ? 1 : 0;
        if (paused)
        {
            PauseWithConfig();
        }
        else
        {
            ResumeFromConfig();
        }

    }

    public void PauseDialog()
    {
        paused = true;
        //    Time.timeScale = 0;
        previousState = GameStateManager.INSTANCE.currentState;
        GameStateManager.INSTANCE.currentState = GameStateManager.GameState.PAUSED;
    }

    public void ResumeDialog()
    {
        GameStateManager.INSTANCE.currentState = previousState;
        paused = false;
        //      Time.timeScale = 1;
    }



    public void PauseWithConfig()
    {
        previousState = GameStateManager.INSTANCE.currentState;
        GameStateManager.INSTANCE.currentState = GameStateManager.GameState.PAUSED;
        ModalPanel panel = ModalPanel.GetInstance();
        panel.ClosePanel();
        panel.modalConfigPanelObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeFromConfig()
    {
        GameStateManager.INSTANCE.currentState = previousState;
        ModalPanel panel = ModalPanel.GetInstance();
        panel.ClosePanel();
        Time.timeScale = 1;
    }

}
