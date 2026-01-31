using System;
using UnityEngine;

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
}