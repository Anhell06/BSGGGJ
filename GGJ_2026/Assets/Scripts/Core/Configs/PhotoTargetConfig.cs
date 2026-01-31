using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PhotoTargetConfig", fileName = "PhotoTargetConfig")]
public class PhotoTargetConfig : ScriptableObject
{
    [SerializeField]
    private List<PhotoObjectNote> _photoObjects;
    public List<PhotoObjectNote> PhotoObjects =>_photoObjects;

    public void Prepare()
    {
        foreach (var po in _photoObjects)
        {
            po.prefab.SetTechnicalColor(po.technicalColor);
        }
    }
}

[Serializable]
public class PhotoObjectNote
{
    public Color technicalColor;
    public int price;
    public int minPixels = 1;
    public PhotoTarget prefab;
}