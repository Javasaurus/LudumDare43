using UnityEngine;

public class HideOnWeb : MonoBehaviour
{

    public void Awake()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            gameObject.SetActive(false);
        }
    }

}
