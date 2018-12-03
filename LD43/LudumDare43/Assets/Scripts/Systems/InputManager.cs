using UnityEngine;

public class InputManager : MonoBehaviour
{

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<PauseManager>().TogglePause();
        }
    }

}
