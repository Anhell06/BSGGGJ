using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsBasedMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Force: ускорение с учётом массы. VelocityChange: мгновенная установка скорости (игнорирует массу).")]
    [SerializeField] private ForceMode forceMode = ForceMode.Force;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private LayerMask groundMask = ~0;
    [Tooltip("Длина луча вниз от точки проверки (должна доставать до земли при pivot в центре).")]
    [SerializeField] private float groundCheckDistance = 1.2f;
    [SerializeField] private Vector3 groundCheckOffset = Vector3.zero;

    [Header("Склоны и ступеньки")]
    [Tooltip("Макс. угол склона в градусах, по которому можно идти (0 = только пол, 90 = любая стена).")]
    [SerializeField, Range(0f, 85f)] private float maxSlopeAngle = 50f;
    [Tooltip("Высота ступеньки, на которую можно подняться.")]
    [SerializeField] private float stepHeight = 0.35f;
    [Tooltip("Дистанция проверки ступеньки вперёд.")]
    [SerializeField] private float stepCheckDistance = 0.3f;

    [Header("Debug (Gizmo)")]
    [SerializeField] private bool showGroundCheckGizmo = true;
    [SerializeField] private float groundCheckGizmoSphereRadius = 0.05f;

    private Rigidbody _rigidbody;
    private IMovementInput _input;
    private bool _shouldJumpThisFixedFrame;
    private RaycastHit _groundHit;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _input = GetComponent<IMovementInput>();

        // Не даём персонажу заваливаться от сил — замораживаем наклон (вращение X и Z).
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        if (_input == null)
            Debug.LogWarning($"[PhysicsBasedMovement] На {gameObject.name} не найден компонент IMovementInput. Добавьте InputSystemMovementInput.");
    }

    private void Update()
    {
        if (_input != null && _input.JumpPressedThisFrame)
            _shouldJumpThisFixedFrame = true;
    }

    private void FixedUpdate()
    {
        if (_input == null) return;

        bool grounded = GetGroundHit(out _groundHit);
        bool slopeOk = grounded && _groundHit.normal.sqrMagnitude > 0.01f
            && Vector3.Angle(Vector3.up, _groundHit.normal) <= maxSlopeAngle;

        Vector3 moveDir = _input.MoveDirection;
        if (moveDir.sqrMagnitude > 0.01f)
        {
            moveDir.Normalize();
            Vector3 worldDir = transform.TransformDirection(moveDir);
            worldDir.y = 0f;
            worldDir.Normalize();

            TryStepUp(worldDir);

            Vector3 slopeDir = Vector3.ProjectOnPlane(worldDir, _groundHit.normal).normalized;
            if (slopeDir.sqrMagnitude < 0.01f)
                slopeDir = worldDir;

            switch (forceMode)
            {
                case ForceMode.Force:
                    _rigidbody.AddForce(slopeDir * (moveSpeed * _rigidbody.mass));
                    ClampVelocity(moveSpeed);
                    break;
                case ForceMode.Acceleration:
                    _rigidbody.AddForce(slopeDir * moveSpeed, ForceMode.Acceleration);
                    ClampVelocity(moveSpeed);
                    break;
                case ForceMode.Impulse:
                    _rigidbody.AddForce(slopeDir * (moveSpeed * _rigidbody.mass), ForceMode.Impulse);
                    break;
                case ForceMode.VelocityChange:
                    _rigidbody.linearVelocity = slopeDir * moveSpeed;
                    break;
            }
        }

        if (_shouldJumpThisFixedFrame && grounded && slopeOk)
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        _shouldJumpThisFixedFrame = false;
    }

    private void ClampVelocity(float maxSpeed)
    {
        Vector3 vel = _rigidbody.linearVelocity;
        if (vel.sqrMagnitude > maxSpeed * maxSpeed)
            _rigidbody.linearVelocity = vel.normalized * maxSpeed;
    }

    private bool GetGroundHit(out RaycastHit hit)
    {
        Vector3 origin = transform.position + groundCheckOffset;

        if (Physics.SphereCast(origin, 0.3f, Vector3.down, out hit, groundCheckDistance, groundMask))
            return true;
        hit = default;
        return false;
    }

    private void TryStepUp(Vector3 worldDir)
    {
        Vector3 origin = transform.position + groundCheckOffset;
        float ourGroundY = _groundHit.point.y;

        Vector3 stepStart = origin + worldDir * stepCheckDistance;
        if (!Physics.Raycast(stepStart, Vector3.down, out RaycastHit stepStartHit, groundCheckDistance * 2f, groundMask))
            return;
        float stepGroundY = stepStartHit.point.y;
        if (stepGroundY <= ourGroundY + 0.05f)
            return;

        float stepUp = stepGroundY - ourGroundY;
        if (stepUp > stepHeight)
            return;

        Vector3 stepTop = origin + Vector3.up * stepHeight + worldDir * stepCheckDistance;
        if (!Physics.Raycast(stepTop, Vector3.down, out RaycastHit stepHit, stepHeight + groundCheckDistance, groundMask))
            return;
        if (Vector3.Angle(Vector3.up, stepHit.normal) > maxSlopeAngle)
            return;
        _rigidbody.MovePosition(_rigidbody.position + Vector3.up * stepUp);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!showGroundCheckGizmo) return;

        Vector3 origin = transform.position + groundCheckOffset;
        Vector3 end = origin + Vector3.down * groundCheckDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, end);
        Gizmos.DrawWireSphere(origin, groundCheckGizmoSphereRadius);
        Gizmos.DrawWireSphere(end, groundCheckGizmoSphereRadius);
    }
#endif
}
