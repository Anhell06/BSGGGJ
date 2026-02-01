using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Blooking : Screen
{
    public List<PhotoCardView> PhotoCardViews;
    public GameObject WinScreen;
    public TMP_Text price;
    private void OnEnable()
    {
        AddPhotoCard(Game.Instance.Profile.PhotoCards.OrderBy(p => p.price * p.rating).Take(PhotoCardViews.Count).ToList());
        price.text = "Final price: " + PhotoCardViews.Where(p => p.PhotoCard != null).Sum(p => p.PhotoCard.price).ToString();
    }

    [Button]
    public void AddPhotoCard(List<PhotoCard> photoCard)
    {
        foreach (var item in PhotoCardViews)
        {
            item.PhotoCard = null;
            item.gameObject.SetActive(false);
        }

        for (var i = 0; i < photoCard.Count && i < PhotoCardViews.Count; i++)
        {
            PhotoCardViews[i].gameObject.SetActive(true);

            PhotoCardViews[i].PhotoCard = photoCard[i];
            PhotoCardViews[i].SetImage(photoCard[i].texture2D, photoCard[i].rating, photoCard[i].finishQuest.ToQuestName());
        }
    }

    public void ShowWinScreen()
    {
        var allcards = PhotoCardViews.All(p => p.PhotoCard != null);

        if (allcards && PhotoCardViews.Sum(p => p.PhotoCard.price) > 0 && PhotoCardViews.Sum(p => p.PhotoCard.rating) > 6)
            WinScreen.SetActive(true);
    }

    protected override void OnShow()
    {
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetGameplayInputEnabled(false);
    }

    protected override void OnHide()
    {
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetGameplayInputEnabled(true);
    }

}