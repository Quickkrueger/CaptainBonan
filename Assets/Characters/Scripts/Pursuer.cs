using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Pursuer : MonoBehaviour
{
    NavMeshAgent agent;
    AnimationControl animControl;

    [SerializeField]
    float escapeDistance = 5;


    private GameObject target;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animControl = GetComponent<AnimationControl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(target == null && other.tag == "Player")
        {
            target = other.gameObject;
            StartCoroutine(FollowTarget(new WaitForFixedUpdate()));
        }
    }

    private IEnumerator FollowTarget(WaitForFixedUpdate waitForFixedUpdate)
    {
        animControl.UpdateFloatProperty("Speed", agent.velocity.magnitude / agent.speed);
        agent.SetDestination(target.transform.position);

        if(Vector3.Distance(target.transform.position, transform.position) > escapeDistance)
        {
            target = null;
            agent.ResetPath();
            animControl.UpdateFloatProperty("Speed", 0);
            StopAllCoroutines();
        }
        yield return waitForFixedUpdate;
        StartCoroutine(FollowTarget(waitForFixedUpdate));
    }
}
