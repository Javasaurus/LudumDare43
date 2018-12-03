using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Procedural/Audio/Instruments")]
public class InstrumentTemplate : ScriptableObject {

    [Header("Beat")]
    public AudioClip beatTone;

    [Header("Chord")]
    public AudioClip chordTone;

    [Header("Harmony")]
    public AudioClip harmonyTone;

    [Header("Bass")]
    public AudioClip bassTone;
}
