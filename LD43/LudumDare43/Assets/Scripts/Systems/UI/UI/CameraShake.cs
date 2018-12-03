using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float amplitude = 0.7f;
    public float frequency;
    private float timer;
    private Vector3 targetPos;
    public float decreaseFactor = 1.0f;
    private Vector3 originalPos;


    private void OnEnable()
    {
        originalPos = Camera.main.transform.localPosition;
    }

    private void OnDisable()
    {
        Camera.main.transform.localPosition = originalPos;
    }

    private void Update()
    {
        if (Time.time > timer)
        {
            targetPos = originalPos + Random.insideUnitSphere * amplitude;
            timer = Time.time + (1 / frequency);
        }
        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, targetPos, Time.deltaTime);
    }
}
