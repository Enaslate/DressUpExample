using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    [SerializeField] private MakeupController _makeupController;
    [SerializeField] private Camera _mainCamera;

    private InputController _inputController;
    private Vector3 _startPosition;

    [SerializeField] private MakeupItemData _currentItem;

    private void Awake()
    {
        _startPosition = _hand.position;
    }

    public void Setup(InputController inputController)
    {
        _inputController = inputController;

        _inputController.OnTapPerformed += OnTap;
        _inputController.OnDragMoved += OnDragMoved;
        _inputController.OnDragEnded += OnDragEnded;
    }

    private void OnTap(Vector2 screenPos)
    {
        var worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        var hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            var view = hit.GetComponent<MakeupItemView>();
            if (view != null)
            {
                var data = view.GetData();
                _currentItem = data;
                Debug.Log("Item selected");
            }
        }
    }

    private void OnDragMoved(Vector2 screenPos)
    {
        if (_currentItem == null) return;

        var worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = _hand.position.z;
        _hand.position = worldPos;
    }

    private void OnDragEnded(Vector2 screenPos)
    {
        var worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        if (_makeupController.IsInZone(worldPoint) && _currentItem != null)
        {
            Debug.Log("Release in face zone");
            _makeupController.Makeup(_currentItem);
            _hand.position = _startPosition;
        }
        else
        {
            _hand.position = _startPosition;
        }

        _currentItem = null;
    }

    private void OnDestroy()
    {
        if (_inputController != null)
        {
            _inputController.OnTapPerformed -= OnTap;
            _inputController.OnDragMoved -= OnDragMoved;
            _inputController.OnDragEnded -= OnDragEnded;
        }
    }
}