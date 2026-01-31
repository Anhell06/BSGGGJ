using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Profile
{
    [SerializeField]
    private List<PhotoCard> _photoCards;
    public List<PhotoCard> PhotoCards => _photoCards;

    
}

[Serializable]
public class PhotoCard
{
    public Texture2D texture2D;
    public List<int> objectIds;
    public int rating;
    public int price;
}
