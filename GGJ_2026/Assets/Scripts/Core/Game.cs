using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance => _instance;

    private static Game _instance;

    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private Configs _configs;
    public Configs Configs => _configs;

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

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
