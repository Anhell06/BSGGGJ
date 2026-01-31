using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    [SerializeField]//[HideInInspector]
    private Color _color;

    [SerializeField]
    private Renderer _renderer;

    public void SetTechnicalColor(Color color)
    {
        _color = color;
    }

    public void Prepare()
    {
        _renderer.material.color = _color;
    }

    public void Restore()
    {
        _renderer.material.color = Color.white;
    }
}
