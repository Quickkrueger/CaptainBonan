using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(AnimationControl))]
[RequireComponent(typeof(NavmeshAgentControl))]
public class EnemyController : MonoBehaviour
{

    AnimationControl animControl;
    NavmeshAgentControl navmeshAgentControl;
    private GameObject target;
    UnityAction action;
    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<AnimationControl>();
        navmeshAgentControl = GetComponent<NavmeshAgentControl>();
        navmeshAgentControl.SubscribeToStopping(Attack);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (target == null && other.tag == "Player")
        {
            target = other.gameObject;
            navmeshAgentControl.BeginFollow(target.transform, animControl);
        }
    }

    private void OnDestroy()
    {
        navmeshAgentControl.UnsubscribeToStopping(Attack);
    }

    private void Attack()
    {

    }

    IEnumerator AttackRoutine(WaitForSeconds seconds)
    {
        yield return seconds;
    }
}
