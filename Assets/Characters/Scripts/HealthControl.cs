using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthControl : MonoBehaviour, IDamageable
{

    [SerializeField]
    private int maxHealth;
    private int currentHealth;

    public UnityAction<int> UpdateHealthAction;

    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        UpdateHealth();
    }

    public void TakeDamage(int damageAmount)
    {
        GetComponent<AudioSource>().Play();
        currentHealth = Mathf.Clamp(currentHealth - damageAmount,0, maxHealth);
        UpdateHealth();
    }

    public void GainHealth(int healAmount)
    {

        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, maxHealth);
        UpdateHealth();

    }

    private void UpdateHealth()
    {
        if (UpdateHealthAction != null)
        {
            UpdateHealthAction.Invoke(currentHealth);
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    // Start is called before the first frame update

}
