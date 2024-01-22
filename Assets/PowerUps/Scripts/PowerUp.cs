using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PowerUp", menuName = "ScriptableObjects/Rogue/PowerUp", order = 0)]
public class PowerUp : ScriptableObject
{
    [SerializeField]
    UpgradeType upgradeType;

    [SerializeField]
    float upgradeValue;
     
}

public enum UpgradeType
{
    Damage,
    Speed,
    AttackSpeed,
    Health,

}
