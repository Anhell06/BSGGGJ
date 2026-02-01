using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Raccoon : MonoBehaviour
{
    [SerializeField]
    private PlaceableItem _raccoonPlaceable;

    [SerializeField]
    private NavMeshAgent _agent;

    private IEnumerator Start()
    {
        yield return null;
        _raccoonPlaceable.OnPicked += NoNoNo;
        _raccoonPlaceable.OnDroped += GoGoGo;
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
