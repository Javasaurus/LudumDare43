using UnityEngine;

public class PerlinNoiseChordGenerator : ChordGenerator
{
    public float frequency = 15000f;
    private float currentPerlinX = 0;
    private float currentPerlinY = 0;

    public override int[][] generateChord(MusicTemplate template, int[] scale)
    {
        int[][] subChord = new int[template.chordSize * 2][];
        int currentScaleIndex;

        for (int i = 0; i < subChord.Length; i++)
        {
            subChord[i] = new int[4];

            if (Random.Range(0f, 1f) > template.toneDensity)
            {
                subChord[i][0] = -1;
            }
            else
            {
                currentScaleIndex = GetNextNote(scale);

                subChord[i][0] = scale[currentScaleIndex];
                subChord[i][1] = scale[Mathf.Clamp(currentScaleIndex + 2, 0, scale.Length - 1)];
                subChord[i][2] = scale[Mathf.Clamp(currentScaleIndex + 4, 0, scale.Length - 1)];
            }
        }
        for (int j = 0; j < subChord.Length; j++)
        {
            subChord[j][3] = generateBassTone(template, scale, subChord[j]);
        }
        return subChord;
    }

    public virtual int GetNextNote(int[] scale)
    {
        currentPerlinX += 1.5f;
        currentPerlinY = Time.deltaTime * 3f;
        float myNote = (Mathf.PerlinNoise(currentPerlinX / frequency, currentPerlinY / frequency) * scale.Length); ;
        return Mathf.RoundToInt(myNote);
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
