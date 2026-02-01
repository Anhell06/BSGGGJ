using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
    }

    public void Exit() => Application.Quit();
    private void Start()
    {
        ScreenController.PushScreen<MainMenuScreen>(showCursor: true);
#if !UNITY_EDITOR
        LoadScene();
#endif
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void LoadScene()
    {
        var sceneCount = SceneManager.sceneCountInBuildSettings;

        Scene lastLoadedScene = default;

        for (var i = 0; i < sceneCount; i++)
        {
            SceneManager.LoadScene(i, LoadSceneMode.Additive);

            lastLoadedScene = SceneManager.GetSceneByBuildIndex(i);
        }

        if (lastLoadedScene.IsValid())
        {
            SceneManager.SetActiveScene(lastLoadedScene);
        }
    }
}
