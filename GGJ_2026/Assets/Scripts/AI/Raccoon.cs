using System;
using UnityEngine;
using UnityEngine.AI;

public class Raccoon : MonoBehaviour
{
    [SerializeField]
    private PlaceableItem _raccoonPlaceable;

    [SerializeField]
    private NavMeshAgent _agent;

    private void Awake()
    {
        GoGoGo();
    }

    private void GoGoGo()
    {
        _agent.enabled = true;
        _agent.SetDestination(RaccoonTarget.Instance.transform.position);
    }

    private void NoNoNo()
    {
        _agent.enabled = false;
    }
    
    
}
