using System;
using UnityEngine;
using System.Collections;

public class PixelVisibilityChecker : MonoBehaviour
{
    public Camera targetCamera;
    public Renderer targetRenderer;
    
    private RenderTexture visibilityRT;
    private Texture2D readTexture;
    
    void Start()
    {
        // Создаем RenderTexture для захвата
        visibilityRT = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        visibilityRT.Create();
    }
    
    public void CheckVisibility()
    {
        StartCoroutine(CheckVisibilityCoroutine());
    }

    public IEnumerator CheckVisibilityCoroutine()
    {
        // Сохраняем текущие настройки камеры
        CameraClearFlags originalClearFlags = targetCamera.clearFlags;
        Color originalBackground = targetCamera.backgroundColor;
        LayerMask originalCullingMask = targetCamera.cullingMask;

        // Настраиваем камеру для захвата только нужного объекта
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = Color.black;
        targetCamera.targetTexture = visibilityRT;

        // Рендерим только нужный слой
        int tempLayer = LayerMask.NameToLayer("TempVisibility");
        if (tempLayer != -1)
        {
            int originalLayer = targetRenderer.gameObject.layer;
            targetRenderer.gameObject.layer = tempLayer;
            targetCamera.cullingMask = 1 << tempLayer;

            targetCamera.Render();

            // Восстанавливаем слой
            targetRenderer.gameObject.layer = originalLayer;
        }

        readTexture = RenderTextureToTexture2D(visibilityRT);

        yield return new WaitForEndOfFrame();

        // Анализируем пиксели
        bool isVisible = CheckPixelsForVisibility(readTexture, targetRenderer);

        // Восстанавливаем настройки камеры
        targetCamera.targetTexture = null;
        targetCamera.clearFlags = originalClearFlags;
        targetCamera.backgroundColor = originalBackground;
        targetCamera.cullingMask = originalCullingMask;
        RenderTexture.active = null;

        Debug.Log("Object visible: " + isVisible);
    }
    
    private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        // Сохраняем активный RenderTexture
        RenderTexture previousActive = RenderTexture.active;
    
        // Устанавливаем наш RenderTexture как активный
        RenderTexture.active = renderTexture;
    
        // Создаем Texture2D с размерами RenderTexture
        Texture2D texture2D = new Texture2D(
            renderTexture.width, 
            renderTexture.height, 
            TextureFormat.RGBA32, 
            false
        );
    
        // Читаем пиксели из RenderTexture в Texture2D
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();
    
        // Восстанавливаем активный RenderTexture
        RenderTexture.active = previousActive;
    
        return texture2D;
    }

    //TODO: распараллелить
    private bool CheckPixelsForVisibility(Texture2D texture, Renderer renderer)
    {
        Color[] pixels = texture.GetPixels();
        
        foreach (Color pixel in pixels)
        {
            if (pixel.AlmostEqual(renderer.material.color))
                return true;
        }
        
        return false;
    }
    
    void OnDestroy()
    {
        if (visibilityRT != null)
            visibilityRT.Release();
    }
}