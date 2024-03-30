using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New LinkedFloatAction", menuName = "Events/LinkedFloatAction", order = 0)]
public class LinkedFloatAction : ScriptableObject
{
    private UnityAction<float> action;

    public void SubscribeToAction(UnityAction<float> func)
    {
        action += func;
    }

    public void UnsubscribeFromAction(UnityAction<float> func)
    {
        action -= func;
    }

    public void InvokeAction(float value)
    {
        action.Invoke(value);
    }
}