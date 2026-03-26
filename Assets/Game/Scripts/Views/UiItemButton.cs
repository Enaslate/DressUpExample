using UnityEngine;
using UnityEngine.UI;

public class UIItemButton : MonoBehaviour
{
    [SerializeField] private Image _image;
    private MakeupItemData _itemData;
    private HandController _handController;

    public void Initialize(MakeupItemData data, HandController hand, GameObject tool)
    {
        _itemData = data;
        _handController = hand;
        _image.sprite = data.ItemSprite;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            var worldPos = transform.position;
            _handController.SelectItem(_itemData, worldPos, tool);
        });
    }
}