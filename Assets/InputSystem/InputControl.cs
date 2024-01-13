using System.Reflection;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class InputControl : MonoBehaviour
{
    [SerializeField]
    private InputSO inputs;

    [SerializeField]
    private UnityEvent<Vector2> moveEvent;

    public 

    void Awake()
    {
        for (int i = 0; i < inputs.map.actions.Count; i++)
        {
            inputs.map.actions[i].started += InputReceived;
            inputs.map.actions[i].performed += InputReceived;
            inputs.map.actions[i].canceled += InputReceived;

            inputs.map.actions[i].Enable();
        }

    }

    public void InputReceived(InputAction.CallbackContext context)
    {
        string tempFuncion;
        if (context.action.name != null)
        {
            tempFuncion = context.action.name;
            if (GetType().GetMethod(tempFuncion) != null)
            {
                MethodInfo method = GetType().GetMethod(tempFuncion);
                method.Invoke(this, new object[] { context });
            }
        }
    }


    public void CHANGEME(InputAction.CallbackContext context) //change change me to the exact name of the control added in the debug input scriptable object
    {
        if (context.started)
        {
            Debug.Log("Started" + "CHANGEME");
        }

        if (context.canceled)
        {
            Debug.Log("Canceled" + "CHANGEME");
        }

        if (context.performed)
        {
            Debug.Log("Performed" + "CHANGEME");
        }
    }

    public void Move(InputAction.CallbackContext context) //change change me to the exact name of the control added in the debug input scriptable object
    {
        Vector2 moveVector = Vector2.zero;
        if(context.valueType != typeof(Vector2))
        {
            return;
        }

        moveVector = context.ReadValue<Vector2>();

        moveEvent.Invoke(moveVector);

        if (context.started)
        {
        }

        if (context.canceled)
        {
        }

        if (context.performed)
        {
        }
    }

}