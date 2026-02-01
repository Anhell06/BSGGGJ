using System;
using System.Collections.Generic;
using UnityEngine;

public class ScenePhotographer : MonoBehaviour
{
    public static ScenePhotographer Instance { get; private set; }
    [SerializeField]
    private List<ScenePhotoObjectData> _photoObjects = new List<ScenePhotoObjectData>();

    [SerializeField]
    private Material _photoMaterial;

    private void Awake()
    {
        Instance = this;
    }

    public void CollectSceneData()
    {
        Restore();
        _photoObjects.Clear();
        var allRenderers = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var renderer in allRenderers)
        {
            var data = new ScenePhotoObjectData();
            data.renderer = renderer as Renderer;
            data.materials = new List<Material>();
            data.materials.AddRange(data.renderer.sharedMaterials);
            data.photoTarget = renderer.GetComponent<PhotoTarget>();
            _photoObjects.Add(data);
        }
    }

    public void BeginHighlight()
    {
        foreach (var po in _photoObjects)
        {
            if (po.photoTarget != null)
            {
                po.photoTarget.BeginHighlight();
            }
        }
    }
    
    public void FinishHighlight()
    {
        foreach (var po in _photoObjects)
        {
            if (po.photoTarget != null)
            {
                po.photoTarget.FinishHighlight();
            }
        }
    }

    private void Start()
    {
        CollectSceneData();
    }

    public void Prepare()
    {
        foreach (var po in _photoObjects)
        {
            if (po.renderer != null)
            {
                List<Material> m = new List<Material>();
                for (int i = 0; i < po.renderer.materials.Length; i++)
                {
                    m.Add(_photoMaterial);
                }

                po.renderer.materials = m.ToArray();
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
                for (int i = 0; i < po.renderer.materials.Length; i++)
                {
                    po.renderer.materials = po.materials.ToArray();
                }
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
    public List<Material> materials;
    public PhotoTarget photoTarget;
}