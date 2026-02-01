using System;
using NUnit.Framework.Constraints;
using UnityEngine;

public class QuestMarker : MonoBehaviour
{
    [SerializeField]
    private QuestId _questId;
    public QuestId QuestId => _questId;
    [SerializeField]
    private Renderer _rend;
    
    public bool IsVisible(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
    
        var isVisible = viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z > 0;

        if (!isVisible)
        {
            _rend.material.color = Color.red;
            return false;
        }

        var cameraOrigin = camera.transform.position;
        var direction = transform.position - cameraOrigin;
        if (Physics.Raycast(cameraOrigin, direction, out var hitInfo, 100, LayerMask.GetMask("Default", "TempVisibility")))
        {
            if (hitInfo.collider.gameObject == this.gameObject)
            {
                _rend.material.color = Color.green;
                return true;
            }
        }

        _rend.material.color = Color.yellow;
        return false;
    }
}
