using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// Задаёт объекту случайный поворот в указанных диапазонах градусов по осям X, Y, Z.
/// Можно применять при старте (Awake/Start) или только по оси Y (например, для декора).
/// </summary>
public class RandomRotation : MonoBehaviour
{
    [Header("Диапазоны углов (градусы)")]
    [Tooltip("Мин/макс поворот по X. Оба 0 = не вращать по X.")]
    [SerializeField] private Vector2 xRange = new Vector2(0f, 0f);
    [Tooltip("Мин/макс поворот по Y. Оба 0 = не вращать по Y.")]
    [SerializeField] private Vector2 yRange = new Vector2(0f, 360f);
    [Tooltip("Мин/макс поворот по Z. Оба 0 = не вращать по Z.")]
    [SerializeField] private Vector2 zRange = new Vector2(0f, 0f);

    [Header("Когда применять")]
    [Tooltip("Применить случайный поворот в Awake (до Start).")]
    [SerializeField] private bool applyOnAwake = true;
    [Tooltip("Использовать локальный поворот (localEulerAngles) вместо мирового.")]
    [SerializeField] private bool useLocalRotation = true;

    private void Awake()
    {
        if (applyOnAwake)
            ApplyRandomRotation();
    }

    /// <summary>Применяет случайный поворот в заданных диапазонах.</summary>
    public void ApplyRandomRotation()
    {
        float x = Random.Range(xRange.x, xRange.y);
        float y = Random.Range(yRange.x, yRange.y);
        float z = Random.Range(zRange.x, zRange.y);
        var euler = new Vector3(x, y, z);

        if (useLocalRotation)
            transform.localEulerAngles = euler;
        else
            transform.eulerAngles = euler;
    }

#if UNITY_EDITOR
    [Button("Применить случайный поворот")]
    private void EditorApplyRandomRotation()
    {
        ApplyRandomRotation();
    }
#endif
}
