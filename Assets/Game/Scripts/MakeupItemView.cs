using UnityEngine;

public class MakeupItemView : MonoBehaviour
{
    [SerializeField] private MakeupItemData _data;
    [SerializeField] private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer ??= GetComponent<SpriteRenderer>();

        if (_data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _renderer.sprite = _data.ItemSprite;
    }

    public void Setup(MakeupItemData data)
    {
        _data = data;
        _renderer.sprite = _data.ItemSprite;
        gameObject.SetActive(true);
    }

    public MakeupItemData GetData()
    {
        return _data;
    }
}