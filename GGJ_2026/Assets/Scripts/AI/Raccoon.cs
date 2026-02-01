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

    [SerializeField]
    private Rigidbody rb;

    private IEnumerator Start()
    {
        yield return null;
        _raccoonPlaceable.OnPicked += NoNoNo;
        _raccoonPlaceable.OnDroped += GoGoGo;
        GoGoGo();
    }

    private void GoGoGo()
    {
        StartCoroutine(GoGoGoRoutine());
    }

    private IEnumerator GoGoGoRoutine()
    {
        _agent.enabled = true;
        yield return null;
        _agent.enabled = true;
        _agent.SetDestination(RaccoonTarget.Instance.transform.position);
        _raccoonPlaceable.Rigidbody.isKinematic = true;
    }

    private void Update()
    {
        if(_agent.enabled)
            _agent.SetDestination(RaccoonTarget.Instance.transform.position);
    }

    private void NoNoNo()
    {
        _agent.enabled = false;
        rb = this.gameObject.AddComponent<Rigidbody>();
        _raccoonPlaceable.Rigidbody.isKinematic = false;
    }
    
    
}
