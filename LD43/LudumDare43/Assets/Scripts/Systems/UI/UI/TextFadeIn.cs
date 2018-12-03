using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFadeIn : MonoBehaviour
{
    public Color targetColor = Color.white;
    public Color startingColor;
    private TextMeshProUGUI _text;
    public int characterIndex;
    public float charactersPerSecond;
    private float characterTimer;
    private string previousText;

    public bool skip;

    public bool isFading;

    private List<Coroutine> coroutines;

    public void Awake()
    {
        coroutines = new List<Coroutine>();
    }

    public void OnEnable()
    {
        StopAllCoroutines();
        coroutines.Clear();
        _text = GetComponent<TextMeshProUGUI>();
        startingColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0);
        characterIndex = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            skip = true;
        }


        if (previousText != _text.text)
        {
            characterIndex = 0;
            previousText = _text.text;
            foreach (Coroutine routine in coroutines)
            {
                StopCoroutine(routine);
            }
            coroutines.Clear();
        }
        else if (Time.time > characterTimer & characterIndex < _text.textInfo.characterCount)
        {
            coroutines.Add(StartCoroutine(lerpColor(characterIndex, startingColor)));
            characterIndex++;
            characterTimer = Time.time + (1 / charactersPerSecond);
        }
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        if (skip)
        {
            characterIndex = _text.textInfo.characterCount;
            foreach (Coroutine routine in coroutines)
            {
                StopCoroutine(routine);
            }
            coroutines.Clear();
            for (int i = 0; i < characterIndex; i++)
            {
                SetCharColor(i, targetColor);
            }
            skip = false;
        }
        isFading = characterIndex == _text.textInfo.characterCount;
    }

    private void SetCharColor(int characterIndex, Color newColor)
    {
        int meshIndex = _text.textInfo.characterInfo[characterIndex].materialReferenceIndex;
        int vertexIndex = _text.textInfo.characterInfo[characterIndex].vertexIndex;
        Color32[] vertexColors = _text.textInfo.meshInfo[meshIndex].colors32;
        vertexColors[vertexIndex + 0] = newColor;
        vertexColors[vertexIndex + 1] = newColor;
        vertexColors[vertexIndex + 2] = newColor;
        vertexColors[vertexIndex + 3] = newColor;
    }

    private IEnumerator lerpColor(int characterIndex, Color color)
    {
        while (color.a < 255f)
        {
            color = new Color(color.r, color.g, color.b, color.a + Time.deltaTime * 5f);
            SetCharColor(characterIndex, color);
            yield return null;
        }
        yield return null;
    }

}