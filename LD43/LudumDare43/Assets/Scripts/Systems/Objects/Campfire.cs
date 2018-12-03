using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour {

    public GameObject fire;

    public void SetBurning()
    {
        fire.SetActive(true);
    }

    public void Extinguish()
    {
        fire.SetActive(false);
    }

}
