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
[RequireComponent (typeof(AnimationControl))]
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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void BeginFollow(Transform newTarget)
    {
            target = newTarget;
            StartCoroutine(FollowTargetRoutine(new WaitForFixedUpdate()));
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
            StopAllCoroutines();
        }
        else if (Vector3.Distance(target.position, transform.position) <= stopDistance - (stopDistance / 4))
        {
            StopAction.Invoke();
        }

        yield return waitForFixedUpdate;
        StartCoroutine(FollowTargetRoutine(waitForFixedUpdate));
    }


}
