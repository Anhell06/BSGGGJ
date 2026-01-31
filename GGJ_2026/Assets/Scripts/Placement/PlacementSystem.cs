using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlacementSystem : MonoBehaviour
{
    [Header("Ввод")]
    [SerializeField] private InputActionReference interactActionReference;

    [Header("Поворот при прилипании")]
    [Tooltip("Скорость изменения угла поворота (градусы за прокрутку колёсика).")]
    [SerializeField] private float stickTwistScrollSpeed = 45f;

    [Header("Луч")]
    [SerializeField] private float pickUpMaxDistance = 5f;
    [SerializeField] private LayerMask pickUpLayerMask = ~0;

    [Header("Перенос")]
    [Tooltip("Точка удержания (для любого переносимого объекта).")]
    [SerializeField] private GameObject holdPosition;
    [SerializeField] private Camera _camera;
    [Tooltip("Слой коллайдеров стен для прилипания (луч от камеры к точке удержания).")]
    [SerializeField] private LayerMask stickToWallLayerMask = ~0;

    [Tooltip("Макс. скорость движения к цели (единиц/сек).")]
    [SerializeField] private float holdMoveSpeed = 15f;

    private PlaceableItem _heldItem;
    private Rigidbody _heldRigidbody;
    private Collider[] _heldColliders;
    private Vector3 _holdVelocity;
    private bool _interactPressedThisFrame;
    private bool _wasStuckToWall;

    private void Awake()
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        interactActionReference?.action?.Enable();
    }

    private void OnDisable()
    {
        interactActionReference?.action?.Disable();
        if (_heldItem != null)
            DropHeldItem();
    }

    private void Update()
    {
        if (interactActionReference?.action != null && interactActionReference.action.WasPressedThisFrame())
            _interactPressedThisFrame = true;
    }

    private void LateUpdate()
    {
        float scrollY = 0f;
        if (Mouse.current != null)
            scrollY = Mouse.current.scroll.ReadValue().y;

        if (_interactPressedThisFrame)
        {
            _interactPressedThisFrame = false;
            if (_heldItem != null)
                DropHeldItem();
            else
                TryPickUp();
        }

        if (_heldItem != null)
            UpdateHeldPosition(scrollY);
    }

    private void TryPickUp()
    {
        var ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out var hit, pickUpMaxDistance, pickUpLayerMask))
            return;

        var item = hit.collider.GetComponentInParent<PlaceableItem>();
        if (item == null)
            return;

        _heldItem = item;
        _heldItem.IsHeld = true;
        _heldRigidbody = _heldItem.Rigidbody ?? _heldItem.GetComponent<Rigidbody>();

        _heldRigidbody.isKinematic = true;
        _heldRigidbody.linearVelocity = Vector3.zero;
        _heldRigidbody.angularVelocity = Vector3.zero;

        _heldColliders = _heldItem.GetComponentsInChildren<Collider>();
        for (int i = 0; i < _heldColliders.Length; i++)
            _heldColliders[i].enabled = false;
        _holdVelocity = Vector3.zero;
    }


    private void UpdateHeldPosition(float scrollY)
    {
        if (_heldItem == null || holdPosition == null) return;

        Vector3 origin = _camera.transform.position;
        Vector3 idealTarget = holdPosition.transform.position;
        Vector3 direction = idealTarget - origin;
        float distance = direction.magnitude;
        if (distance < 0.001f)
            direction = _camera.transform.forward;
        else
            direction /= distance;

        Vector3 targetPos = idealTarget;
        Quaternion targetRot = _camera.transform.rotation;
        _wasStuckToWall = false;

        if (_heldItem.StickToWallsWhenCarrying && Physics.Raycast(origin, direction, out RaycastHit hit, distance, stickToWallLayerMask))
        {
            _wasStuckToWall = true;

            if (Mathf.Abs(scrollY) > 0.001f)
            {
                _heldItem.StickTwistDegrees += scrollY * stickTwistScrollSpeed;
                _heldItem.StickTwistDegrees = Mathf.Repeat(_heldItem.StickTwistDegrees + 180f, 360f) - 180f;
            }

            targetPos = hit.point + hit.normal * _heldItem.StickToWallOffset;
            targetRot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (Mathf.Abs(_heldItem.StickTwistDegrees) > 0.001f)
                targetRot = Quaternion.AngleAxis(_heldItem.StickTwistDegrees, hit.normal) * targetRot;
        }

        Vector3 currentPos = _heldItem.transform.position;
        _heldItem.transform.position = Vector3.MoveTowards(currentPos, targetPos, holdMoveSpeed * Time.deltaTime);
        _heldItem.transform.rotation = Quaternion.Slerp(
            _heldItem.transform.rotation,
            targetRot,
            _heldItem.HoldSmoothTime > 0.001f ? Time.deltaTime / _heldItem.HoldSmoothTime : 1f
        );
    }

    private void DropHeldItem()
    {
        if (_heldItem == null) return;
        _heldItem.IsHeld = false;
        if (_heldColliders != null)
        {
            for (int i = 0; i < _heldColliders.Length; i++)
                if (_heldColliders[i] != null)
                    _heldColliders[i].enabled = true;
            _heldColliders = null;
        }
        if (_heldRigidbody != null)
        {
            if (!_wasStuckToWall)
                _heldRigidbody.isKinematic = false;
        }
        _heldItem = null;
        _heldRigidbody = null;
    }
}
