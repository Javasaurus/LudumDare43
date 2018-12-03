using UnityEngine;

public class UIDisable : MonoBehaviour
{

    public GameObject objectToDisable;
    public KeyCode keyCode;

    public void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            objectToDisable.SetActive(!objectToDisable.activeSelf);
        }
    }
}
