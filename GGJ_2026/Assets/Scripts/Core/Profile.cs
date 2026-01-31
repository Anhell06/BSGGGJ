using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[Serializable]
public class Profile
{
    [SerializeField]
    private ObservableCollection<PhotoCard> _photoCards = new ObservableCollection<PhotoCard>();
    public ObservableCollection<PhotoCard> PhotoCards => _photoCards;
}

[Serializable]
public class PhotoCard
{
    public Texture2D texture2D;
    public Texture2D dataTexture2D;
    public List<int> objectIds = new List<int>();
    public int rating;
    public int price;
    public QuestId finishQuest = QuestId.None;
}
