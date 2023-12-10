using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEquipped : UnityEvent<IEquippable> { }
public class OnUnequipped : UnityEvent<IEquippable> { }
public interface IEquippable
{
    public void Equip(Avatar avatar);
    public void Unequip(Avatar avatar);
}
