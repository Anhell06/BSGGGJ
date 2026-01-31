using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhotoTargetConfig))]
public class PhotoTargetConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Prepare"))
        {
            var x = target as PhotoTargetConfig;
            x.Prepare();
            foreach (var po in x.PhotoObjects)
            {
                EditorUtility.SetDirty(po.prefab);
                UpdateAllInstancesOfPrefab(po.prefab);
            }

            var all = Resources.FindObjectsOfTypeAll<PhotoTarget>();
            foreach (var pt in all)
            {
                if (pt != null && PrefabUtility.IsPartOfAnyPrefab(pt))
                {
                    UpdateAllInstancesOfPrefab(pt);
                }
            }
        }
    }
    void UpdateAllInstancesOfPrefab(PhotoTarget prefabInstance)
    {
        PhotoTarget prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(prefabInstance);
        
        if (prefabAsset != null)
        {
            string prefabPath = AssetDatabase.GetAssetPath(prefabAsset);
            
            // Находим все инстансы в сцене
            PhotoTarget[] instances = FindObjectsOfType<PhotoTarget>();
            
            foreach (PhotoTarget instance in instances)
            {
                GameObject instanceObj = instance.gameObject;
                
                if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instanceObj) == prefabPath)
                {
                    // Обновляем инстанс
                    PrefabUtility.RevertPrefabInstance(instanceObj, InteractionMode.AutomatedAction);
                    
                    // Или применяем изменения из префаба
                    PrefabUtility.ApplyPrefabInstance(instanceObj.gameObject, InteractionMode.AutomatedAction);
                }
            }
        }
    }
}
