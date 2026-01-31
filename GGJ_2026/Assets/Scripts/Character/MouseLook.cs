using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Единственная ответственность: поворачивать объект(ы) по вводу Look (мышь / правый стик).
/// Горизонтальный поворот — тело персонажа, вертикальный — опционально камера.
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Поворот по горизонтали (обычно тело персонажа). Если не задан — используется этот объект.")]
    [SerializeField] private Transform horizontalTransform;
    [Tooltip("Поворот по вертикали (камера или голова). Если не задан — только горизонталь.")]
    [SerializeField] private Transform verticalTransform;

    [Header("Input")]
    [SerializeField] private InputActionReference lookActionReference;
    [SerializeField] private float sensitivityX = 0.1f;
    [SerializeField] private float sensitivityY = 0.1f;

    [Header("Vertical Look")]
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    private float _yaw;
    private float _pitch;

    private void Awake()
    {
        if (horizontalTransform == null)
            horizontalTransform = transform;
        _yaw = horizontalTransform.eulerAngles.y;
        if (verticalTransform != null)
            _pitch = verticalTransform.localEulerAngles.x;
    }

    private void OnEnable()
    {
        lookActionReference?.action?.Enable();
    }

    private void OnDisable()
    {
        lookActionReference?.action?.Disable();
    }

    private void Update()
    {
        if (lookActionReference?.action == null) return;

        Vector2 look = lookActionReference.action.ReadValue<Vector2>();
        _yaw += look.x * sensitivityX;
        _pitch -= look.y * sensitivityY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        horizontalTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        if (verticalTransform != null)
            verticalTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}
