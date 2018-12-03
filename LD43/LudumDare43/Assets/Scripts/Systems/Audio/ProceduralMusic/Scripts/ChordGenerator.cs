using System.Linq;
using UnityEngine;

public abstract class ChordGenerator : MonoBehaviour
{

    /// <summary>
    /// Gathers notes in the scale that are no more than 2 away from our current note
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    public abstract int[][] generateChord(MusicTemplate template, int[] scale);

    /// <summary>
    /// extract the bass tone from the scale or chord, depending on the chord follow parameter
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="chord"></param>
    /// <returns></returns>
    /// <summary>
    /// extract the bass tone from the scale or chord, depending on the chord follow parameter
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="chord"></param>
    /// <returns></returns>
    public virtual int generateBassTone(MusicTemplate template, int[] scale, int[] chord)
    {
        if (UnityEngine.Random.Range(0f, 1f) > template.chordFollowRatio)
        {
            return chord.Min();
        }
        else
        {
            return scale[0];
        }
    }
}
