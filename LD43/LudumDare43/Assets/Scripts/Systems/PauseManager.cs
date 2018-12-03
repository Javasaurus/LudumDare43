using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public bool paused;

    public void TogglePause()
    {
        paused = !paused;
        Time.timeScale = paused ? 1 : 0;
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
        Time.timeScale = 0;
    }

    public void ResumeDialog()
    {
        paused = false;
        Time.timeScale = 1;
    }

    public void PauseWithConfig()
    {
        ModalPanel.GetInstance().modalConfigPanelObject.SetActive(true);
    }

    public void ResumeFromConfig()
    {
        ModalPanel.GetInstance().modalConfigPanelObject.SetActive(false);
    }

}
