using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance => _instance;

    private static Game _instance;

    public ScreenController  ScreenController;

    [SerializeField]
    private Configs _configs;
    public Configs Configs => _configs;

    public Profile Profile { get; private set; } = new Profile();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        ScreenController.PushScreen<MainMenuScreen>(showCursor: true);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
