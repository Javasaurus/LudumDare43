using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRandomizer : MonoBehaviour
{

    public Sprite[] options;

    // Use this for initialization
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = options[Random.Range(0, options.Length)];
        GameObject.Destroy(this);
    }


}
