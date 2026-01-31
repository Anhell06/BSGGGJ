using System;
using UnityEngine;

/// <summary>
/// Единственная ответственность: читать старый стандартный Input (Input Manager)
/// и предоставлять данные через IMovementInput.
/// </summary>
[Obsolete("Используйте InputSystemMovementInput с новой системой ввода.")]
public class StandardMovementInput : MonoBehaviour, IMovementInput
{
    [SerializeField] private string horizontalAxisName = "Horizontal";
    [SerializeField] private string verticalAxisName = "Vertical";
    [SerializeField] private string jumpButtonName = "Jump";

    private float _horizontal;
    private float _vertical;
    private bool _jumpPressedThisFrame;

    public Vector3 MoveDirection => new Vector3(_horizontal, 0f, _vertical);

    public bool JumpPressedThisFrame => _jumpPressedThisFrame;

    private void Update()
    {
        _horizontal = Input.GetAxisRaw(horizontalAxisName);
        _vertical = Input.GetAxisRaw(verticalAxisName);
        _jumpPressedThisFrame = Input.GetButtonDown(jumpButtonName);
    }
}
