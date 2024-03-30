using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New LinkedAction", menuName = "Events/LinkedAction", order = 0)]
public class LinkedAction : ScriptableObject
{
    private UnityAction action;

    public void SubscribeToAction(UnityAction func)
    {
        action += func;
    }

    public void UnsubscribeFromAction(UnityAction func)
    {
        action -= func;
    }

    public void InvokeAction()
    {
        action.Invoke();
    }
}

