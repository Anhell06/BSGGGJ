using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;
using System.Linq;


#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// Управляет стэком экранов: PushScreen (открыть), PopScreen (закрыть).
/// Экраны — классы на объектах в сцене (наследники Screen).
/// Overlay-экраны открываются поверх; остальные скрывают текущий экран.
/// Если стэк пуст — показывается дефолтный экран.
/// </summary>
public class ScreenController : MonoBehaviour
{
    public static ScreenController Instance => _instance;
    private static ScreenController _instance;

    [SerializeField] private CursorLock _cursorLock;

    [Header("Дефолтный экран")]
    [Tooltip("Показывается, когда стэк экранов пуст. Обязательно назначь.")]
    [SerializeField] private Screen defaultScreen;
    [SerializeField] private List<Screen> screens;

    private readonly Stack<Screen> _stack = new Stack<Screen>();

    /// <summary>Текущий верхний экран или null, если показывается только дефолт.</summary>
    public Screen CurrentScreen => _stack.Count > 0 ? _stack.Peek() : null;

    /// <summary>Есть ли поверх дефолтного хотя бы один экран.</summary>
    public bool HasScreenStack => _stack.Count > 0;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        if (defaultScreen != null)
            defaultScreen.Hide();

        ShowDefault();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public void PushScreen<T>(bool? overlay = null) where T : Screen
    {
        var screen = screens.FirstOrDefault(s => s is T);
        if (screen == null) return;

        var isOverlay = overlay ?? screen.IsOverlay;

        if (_stack.Count > 0)
        {
            var top = _stack.Peek();
            if (top == screen) return;
            if (!isOverlay)
                top.Hide();
        }
        else if (defaultScreen != null && defaultScreen != screen)
        {
            defaultScreen.Hide();
        }

        _stack.Push(screen);
        screen.Show();

        _cursorLock.enabled = false;
    }

    /// <summary>Закрыть верхний экран. Если стэк опустел — показывается дефолтный.</summary>
    public void PopScreen()
    {
        if (_stack.Count == 0)
            return;

        Screen top = _stack.Pop();
        top.Hide();

        if (_stack.Count > 0)
        {
            _stack.Peek().Show();
        }
        else
        {
            ShowDefault();
        }
    }

    /// <summary>Закрыть все экраны и показать только дефолтный.</summary>
    public void PopAllToDefault()
    {
        while (_stack.Count > 0)
        {
            _stack.Pop().Hide();
        }
        ShowDefault();
    }

    private void ShowDefault()
    {
        if (defaultScreen != null)
            defaultScreen.Show();

        _cursorLock.enabled = true;
    }

#if UNITY_EDITOR
    [Button("Pop (тест)")]
    private void EditorPop() => PopScreen();
#endif
}
