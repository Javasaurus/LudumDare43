using UnityEngine;

public class Note
{
    public float time;

    public float attack;
    public float decay;
    public AudioClip noteClip;
    public AudioSource channel;
    public float requiredVolume;

    public bool isDone;

    /// <summary>
    /// This is the time for the volume to go from 0 to the required volume
    /// </summary>
    private float attackTime;
    /// <summary>
    /// This is the time for the volume to go from required volume to 0
    /// </summary>
    private float decayTime;

    public Note(float requiredVolume,float attack,float decay, AudioSource channel, AudioClip noteClip)
    {
        this.attack = attack;
        this.decay = decay;
        this.requiredVolume = requiredVolume;
        this.noteClip = noteClip;
        this.channel = channel;
        init();
    }

    private void init()
    {
        isDone = false;
        attackTime = noteClip.length * attack;
        decayTime = noteClip.length - (noteClip.length * decay);
    }

    public void Update(float deltaTime)
    {
        if (!isDone)
        {
            time += deltaTime;
            if (time < attackTime)
            {
                channel.volume = Mathf.Lerp(channel.volume, 1, 5*deltaTime);
            }
            else if (time > decayTime)
            {
                channel.volume = Mathf.Lerp(channel.volume, 0, 5*deltaTime);
            }
            isDone = (time >= noteClip.length);
        }
    }

}
