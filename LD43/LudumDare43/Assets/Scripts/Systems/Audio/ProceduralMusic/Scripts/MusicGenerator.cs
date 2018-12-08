using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicGenerator : MonoBehaviour
{
    /// <summary>
    /// the parent mixer for ambient music
    /// </summary>
    public AudioMixerGroup mixer;
    /// <summary>
    /// The template of the music to be generated procedurally (Scriptable Object)
    /// </summary>
    public MusicTemplate musicTemplate;
    /// <summary>
    ///  The template of the instruments to be used procedurally (Scriptable Object)
    /// </summary>
    public InstrumentTemplate instrumentTemplate;
    /// <summary>
    /// The generators for chords
    /// </summary>
    private List<ChordGenerator> chordGenerators;
    /// <summary>
    /// The queue of notes to be played
    /// </summary>
    public List<int[]> noteQueue;

    /// <summary>
    /// all note channels that need to be managed by this generator
    /// </summary>
    [HideInInspector]
    public List<NoteChannel> allNoteChannels;

    /// <summary>
    /// the collection of audio sources for the notes (multiple notes can be played)
    /// </summary>
    private NoteChannel[] chordChannels;
    /// <summary>
    /// the channel for the beat
    /// </summary>
    private NoteChannel beatChannel;
    /// <summary>
    /// the currently active channel (the first one that isn't playing)
    /// </summary>
    private NoteChannel currentChordChannel;
    /// <summary>
    /// the third harmonic channel
    /// </summary>
    private NoteChannel thirdHarmonicChannel;
    /// <summary>
    /// the fifth harmonic channel
    /// </summary>
    private NoteChannel fifthHarmonicChannel;
    /// <summary>
    /// The bass channel
    /// </summary>
    private NoteChannel bassChannel;
    /// <summary>
    /// A timer used to determine when the next note needs to be played
    /// </summary>
    private float noteTimer;
    /// <summary>
    /// the minimal time between notes
    /// </summary>
    private float restTimer = 0.2f;
    /// <summary>
    /// The beat counter (metronome)
    /// </summary>
    private int beatCounter;
    /// <summary>
    /// The pattern of a beat
    /// </summary>
    private int[] beatPattern;
    /// <summary>
    /// A buffer of chords to chose from (plus their harmonics and base)
    /// </summary>
    private int[][][] chordBuffer;
    /// <summary>
    /// Counter to keep track of how many cords have been played from the current buffer before we slide 
    /// to the new one
    /// </summary>
    private int chordsPlayed;
    /// <summary>
    /// the last played semitone
    /// </summary>
    private int previousSemiTone;



    // Use this for initialization
    private void Start()
    {
        SetInstruments();

        UnityEngine.Random.InitState(musicTemplate.seed);

        noteQueue = new List<int[]>();
        allNoteChannels = new List<NoteChannel>();

        chordGenerators = new List<ChordGenerator>();
        //setup Beat
        beatChannel = new NoteChannel(this);
        beatChannel.channel = (gameObject.AddComponent<AudioSource>());
        beatChannel.channel.spatialBlend = 0;
        beatChannel.channel.panStereo = 0;
        //we should map the beat to the scale... for example if there are 5 notes in a scale, but beat goes to 10...
        MapBeats(getScale(musicTemplate.scale), musicTemplate.beatPattern);

        //setup Chords
        chordChannels = new NoteChannel[musicTemplate.ChordChannelCount];
        for (int i = 0; i < musicTemplate.ChordChannelCount; i++)
        {
            chordChannels[i] = new NoteChannel(this);
            chordChannels[i].channel = (gameObject.AddComponent<AudioSource>());
            chordChannels[i].channel.playOnAwake = false;
            chordChannels[i].channel.spatialBlend = 0;
            chordChannels[i].channel.panStereo = 0;
        }
        currentChordChannel = chordChannels[0];
        //initialize the cord generators from the template
        foreach (ChordGenerator generator in musicTemplate.chordGenerators)
        {
            ChordGenerator instance = Instantiate(generator) as ChordGenerator;
            instance.transform.SetParent(transform);
            instance.name = generator.name;
            chordGenerators.Add(instance);
        }
        CreateRandomChords();

        //setup Harmonics
        //  if (template.thirdHarmonic)
        {
            thirdHarmonicChannel = new NoteChannel(this);
            thirdHarmonicChannel.channel = (gameObject.AddComponent<AudioSource>());
            thirdHarmonicChannel.channel.playOnAwake = false;
            thirdHarmonicChannel.channel.spatialBlend = 0;
            thirdHarmonicChannel.channel.panStereo = 0;

        }
        //   if (template.fifthHarmonic)
        {
            fifthHarmonicChannel = new NoteChannel(this);
            fifthHarmonicChannel.channel = (gameObject.AddComponent<AudioSource>());
            fifthHarmonicChannel.channel.playOnAwake = false;
            fifthHarmonicChannel.channel.spatialBlend = 0;
            fifthHarmonicChannel.channel.panStereo = 0;
        }

        //setup Bass
        //     if (template.bass)
        {
            bassChannel = new NoteChannel(this);
            bassChannel.channel = (gameObject.AddComponent<AudioSource>());
            bassChannel.channel.playOnAwake = false;
            bassChannel.channel.spatialBlend = 0;
            bassChannel.channel.panStereo = 0;
        }

        //set the audio mixer of all these to ambient

        foreach (NoteChannel source in allNoteChannels)
        {
            source.channel.outputAudioMixerGroup = mixer;
        }
    }

    private void LateUpdate()
    {
        SetInstruments();
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (NoteChannel channel in allNoteChannels)
        {
            channel.Update(Time.deltaTime);
        }
        if (Time.time > noteTimer)
        {
            if (Time.time > restTimer)
            {
                PlayBeat();
                restTimer = Time.time + 0.2f;
                if (noteQueue.Count == 0)
                {
                    PlayNextChord();
                }
                PlaySemitone(noteQueue[0]);
                noteTimer = Time.time + (60f / (float)musicTemplate.BPM);
            }
        }
    }



    private ChordGenerator selectChordGenerator()
    {
        return chordGenerators[UnityEngine.Random.Range(0, chordGenerators.Count)];
    }


    private void SetInstruments()
    {
        musicTemplate.bassTone = instrumentTemplate.bassTone;
        musicTemplate.chordTone = instrumentTemplate.chordTone;
        musicTemplate.harmonicTone = instrumentTemplate.harmonyTone;
        musicTemplate.beatTone = instrumentTemplate.beatTone;
    }

    /// <summary>
    /// Plays a semitone using the appropriate channels
    /// </summary>
    /// <param name="semitones"></param>
    public void PlaySemitone(int[] semitones)
    {
        GetAvailableAudioSource();
        noteQueue.RemoveAt(0);
        if (semitones[0] == -1 || previousSemiTone == semitones[0])
        {
            previousSemiTone = semitones[0];
            return;
        }
        else
        {
            currentChordChannel.channel.pitch = musicTemplate.pitchOffset + Mathf.Pow(1.05946f, semitones[0]);

            currentChordChannel.PlayNote(new Note(
                musicTemplate.chordVolume,
                musicTemplate.chordGain,
                musicTemplate.chordDecay,
                currentChordChannel.channel,
                musicTemplate.chordTone));

            if (musicTemplate.thirdHarmonic)
            {
                thirdHarmonicChannel.channel.pitch = musicTemplate.pitchOffset + Mathf.Pow(1.05946f, semitones[1]);
                thirdHarmonicChannel.PlayNote(new Note(
                musicTemplate.harmonicVolume,
                musicTemplate.harmonicGain,
                musicTemplate.harmonicDecay,
                thirdHarmonicChannel.channel,
                musicTemplate.harmonicTone));
            }
            if (musicTemplate.fifthHarmonic)
            {
                fifthHarmonicChannel.channel.pitch = musicTemplate.pitchOffset + Mathf.Pow(1.05946f, semitones[2]);
                fifthHarmonicChannel.PlayNote(new Note(
               musicTemplate.harmonicVolume,
               musicTemplate.harmonicGain,
               musicTemplate.harmonicDecay,
                fifthHarmonicChannel.channel,
                musicTemplate.harmonicTone));
            }
            if (musicTemplate.bass)
            {
                bassChannel.channel.pitch = musicTemplate.pitchOffset + Mathf.Pow(1.05946f, semitones[3]);
                bassChannel.PlayNote(new Note(
                musicTemplate.bassVolume,
                musicTemplate.bassGain,
                musicTemplate.bassDecay,
                bassChannel.channel,
                musicTemplate.bassTone));
            }
            previousSemiTone = semitones[0];
        }

    }

    /// <summary>
    /// Plays the beat continuously (even during pauses)
    /// </summary>
    public void PlayBeat()
    {
        if (musicTemplate.beat)
        {
            if (beatPattern[beatCounter] > 0)
            {
                beatChannel.channel.clip = musicTemplate.beatTone;
                beatChannel.channel.pitch = musicTemplate.pitchOffset + Mathf.Pow(1.05946f, beatPattern[beatCounter]);
                beatChannel.PlayNote(new Note(
                musicTemplate.beatVolume,
                musicTemplate.beatGain,
                musicTemplate.beatDecay,
                beatChannel.channel,
                musicTemplate.beatTone));
            }
        }
        beatCounter = (beatCounter + 1) % beatPattern.Length;
    }


    /// <summary>
    /// Maps the beats to the range of the scale
    /// </summary>
    /// <param name="scale"></param>
    /// <param name="beatPattern"></param>
    private void MapBeats(int[] scale, int[] beatPattern)
    {
        // we go over each part in the beat and map it between the min and max of the scale
        int minScaleValue = 0;
        int maxScaleValue = scale.Length;

        int minBeatValue = 0;
        int maxBeatValue = beatPattern.Max();

        this.beatPattern = new int[beatPattern.Length];
        for (int i = 0; i < beatPattern.Length; i++)
        {
            int aValue = beatPattern[i];
            if (aValue == -1)
            {
                this.beatPattern[i] = -1;
            }
            else
            {
                float normal = Mathf.InverseLerp(minBeatValue, maxBeatValue, aValue);
                float bValue = Mathf.Lerp(minScaleValue, maxScaleValue, normal);
                this.beatPattern[i] = (int)bValue;
            }
        }
    }

    /// <summary>
    /// gets the currently open chanel for the chord
    /// </summary>
    private void GetAvailableAudioSource()
    {
        NoteChannel sourceChannel = chordChannels[0];
        foreach (NoteChannel noteChannel in chordChannels)
        {
            noteChannel.channel.clip = musicTemplate.chordTone;
            if (!noteChannel.channel.isPlaying)
            {
                sourceChannel = noteChannel;
                break;
            }
        }
        sourceChannel.Stop();
        currentChordChannel = sourceChannel;
    }

    /// <summary>
    /// Create a random chord (should be swapped with perlin)
    /// </summary>
    private void CreateRandomChords()
    {
        int[] scaleValues = getScale(musicTemplate.scale);
        int[] convertedScaleValues = convertScale(scaleValues);
        chordBuffer = new int[musicTemplate.chordBufferSize][][];
        for (int i = 0; i < chordBuffer.Length; i++)
        {
            chordBuffer[i] = selectChordGenerator().generateChord(musicTemplate, convertedScaleValues);
        }
    }

    /// <summary>
    /// Updates the buffer of chords with a new one, eliminating the first
    /// </summary>
    private void UpdateRandomChords()
    {
        int[] scaleValues = getScale(musicTemplate.scale);
        int[] convertedScaleValues = convertScale(scaleValues);
        int[][][] tmp = new int[chordBuffer.Length][][];
        Array.Copy(chordBuffer, 1, tmp, 0, chordBuffer.Length - 1);
        if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
        {
            chordBuffer[chordBuffer.Length - 1] = selectChordGenerator().generateChord(musicTemplate, convertedScaleValues);
        }
        else
        {
            chordBuffer[chordBuffer.Length - 1] = getRandomChord(convertedScaleValues);
        }
        //we add a break here of a few notes
        int breakNotes = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < breakNotes; i++)
        {
            noteQueue.Add(new int[] { -1, -1, -1, -1 });
        }
    }

    /// <summary>
    /// Plays the next chord
    /// </summary>
    private void PlayNextChord()
    {
        chordsPlayed++;
        if (chordsPlayed > musicTemplate.chordUpdateInterval)
        {
            chordsPlayed = 0;
            UpdateRandomChords();
        }
        noteQueue.Clear();
        noteQueue.AddRange(chordBuffer[UnityEngine.Random.Range(0, chordBuffer.Length)]);

    }

    /// <summary>
    /// Generates a random chord
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    private int[][] getRandomChord(int[] scale)
    {
        Array values = Enum.GetValues(typeof(NoteScales.Chord));
        NoteScales.Chord randomChord = (NoteScales.Chord)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        return convertChord(getChord(randomChord), scale);
    }

    /// <summary>
    /// gets a scale by the enum
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    private int[] getScale(NoteScales.Scale scale)
    {
        return NoteScales.ScaleCollection[(int)scale];
    }

    /// <summary>
    /// gets a chord by the enum
    /// </summary>
    /// <param name="chord"></param>
    /// <returns></returns>
    private int[] getChord(NoteScales.Chord chord)
    {
        return NoteScales.chordCollection[(int)chord];
    }

    /// <summary>
    /// Converts a scale tobe useable (cumulative of notes)
    /// </summary>
    /// <param name="scale"></param>
    /// <returns></returns>
    private int[] convertScale(int[] scale)
    {
        int[] convertedScale = new int[scale.Length];
        int cumulative = 0;
        for (int i = 0; i < convertedScale.Length; i++)
        {
            cumulative += scale[i];
            convertedScale[i] = cumulative;
        }
        return convertedScale;
    }

    /// <summary>
    /// converts a chord to match our schale, clamping the harmonics
    /// </summary>
    /// <param name="chord"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    private int[][] convertChord(int[] chord, int[] scale)
    {
        int[][] convertedChord = new int[chord.Length][];
        for (int i = 0; i < convertedChord.Length; i++)
        {
            convertedChord[i] = new int[4];
            convertedChord[i][0] = scale[0] + chord[i];
            convertedChord[i][1] = scale[Mathf.Clamp(2, 0, scale.Length - 1)];
            convertedChord[i][2] = scale[Mathf.Clamp(4, 0, scale.Length - 1)];
        }
        return convertedChord;
    }


}

[System.Serializable]
public class NoteChannel
{
    public AudioSource channel;
    public Note note;

    public NoteChannel(MusicGenerator generator)
    {
        generator.allNoteChannels.Add(this);
        channel = null;
        note = null;
    }

    public void Update(float deltaTime)
    {
        if (note != null)
        {
            note.Update(deltaTime);
        }
    }

    public void PlayNote(Note note)
    {
        this.note = note;
        channel.volume = 0;
        channel.PlayOneShot(note.noteClip);
    }

    public void Stop()
    {
        /*        note = null;
                channel.volume = 0;*/
        channel.Stop();
    }
}