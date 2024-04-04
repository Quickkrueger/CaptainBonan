using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthbarManager : MonoBehaviour
{
    public LinkedFloatAction healthUpdateAction;
    public Slider healthSlider;
    private Coroutine healthLerp;

    private void Awake()
    {
        healthUpdateAction.action += UpdateHealth;
    }
    public void UpdateHealth(float health)
    {
        if (healthLerp != null)
        {
            StopCoroutine(healthLerp);
        }

        healthLerp = StartCoroutine(LerpHealthRoutine(new WaitForFixedUpdate(), health));
    }

    private IEnumerator LerpHealthRoutine(WaitForFixedUpdate waitForFixed, float health)
    {
        float lerpAmount = (healthSlider.value - health) * Time.deltaTime;
        healthSlider.value -= lerpAmount;

        yield return waitForFixed;

        if(Mathf.Abs(healthSlider.value - health) <= 0.01f)
        {
            healthSlider.value = health;
            StopCoroutine(healthLerp);
        }
        else
        {
            healthLerp = StartCoroutine(LerpHealthRoutine(waitForFixed, health));
        }
    }

    private void OnDestroy()
    {
        healthUpdateAction.action -= UpdateHealth;
    }
}
