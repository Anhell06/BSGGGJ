using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// Предмет, который можно поднять, переносить и размещать.
/// Единственная ответственность: хранить данные и состояние переносимого предмета.
/// </summary>
public class PlaceableItem : MonoBehaviour
{
    [Header("Идентификация")]
    [Tooltip("Название предмета для UI и отладки.")]
    [SerializeField] private string itemName = "Предмет";
    [SerializeField, TextArea(1, 3)] private string description;
    [SerializeField] public Rigidbody Rigidbody;

    [Header("Поворот и крепление")]
    [Tooltip("Дефолтный поворот (углы Эйлера в градусах), добавляется к предмету при переносе и размещении.")]
    [SerializeField] private Vector3 defaultRotationEuler = Vector3.zero;
    [Tooltip("Точка крепления: сюда выравнивается предмет (если не задана — используется pivot объекта).")]
    [SerializeField] private Transform attachmentPoint;

    [Header("Перенос")]
    [Tooltip("Плавность следования за целью при переносе (0 = мгновенно).")]
    [SerializeField, Range(0f, 1f)] private float holdSmoothTime = 0.1f;
    [Tooltip("При переносе прилипать к стенам (позиция и поворот по поверхности).")]
    [SerializeField] private bool stickToWallsWhenCarrying;
    [Tooltip("Отступ от поверхности стены при прилипании.")]
    [SerializeField] private float stickToWallOffset = 0.02f;
    [Tooltip("Дополнительный поворот вокруг нормали поверхности (градусы).")]
    [SerializeField] private float stickTwistDegrees = 0f;

    public string ItemName => itemName;
    public float HoldSmoothTime => holdSmoothTime;
    public bool StickToWallsWhenCarrying => stickToWallsWhenCarrying;
    public float StickToWallOffset => stickToWallOffset;
    public float StickTwistDegrees { get => stickTwistDegrees; set => stickTwistDegrees = value; }
    public Vector3 DefaultRotationEuler => defaultRotationEuler;
    public Transform AttachmentPoint => attachmentPoint;

    public bool IsHeld { get; set; }

}
