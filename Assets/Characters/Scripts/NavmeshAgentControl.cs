using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;
[RequireComponent(typeof(NavMeshAgent))]
public class NavmeshAgentControl : MonoBehaviour
{
    [SerializeField]
    private float escapeDistance = 5;
    [SerializeField]
    private float stopDistance;

    private NavMeshAgent agent;
    private Transform target;
    [HideInInspector]
    public UnityAction StopAction;
    [HideInInspector]
    public UnityAction<float> MoveAction;

    Coroutine followRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance - (stopDistance * 0.1f);
    }

    public void BeginFollow(Transform newTarget)
    {
            target = newTarget;
        if (followRoutine == null)
        {
            followRoutine = StartCoroutine(FollowTargetRoutine(new WaitForFixedUpdate()));
        }
    }

    private IEnumerator FollowTargetRoutine(WaitForFixedUpdate waitForFixedUpdate)
    {

        MoveAction.Invoke(agent.velocity.magnitude / agent.speed);
        agent.SetDestination(target.transform.position);

        if (Vector3.Distance(target.position, transform.position) > escapeDistance)
        {
            target = null;
            agent.ResetPath();
            MoveAction.Invoke(0);
            StopCoroutine(followRoutine);
            followRoutine = null;
        }
        else if (Vector3.Distance(target.position, transform.position) <= stopDistance)
        {
            StopAction.Invoke();
        }

        yield return waitForFixedUpdate;
        followRoutine = StartCoroutine(FollowTargetRoutine(waitForFixedUpdate));
    }


}
