using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Окно редактора: по нажатию загружаются все сцены из Build Settings в редактор.
/// Меню: Tools → Build Scenes Loader
/// </summary>
public class BuildScenesLoaderWindow : EditorWindow
{
    [MenuItem("Tools/Build Scenes Loader")]
    public static void ShowWindow()
    {
        var window = GetWindow<BuildScenesLoaderWindow>("Build Scenes");
        window.minSize = new Vector2(260f, 80f);
    }

    private void OnGUI()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

        if (scenes.Length == 0)
        {
            EditorGUILayout.HelpBox("В Build Settings нет сцен.\nFile → Build Settings → Add Open Scenes", MessageType.Info);
            if (GUILayout.Button("Открыть Build Settings"))
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            return;
        }

        if (GUILayout.Button("Загрузить все сцены", GUILayout.Height(32f)))
            LoadAllBuildScenes();

        EditorGUILayout.Space(4f);
        if (GUILayout.Button("Открыть Build Settings"))
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
    }

    private static void LoadAllBuildScenes()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        int loaded = 0;

        for (int i = 0; i < scenes.Length; i++)
        {
            if (string.IsNullOrEmpty(scenes[i].path))
                continue;

            OpenSceneMode mode = loaded == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive;
            EditorSceneManager.OpenScene(scenes[i].path, mode);
            loaded++;
        }

        if (loaded > 0)
            Debug.Log($"[Build Scenes Loader] Загружено сцен: {loaded}");
    }
}
