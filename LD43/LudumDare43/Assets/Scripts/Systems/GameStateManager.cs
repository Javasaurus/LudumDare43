using UnityEngine;

public class GameStateManager : MonoBehaviour
{


    public static GameStateManager INSTANCE;

    public enum GameState
    {
        WAIT_INPUT, PAUSED, SELECTING, DIALOG, GAME_OVER
    }

    public GameState currentState;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }



}
