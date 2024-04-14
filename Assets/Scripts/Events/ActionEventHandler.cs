
using UnityEngine;
using UnityEngine.Events;

public class ActionEventHandler : MonoBehaviour
{
    public UnityEvent actionEvent;
    public LinkedAction linkedAction;
    private void Awake()
    {
        if (actionEvent != null && linkedAction != null)
        {
            for(int i = 0; i < actionEvent.GetPersistentEventCount(); i++)
            {
                if(actionEvent.GetPersistentTarget(i) is Object target)
                {
                    string method = actionEvent.GetPersistentMethodName(i);
                    UnityAction newAction = (UnityAction)UnityAction.CreateDelegate(typeof(UnityAction), target, method);
                    linkedAction.SubscribeToAction(newAction);
                }
                
            }
        }
    }

    private void OnDestroy()
    {
        if (actionEvent != null && linkedAction != null)
        {
            for (int i = 0; i < actionEvent.GetPersistentEventCount(); i++)
            {
                if (actionEvent.GetPersistentTarget(i) is Object target)
                {
                    string method = actionEvent.GetPersistentMethodName(i);
                    UnityAction newAction = (UnityAction)UnityAction.CreateDelegate(typeof(UnityAction), target, method);
                    linkedAction.UnsubscribeFromAction(newAction);
                }

            }
        }
    }
}
