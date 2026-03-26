using UnityEngine;
using UnityEngine.UI;

public class TabUiView : MonoBehaviour
{
    [SerializeField] private MakeupType _type;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _activeSprite;
    [SerializeField] private RectTransform _contentPanel;
    [SerializeField] private UIItemButton _itemPrefab;

    public MakeupType Type => _type;

    public void SetActive(bool active)
    {
        _image.sprite = active ? _activeSprite : _defaultSprite;
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
            uiButton.Initialize(item, handController);
        }
    }
}