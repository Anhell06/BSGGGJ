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
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private Vector3 groundCheckOffset = Vector3.zero;

    [Header("Debug (Gizmo)")]
    [SerializeField] private bool showGroundCheckGizmo = true;
    [SerializeField] private float groundCheckGizmoSphereRadius = 0.05f;

    private Rigidbody _rigidbody;
    private IMovementInput _input;
    private bool _shouldJumpThisFixedFrame;

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

        Vector3 moveDir = _input.MoveDirection;
        if (moveDir.sqrMagnitude > 0.01f)
        {
            moveDir.Normalize();
            Vector3 worldDir = transform.TransformDirection(moveDir);
            worldDir.y = 0f;
            worldDir.Normalize();

            switch (forceMode)
            {
                case ForceMode.Force:
                    _rigidbody.AddForce(worldDir * (moveSpeed * _rigidbody.mass));
                    ClampHorizontalVelocity(moveSpeed);
                    break;
                case ForceMode.Acceleration:
                    _rigidbody.AddForce(worldDir * moveSpeed, ForceMode.Acceleration);
                    ClampHorizontalVelocity(moveSpeed);
                    break;
                case ForceMode.Impulse:
                    _rigidbody.AddForce(worldDir * (moveSpeed * _rigidbody.mass), ForceMode.Impulse);
                    break;
                case ForceMode.VelocityChange:
                    Vector3 vel = _rigidbody.linearVelocity;
                    vel.x = worldDir.x * moveSpeed;
                    vel.z = worldDir.z * moveSpeed;
                    _rigidbody.linearVelocity = vel;
                    break;
            }
        }

        if (_shouldJumpThisFixedFrame && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        _shouldJumpThisFixedFrame = false;
    }

    private void ClampHorizontalVelocity(float maxSpeed)
    {
        Vector3 vel = _rigidbody.linearVelocity;
        vel.y = 0f;
        if (vel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
            _rigidbody.linearVelocity = new Vector3(vel.x, _rigidbody.linearVelocity.y, vel.z);
        }
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + groundCheckOffset;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundMask);
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
