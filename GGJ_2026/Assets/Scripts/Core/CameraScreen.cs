using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraScreen : Screen
{
    public event Action onHide;
    public TMP_Text countPhoto;
    public GameObject NoFreeSpace;

    protected override void OnShow()
    {
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetPhotographerMode(true);

    }

    protected override void OnHide()
    {
        onHide?.Invoke();
        var player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetPhotographerMode(false);
    }
    
    public Image canFinishQuest;

    private void Update()
    {
        canFinishQuest.color = PhotoMaker.CanFinishAnyQuest ? Color.green : Color.red;
        var clr = canFinishQuest.color;
        clr.a = 0.2f;
        canFinishQuest.color = clr;
        countPhoto.text = $"{Game.Instance.Profile.PhotoCards.Count}/12";
        NoFreeSpace.SetActive(Game.Instance.Profile.PhotoCards.Count >= 12);
    }
}