using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlickering : MonoBehaviour
{

    [Range(-1f, 1f)]
    public float minDeltaIntensity;

    [Range(-1f, 1f)]
    public float maxDeltaIntensity;

    [Range(-1f, 1f)]
    public float minDeltaRange;

    [Range(-1f, 1f)]
    public float maxDeltaRange;


    public float flickeringSpeed;
    private float flickeringTimer;

    private float targetIntensity;
    private float targetRange;

    private float initialIntensity;
    private float initialRange;

    private Light _light;

    // Use this for initialization
    private void Start()
    {
        _light = GetComponent<Light>();
        initialRange = _light.range;
        initialIntensity = _light.intensity;
    }

    // Update is called once per frame
    private void Update()
    {

        if (Time.time > flickeringTimer)
        {
            targetIntensity = initialIntensity + (Random.Range(minDeltaIntensity, maxDeltaIntensity) * initialIntensity);
            targetRange = initialRange + Random.Range(minDeltaRange, maxDeltaRange);
            flickeringTimer = Time.time + flickeringSpeed;
        }
        _light.intensity = Mathf.Lerp(_light.intensity, targetIntensity, Time.deltaTime);
        _light.range = Mathf.Lerp(_light.range, targetRange, Time.deltaTime);
    }
}
