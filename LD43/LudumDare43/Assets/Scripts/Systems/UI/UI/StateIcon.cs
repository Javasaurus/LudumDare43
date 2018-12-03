using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StateIcon : MonoBehaviour
{

    public Sprite gitty;
    public Sprite happy;
    public Sprite sad;
    public Sprite depressed;
    public Sprite sick;
    public Sprite confused;

    private float duration = 3f;
    private float timer;

    private Coroutine resetCoroutine;
    private SpriteRenderer _spriteRenderer;

    // Use this for initialization
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator waitToReset()
    {
        yield return new WaitForSeconds(duration);
        _spriteRenderer.enabled = false;
    }

    private void ScheduleReset()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(waitToReset());
    }

    public void SetGitty()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = gitty;
        ScheduleReset();
    }

    public void SetHappy()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = happy;
        ScheduleReset();
    }

    public void SetSad()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = sad;
        ScheduleReset();
    }

    public void SetDepressed()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = depressed;
        ScheduleReset();
    }

    public void SetSick()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = sick;
        ScheduleReset();
    }

    public void setConfused()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = confused;
        ScheduleReset();
    }
}
