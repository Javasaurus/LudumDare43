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
    public GameObject target;

    private void OnEnable()
    {
        originalPos = target.transform.localPosition;
    }

    private void OnDisable()
    {
        target.transform.localPosition = originalPos;
    }

    private void Update()
    {
        if (Time.time > timer)
        {
            targetPos = originalPos + Random.insideUnitSphere * amplitude;
            timer = Time.time + (1 / frequency);
        }
        target.transform.localPosition = Vector3.Lerp(target.transform.localPosition, targetPos, Time.deltaTime);
    }
}
