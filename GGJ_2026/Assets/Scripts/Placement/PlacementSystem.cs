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

    [Tooltip("Если линейная скорость (м/с) ниже этого порога — обнуляем, чтобы объект не трясся.")]
    [SerializeField] private float linearVelocityDeadZone = 0.01f;
    [Tooltip("Если угловая скорость (рад/с) ниже этого порога — обнуляем, чтобы объект не трясся.")]
    [SerializeField] private float angularVelocityDeadZone = 0.01f;

    [Tooltip("Корень персонажа (коллайдеры персонажа не будут сталкиваться с переносимым объектом). Если не задан — берётся из родителя с компонентом Player.")]
    [SerializeField] private Transform playerRoot;

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

    private void FixedUpdate()
    {
        float scrollY = 0f;
        if (Mouse.current != null)
            scrollY = Mouse.current.scroll.ReadValue().y;

        if (_interactPressedThisFrame)
        {
            _interactPressedThisFrame = false;
            if (_heldItem != null)
            {
                DropHeldItem();
            }
            else
            { 
                TryPickUp();
                _heldItem?.OnPicked?.Invoke();
            }
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

        _heldRigidbody.useGravity = false;
        _heldRigidbody.linearVelocity = Vector3.zero;
        _heldRigidbody.angularVelocity = Vector3.zero;

        // Коллайдеры не отключаем — объект сталкивается со стенами и другими объектами
        _heldColliders = _heldItem.GetComponentsInChildren<Collider>();
        SetIgnoreCollisionWithPlayer(_heldColliders, ignore: true);
        _holdVelocity = Vector3.zero;
    }

    /// <summary>
    /// Луч от камеры к holdPosition: первое попадание по стене, игнорируя персонажа и сам переносимый объект.
    /// </summary>
    private RaycastHit? GetFirstWallHit(Vector3 origin, Vector3 direction, float distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, stickToWallLayerMask);
        Transform playerRootTransform = playerRoot != null ? playerRoot : GetComponentInParent<Player>()?.transform;
        Transform heldRoot = _heldItem != null ? _heldItem.transform : null;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        for (int i = 0; i < hits.Length; i++)
        {
            Transform hitTransform = hits[i].collider.transform;
            if (playerRootTransform != null && (hitTransform == playerRootTransform || hitTransform.IsChildOf(playerRootTransform)))
                continue;
            if (heldRoot != null && (hitTransform == heldRoot || hitTransform.IsChildOf(heldRoot)))
                continue;
            return hits[i];
        }
        return null;
    }

    private void SetIgnoreCollisionWithPlayer(Collider[] heldColliders, bool ignore)
    {
        Transform root = playerRoot != null ? playerRoot : GetComponentInParent<Player>()?.transform;
        if (root == null || heldColliders == null) return;
        Collider[] playerColliders = root.GetComponentsInChildren<Collider>();
        for (int i = 0; i < heldColliders.Length; i++)
        {
            if (heldColliders[i] == null || !heldColliders[i].enabled) continue;
            for (int j = 0; j < playerColliders.Length; j++)
            {
                if (playerColliders[j] == null || !playerColliders[j].enabled) continue;
                Physics.IgnoreCollision(heldColliders[i], playerColliders[j], ignore);
            }
        }
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

        RaycastHit? wallHit = GetFirstWallHit(origin, direction, distance);
        if (wallHit.HasValue)
        {
            RaycastHit hit = wallHit.Value;
            if (_heldItem.StickToWallsWhenCarrying)
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
            else
            {
                // Не прилипаем к стене — ограничиваем позицию, чтобы объект не проходил сквозь стену
                targetPos = hit.point + hit.normal * _heldItem.StickToWallOffset;
            }
        }

        targetRot = targetRot * Quaternion.Euler(_heldItem.DefaultRotationEuler);

        Vector3 localAttach = _heldItem.AttachmentPoint != null ? _heldItem.AttachmentPoint.localPosition : Vector3.zero;
        Vector3 targetTransformPos = targetPos - (targetRot * localAttach);

        // Движение через физику — коллизии обрабатывает движок, объект не проходит сквозь стены и другие коллайдеры
        float dt = Time.fixedDeltaTime;
        Vector3 nextPos = Vector3.MoveTowards(_heldItem.transform.position, targetTransformPos, holdMoveSpeed * dt);
        Quaternion nextRot = Quaternion.Slerp(
            _heldItem.transform.rotation,
            targetRot,
            _heldItem.HoldSmoothTime > 0.001f ? Mathf.Clamp01(dt / _heldItem.HoldSmoothTime) : 1f
        );
        if (_heldRigidbody != null)
        {
            _heldRigidbody.MovePosition(nextPos);
            _heldRigidbody.MoveRotation(nextRot);
            // Обнуляем малые скорости, чтобы объект не трясся
            if (_heldRigidbody.linearVelocity.sqrMagnitude < linearVelocityDeadZone * linearVelocityDeadZone)
                _heldRigidbody.linearVelocity = Vector3.zero;
            if (_heldRigidbody.angularVelocity.sqrMagnitude < angularVelocityDeadZone * angularVelocityDeadZone)
                _heldRigidbody.angularVelocity = Vector3.zero;
        }
        else
        {
            _heldItem.transform.SetPositionAndRotation(nextPos, nextRot);
        }
    }

    private void DropHeldItem()
    {
        if (_heldItem == null) return;
        SetIgnoreCollisionWithPlayer(_heldColliders, ignore: false);
        _heldColliders = null;
        _heldItem.IsHeld = false;
        if (_heldRigidbody != null)
        {
            _heldRigidbody.isKinematic = _wasStuckToWall;

            _heldRigidbody.useGravity = true;
        }
        if(_heldItem != null)
            _heldItem?.OnDroped?.Invoke();
        _heldItem = null;
        _heldRigidbody = null;
    }
}
