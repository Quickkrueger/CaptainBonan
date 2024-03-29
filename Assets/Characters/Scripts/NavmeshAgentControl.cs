using System;
using System.Collections;
using System.Collections.Generic;
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

    private UnityAction withinStoppingAction;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    public void SubscribeToStopping(UnityAction attack)
    {
        withinStoppingAction += attack;
    }

    public void UnsubscribeToStopping(UnityAction attack)
    {
        withinStoppingAction -= attack;
    }

    public void BeginFollow(Transform newTarget, AnimationControl animationControl)
    {
            target = newTarget;
            StartCoroutine(FollowTargetRoutine(new WaitForFixedUpdate(), animationControl));
    }

    private IEnumerator FollowTargetRoutine(WaitForFixedUpdate waitForFixedUpdate, AnimationControl animationControl)
    {
        animationControl.UpdateFloatProperty("Speed", agent.velocity.magnitude / agent.speed);
        agent.SetDestination(target.transform.position);

        if (Vector3.Distance(target.position, transform.position) > escapeDistance)
        {
            target = null;
            agent.ResetPath();
            animationControl.UpdateFloatProperty("Speed", 0);
            StopAllCoroutines();
        }
        else if (Vector3.Distance(target.position, transform.position) <= stopDistance - (stopDistance / 4))
        {
            withinStoppingAction.Invoke();
        }

        yield return waitForFixedUpdate;
        StartCoroutine(FollowTargetRoutine(waitForFixedUpdate, animationControl));
    }


}
