using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AudioVisualizer : MonoBehaviour
{
    //An AudioSource object so the music can be played  
    public AudioSource[] audioSources;
    //A float array that stores the audio samples  
    public float[] samples = new float[64];
    //The transform attached to this game object  
    private Transform goTransform;
    //The position of the current cube. Will also be the position of each point of the line.  
    private Vector3 cubePos;
    //An array that stores the Transforms of all instantiated cubes  
    private Transform[] cubesTransform;
    //The velocity that the cubes will drop  
    private Vector3 gravity = new Vector3(0.0f, 0.25f, 0.0f);

    private void Awake()
    {
        //Get and store a reference to the following attached components:  


        //Transform  
        goTransform = GetComponent<Transform>();
    }

    private void Start()
    {

        //The cubesTransform array should be initialized with the same length as the samples array  
        cubesTransform = new Transform[samples.Length];
        //Center the audio visualization line at the X axis, according to the samples array length  
        goTransform.position = new Vector3(-samples.Length / 2, goTransform.position.y, goTransform.position.z);

        //Create a temporary GameObject, that will serve as a reference to the most recent cloned cube  
        GameObject tempCube;

        //For each sample  
        for (int i = 0; i < samples.Length; i++)
        {
            //Instantiate a cube placing it at the right side of the previous one  
            tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tempCube.transform.position= new Vector3(goTransform.position.x + i, goTransform.position.y, goTransform.position.z);
            //Get the recently instantiated cube Transform component  
            cubesTransform[i] = tempCube.GetComponent<Transform>();
            //Make the cube a child of this game object  
            cubesTransform[i].parent = goTransform;
        }

  
    }

    private void Update()
    {
        audioSources = GameObject.FindObjectsOfType<AudioSource>();
        if (audioSources.Length == 0)
        {
            return;
        }
        //Obtain the samples from the frequency bands of the attached AudioSource  
        Array.Clear(samples, 0, samples.Length);
        foreach (AudioSource aSource in audioSources)
        {
            float[] tmp = new float[samples.Length];
            aSource.GetSpectrumData(tmp, 0, FFTWindow.BlackmanHarris);
            for (int i = 0; i < tmp.Length; i++)
            {
                samples[i] += tmp[i];
            }
        }


        //For each sample  
        for (int i = 0; i < samples.Length; i++)
        {
            /*Set the cubePos Vector3 to the same value as the position of the corresponding 
             * cube. However, set it's Y element according to the current sample.*/
            cubePos.Set(cubesTransform[i].position.x, Mathf.Clamp(samples[i] * (30* i * i), 0, 30), cubesTransform[i].position.z);

            //If the new cubePos.y is greater than the current cube position  
            if (cubePos.y >= cubesTransform[i].position.y)
            {
                //Set the cube to the new Y position  
                cubesTransform[i].position = cubePos;
            }
            else
            {
                //The spectrum line is below the cube, make it fall  
                cubesTransform[i].position -= gravity;
            }


        }
    }
}