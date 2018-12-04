using UnityEngine;

public class HelpManager : MonoBehaviour
{

    public GameObject HelpPanel;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            HelpPanel.SetActive(!HelpPanel.activeSelf);
        }
    }
}
