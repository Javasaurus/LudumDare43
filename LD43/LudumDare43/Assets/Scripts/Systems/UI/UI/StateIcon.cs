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
    public Sprite thinking;
    
    public Sprite lookingForFood;
    public Sprite lookingForWater;
    public Sprite lookingForWood;
    public Sprite lookingForGold;


    private float duration = 3f;
    private float timer;

    private Coroutine resetCoroutine;
    private SpriteRenderer _spriteRenderer;

    // Use this for initialization
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator waitToReset(float duration)
    {
        yield return new WaitForSeconds(duration);
        _spriteRenderer.enabled = false;
    }

    private void ScheduleReset(float duration)
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(waitToReset(duration));
    }

    public void SetLooking(WorldResource.ResourceType type)
    {
        Debug.Log("Now looking for " + type);
        _spriteRenderer.enabled = true;
        switch (type)
        {
            case WorldResource.ResourceType.FOOD:
                _spriteRenderer.sprite = lookingForFood;
                break;
            case WorldResource.ResourceType.GOLD:
                _spriteRenderer.sprite = lookingForGold;
                break;
            case WorldResource.ResourceType.WATER:
                _spriteRenderer.sprite = lookingForWater;
                break;
            case WorldResource.ResourceType.WOOD:
                _spriteRenderer.sprite = lookingForWood;
                break;
            default:
                setConfused();
                return;
        }
        ScheduleReset(4f);
    }

    public void SetGitty()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = gitty;
        ScheduleReset(4f);
    }

    public void SetHappy()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = happy;
        ScheduleReset(4f);
    }

    public void SetSad()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = sad;
        ScheduleReset(4f);
    }

    public void SetDepressed()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = depressed;
        ScheduleReset(4f);
    }

    public void SetSick()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = sick;
        ScheduleReset(4f);
    }

    public void setConfused()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = confused;
        ScheduleReset(4f);
    }

    public void SetThinking()
    {
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = thinking;
        ScheduleReset(4f);
    }
}
