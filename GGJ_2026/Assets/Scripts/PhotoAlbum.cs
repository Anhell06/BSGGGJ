using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        foreach (var item in Game.Instance.Profile.PhotoCards)
            TryAddPhotoCard(item.texture2D, item.rating, "temp");

        Game.Instance.Profile.PhotoCards.CollectionChanged += OnPhotoCardsCollectionChanged;

    }

    private void OnPhotoCardsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in PhotoCardViews)
            OnPhotoDelete(item);

        var photoCards = e.NewItems;

        foreach (var item in Game.Instance.Profile.PhotoCards)
            TryAddPhotoCard(item.texture2D, item.rating, "temp");

    }

    [Button]
    public bool TryAddPhotoCard(Texture2D texture2D, int star, string name)
    {
        foreach (var item in PhotoCardViews)
        {
            if (!item.gameObject.activeSelf)
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
