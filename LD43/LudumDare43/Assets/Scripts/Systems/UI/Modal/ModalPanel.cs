using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModalPanel : MonoBehaviour
{
    public TMPro.TextMeshProUGUI prompt;
    public Image iconImage;
    public Button[] dialogButtons;
    public Button[] promptButtons;
    public GameObject modalConfigPanelObject;
    public GameObject modalDialogPanelObject;
    public GameObject modalActionPromptPanelObject;

    private static ModalPanel modalPanel;

    public static ModalPanel GetInstance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType<ModalPanel>();
            modalPanel.modalDialogPanelObject.SetActive(false);
            modalPanel.modalActionPromptPanelObject.SetActive(false);
            modalPanel.modalConfigPanelObject.SetActive(false);
        }
        return modalPanel;
    }

    private void OnEnable()
    {
        ClosePanel();
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            modalDialogPanelObject.SetActive(false);
            modalActionPromptPanelObject.SetActive(false);
        }
    }

    /// <summary>
    /// Prompts the user for an input for an action
    /// </summary>
    /// <param name="buttonText"></param>
    /// <param name="buttonEvents"></param>
    public void PromptUser(string[] buttonText, UnityAction[] buttonEvents)
    {
        UITools.BringToFront(transform);
        GameStateManager.INSTANCE.currentState = GameStateManager.GameState.DIALOG;
        modalActionPromptPanelObject.SetActive(true);
        DisableButtons();
        for (int i = 0; i < buttonText.Length; i++)
        {
            SetButtonAction(promptButtons[i], buttonText[i], buttonEvents[i]);
        }
    }



    /// <summary>
    /// Prompts the user with the given question, options and a sprite 
    /// </summary>
    /// <param name="question"></param>
    /// <param name="buttonText"></param>
    /// <param name="buttonEvents"></param>
    public void DialogUser(string question, string[] buttonText, UnityAction[] buttonEvents)
    {
        UITools.BringToFront(transform);
        if (GameStateManager.INSTANCE != null)
        {
            GameStateManager.INSTANCE.currentState = GameStateManager.GameState.DIALOG;
        }
        iconImage.gameObject.SetActive(false);
        modalActionPromptPanelObject.SetActive(false);
        modalConfigPanelObject.SetActive(false);
        modalDialogPanelObject.SetActive(true);
        modalDialogPanelObject.transform.localScale = Vector3.one;
        DisableButtons();
        for (int i = 0; i < buttonText.Length; i++)
        {
            SetButtonAction(dialogButtons[i], buttonText[i], buttonEvents[i]);
        }

        prompt.text = question;
    }

    /// <summary>
    /// Prompts the user with the igven question, options and a sprite 
    /// </summary>
    /// <param name="image"></param>
    /// <param name="question"></param>
    /// <param name="buttonText"></param>
    /// <param name="buttonEvents"></param>
    public void DialogUser(Sprite image, string question, string[] buttonText, UnityAction[] buttonEvents)
    {
        DialogUser(question, buttonText, buttonEvents);
        iconImage.gameObject.SetActive(true);
        iconImage.sprite = image;

    }

    /// <summary>
    /// Sets up a button
    /// </summary>
    /// <param name="button"></param>
    /// <param name="text"></param>
    /// <param name="action"></param>
    /// <param name="autoClose"></param>
    private void SetButtonAction(Button button, string text, UnityAction action)
    {
        button.gameObject.SetActive(true);

        button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);

        button.gameObject.SetActive(true);
    }



    /// <summary>
    /// disables the buttons
    /// </summary>
    private void DisableButtons()
    {
        foreach (Button button in dialogButtons)
        {
            button.gameObject.SetActive(false);
        }
        foreach (Button button in promptButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// closes the panels
    /// </summary>
    public void ClosePanel()
    {
        modalDialogPanelObject.SetActive(false);
        modalActionPromptPanelObject.SetActive(false);
        modalConfigPanelObject.SetActive(false);
        if (GameStateManager.INSTANCE != null)
        {
            StartCoroutine(WaitForFeedback());
        }
    }

    private IEnumerator WaitForFeedback()
    {
        yield return new WaitForSeconds(0.5f);
        GameStateManager.INSTANCE.currentState = GameStateManager.GameState.WAIT_INPUT;
    }


}
