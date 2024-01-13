using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 0;

    private Rigidbody _rb;

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 moveVector)
    {
        Vector3 move3D = Vector3.zero;

        move3D.x = moveVector.x;
        move3D.z = moveVector.y;

        _rb.velocity = move3D * _moveSpeed;
        Rotate();
    }

    public void Rotate(float angle = 0)
    {

        if(angle == 0)
        {
            transform.rotation = Quaternion.LookRotation(transform.position + _rb.velocity, Vector3.up);
        }
    }
}
