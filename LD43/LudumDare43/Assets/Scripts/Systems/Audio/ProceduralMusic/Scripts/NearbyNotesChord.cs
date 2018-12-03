using System.Linq;
using UnityEngine;

public class NearbyNotesChord : ChordGenerator
{


    /// <summary>
    /// Gathers notes in the scale that are no more than 2 away from our current note
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    public override int[][] generateChord(MusicTemplate template, int[] scale)
    {
        int[][] subCord = new int[template.chordSize * 2][];
        int currentScaleIndex = UnityEngine.Random.Range(0, scale.Length);
        for (int i = 0; i < subCord.Length; i++)
        {
            subCord[i] = new int[4];
            if (currentScaleIndex >= scale.Length)
            {
                currentScaleIndex -= UnityEngine.Random.Range(0, scale.Length);
            }
            if (Random.Range(0f, 1f) > template.toneDensity)
            {
                subCord[i][0] = -1;
            }
            else
            {
                subCord[i][0] = scale[currentScaleIndex];
                subCord[i][1] = scale[Mathf.Clamp(currentScaleIndex + 2, 0, scale.Length - 1)];
                subCord[i][2] = scale[Mathf.Clamp(currentScaleIndex + 4, 0, scale.Length - 1)];

                int offset = 0;
                if (template.positivity > 0)
                {
                    offset = Random.Range(1, 2);
                }
                else if (template.positivity < 0)
                {
                    offset = UnityEngine.Random.Range(-2, -1);
                }
                else
                {
                    offset = UnityEngine.Random.Range(-2, 2);
                }

                currentScaleIndex = Mathf.Clamp(currentScaleIndex + offset, 0, scale.Length - 1);
            }
        }
        for (int j = 0; j < subCord.Length; j++)
        {
            subCord[j][3] = generateBassTone(template,scale, subCord[j]);
        }
        return subCord;
    }

    /// <summary>
    /// extract the bass tone from the scale or chord, depending on the chord follow parameter
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="chord"></param>
    /// <returns></returns>
    public override int generateBassTone(MusicTemplate template, int[] scale, int[] chord)
    {
        if (Random.Range(0f, 1f) > template.chordFollowRatio)
        {
            return chord[0];
        }
        else
        {
            return scale[0];
        }
    }
}
