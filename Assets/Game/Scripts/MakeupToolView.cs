using UnityEngine;

public class MakeupToolView : MonoBehaviour
{
    [SerializeField] private MakeupItemData _data;
    [SerializeField] private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer ??= GetComponent<SpriteRenderer>();
    }

    public void Setup(MakeupItemData data)
    {
        _data = data;
        if (data.Type == MakeupType.Lipstick)
            _renderer.sprite = _data.ItemSprite;

        gameObject.SetActive(true);
    }

    public MakeupItemData GetData()
    {
        return _data;
    }
}