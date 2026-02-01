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
            TryAddPhotoCard(item);

        Game.Instance.Profile.PhotoCards.CollectionChanged += OnPhotoCardsCollectionChanged;

    }

    private void OnPhotoCardsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in PhotoCardViews)
            item.gameObject.SetActive(false);

        var photoCards = e.NewItems;

        foreach (var item in Game.Instance.Profile.PhotoCards)
            TryAddPhotoCard(item);
    }

    [Button]
    public bool TryAddPhotoCard(PhotoCard photoCard)
    {
        foreach (var item in PhotoCardViews)
        {
            if (!item.gameObject.activeSelf)
            {
                item.SetImage(photoCard.texture2D, photoCard.rating, photoCard.finishQuest.ToQuestName(), photoCard.dataTexture2D);
                item.PhotoCard = photoCard;
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
        Game.Instance.Profile.PhotoCards.Remove(photoCardView.PhotoCard);
        _activePhotoCount--;
    }
}
