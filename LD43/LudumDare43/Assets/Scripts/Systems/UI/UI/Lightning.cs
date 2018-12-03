using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Lightning : MonoBehaviour
{

    private float lightningTimer;
    private AudioSource myAudioSource;

    public Color flashColor = Color.white;
    public Color normalColor = Color.clear;

    private Image image;

    // Use this for initialization
    private void Start()
    {
        image = GetComponent<Image>();
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {

        if (Time.time > lightningTimer)
        {
            image.color = flashColor;
            lightningTimer = Time.time + Random.Range(2f, 8f);
            myAudioSource.pitch = Random.Range(0.4f, 0.7f);
            myAudioSource.PlayOneShot(myAudioSource.clip);
        }
        image.color = Color.Lerp(image.color, normalColor, Time.deltaTime * 3f);
    }


}
