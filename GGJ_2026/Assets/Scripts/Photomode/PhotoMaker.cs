using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PhotoMaker : MonoBehaviour
{
    public Camera targetCamera;
    private readonly int _maxPhoto = 12;

    private RenderTexture visibilityRT;
    private RenderTexture photoRT;
    private Texture2D dataTexture;
    private Texture2D photoTexture;
    private Dictionary<int, int> pixelsCount = new Dictionary<int, int>();
    [SerializeField] private CameraFlashEffect flash;


    [SerializeField]
    private ScenePhotographer sp;
    
    void Start()
    {
        int scale = 1;
        // Создаем RenderTexture для захвата
        visibilityRT = new RenderTexture(160*scale, 90*scale, 0, RenderTextureFormat.ARGB32);
        visibilityRT.Create();
        photoRT = new RenderTexture(160*scale, 90*scale, 0, RenderTextureFormat.ARGB32);
        photoRT.Create();
    }
    
    public void MakePhoto()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("CheckVisibility available only in Play Mode!");
            return;
        }

        if (Game.Instance.Profile.PhotoCards.Count < _maxPhoto)
        {
            StartCoroutine(MakePhotoCoroutine());
        }
    }

    private IEnumerator MakePhotoCoroutine()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("CheckVisibility available only in Play Mode!");
            yield break;
        }
        // Сохраняем текущие настройки камеры
        CameraClearFlags originalClearFlags = targetCamera.clearFlags;
        Color originalBackground = targetCamera.backgroundColor;

        // Настраиваем камеру для захвата только нужного объекта
        //targetCamera.clearFlags = CameraClearFlags.SolidColor;
        //targetCamera.backgroundColor = Color.black;
        targetCamera.targetTexture = visibilityRT;

        if (flash != null)
            flash.PlayFlash();

        sp.Prepare();
        targetCamera.Render();
        sp.Restore();

        dataTexture = RenderTextureToTexture2D(visibilityRT);

        yield return new WaitForEndOfFrame();
        targetCamera.targetTexture = photoRT;
        targetCamera.Render();
        photoTexture = RenderTextureToTexture2D(photoRT);
        
        yield return new WaitForEndOfFrame();

        // Анализируем пиксели
        CheckPixelsForVisibilityAndEvaluatePrice(dataTexture, photoTexture);

        // Восстанавливаем настройки камеры
        targetCamera.targetTexture = null;
        //targetCamera.clearFlags = originalClearFlags;
       //targetCamera.backgroundColor = originalBackground;
        RenderTexture.active = null;
    }

    private float _nextCheckQuestTime;

    public static bool CanFinishAnyQuest { get; private set; } = false;
    private QuestId _canFinishQuest = QuestId.None;

    private void OnEnable()
    {
        CanFinishAnyQuest = false;
    }

    private void Update()
    {
        if (Time.time > _nextCheckQuestTime)
        {
            _nextCheckQuestTime = Time.time + 1.0f;
            var q = QuestHolder.Instance.CheckQuest(this.targetCamera);

            CanFinishAnyQuest = q != QuestId.None;
            _canFinishQuest = q;
        }
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
            TextureFormat.ARGB32, 
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
    private int CheckPixelsForVisibilityAndEvaluatePrice(Texture2D textureData, Texture2D texturePhoto)
    {
        pixelsCount.Clear();
        var pixels = textureData.GetPixels();

        int totalPrice = 0;
        int totalRating = 2;
        
        var pc = new PhotoCard();
        var processedPixels = new List<Color>();
        
        for (int k = 0; k < pixels.Length; k++)
        {
            var pixel = pixels[k];
            for (int i = 0; i < Game.Instance.Configs.PhotoTargetConfig.PhotoObjects.Count; i++)
            {
                var po = Game.Instance.Configs.PhotoTargetConfig.PhotoObjects[i];
                if (pixel.AlmostEqual(po.technicalColor))
                {
                    if (pixelsCount.ContainsKey(i))
                    {
                        pixelsCount[i] = pixelsCount[i] + 1;
                    }
                    else
                    {
                        pixelsCount[i] = 1;
                    }

                    if (pixelsCount[i] == po.minPixels)
                    {
                        totalPrice += po.price;
                        totalRating += po.rating;
                        pc.objectIds.Add(i);
                    }
                    processedPixels.Add(pixel);
                }
                else
                {
                    pixel.a = 0f;
                    processedPixels.Add(pixel);
                }
            }
        }

        textureData.SetPixels(processedPixels.ToArray());

        pc.price = totalPrice;
        pc.rating = Mathf.Clamp(totalRating,0,3);
        pc.texture2D = texturePhoto;
        pc.dataTexture2D = textureData;
        pc.finishQuest = _canFinishQuest;
        Game.Instance.Profile.PhotoCards.Add(pc);
        Debug.LogError($"Evaluate Stats: Price = {totalPrice} ; Rating = {totalRating}");
        return totalPrice;
    }
    
    
    void OnDestroy()
    {
        if (visibilityRT != null)
            visibilityRT.Release();
    }
}