using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Inputs", menuName = "ScriptableObjects/Inputs/InputSO", order = 0)]
public class InputSO : ScriptableObject 
{

    public InputActionMap map;
}
