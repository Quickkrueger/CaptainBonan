using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New LinkedFloatAction", menuName = "Events/LinkedFloatAction", order = 0)]
public class LinkedFloatAction : ScriptableObject
{
    public UnityAction<float> action;


    public void InvokeAction(float value)
    {
        action.Invoke(value);
    }
}