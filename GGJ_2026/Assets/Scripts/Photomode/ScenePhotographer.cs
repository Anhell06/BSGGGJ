using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenePhotographer : MonoBehaviour
{
    [SerializeField]
    private List<ScenePhotoObjectData> _photoObjects = new List<ScenePhotoObjectData>();

    [SerializeField]
    private Material _photoMaterial;
    
    public void CollectSceneData()
    {
        Restore();
        _photoObjects.Clear();
        var allRenderers = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var renderer in allRenderers)
        {
            var data = new ScenePhotoObjectData();
            data.renderer = renderer as Renderer;
            data.material = data.renderer.sharedMaterial;
            data.photoTarget = renderer.GetComponent<PhotoTarget>();
            _photoObjects.Add(data);
        }
    }

    public void Prepare()
    {
        foreach (var po in _photoObjects)
        {
            if (po.renderer != null)
            {
                po.renderer.material = _photoMaterial;
                po?.photoTarget?.Prepare();
            }
        }
    }

    public void Restore()
    {
        foreach (var po in _photoObjects)
        {
            if (po.renderer != null)
            {
                po.renderer.material = po.material;
                po?.photoTarget?.Restore();
            }
        }
    }

    public void Clear()
    {
        Restore();
        _photoObjects.Clear();
    }
}

[Serializable]
public class ScenePhotoObjectData
{
    public Renderer renderer;
    public Material material;
    public PhotoTarget photoTarget;
}