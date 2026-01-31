using UnityEngine;

public class MainMenuScreen : Screen
{
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

    public static void StartGame()
    {
        Game.Instance.ScreenController.PopScreen();
    }
}
