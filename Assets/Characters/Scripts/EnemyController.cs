using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimationControl))]
[RequireComponent(typeof(NavmeshAgentControl))]
public class EnemyController : MonoBehaviour
{

    AnimationControl animControl;
    NavmeshAgentControl navmeshAgentControl;
    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<AnimationControl>();
        navmeshAgentControl = GetComponent<NavmeshAgentControl>();
        navmeshAgentControl.StopAction += Attack;
        navmeshAgentControl.MoveAction += Move;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (target == null && other.tag == "Player")
        {
            target = other.gameObject;
            navmeshAgentControl.BeginFollow(target.transform);
        }
    }



    private void OnDestroy()
    {
        navmeshAgentControl.StopAction -= Attack;
        navmeshAgentControl.MoveAction -= Move;
    }

    private void Move(float speed)
    {
        animControl.UpdateFloatProperty("Speed", speed);
    }

    private void Attack()
    {

    }

    IEnumerator AttackRoutine(WaitForSeconds seconds)
    {
        yield return seconds;
    }
}
