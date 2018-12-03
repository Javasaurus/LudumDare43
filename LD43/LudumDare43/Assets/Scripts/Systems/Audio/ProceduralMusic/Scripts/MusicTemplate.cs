using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Procedural/Audio/Music")]
public class MusicTemplate : ScriptableObject
{

    [Header("General")]
    public NoteScales.Scale scale;
    public int seed = 123456;

    [Range(0f, 4f)]
    public float pitchOffset = 0f;
    [Range(0f, 1f)]
    public float toneDensity = 0.75f;
    [Range(-1, 1)]
    public int positivity;

    public int BPM;


    [HideInInspector]
    public AudioClip beatTone;

    [Header("Beat")]
    [Range(0f, 1f)]
    public float beatVolume;
    [Range(0f, 1f)]
    public float beatGain;
    [Range(0f, 1f)]
    public float beatDecay;
    public int[] beatPattern;
    public bool beat;


    [HideInInspector]

    public AudioClip chordTone;

    [Header("Chord")]
    [Range(0f, 1f)]
    public float chordVolume;

    [Range(0f, 1f)]
    public float chordGain;
    [Range(0f, 1f)]
    public float chordDecay;

    [Range(1, 8)]
    public int ChordChannelCount = 1;

    [Range(0, 10)]
    public int chordUpdateInterval = 5;

    [Range(1, 10)]
    public int chordBufferSize = 5;

    [Range(1, 8)]
    public int chordSize;

    public List<ChordGenerator> chordGenerators;


    [HideInInspector]
    public AudioClip harmonicTone;
    [Header("Harmony")]
    [Range(0f, 1f)]
    public float harmonicVolume;
    [Range(0f, 1f)]
    public float harmonicGain;
    [Range(0f, 1f)]
    public float harmonicDecay;

    public bool thirdHarmonic;
    public bool fifthHarmonic;


    [HideInInspector]
    public AudioClip bassTone;
    [Header("Bass")]
    [Range(0f, 1f)]
    public float bassVolume;
    [Range(0f, 1f)]
    public float bassGain;
    [Range(0f, 1f)]
    public float bassDecay;
    [Range(0f, 1f)]
    public float chordFollowRatio = 0.4f;
    public bool bass;


}
