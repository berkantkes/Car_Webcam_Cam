using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Banner : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> _bannerSpriteRenderers;
    
    private bool _isAttachedSprite = false;
    public bool IsAttachedSprite => _isAttachedSprite;

    public void SetSprites(Sprite sprite)
    {
        foreach (var bannerSpriteRenderer in _bannerSpriteRenderers)
        {
            bannerSpriteRenderer.sprite = sprite;
        }

        _isAttachedSprite = true;
    }
}
