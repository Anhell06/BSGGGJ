using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Единственная ответственность: читать новую Input System (InputSystem_Actions)
/// и предоставлять данные через IMovementInput.
/// </summary>
public class InputSystemMovementInput : MonoBehaviour, IMovementInput
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private Vector2 _moveValue;
    private bool _jumpPressedThisFrame;

    public Vector3 MoveDirection => new Vector3(_moveValue.x, 0f, _moveValue.y);

    public bool JumpPressedThisFrame => _jumpPressedThisFrame;

    private void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogWarning("[InputSystemMovementInput] InputActionAsset не назначен.");
            return;
        }

        InputActionMap map = inputActions.FindActionMap(actionMapName);
        if (map == null)
        {
            Debug.LogWarning($"[InputSystemMovementInput] Action map '{actionMapName}' не найден.");
            return;
        }

        _moveAction = map.FindAction(moveActionName);
        _jumpAction = map.FindAction(jumpActionName);

        if (_moveAction == null)
            Debug.LogWarning($"[InputSystemMovementInput] Action '{moveActionName}' не найдена.");
        if (_jumpAction == null)
            Debug.LogWarning($"[InputSystemMovementInput] Action '{jumpActionName}' не найдена.");

        if (_moveAction != null)
            _moveAction.Enable();
        if (_jumpAction != null)
            _jumpAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction?.Disable();
        _jumpAction?.Disable();
    }

    private void Update()
    {
        if (_moveAction != null)
            _moveValue = _moveAction.ReadValue<Vector2>();
        if (_jumpAction != null)
            _jumpPressedThisFrame = _jumpAction.WasPressedThisFrame();
    }
}
