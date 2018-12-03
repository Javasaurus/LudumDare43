using UnityEngine;
using UnityEngine.Events;

public class EventNotifier : MonoBehaviour
{

    private ModalPanel modalPanel;

    public Sprite iconImage;

    public UnityAction confirmAction;
    public UnityAction backAction;
    public UnityAction helpAction;

    public string message;
    public string messageConfirmText;
    public string helpMessage;

    public void OnEnable()
    {
        ShowMessage();
    }

    public void ShowMessage()
    {    
        modalPanel = ModalPanel.GetInstance();
        confirmAction = new UnityAction(ClosePanel);
        helpAction = new UnityAction(ShowHelp);
        backAction = new UnityAction(ShowDialog);
        ShowDialog();
    }

    private void ShowDialog()
    {
        modalPanel.DialogUser(iconImage, message,
    new string[] { messageConfirmText, "Help" },
    new UnityAction[] { confirmAction, helpAction });
  //      GameObject.FindObjectOfType<PauseManager>().PauseDialog();
    }

    private void ShowHelp()
    {
        modalPanel.DialogUser(iconImage, helpMessage,
            new string[] { "BACK", "OK" },
            new UnityAction[] { backAction, confirmAction });
    }

    private void ClosePanel()
    {
  //      GameObject.FindObjectOfType<PauseManager>().ResumeDialog();
        modalPanel.ClosePanel();
    }


}
