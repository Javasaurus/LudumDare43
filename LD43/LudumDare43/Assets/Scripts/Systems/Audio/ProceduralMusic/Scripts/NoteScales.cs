[System.Serializable]
public class NoteScales
{
    /// <summary>
    /// Supported scales
    /// </summary>
    public enum Scale
    {
        ALL,
        HAPPY,
        CREEPY,
        BLUES,
        EPIC,
        MYSTERIOUS,
        SOULFUL,
        JAZZ,
        COMPLEX,
        EXOTIC,
        COUNTRY,
        BIZARRE
    }

    // musical scales in semitone intervals:
    public static int[] scaleChromatic = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }; // (random, atonal: all twelve notes)
    public static int[] scaleMajor = { 2, 2, 1, 2, 2, 2, 1 }; // (classic, happy)
    public static int[] scaleHarmonicMinor = { 2, 1, 2, 2, 1, 3, 1 }; // (haunting, creepy)
    public static int[] scaleMinorPentatonic = { 3, 2, 2, 3, 2 }; // (blues, rock)
    public static int[] scaleNaturalMinor = { 2, 1, 2, 2, 1, 2, 2 }; // (scary, epic)
    public static int[] scaleMelodicMinorUp = { 2, 1, 2, 2, 2, 2, 1 }; // (wistful, mysterious)
    public static int[] scaleMelodicMinorDown = { 2, 2, 1, 2, 2, 1, 2 };  // (sombre, soulful)
    public static int[] scaleDorian = { 2, 1, 2, 2, 2, 1, 2 }; // (cool, jazzy)
    public static int[] scaleMixolydian = { 2, 2, 1, 2, 2, 1, 2 }; // (progressive, complex)
    public static int[] scaleAhavaRaba = { 1, 3, 1, 2, 1, 2, 2 }; // (exotic, unfamiliar)
    public static int[] scaleMajorPentatonic = { 2, 2, 3, 2, 3 }; // (country, gleeful)
    public static int[] scaleDiatonic = { 2, 2, 2, 2, 2, 2 }; // (bizarre, symmetrical)

    public static int[][] ScaleCollection = new int[][]
    {
        scaleChromatic,
        scaleMajor,
        scaleHarmonicMinor,
        scaleMinorPentatonic,
        scaleNaturalMinor,
        scaleMelodicMinorUp,
        scaleMelodicMinorDown,
        scaleDorian,
        scaleMixolydian,
        scaleAhavaRaba,
        scaleMajorPentatonic,
        scaleDiatonic
    };



    public enum Chord
    {
        MAJOR, MINOR, REL_MINOR_1_INV, SUBDOMINANT_2_INV, MAJOR7, MINOR7, MAJOR9, MINOR9, MAJOR6, MINOR6, MAJOR79, MINOR79, MAJOR711, MINOR711
    }

    // chord voicings in semitone distance from root note
    public static int[] chordMajor = { 0, 4, 7 };
    public static int[] chordMinor = { 0, 3, 7 };
    public static int[] chordRelMinor1stInv = { 0, 4, 9 };
    public static int[] chordSubdominant2ndInv = { 0, 5, 9 };
    public static int[] chordMajor7th = { 0, 4, 7, 11 };
    public static int[] chordMinor7th = { 0, 3, 7, 10 };
    public static int[] chordMajor9th = { 0, 4, 7, 14 };
    public static int[] chordMinor9th = { 0, 3, 7, 13 };
    public static int[] chordMajor6th = { 0, 4, 9 };
    public static int[] chordMinor6th = { 0, 3, 8 };
    public static int[] chordMajor7th9th = { 0, 4, 7, 11, 14 };
    public static int[] chordMinor7th9th = { 0, 3, 7, 10, 13 };
    public static int[] chordMajor7th11th = { 0, 4, 7, 11, 18 };
    public static int[] chordMinor7th11th = { 0, 3, 7, 10, 17 };

    public static int[][] chordCollection = new int[][]
  {
        chordMajor,
        chordMinor,
        chordRelMinor1stInv,
        chordSubdominant2ndInv,
        chordMajor7th,
        chordMinor7th,
        chordMajor9th,
        chordMinor9th,
        chordMajor6th,
        chordMinor6th,
        chordMajor7th9th,
        chordMinor7th9th,
        chordMajor7th11th,
        chordMinor7th11th
  };

 



}
