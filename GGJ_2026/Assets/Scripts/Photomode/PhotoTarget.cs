using System;
using System.Collections;
using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    [SerializeField]//[HideInInspector]
    private Color _color;

    [SerializeField][HideInInspector]
    private Renderer _renderer;

    [SerializeField][HideInInspector]
    private int _rating = 0;

    public void SetTechnicalColor(Color color)
    {
        _color = color;
    }

    public void SetRating(int rating)
    {
        _rating = rating;
    }

    public void Prepare()
    {
        _renderer.material.color = _color;
    }

    public void Restore()
    {
        _renderer.material.color = Color.white;
    }

    public void BeginHighlight()
    {
        if (_rating == 0)
        {
            return;
        }
        StartCoroutine(HighlightRoutine());
    }
    
    private IEnumerator HighlightRoutine()
    {
        float pulseDuration = 1f;
        var mat = _renderer.material;
        string emissionProperty = "_EmissionColor";
        mat.EnableKeyword("_EMISSION");
        float timer = 0f;
        Color targetEmissionColor = _rating < 0 ? (Color.red) : Color.green;
        while (true)
        {
            // Туда (от черного к красному)
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                float t = timer / pulseDuration;
                Color currentColor = Color.Lerp(Color.black, targetEmissionColor, t);
                mat.SetColor(emissionProperty, currentColor);
                yield return null;
            }
            
            timer = 0f;
            
            // Обратно (от красного к черному)
            while (timer < pulseDuration)
            {
                timer += Time.deltaTime;
                float t = timer / pulseDuration;
                Color currentColor = Color.Lerp(targetEmissionColor, Color.black, t);
                mat.SetColor(emissionProperty, currentColor);
                yield return null;
            }
            
            timer = 0f;
        }
    }
    
    public void FinishHighlight()
    {
        StopAllCoroutines();
        var mat = _renderer.material;
        Color currentColor = Color.black;
        string emissionProperty = "_EmissionColor";
        mat.SetColor(emissionProperty, currentColor);
    }
}
