using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    [SerializeField] private Collider2D _faceZone;
    [SerializeField] private Camera _mainCamera;

    private InputController _inputController;
    private Vector3 _startPosition;

    private void Awake()
    {
        _startPosition = _hand.position;
    }

    public void Setup(InputController inputController)
    {
        _inputController = inputController;

        _inputController.OnDragStarted += OnDragStarted;
        _inputController.OnDragMoved += OnDragMoved;
        _inputController.OnDragEnded += OnDragEnded;
    }

    private void OnDragStarted(Vector2 screenPos)
    {
    }

    private void OnDragMoved(Vector2 screenPos)
    {
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = _hand.position.z;
        _hand.position = worldPos;
    }

    private void OnDragEnded(Vector2 screenPos)
    {
        Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        if (_faceZone.OverlapPoint(worldPoint))
        {
            Debug.Log("Release in face zone");
        }
        else
        {
            _hand.position = _startPosition;
        }
    }

    private void OnDestroy()
    {
        if (_inputController != null)
        {
            _inputController.OnDragStarted -= OnDragStarted;
            _inputController.OnDragMoved -= OnDragMoved;
            _inputController.OnDragEnded -= OnDragEnded;
        }
    }
}