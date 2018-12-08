using UnityEngine;
using UnityEngine.Events;

public class DialogRunner : MonoBehaviour
{
    public enum DialogAction
    {
        NONE, GOODBYE, ACCEPT, DECLINE
    }


    public DialogLine[] lines;
    private int dialogIndex = 0;

    private ModalPanel modalPanel;

    public Sprite iconImage;

    public UnityAction confirmAction;
    private UnityAction refuteAction;
    private UnityAction skipTutorialAction;


    public void OnEnable()
    {
        modalPanel = ModalPanel.GetInstance();
        confirmAction = new UnityAction(DisplayNextLine);
        refuteAction = new UnityAction(DisplayPreviousLine);
        skipTutorialAction = new UnityAction(SkipTutorial);
        StartDialog();
    }

    private void SkipTutorial()
    {
        modalPanel.ClosePanel();
        GameObject.Destroy(gameObject);
    }

    public void StartDialog()
    {
        if (ProceduralMusicChanger.INSTANCE != null)
        {
            ProceduralMusicChanger.INSTANCE.SetMusicState(ProceduralMusicChanger.MusicState.AMBIENT);
        }
        dialogIndex = 0;
        DisplayLine(lines[dialogIndex]);
    }

    //Set up the modal panel to set up buttons and functions

    public void DisplayNextLine()
    {
        if (dialogIndex == lines.Length - 1)
        {
            dialogIndex = 0;
            modalPanel.ClosePanel();
            GameObject.Destroy(this);
        }
        else
        {
            dialogIndex = Mathf.Min(lines.Length - 1, dialogIndex + 1);
            DisplayLine(lines[dialogIndex]);
        }
    }

    public void DisplayPreviousLine()
    {
        dialogIndex = Mathf.Max(0, dialogIndex - 1);
        DisplayLine(lines[dialogIndex]);
    }

    public void DisplayLine(DialogLine line)
    {
        if (line.action == DialogAction.NONE)
        {
            if (dialogIndex == 0)
            {
                modalPanel.DialogUser(line.spriteToDisplay, line.line, new string[] { "Next", "Skip Tutorial" }, new UnityAction[] { confirmAction, skipTutorialAction });
            }
            else if (dialogIndex == lines.Length - 1)
            {
                modalPanel.DialogUser(line.spriteToDisplay, line.line, new string[] { "End Conversation" }, new UnityAction[] { skipTutorialAction });
            }
            else
            {
                modalPanel.DialogUser(line.spriteToDisplay, line.line, new string[] { "Prev", "Next", "Skip Tutorial" }, new UnityAction[] { refuteAction, confirmAction, skipTutorialAction });
            }
        }
        else
        {
            if (line.action == DialogAction.GOODBYE)
            {
                modalPanel.DialogUser(line.spriteToDisplay, line.line, new string[] { "Goodbye", "End Conversation" }, new UnityAction[] { confirmAction, skipTutorialAction });
            }
            if (line.action == DialogAction.ACCEPT)
            {
                modalPanel.DialogUser(line.spriteToDisplay, line.line, new string[] { "OK", }, new UnityAction[] { confirmAction, skipTutorialAction });
            }
            if (line.action == DialogAction.DECLINE)
            {
                modalPanel.DialogUser(line.spriteToDisplay, line.line, new string[] { "No", }, new UnityAction[] { confirmAction, skipTutorialAction });
            }
        }
    }

}

[System.Serializable]
public class DialogLine
{
    public DialogRunner.DialogAction action;
    public DialogLine nextLine;
    public string line;
    public Sprite spriteToDisplay;
}