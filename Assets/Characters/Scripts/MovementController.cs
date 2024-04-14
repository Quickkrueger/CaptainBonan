using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 0;

    private Rigidbody _rb;
    private Vector3 _moveVector = Vector3.zero;
    private Vector3 _lookVector = Vector3.zero;

    private Coroutine _movementRoutine;
    private WaitForFixedUpdate _wffu = new WaitForFixedUpdate();

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector2 moveVector)
    {

        _moveVector.x = moveVector.x;
        _moveVector.z = moveVector.y;

        if( _moveVector.magnitude > 0)
        {
            _movementRoutine = StartCoroutine(UpdateMovement());
        }
        else if(_movementRoutine != null)
        {
            StopCoroutine(_movementRoutine);
            _rb.velocity = Vector3.zero;
            _movementRoutine = null;
        }
    }

    public IEnumerator UpdateMovement()
    {
        _rb.velocity = _moveVector * _moveSpeed;
        Rotate();
        yield return _wffu;

        _movementRoutine = StartCoroutine(UpdateMovement());

    }

    public void Rotate()
    {

        if(_lookVector == Vector3.zero && _moveVector != Vector3.zero)
        {
            _rb.rotation = Quaternion.LookRotation(_moveVector);
        }
        else if(_lookVector != Vector3.zero)
        {
            _rb.rotation = Quaternion.LookRotation(_lookVector);
        }
    }
}
