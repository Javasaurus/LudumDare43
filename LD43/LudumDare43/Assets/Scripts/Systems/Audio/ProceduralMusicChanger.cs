using UnityEngine;

[RequireComponent(typeof(MusicGenerator))]
public class ProceduralMusicChanger : MonoBehaviour
{
    private MusicGenerator _musicGenerator;

    public enum MusicState
    {
        MAIN_MENU, AMBIENT, REWARD, PUNISH, GAME_OVER
    }

    public AudioTrack mainMenu;
    public AudioTrack ambientNormal;
    public AudioTrack ambientReward;
    public AudioTrack ambientPunish;
    public AudioTrack gameOver;

    public InstrumentTemplate defaultPiano;
    public InstrumentTemplate previous;

    public static ProceduralMusicChanger INSTANCE;

    public void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            _musicGenerator = GetComponent<MusicGenerator>();
            previous = _musicGenerator.instrumentTemplate;
        }
        else
        {
            GameObject.Destroy(gameObject);
        }
    }

    public void SetMusicState(MusicState state)
    {
        switch (state)
        {
            case MusicState.AMBIENT:
                SetMusic(ambientNormal);
                break;
            case MusicState.GAME_OVER:
                SetMusic(gameOver);
                break;
            case MusicState.MAIN_MENU:
                SetMusic(mainMenu);
                break;
            case MusicState.PUNISH:
                SetMusic(ambientPunish);
                break;
            case MusicState.REWARD:
                SetMusic(ambientReward);
                break;
        }
    }

    private void SetMusic(AudioTrack track)
    {
        _musicGenerator.musicTemplate = track.song;
        previous = _musicGenerator.instrumentTemplate;
        if (AudioManager.PIANO)
        {
            _musicGenerator.instrumentTemplate = defaultPiano;
        }
        else
        {
            _musicGenerator.instrumentTemplate = track.instrument;
        }

    }

    public void setPiano(bool piano)
    {
        if (piano)
        {
            _musicGenerator.instrumentTemplate = defaultPiano;
        }
        else
        {
            _musicGenerator.instrumentTemplate = previous;
        }
    }




}

[System.Serializable]
public struct AudioTrack
{
    public MusicTemplate song;
    public InstrumentTemplate instrument;
}