using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(MovementController))]
[RequireComponent (typeof(AnimationControl))]
[RequireComponent (typeof (HealthControl))]
public class CharacterControl : MonoBehaviour
{
    private MovementController _movementController;
    private AnimationControl _animationControl;
    private HealthControl _healthControl;
    public WeaponController _weaponController;
    public SkinnedMeshRenderer _skinnedMeshRenderer;

    public LinkedFloatAction _healthUIAction;


    public GameObject _ragdoll;


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

    public void WeaponActivate()
    {
        _weaponController.transform.rotation = transform.rotation;
        _weaponController.UseWeapon();
        _weaponController.transform.localRotation = Quaternion.identity;
    }

    private void UpdateHealth(int newHealth)
    {
        if (_healthUIAction.action != null)
        {
            float healthRatio = (float)newHealth / (float)_healthControl.GetMaxHealth();
            _healthUIAction.InvokeAction(healthRatio);
        }

        if (newHealth <=  0)
        {
            GameObject ragdoll = Instantiate(_ragdoll);

            Transform[] ragTransforms = ragdoll.GetComponentsInChildren<Transform>();
            Transform[] transforms = GetComponentsInChildren<Transform>();

            for (int i = 0; i < ragTransforms.Length; i++)
            {
                ragTransforms[i].position = transforms[i].position;
                ragTransforms[i].rotation = transforms[i].rotation;
            }

            _skinnedMeshRenderer.material.SetFloat("_dead", 1);

            SkinnedMeshRenderer[] ragdollRenderers = ragdoll.GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i = 0;i < ragdollRenderers.Length; i++)
            {
                if (ragdollRenderers[i].gameObject.name == "Bananaman")
                {
                    ragdollRenderers[i].material.SetFloat("_dead", 1);
                    break;
                }
            }

            Destroy(gameObject);
        }
    }
}
