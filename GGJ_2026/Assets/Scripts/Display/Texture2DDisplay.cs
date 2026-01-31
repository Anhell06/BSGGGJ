using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// Отображает Texture2D на объекте в сцене (MeshRenderer/SpriteRenderer) и/или на Canvas (UI Image).
/// Назначь текстуру и целевые объекты — компонент применит её ко всем целям.
/// </summary>
public class Texture2DDisplay : MonoBehaviour
{
    [Header("Текстура")]
    [Tooltip("Текстура для отображения. Можно задать в инспекторе или через SetTexture().")]
    [SerializeField] private Texture2D texture;

    [Header("Объекты в сцене")]
    [Tooltip("Рендереры в сцене (MeshRenderer, SpriteRenderer и т.д.), на которых показывать текстуру.")]
    [SerializeField] private Renderer[] sceneRenderers = System.Array.Empty<Renderer>();

    [Header("Canvas (UI)")]
    [Tooltip("UI Image на канвасе, на которых показывать текстуру как спрайт.")]
    [SerializeField] private UnityEngine.UI.Image[] canvasImages = System.Array.Empty<UnityEngine.UI.Image>();

    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");

    /// <summary>Текущая назначенная текстура.</summary>
    public Texture2D Texture
    {
        get => texture;
        set { texture = value; ApplyTexture(); }
    }

    private void OnValidate()
    {
        if (texture != null)
            ApplyTexture();
    }

    private void Start()
    {
        ApplyTexture();
    }

    /// <summary>Назначает текстуру и сразу применяет её ко всем целям.</summary>
    public void SetTexture(Texture2D newTexture)
    {
        texture = newTexture;
        ApplyTexture();
    }

    /// <summary>Применяет текущую Texture2D ко всем назначенным объектам (сцена + канвас).</summary>
    public void ApplyTexture()
    {
        if (texture == null)
            return;

        ApplyToSceneObjects();
        ApplyToCanvas();
    }

    private void ApplyToSceneObjects()
    {
        foreach (var r in sceneRenderers)
        {
            if (r == null) continue;

            if (r is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.sprite = Texture2DToSprite(texture);
                spriteRenderer.enabled = true;
            }
            else if (r.TryGetComponent<MeshRenderer>(out var meshRenderer) && meshRenderer.material != null)
            {
                meshRenderer.material.mainTexture = texture;
            }
            else if (r.sharedMaterial != null)
            {
                r.sharedMaterial.SetTexture(MainTexId, texture);
            }
        }
    }

    private void ApplyToCanvas()
    {
        var sprite = texture != null ? Texture2DToSprite(texture) : null;

        foreach (var img in canvasImages)
        {
            if (img == null) continue;
            img.sprite = sprite;
            img.enabled = sprite != null;
        }
    }

    private static Sprite Texture2DToSprite(Texture2D tex)
    {
        if (tex == null) return null;
        return Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}
