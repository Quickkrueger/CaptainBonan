using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public Graphic[] uiGraphics;
    float maxOpacity = 1f;

    public UnityEvent fadeInEvent;
    public float pauseTime = 1;
    public UnityEvent pauseEvent;
    public UnityEvent fadeOutEvent;

    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(new WaitForFixedUpdate(), 1));
    }

    public void PauseOnBlack()
    {
        StartCoroutine(Pause(new WaitForSeconds(pauseTime)));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(new WaitForFixedUpdate(), -1));
    }

    private IEnumerator Pause(WaitForSeconds waitForSeconds)
    {
        yield return waitForSeconds;
        pauseEvent.Invoke();
    }

    private IEnumerator FadeRoutine(WaitForFixedUpdate waitForFixed, float stepAmount)
    {
        if(uiGraphics.Length < 1)
        {
            StopAllCoroutines();
        }
        for (int i = 0; i < uiGraphics.Length; i++)
        {
            uiGraphics[i].color = uiGraphics[i].color + Color.black * stepAmount * Time.deltaTime;
        }
        yield return waitForFixed;

        if (stepAmount > 0 && uiGraphics[0].color.a >= maxOpacity)
        {
            fadeInEvent.Invoke();
        }
        else if (stepAmount < 0 && uiGraphics[0].color.a <= 0)
        {
            fadeOutEvent.Invoke();
        }
        else
        {
            StartCoroutine(FadeRoutine(waitForFixed, stepAmount));
        }
    }
}
