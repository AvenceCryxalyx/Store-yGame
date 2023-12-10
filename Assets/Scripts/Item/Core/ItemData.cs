using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KenScripts.Game.Items;

namespace StoreyGame.Items
{
    public enum ItemType
    {
        Materials,
        Consumables,
        Armor,
        Weapon,
        KeyItems,
        Clothes
    }

    [CreateAssetMenu(fileName = "Item Data", menuName = "Data/Items")]
    public class ItemData : ScriptableObject
    {
        public string Id;
        public string Name;
        public float BasePrice;
        public ItemType Type;
        public Sprite DisplaySprite;
        [TextArea] public string Description;
    }
}