using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimationSynchronizer : MonoBehaviour
{


    public string pathToTexture;
    public SpriteRenderer referenceRenderer;
    private SpriteRenderer _spriteRenderer;
    private Sprite[] sprites;
    private Dictionary<string, Sprite> spriteMap;

    public void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        sprites = Resources.LoadAll<Sprite>(pathToTexture);
        spriteMap = new Dictionary<string, Sprite>();
        foreach (Sprite sprite in sprites)
        {
            spriteMap.Add(parseIdentifier(sprite.name), sprite);
        }
    }

    public void LateUpdate()
    {
        Sprite tmp = sprites[0];
        spriteMap.TryGetValue(parseIdentifier(referenceRenderer.sprite.name), out tmp);
        _spriteRenderer.sprite = tmp;
    }

    private string parseIdentifier(string spriteName)
    {
        string[] identifiers = spriteName.Split('_');
        return identifiers[identifiers.Length - 1];
    }
}
