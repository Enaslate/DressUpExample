using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private HandView _handView;
    [SerializeField] private MakeupController _makeupController;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _chestPoint;
    [SerializeField] private Transform _facePoint;

    [SerializeField] private MakeupToolView _creamTool;
    [SerializeField] private MakeupToolView _eyeBrushTool;
    [SerializeField] private MakeupToolView _lipstickTool;
    [SerializeField] private MakeupToolView _blushBrushTool;

    private InputController _inputController;
    private Vector3 _startPosition;
    private MakeupItemData _currentItem;
    private GameObject _currentItemObject;
    private HandState _state = HandState.Idle;
    private SpriteRenderer _currentItemRenderer;

    private void Awake()
    {
        _startPosition = _handView.transform.position;
        SetAllToolsActive(false);
    }

    public void Setup(InputController inputController)
    {
        _inputController = inputController;
        _inputController.OnTapPerformed += OnTap;
        _inputController.OnDragMoved += OnDragMoved;
        _inputController.OnDragEnded += OnDragEnded;
    }

    public void SelectItem(MakeupItemData itemData, Vector3 itemPosition, GameObject itemObject)
    {
        if (_state != HandState.Idle) return;
        _currentItem = itemData;
        _currentItemObject = itemObject;
        _currentItemRenderer = itemObject.GetComponent<SpriteRenderer>();
        _state = HandState.Taking;

        var tool = itemData.Type switch
        {
            MakeupType.Cream => _creamTool,
            MakeupType.Eyeshadow => _eyeBrushTool,
            MakeupType.Lipstick => _lipstickTool,
            MakeupType.Blush => _blushBrushTool,
            _ => null
        };
        if (tool != null)
        {
            tool.Setup(itemData);
        }

        StartCoroutine(AnimateTakeItem(itemPosition, itemData.Type));
    }

    private IEnumerator AnimateTakeItem(Vector3 itemPosition, MakeupType type)
    {
        if (_currentItemObject == null)
        {
            _state = HandState.Dragging;
            yield break;
        }

        _currentItemRenderer ??= _currentItemObject.GetComponent<SpriteRenderer>();

        yield return _handView.MoveTo(_currentItemObject.transform.position, 0.3f, withOffset: true).WaitForCompletion();

        yield return HideWorldTool();

        var handTool = GetHandTool(type);
        if (handTool != null) 
            handTool.gameObject.SetActive(true);

        yield return _handView.MoveTo(itemPosition, 0.3f, withOffset: true).WaitForCompletion();

        if (type != MakeupType.Lipstick && type != MakeupType.Cream)
        {
            yield return _handView.ShakeX(0.05f, 3).WaitForCompletion();
        }

        yield return _handView.MoveTo(_chestPoint.position, 0.3f).WaitForCompletion();

        _state = HandState.Dragging;
    }

    private IEnumerator ReturnCoroutine()
    {
        _state = HandState.Returning;

        if (_currentItemObject == null || _currentItem == null)
        {
            _state = HandState.Idle;
            yield break;
        }

        _currentItemRenderer ??= _currentItemObject.GetComponent<SpriteRenderer>();

        yield return _handView.MoveTo(_currentItemObject.transform.position, 0.3f, withOffset: true).WaitForCompletion();

        var handTool = GetHandTool(_currentItem.Type);
        if (handTool != null)
            handTool.gameObject.SetActive(false);

        yield return ShowWorldTool();

        yield return _handView.MoveTo(_startPosition, 0.3f, Ease.InOutQuad).WaitForCompletion();

        SetAllToolsActive(false);

        _currentItem = null;
        _currentItemObject = null;
        _currentItemRenderer = null;
        _state = HandState.Idle;
    }

    private IEnumerator AnimateApply()
    {
        yield return _handView.MoveTo(_facePoint.position, 0.1f, Ease.InOutSine, true).WaitForCompletion();

        yield return _handView.ApplyBrushStrokes(0.03f, 5).WaitForCompletion();

        _makeupController.Makeup(_currentItem);

        StartCoroutine(ReturnCoroutine());
    }

    private void OnDragMoved(Vector2 screenPos)
    {
        if (_state != HandState.Dragging) return;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = _handView.transform.position.z;
        _handView.transform.position = worldPos;
    }

    private void OnDragEnded(Vector2 screenPos)
    {
        if (_state != HandState.Dragging) return;
        Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(screenPos);
        if (_makeupController.IsInZone(worldPoint) && _currentItem != null)
        {
            _state = HandState.Applying;
            StartCoroutine(AnimateApply());
        }
    }

    private void OnTap(Vector2 screenPos)
    {
        var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        var hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            if (hit.TryGetComponent<LoofahView>(out var loofah))
            {
                loofah.Clear(_makeupController);
                return;
            }

            if (hit.TryGetComponent<MakeupToolView>(out var view))
            {
                SelectItem(view.GetData(), hit.transform.position, view.gameObject);
                Debug.Log("Item selected via tap");
            }
        }
    }

    private void SetAllToolsActive(bool active)
    {
        if (_creamTool != null) _creamTool.gameObject.SetActive(active);
        if (_eyeBrushTool != null) _eyeBrushTool.gameObject.SetActive(active);
        if (_lipstickTool != null) _lipstickTool.gameObject.SetActive(active);
        if (_blushBrushTool != null) _blushBrushTool.gameObject.SetActive(active);
    }

    private MakeupToolView GetHandTool(MakeupType type)
    {
        return type switch
        {
            MakeupType.Cream => _creamTool,
            MakeupType.Eyeshadow => _eyeBrushTool,
            MakeupType.Lipstick => _lipstickTool,
            MakeupType.Blush => _blushBrushTool,
            _ => null
        };
    }

    public IEnumerator ShowWorldTool()
    {
        _currentItemObject.SetActive(true);
        Sequence appearSequence = DOTween.Sequence();
        appearSequence.Join(_currentItemObject.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        if (_currentItemRenderer != null)
            appearSequence.Join(_currentItemRenderer.DOFade(1, 0.2f));
        yield return appearSequence.WaitForCompletion();
    }

    public IEnumerator HideWorldTool()
    {
        Sequence disappearSequence = DOTween.Sequence();
        disappearSequence.Join(_currentItemObject.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        if (_currentItemRenderer != null)
            disappearSequence.Join(_currentItemRenderer.DOFade(0, 0.2f));
        yield return disappearSequence.WaitForCompletion();
        _currentItemObject.SetActive(false);
        _currentItemObject.transform.localScale = Vector3.one;
        if (_currentItemRenderer != null)
            _currentItemRenderer.color = Color.white;
    }

    private void OnDestroy()
    {
        _handView?.KillTween();
        if (_inputController != null)
        {
            _inputController.OnTapPerformed -= OnTap;
            _inputController.OnDragMoved -= OnDragMoved;
            _inputController.OnDragEnded -= OnDragEnded;
        }
    }
}