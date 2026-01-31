using Unity.VisualScripting;

public class MainMenuScreen : Screen 
{
    protected override void OnShow()
    {
        base.OnShow();
    }


    public static void StartGame()
    { 
        Game.Instance.ScreenController.PopScreen();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }
};
