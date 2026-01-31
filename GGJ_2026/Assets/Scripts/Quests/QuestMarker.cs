using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public class QuestMarker : MonoBehaviour
{
    [SerializeField]
    private QuestId _questId;
    public QuestId QuestId => _questId;

    private bool isVisible = false;
    
    public bool IsVisible(Camera camera)
    {
        if (!isVisible)
        {
            return false;
        }

        var cameraOrigin = camera.transform.position;
        var direction = transform.position - cameraOrigin;
        if (Physics.Raycast(cameraOrigin, direction, 1000, LayerMask.NameToLayer("TempVisibility")))
        {
            return true;
        }

        return false;
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
}
