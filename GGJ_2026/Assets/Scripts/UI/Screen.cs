using UnityEngine;

/// <summary>
/// Базовый класс экрана. Вешается на GameObject в сцене (панель Canvas и т.д.).
/// Контроллер вызывает Show/Hide при Push/Pop. Переопредели OnShow/OnHide при необходимости.
/// Для своего экрана создай класс, наследующий Screen (например MenuScreen : Screen).
/// </summary>
public class Screen : MonoBehaviour
{
    [Header("Экран")]
    [Tooltip("Если true — экран открывается поверх предыдущего (overlay). Если false — предыдущий скрывается (exclusive).")]
    [SerializeField] private bool overlay;

    public bool IsOverlay => overlay;

    public bool IsVisible { get; private set; }

    public void Show()
    {
        if (IsVisible) return;
        SetActive(true);
        IsVisible = true;
        OnShow();
    }

    public void Hide()
    {
        if (!IsVisible) return;
        OnHide();
        IsVisible = false;
        SetActive(false);
    }

    protected virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    protected virtual void OnShow() { }

    protected virtual void OnHide() { }
}
