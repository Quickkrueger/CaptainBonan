using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
    Coroutine shrinkRoutine;
    public void StartShrink()
    {
        if (shrinkRoutine == null)
        {
            shrinkRoutine = StartCoroutine(ShrinkRoutine(new WaitForFixedUpdate()));
        }
    }

    IEnumerator ShrinkRoutine(WaitForFixedUpdate waitForFixedUpdate)
    {

        transform.localScale = transform.localScale - Vector3.one * Time.deltaTime;
        yield return waitForFixedUpdate;
        if (transform.localScale.magnitude > 0.02f)
        {
            shrinkRoutine = StartCoroutine(ShrinkRoutine(waitForFixedUpdate));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
