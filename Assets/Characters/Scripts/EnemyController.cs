using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimationControl))]
[RequireComponent(typeof(NavmeshAgentControl))]
[RequireComponent (typeof(HealthControl))]
public class EnemyController : MonoBehaviour
{

    AnimationControl animControl;
    NavmeshAgentControl navmeshAgentControl;
    HealthControl healthControl;
    [Range(.1f, 2f)]
    public float attackSpeed;

    Coroutine attackRoutine;
    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<AnimationControl>();
        navmeshAgentControl = GetComponent<NavmeshAgentControl>();
        healthControl = GetComponent<HealthControl>();

        navmeshAgentControl.InitializeAgentControl();
        healthControl.InitializeHealth();

        navmeshAgentControl.StopAction += Attack;
        navmeshAgentControl.MoveAction += Move;
        healthControl.UpdateHealthAction += UpdateHealth;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            navmeshAgentControl.BeginFollow(other.transform);
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

    private void UpdateHealth(int health)
    {
        
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
