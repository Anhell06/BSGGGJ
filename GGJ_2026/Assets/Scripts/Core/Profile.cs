using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class Profile
{
    [SerializeField]
    private ObservableCollection<PhotoCard> _photoCards;
    public ObservableCollection<PhotoCard> PhotoCards => _photoCards;
}

[Serializable]
public class PhotoCard
{
    public Texture2D texture2D;
    public List<int> objectIds;
    public int rating;
    public int price;
}
