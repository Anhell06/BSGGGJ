using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCardView : MonoBehaviour
{
    public PhotoCard PhotoCard { get; set; }
    [SerializeField] private Image _image;
    [SerializeField] private List<GameObject> _stars;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Button _button;

    public event Action OnDelete;

    private void Awake()
    {
        _button.onClick.AddListener(() => OnDelete?.Invoke());
    }

    [Button]
    public void SetImage(Texture2D texture2D, int star, string name)
    {
        var sprite = Texture2DToSprite(texture2D);

        for (var i = 0; i < _stars.Count; i++)
            _stars[i].SetActive(i < star);

        _name.text = name;

        _image.sprite = sprite;
        _image.enabled = sprite != null;
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