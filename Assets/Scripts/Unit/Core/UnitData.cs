using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Unit Data", menuName ="Data")]
public class UnitData : ScriptableObject
{
    public string UnitName;
    public string Description;
    public float MovementSpeed;
}
