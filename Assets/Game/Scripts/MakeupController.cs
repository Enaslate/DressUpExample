using UnityEngine;

public class MakeupController : MonoBehaviour
{
    [SerializeField] private Collider2D _makeupZone;
    [SerializeField] private SpriteRenderer _eyeshadowRenderer;
    [SerializeField] private SpriteRenderer _mouthRenderer;
    [SerializeField] private SpriteRenderer _blushRenderer;
    [SerializeField] private SpriteRenderer _acneRenderer;

    public void Makeup(MakeupItemData data)
    {
        var renderer = data.Type switch
        {
            MakeupType.Blush => _blushRenderer,
            MakeupType.Eyeshadow => _eyeshadowRenderer,
            MakeupType.Lipstick => _mouthRenderer,
            MakeupType.Cream => _acneRenderer,
            _ => null
        };

        if (renderer == null)
        {
            Debug.LogError($"Error get renderer by type: {data.Type}");
            return;
        }

        renderer.sprite = data.ResultSprite;
    }

    public bool IsInZone(Vector3 worldPoint)
    {
        return _makeupZone.OverlapPoint(worldPoint);
    }

    public void Clear()
    {
        _eyeshadowRenderer.sprite = null;
        _mouthRenderer.sprite = null;
        _eyeshadowRenderer.sprite = null;
        _acneRenderer.gameObject.SetActive(true);
    }
}