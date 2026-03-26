using DG.Tweening;
using System.Collections;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField] private Transform _hand;
    [SerializeField] private MakeupController _makeupController;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _chestPoint;
    [SerializeField] private Transform _facePoint;

    [SerializeField] private MakeupToolView _creamTool;
    [SerializeField] private MakeupToolView _eyeBrushTool;
    [SerializeField] private MakeupToolView _lipstickTool;
    [SerializeField] private MakeupToolView _blushBrushTool;

    [SerializeField] private Vector3 _offset = new Vector3(0.05f, -0.5f, 0);

    private InputController _inputController;
    private Vector3 _startPosition;
    private MakeupItemData _currentItem;
    private GameObject _currentItemObject;
    private HandState _state = HandState.Idle;
    private Tween _currentTween;
    private SpriteRenderer _currentItemRenderer;

    private void Awake()
    {
        _startPosition = _hand.position;
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

        Vector3 toolPos = _currentItemObject.transform.position + _offset;
        toolPos.z = _hand.position.z;
        _currentTween?.Kill();
        yield return _hand.DOMove(toolPos, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();

        Sequence disappearSequence = DOTween.Sequence();
        disappearSequence.Join(_currentItemObject.transform.DOScale(0, 0.2f).SetEase(Ease.InBack));
        if (_currentItemRenderer != null)
            disappearSequence.Join(_currentItemRenderer.DOFade(0, 0.2f));
        yield return disappearSequence.WaitForCompletion();
        _currentItemObject.SetActive(false);
        _currentItemObject.transform.localScale = Vector3.one;
        if (_currentItemRenderer != null)
            _currentItemRenderer.color = Color.white;

        var handTool = GetHandTool(type);
        if (handTool != null) handTool.gameObject.SetActive(true);

        Vector3 targetItem = new Vector3(itemPosition.x, itemPosition.y, _hand.position.z) + _offset;
        yield return _hand.DOMove(targetItem, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();

        if (type != MakeupType.Lipstick && type != MakeupType.Cream)
        {
            for (int i = 0; i < 3; i++)
            {
                float offsetX = Random.Range(-0.05f, 0.05f);
                yield return _hand.DOMoveX(_hand.position.x + offsetX, 0.05f).SetEase(Ease.InOutSine).WaitForCompletion();
                yield return _hand.DOMoveX(_hand.position.x - offsetX, 0.05f).SetEase(Ease.InOutSine).WaitForCompletion();
            }
        }

        Vector3 targetChest = new Vector3(_chestPoint.position.x, _chestPoint.position.y, _hand.position.z);
        yield return _hand.DOMove(targetChest, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();

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

        Vector3 toolPos = _currentItemObject.transform.position + _offset;
        toolPos.z = _hand.position.z;
        yield return _hand.DOMove(toolPos, 0.3f).SetEase(Ease.OutQuad).WaitForCompletion();

        var handTool = GetHandTool(_currentItem.Type);
        if (handTool != null) handTool.gameObject.SetActive(false);

        _currentItemObject.SetActive(true);
        Sequence appearSequence = DOTween.Sequence();
        appearSequence.Join(_currentItemObject.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack));
        if (_currentItemRenderer != null)
            appearSequence.Join(_currentItemRenderer.DOFade(1, 0.2f));
        yield return appearSequence.WaitForCompletion();

        Vector3 targetStart = new Vector3(_startPosition.x, _startPosition.y, _hand.position.z);
        yield return _hand.DOMove(targetStart, 0.3f).SetEase(Ease.InOutQuad).WaitForCompletion();

        SetAllToolsActive(false);

        _currentItem = null;
        _currentItemObject = null;
        _currentItemRenderer = null;
        _state = HandState.Idle;
    }

    private void OnDragMoved(Vector2 screenPos)
    {
        if (_state != HandState.Dragging) return;
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = _hand.position.z;
        _hand.position = worldPos;
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

    private IEnumerator AnimateApply()
    {
        Vector3 targetFace = new Vector3(
            _facePoint.position.x,
            _facePoint.position.y,
            _hand.position.z) + _offset;

        yield return _hand.DOMove(targetFace, 0.1f).SetEase(Ease.InOutSine).WaitForCompletion();

        for (int i = 0; i < 5; i++)
        {
            float offsetX = Random.Range(-0.03f, 0.03f);
            yield return _hand.DOMoveX(_hand.position.x + offsetX, 0.03f).SetEase(Ease.InOutSine).WaitForCompletion();
            yield return _hand.DOMoveX(_hand.position.x - offsetX, 0.03f).SetEase(Ease.InOutSine).WaitForCompletion();
        }

        _makeupController.Makeup(_currentItem);

        Vector3 targetChest = new Vector3(_chestPoint.position.x, _chestPoint.position.y, _hand.position.z);
        yield return _hand.DOMove(targetChest, 0.2f).SetEase(Ease.InOutSine).WaitForCompletion();

        StartCoroutine(ReturnCoroutine());
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

    private void OnDestroy()
    {
        _currentTween?.Kill();
        if (_inputController != null)
        {
            _inputController.OnTapPerformed -= OnTap;
            _inputController.OnDragMoved -= OnDragMoved;
            _inputController.OnDragEnded -= OnDragEnded;
        }
    }
}