using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField] private HandController _handController;

    private InputActions _inputActions;
    private InputController _inputController;

    private void Awake()
    {
        ConfigureInput();
    }

    private void Start()
    {
        _handController.Setup(_inputController);
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