using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Items;

public class Clothes : Item, IEquippable
{
    public OnEquipped EvtEquipped = new OnEquipped();
    public OnUnequipped EvtUnequipped = new OnUnequipped();

    private ClothesData m_data;

    public override bool Initialize(ItemData data)
    {
        if(!(data is ClothesData))
        {
            Debug.LogError("Data provided is not a ClothesData");
        }
        //creates a duplicate of the data to prevent manipulation during runtime.
        m_data = Instantiate(data as ClothesData, transform);
        return true;
    }

    public void Equip(Avatar avatar)
    {
        avatar.OverrideAnimations(m_data.animData);
        EvtEquipped.Invoke(this);
    }

    public void Unequip(Avatar avatar)
    {
        avatar.OverrideAnimations(null);
        EvtUnequipped.Invoke(this);
    }
}
