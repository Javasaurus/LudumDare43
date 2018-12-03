using UnityEngine;
using UnityEngine.Events;

public class TestModalPrompt : MonoBehaviour
{

    private ModalPanel modalPanel;

    public Sprite iconImage;

    private UnityAction confirmAction;
    private UnityAction refuteAction;
    private UnityAction cancelAction;

    public void OnEnable()
    {

        modalPanel = ModalPanel.GetInstance();

        confirmAction = new UnityAction(TestYesFunction);
        refuteAction = new UnityAction(TestNoFunction);
        cancelAction = new UnityAction(TestCancelFunction);

        modalPanel.DialogUser("Want to see a nice sprite pop up?", new string[] { "Yes", "No", "Cancel" }, new UnityAction[] { confirmAction, refuteAction, cancelAction });
    }

    //Set up the modal panel to set up buttons and functions

    private void TestYesFunction()
    {
        modalPanel.DialogUser(iconImage,"Okido, but you'll need to close this one...", new string[] { "OK" }, new UnityAction[] { refuteAction });
    }

    private void TestNoFunction()
    {
        Debug.Log("No was pressed");
        modalPanel.ClosePanel();
    }

    private void TestCancelFunction()
    {
        Debug.Log("Maybe was pressed");
        modalPanel.ClosePanel();
    }


}
