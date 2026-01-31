using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhotoMaker))]
public class PixelVisibilityCheckerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("CHECK VISIBILITY"))
        {
            var x = target as PhotoMaker;
            x.MakePhoto();
        }
    }
}
