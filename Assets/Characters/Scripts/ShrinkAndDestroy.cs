using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShrinkAndDestroy : MonoBehaviour
{
    Coroutine shrinkRoutine;
    public GameObject meshToShrink;
    public float shrinkRate = 0f;
    public float destroyDelay;
    public UnityEvent fullyShrunk;
    public void StartShrink()
    {
        if (shrinkRoutine == null)
        {
            shrinkRoutine = StartCoroutine(ShrinkRoutine(new WaitForFixedUpdate()));
        }
    }

    IEnumerator ShrinkRoutine(WaitForFixedUpdate waitForFixedUpdate)
    {

        meshToShrink.transform.localScale = meshToShrink.transform.localScale - (Vector3.one * Time.deltaTime * shrinkRate);
        yield return waitForFixedUpdate;
        if (meshToShrink.transform.localScale.magnitude > 0.02f)
        {
            shrinkRoutine = StartCoroutine(ShrinkRoutine(waitForFixedUpdate));
        }
        else
        {
            fullyShrunk.Invoke();
            Destroy(gameObject, destroyDelay);
        }
    }
}
