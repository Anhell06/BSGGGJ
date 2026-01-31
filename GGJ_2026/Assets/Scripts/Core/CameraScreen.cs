using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraScreen : Screen
{
    public Image canFinishQuest;

    private void Update()
    {
        canFinishQuest.color = PhotoMaker.CanFinishAnyQuest ? Color.green : Color.red;
        var clr = canFinishQuest.color;
        clr.a = 0.5f;
        canFinishQuest.color = clr;
    }
}