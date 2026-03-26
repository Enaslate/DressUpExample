using System.Collections.Generic;
using UnityEngine;

public class BookController : MonoBehaviour
{
    [SerializeField] private TabUiView[] _tabs;
    [SerializeField] private HandController _handController;
    [SerializeField] private MakeupLibrary _library;

    private TabUiView _currentTab;
    private Dictionary<MakeupType, List<MakeupItemData>> _itemsByType;

    private void Awake()
    {
        BuildDictionary();
    }

    private void Start()
    {
        if (_tabs.Length > 0)
            ChangeTab(_tabs[0]);
    }

    private void BuildDictionary()
    {
        _itemsByType = new Dictionary<MakeupType, List<MakeupItemData>>();
        foreach (var item in _library.allItems)
        {
            if (!_itemsByType.ContainsKey(item.Type))
                _itemsByType[item.Type] = new List<MakeupItemData>();
            _itemsByType[item.Type].Add(item);
        }
    }

    public void ChangeTab(TabUiView tab)
    {
        if (_currentTab == tab) return;

        if (_currentTab != null)
            _currentTab.SetActive(false);

        _currentTab = tab;
        _currentTab.SetActive(true);

        if (_itemsByType.TryGetValue(tab.Type, out var items))
        {
            _currentTab.PopulateContent(items.ToArray(), _handController);
        }
        else
        {
            _currentTab.ClearContent();
        }
    }
}