#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlaceableItem))]
/// <summary>
/// Редактор PlaceableItem с использованием Odin Inspector.
/// </summary>
public class PlaceableItemEditor : OdinEditor
{
    private const int PreviewSize = 128;

    public override void OnInspectorGUI()
    {
        GUILayout.Space(4f);
        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.BeginBoxHeader();
        EditorGUILayout.LabelField("Переносимый предмет", SirenixGUIStyles.BoldLabel);
        SirenixEditorGUI.EndBoxHeader();
        GUILayout.Space(4f);

        // Автоматическое превью объекта
        DrawAutomaticPreview();

        base.OnInspectorGUI();
        GUILayout.Space(4f);
        SirenixEditorGUI.EndBox();
    }

    private void DrawAutomaticPreview()
    {
        var item = (PlaceableItem)target;
        if (item == null) return;

        var preview = AssetPreview.GetAssetPreview(item.gameObject);
        if (AssetPreview.IsLoadingAssetPreview(item.gameObject.GetInstanceID()))
            Repaint();

        var rect = GUILayoutUtility.GetRect(PreviewSize, PreviewSize);
        if (preview != null)
        {
            EditorGUI.DrawPreviewTexture(rect, preview, null, ScaleMode.ScaleToFit);
        }
        else
        {
            EditorGUI.DrawRect(rect, new Color(0.22f, 0.22f, 0.22f));
            var style = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleCenter };
            EditorGUI.LabelField(rect, "Превью...", style);
        }

        GUILayout.Space(6f);
    }
}
#endif
