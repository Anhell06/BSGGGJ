using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

[Serializable]
public class ScreenInputBinding
{
    [Tooltip("Экшен из новой Input System (например, из InputSystem_Actions). При performed открывается экран.")]
    public InputActionReference actionReference;
    public bool ShowCursor;
    [Tooltip("Экран, который открывается по срабатыванию экшена.")]
    public Screen screen;
}

/// <summary>
/// Управляет стэком экранов: PushScreen (открыть), PopScreen (закрыть).
/// Экраны — классы на объектах в сцене (наследники Screen).
/// Overlay-экраны открываются поверх; остальные скрывают текущий экран.
/// Если стэк пуст — показывается дефолтный экран.
/// Настройки ввода — в списке Input Bindings (новая Input System): по срабатыванию экшена откроется нужный экран.
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

    [Header("Новая Input System")]
    [Tooltip("Экшен → экран. При performed экшена открывается соответствующий экран.")]
    [SerializeField] private List<ScreenInputBinding> inputBindings = new List<ScreenInputBinding>();

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

    private readonly List<SubscribedScreenAction> _subscribedActions = new List<SubscribedScreenAction>();

    private void OnEnable()
    {
        _subscribedActions.Clear();
        if (inputBindings == null) return;
        foreach (var binding in inputBindings)
        {
            if (binding.actionReference == null || binding.screen == null) continue;
            var action = binding.actionReference.action;
            if (action == null) continue;
            var screen = binding.screen;
            void OnPerformed(InputAction.CallbackContext ctx)
            {
                if (CurrentScreen == screen)
                    PopScreen();
                else
                    PushScreen(screen, showCursor: binding.ShowCursor);
            }
            action.performed += OnPerformed;
            action.Enable();
            _subscribedActions.Add(new SubscribedScreenAction { Action = action, Callback = OnPerformed });
        }
    }

    private void OnDisable()
    {
        foreach (var sub in _subscribedActions)
        {
            sub.Action.performed -= sub.Callback;
            sub.Action.Disable();
        }
        _subscribedActions.Clear();
    }

    private class SubscribedScreenAction
    {
        public InputAction Action;
        public Action<InputAction.CallbackContext> Callback;
    }

    /// <summary>Открыть экран по ссылке (используется настройками клавиш).</summary>
    public void PushScreen(Screen screen, bool? overlay = null, bool? showCursor = null)
    {
        if (screen == null) return;

        bool isOverlay = overlay ?? screen.IsOverlay;

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

        if (_cursorLock != null && showCursor == true)
            _cursorLock.enabled = false;
    }

    /// <summary>Открыть экран по типу (ищет в списке screens).</summary>
    public void PushScreen<T>(bool? overlay = null, bool? showCursor = null) where T : Screen
    {
        var screen = screens != null ? screens.FirstOrDefault(s => s is T) : null;
        if (screen == null) return;
        PushScreen(screen, overlay, showCursor);
    }

    /// <summary>Закрыть верхний экран. Если стэк опустел — показывается дефолтный.</summary>
    public void PopScreen()
    {
        if (_stack.Count == 0)
            return;

        Screen top = _stack.Pop();
        top.Hide();

        if (_cursorLock != null)
            _cursorLock.enabled = true;

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
    }

#if UNITY_EDITOR
    [Button("Pop (тест)")]
    private void EditorPop() => PopScreen();
#endif
}
