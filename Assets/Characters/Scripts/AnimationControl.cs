using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationControl : MonoBehaviour
{
    private Animator _animator;
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateFloatProperty(string propertyName, float propertyValue)
    {
        _animator.SetFloat(propertyName, propertyValue);
    }

    public void UpdateIntProperty(string propertyName, int propertyValue)
    {

    }

    public void UpdateBoolProperty(string propertyName, bool propertyValue)
    {

    }
}
