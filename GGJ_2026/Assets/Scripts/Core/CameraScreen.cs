using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraScreen : Screen
{
    public event Action onHide;

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
    }
}