using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimationControl))]
[RequireComponent(typeof(NavmeshAgentControl))]
public class EnemyController : MonoBehaviour
{

    AnimationControl animControl;
    NavmeshAgentControl navmeshAgentControl;
    private GameObject target;
    [Range(.1f, 2f)]
    public float attackSpeed;

    Coroutine attackRoutine;
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
        if (attackRoutine != null & speed > 0.3f)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        animControl.UpdateFloatProperty("Speed", speed);
    }

    private void Attack()
    {
        if (attackRoutine == null)
        {
            attackRoutine = StartCoroutine(AttackRoutine(new WaitForSeconds(attackSpeed)));
        }
    }

    IEnumerator AttackRoutine(WaitForSeconds seconds)
    {
        animControl.UpdateTriggerProperty("Attack");
        yield return seconds;
        attackRoutine = StartCoroutine(AttackRoutine(seconds));
    }
}
