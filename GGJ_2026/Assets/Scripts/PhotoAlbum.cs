using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotoAlbum : MonoBehaviour
{
    public List<PhotoCardView> PhotoCardViews;

    public int ActivePhotoCount => _activePhotoCount;

    private int _activePhotoCount;

    private void Start()
    {
        foreach (var item in PhotoCardViews)
        {
            item.OnDelete += () => OnPhotoDelete(item);
        }
    }

    [Button]
    public bool TryAddPhotoCard(Texture2D texture2D, int star, string name)
    {
        foreach (var item in PhotoCardViews)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                item.SetImage(texture2D, star, name);
                item.gameObject.SetActive(true);
                _activePhotoCount++;
                return true;
            }
        }

        return false;
    }

    private void OnPhotoDelete(PhotoCardView photoCardView)
    {
        photoCardView.gameObject.SetActive(false);
        _activePhotoCount--;
    }
}
