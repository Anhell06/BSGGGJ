using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Реестр переносимых предметов. Хранит ссылки на все PlaceableItem (префабы или объекты в сцене).
/// </summary>
[CreateAssetMenu(fileName = "PlaceableItemRegistry", menuName = "Placement/Placeable Item Registry")]
public class PlaceableItemRegistry : ScriptableObject
{
    [SerializeField] private List<PlaceableItem> items = new List<PlaceableItem>();

    public IReadOnlyList<PlaceableItem> Items => items;

    public void Add(PlaceableItem item)
    {
        if (item != null && !items.Contains(item))
            items.Add(item);
    }

    public void Remove(PlaceableItem item)
    {
        items.Remove(item);
    }

    public void RemoveAt(int index)
    {
        if (index >= 0 && index < items.Count)
            items.RemoveAt(index);
    }

    public void Clear()
    {
        items.Clear();
    }
}
