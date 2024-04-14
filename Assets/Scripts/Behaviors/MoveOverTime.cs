using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
    public float speed;
    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
