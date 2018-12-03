using System.Collections;
using UnityEngine;

public class UIEffects : MonoBehaviour
{

    private Coroutine scaleRoutine;
    private Vector3 originalScale;

    public void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnEnable()
    {
        if (scaleRoutine == null)
        {
            scaleRoutine = StartCoroutine(ScaleOverTime(transform));
        }
    }

    public void OnDisable()
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }
    }

    private IEnumerator ScaleOverTime(Transform transform)
    {

        Vector3 originalScale = Vector3.zero;
        Vector3 destinationScale = this.originalScale;
        float time = 0.2f;
        float currentTime = 0.0f;

        while (currentTime <= time)
        {
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = destinationScale;
        scaleRoutine = null;

    }


}
