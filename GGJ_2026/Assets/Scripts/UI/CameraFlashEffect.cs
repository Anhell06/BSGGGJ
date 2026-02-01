using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Эффект срабатывания камеры: белая вспышка на весь экран и звук.
/// Вызовите PlayFlash() при съёмке (например из PhotoMaker.MakePhoto).
/// </summary>
[RequireComponent(typeof(Canvas))]
public class CameraFlashEffect : MonoBehaviour
{
    [Header("Вспышка")]
    [Tooltip("Белый Image на весь экран. Если не задан — создаётся автоматически при первом вызове PlayFlash().")]
    [SerializeField] private Image flashImage;

    [Tooltip("Длительность видимой вспышки в секундах (затухание до нуля).")]
    [SerializeField] private float flashDuration = 0.25f;

    [Tooltip("Кривая затухания: как быстро гаснет вспышка.")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    [Header("Звук")]
    [Tooltip("Звук вспышки. Воспроизводится в момент вызова PlayFlash().")]
    [SerializeField] private AudioClip flashSound;

    [Tooltip("Громкость звука вспышки (0–1).")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private AudioSource _audioSource;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 32767; // поверх остального UI
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (flashSound != null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        EnsureFlashImage();
        SetFlashAlpha(0f);
        _canvas.enabled = false;
    }

    private void EnsureFlashImage()
    {
        if (flashImage != null) return;

        var go = new GameObject("FlashImage");
        go.transform.SetParent(transform, false);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        flashImage = go.AddComponent<Image>();
        flashImage.color = Color.white;
        flashImage.raycastTarget = false;
    }

    private void SetFlashAlpha(float a)
    {
        if (flashImage != null)
        {
            var c = flashImage.color;
            c.a = a;
            flashImage.color = c;
        }
        if (_canvasGroup != null)
            _canvasGroup.alpha = a;
    }

    /// <summary>
    /// Запускает эффект вспышки: белый экран и звук, затем затухание.
    /// </summary>
    public void PlayFlash()
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        if (flashSound != null && _audioSource != null)
            _audioSource.PlayOneShot(flashSound, volume);

        EnsureFlashImage();
        _canvas.enabled = true;
        SetFlashAlpha(1f);

        _flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            float a = fadeCurve.Evaluate(t);
            SetFlashAlpha(a);
            yield return null;
        }

        SetFlashAlpha(0f);
        _canvas.enabled = false;
        _flashCoroutine = null;
    }
}
