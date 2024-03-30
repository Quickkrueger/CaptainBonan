using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MovementController))]
[RequireComponent (typeof(AnimationControl))]
[RequireComponent (typeof (HealthControl))]
public class CharacterControl : MonoBehaviour
{
    private MovementController _movementController;
    private AnimationControl _animationControl;
    private HealthControl _healthControl;

    private LinkedIntAction _healthUIAction;

    private void Start()
    {
        _movementController = GetComponent<MovementController>();
        _animationControl = GetComponent<AnimationControl>();
        _healthControl = GetComponent<HealthControl>();


        _healthControl.UpdateHealthAction += UpdateHealth;
        _healthControl.InitializeHealth();
    }

    public void MoveCharacter(Vector2 moveVector)
    {
        _movementController.Move(moveVector);
        _animationControl.UpdateFloatProperty("speed", Mathf.Clamp(moveVector.magnitude, 0, 1));
    }

    public void Primary()
    {
        _animationControl.UpdateTriggerProperty("shoot");
    }

    private void UpdateHealth(int newHealth)
    {
        if (_healthUIAction != null)
        {
            _healthUIAction.InvokeAction(newHealth);
        }

        if (newHealth <=  0)
        {
            
        }
    }
}
