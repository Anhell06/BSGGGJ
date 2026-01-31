using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScenePhotographer))]
public class ScenePhotographerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Collect scene data"))
        {
            var x = target as ScenePhotographer;
            x.CollectSceneData();
        }
        if (GUILayout.Button("Prepare"))
        {
            var x = target as ScenePhotographer;
            x.Prepare();
        }
        if (GUILayout.Button("Restore"))
        {
            var x = target as ScenePhotographer;
            x.Restore();
        }
        if (GUILayout.Button("Clear"))
        {
            var x = target as ScenePhotographer;
            x.Clear();
        }
    }
}
