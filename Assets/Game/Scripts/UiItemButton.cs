using UnityEngine;
using UnityEngine.UI;

public class UIItemButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    private MakeupItemData _itemData;
    private HandController _handController;

    public void Initialize(MakeupItemData data, HandController hand)
    {
        _itemData = data;
        _handController = hand;
        _iconImage.sprite = data.ItemSprite;
        GetComponent<Button>().onClick.AddListener(() => _handController.SelectItem(_itemData));
    }
}