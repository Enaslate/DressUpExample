using DG.Tweening;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField] private Transform _handTransform;
    [SerializeField] private Vector3 _offset = new Vector3(0.05f, -0.4f, 0);

    private Tween _currentTween;

    public void KillTween() => _currentTween?.Kill();

    public Tween MoveTo(Vector3 target, float duration = 0.3f, Ease ease = Ease.OutQuad, bool withOffset = false)
    {
        KillTween();
        
        var offset = withOffset
            ? _offset
            : Vector3.zero;

        var targetPos = new Vector3(target.x, target.y, _handTransform.position.z) + offset;
        _currentTween = _handTransform.DOMove(targetPos, duration).SetEase(ease);
        return _currentTween;
    }

    public Tween ShakeX(float amplitude, int count, float durationPerShake = 0.05f)
    {
        KillTween();
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < count; i++)
        {
            float offsetX = Random.Range(-amplitude, amplitude);
            sequence.Append(_handTransform.DOMoveX(_handTransform.position.x + offsetX, durationPerShake).SetEase(Ease.InOutSine));
            sequence.Append(_handTransform.DOMoveX(_handTransform.position.x - offsetX, durationPerShake).SetEase(Ease.InOutSine));
        }
        _currentTween = sequence;
        return sequence;
    }

    public Tween ApplyBrushStrokes(float amplitude, int count, float durationPerStroke = 0.03f)
    {
        KillTween();
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < count; i++)
        {
            float offsetX = Random.Range(-amplitude, amplitude);
            sequence.Append(_handTransform.DOMoveX(_handTransform.position.x + offsetX, durationPerStroke).SetEase(Ease.InOutSine));
            sequence.Append(_handTransform.DOMoveX(_handTransform.position.x - offsetX, durationPerStroke).SetEase(Ease.InOutSine));
        }
        _currentTween = sequence;
        return sequence;
    }
}