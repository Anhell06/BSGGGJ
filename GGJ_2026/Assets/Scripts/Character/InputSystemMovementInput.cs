using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputSystemMovementInput : MonoBehaviour, IMovementInput
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Player";
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string jumpActionName = "Jump";
    [FormerlySerializedAs("photoActionName")]
    [SerializeField] private string makePhotoActionName = "MakePhoto";
    [SerializeField] private string switchCameraActionName = "SwitchPhotoCamera";

    public Action OnPhotoMaked;
    public Action OnCameraSwitched;

    private InputAction _moveAction;
    private InputAction _makePhotoAction;
    private InputAction _switchCameraAction;
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
        _makePhotoAction = map.FindAction(makePhotoActionName);
        _switchCameraAction = map.FindAction(switchCameraActionName);

        if (_moveAction == null)
            Debug.LogWarning($"[InputSystemMovementInput] Action '{moveActionName}' не найдена.");
        if (_jumpAction == null)
            Debug.LogWarning($"[InputSystemMovementInput] Action '{jumpActionName}' не найдена.");
        if (_makePhotoAction == null)
            Debug.LogWarning($"[InputSystemMovementInput] Action '{makePhotoActionName}' не найдена.");
        if (_switchCameraAction == null)
            Debug.LogWarning($"[InputSystemMovementInput] Action '{_switchCameraAction}' не найдена.");

        if (_moveAction != null)
            _moveAction.Enable();
        if (_jumpAction != null)
            _jumpAction.Enable();
        if (_makePhotoAction != null)
            _makePhotoAction.Enable();
        if (_switchCameraAction != null)
            _switchCameraAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction?.Disable();
        _jumpAction?.Disable();
        _makePhotoAction?.Disable();
        _switchCameraAction?.Disable();
    }

    private void Update()
    {
        if (_moveAction != null)
            _moveValue = _moveAction.ReadValue<Vector2>();
        if (_jumpAction != null)
            _jumpPressedThisFrame = _jumpAction.WasPressedThisFrame();
        if (_makePhotoAction != null && _makePhotoAction.WasPressedThisFrame())
        {
            OnPhotoMaked?.Invoke();
        }
        if (_switchCameraAction != null && _switchCameraAction.WasPressedThisFrame())
        {
            OnCameraSwitched?.Invoke();
        }
    }
}
