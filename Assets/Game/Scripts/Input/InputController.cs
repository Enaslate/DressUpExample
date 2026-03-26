using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : IDisposable
{
    private readonly InputActions _actions;

    private bool _isDragging;

    public event Action<Vector2> OnTapPerformed;
    public event Action<Vector2> OnDragStarted;
    public event Action<Vector2> OnDragMoved;
    public event Action<Vector2> OnDragEnded;

    public InputController(InputActions actions)
    {
        _actions = actions;

        _actions.Player.Tap.performed += OnTap;
        _actions.Player.Touch.performed += OnTouchStarted;
        _actions.Player.Touch.canceled += OnTouchEnded;

        _actions.Player.PointerPosition.performed += OnPointerMoved;
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        if (_isDragging) return;

        Vector2 pos = _actions.Player.PointerPosition.ReadValue<Vector2>();
        OnTapPerformed?.Invoke(pos);
        Debug.Log("Tag performed");
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        if (_isDragging) return;
        _isDragging = true;

        Vector2 pos = _actions.Player.PointerPosition.ReadValue<Vector2>();
        OnDragStarted?.Invoke(pos);
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        if (!_isDragging) return;
        _isDragging = false;

        Vector2 pos = _actions.Player.PointerPosition.ReadValue<Vector2>();
        OnDragEnded?.Invoke(pos);
    }

    private void OnPointerMoved(InputAction.CallbackContext context)
    {
        if (!_isDragging) return;

        Vector2 pos = context.ReadValue<Vector2>();
        OnDragMoved?.Invoke(pos);
        Debug.Log("Drag at " + pos);
    }

    public void Dispose()
    {
        _actions.Player.Tap.performed -= OnTap;
        _actions.Player.Touch.performed -= OnTouchStarted;
        _actions.Player.Touch.canceled -= OnTouchEnded;
        _actions.Player.PointerPosition.performed -= OnPointerMoved;
    }
}