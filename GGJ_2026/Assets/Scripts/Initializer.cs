using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
    public List<string> Scenes = new List<string>()
    {
        "MainMenu",
        "GamePlay",
        "Environment",
    };

    public IEnumerator Start()
    {
        foreach (var item in Scenes)
        {
            SceneManager.LoadScene(item, LoadSceneMode.Additive);
        }

        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Environment"));
    }

}
