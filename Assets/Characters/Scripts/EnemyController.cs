using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AnimationControl))]
[RequireComponent(typeof(NavmeshAgentControl))]
[RequireComponent (typeof(HealthControl))]
[RequireComponent(typeof(WeaponController))]
public class EnemyController : MonoBehaviour
{

    AnimationControl animControl;
    NavmeshAgentControl navmeshAgentControl;
    HealthControl healthControl;
    WeaponController weaponController;

    [Range(.1f, 2f)]
    public float attackSpeed;
    public UnityEvent deathEvent;

    Coroutine attackRoutine;
    // Start is called before the first frame update
    void Start()
    {
        animControl = GetComponent<AnimationControl>();
        navmeshAgentControl = GetComponent<NavmeshAgentControl>();
        healthControl = GetComponent<HealthControl>();
        weaponController = GetComponent<WeaponController>();

        navmeshAgentControl.StopAction += Attack;
        navmeshAgentControl.MoveAction += Move;
        healthControl.UpdateHealthAction += UpdateHealth;

        navmeshAgentControl.InitializeAgentControl();
        healthControl.InitializeHealth();

        
    }


    public void AcquireTarget(Collider other)
    {
        if (other.tag == "Player")
        {
            navmeshAgentControl.BeginFollow(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform == navmeshAgentControl.GetTarget())
        {
            if(attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }
        }
    }

    private void OnDestroy()
    {
        navmeshAgentControl.StopAction -= Attack;
        navmeshAgentControl.MoveAction -= Move;
        healthControl.UpdateHealthAction -= UpdateHealth;
    }

    private void Move(float speed)
    {
        animControl.UpdateFloatProperty("Speed", speed);
    }

    private void UpdateHealth(int health)
    {

        if(health <= 0)
        {
            animControl.UpdateTriggerProperty("Die");
            deathEvent.Invoke();
            this.enabled = false;
        }
        
    }

    private void Attack()
    {
        if (attackRoutine == null)
        {
            attackRoutine = StartCoroutine(AttackRoutine(new WaitForSeconds(attackSpeed / 2)));
        }
    }

    IEnumerator AttackRoutine(WaitForSeconds seconds)
    {
        
        yield return seconds;
        animControl.UpdateTriggerProperty("Attack");
        weaponController.UseWeapon();
        yield return seconds;
        attackRoutine = StartCoroutine(AttackRoutine(seconds));
    }
}
