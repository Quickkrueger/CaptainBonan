using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New LinkedIntAction", menuName = "Events/LinkedIntAction", order = 0)]
public class LinkedIntAction : ScriptableObject
{
    private UnityAction<int> action;

    public void SubscribeToAction(UnityAction<int> func)
    {
        action += func;
    }

    public void UnsubscribeFromAction(UnityAction<int> func)
    {
        action -= func;
    }

    public void InvokeAction(int value)
    {
        action.Invoke(value);
    }
}