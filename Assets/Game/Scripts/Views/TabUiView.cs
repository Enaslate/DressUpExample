using UnityEngine;
using UnityEngine.UI;

public class TabUiView : MonoBehaviour
{
    [SerializeField] private MakeupType _type;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private Sprite _toolSprite;
    [SerializeField] private RectTransform _contentPanel;
    [SerializeField] private UIItemButton _itemPrefab;
    [SerializeField] private Image _toolImage;

    public MakeupType Type => _type;

    public void SetActive(bool active)
    {
        _image.sprite = active ? _activeSprite : _defaultSprite;

        if (_toolImage != null)
        {
            if (active && _toolSprite != null)
            {
                _toolImage.sprite = _toolSprite;
                _toolImage.gameObject.SetActive(active);
            }
            else
            {
                _toolImage.gameObject.SetActive(false);
            }
        }

        _contentPanel.gameObject.SetActive(active);
    }

    public void ClearContent()
    {
        foreach (Transform child in _contentPanel)
            Destroy(child.gameObject);
    }

    public void PopulateContent(MakeupItemData[] items, HandController handController)
    {
        ClearContent();
        foreach (var item in items)
        {
            var btnObj = Instantiate(_itemPrefab, _contentPanel);
            var uiButton = btnObj.GetComponent<UIItemButton>();
            var tool = item.Type == MakeupType.Lipstick
                ? uiButton.gameObject
                : _toolImage.gameObject;
            uiButton.Initialize(item, handController, tool);
        }
    }
}