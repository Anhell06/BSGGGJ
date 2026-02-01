using System;
using UnityEngine;

public class RaccoonTarget : MonoBehaviour
{
    public static RaccoonTarget Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
