using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PixelVisibilityChecker))]
public class PixelVisibilityCheckerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("CHECK VISIBILITY"))
        {
            var x = target as PixelVisibilityChecker;
            x.CheckVisibility();
        }
    }
}
