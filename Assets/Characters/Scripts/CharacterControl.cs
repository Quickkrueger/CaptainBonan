using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MovementController))]
[RequireComponent (typeof(AnimationControl))]
public class CharacterControl : MonoBehaviour
{
    private MovementController _movementController;
    private AnimationControl _animationControl;

    private void Start()
    {
        _movementController = GetComponent<MovementController>();
        _animationControl = GetComponent<AnimationControl>();
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
}
