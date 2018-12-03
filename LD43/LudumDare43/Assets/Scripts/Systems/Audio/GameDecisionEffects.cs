using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameDecisionEffects : MonoBehaviour {

    private static GameDecisionEffects INSTANCE;
    private AudioSource _source;

    public AudioClip characterConfirm;
    public AudioClip characterDecline;
    public AudioClip villagerSacrifice;

	// Use this for initialization
	void Awake () {
        if (INSTANCE == null)
        {
            INSTANCE = this;
            _source = GetComponent<AudioSource>();
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
	}
	
    public static void PlayConfirmSound()
    {
        if (INSTANCE != null)
        {
            INSTANCE._source.pitch = Random.Range(0.8f, 1.2f);
            INSTANCE._source.PlayOneShot(INSTANCE.characterConfirm);
        }
    }

    public static void PlayDeclineSound()
    {
        if (INSTANCE != null)
        {
            INSTANCE._source.pitch = Random.Range(0.8f, 1.2f);
            INSTANCE._source.PlayOneShot(INSTANCE.characterDecline);
        }
    }

    public static void PlaySacrificeConfirm()
    {
        if (INSTANCE != null)
        {
            INSTANCE._source.pitch = Random.Range(0.8f, 1.2f);
            INSTANCE._source.PlayOneShot(INSTANCE.villagerSacrifice);
        }
    }

}
