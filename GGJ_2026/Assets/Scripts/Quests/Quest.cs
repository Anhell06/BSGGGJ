using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestHolder: MonoBehaviour
{
    public static QuestHolder Instance { get; private set; }
    [SerializeField]
    private int _markersToFinishQuest = 5;
    public int MarkersToFinishQuest => _markersToFinishQuest;

    [SerializeField]
    private List<QuestMarker> _questMarkers;

    private void Awake()
    {
        Instance = this;
    }

    public QuestId CheckQuest(Camera camera)
    {
        var x = new Dictionary<QuestId, int>();
        foreach (var qm in _questMarkers)
        {
            if (qm.IsVisible(camera))
            {
                if (x.ContainsKey(qm.QuestId))
                {
                    x[qm.QuestId] = x[qm.QuestId] + 1;
                }
                else
                {
                    x[qm.QuestId] = 1;
                }

                if (x[qm.QuestId] >= _markersToFinishQuest)
                {
                    return qm.QuestId;
                }
            }
        }

        return QuestId.None;
    }
}

public static class QuestIdExtensions
{
    public static string ToQuestName(this QuestId q)
	{
        switch (q)
        {
            case QuestId.Quest01:
                return "Facade";
            case QuestId.Quest02:
                return "Courtyard";
            case QuestId.Quest03:
                return "Kitchen";
            case QuestId.Quest04:
                return "Basement";
            case QuestId.Quest05:
                return "Living room";
            case QuestId.Quest06:
                return "Bedroom";
            case QuestId.Quest07:
                return "Bedroom";
            default:
                return "Photo";

        }
    }
}
public enum QuestId
{
    Quest01 = 1,
    Quest02 = 2,
    Quest03 = 3,
    Quest04 = 4,
    Quest05 = 5,
    Quest06 = 6,
    Quest07 = 7,
    
    
    None = 99
}