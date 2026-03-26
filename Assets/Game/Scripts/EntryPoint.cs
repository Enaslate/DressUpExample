using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private float _targetWidth = 10f;
    [SerializeField] private HandController _handController;
    [SerializeField] private Camera _camera;

    private InputActions _inputActions;
    private InputController _inputController;

    private void Awake()
    {
        ConfigureInput();
        _camera ??= Camera.main;
    }

    private void Start()
    {
        _handController.Setup(_inputController);

        if (_camera != null)
        {
            float aspect = (float)Screen.width / Screen.height;
            float targetHeight = _targetWidth / aspect;
            _camera.orthographicSize = targetHeight / 2;
        }
    }

    private void ConfigureInput()
    {
        _inputActions = new InputActions();
        _inputActions.Enable();
        _inputController = new InputController(_inputActions);
    }

    private void OnDestroy()
    {
        _inputController?.Dispose();
        _inputActions?.Disable();
    }
}