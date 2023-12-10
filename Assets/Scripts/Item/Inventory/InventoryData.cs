using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Items;

namespace KenScripts.Game.Items
{


    [CreateAssetMenu(fileName = "Inventory Data", menuName = "Data/Inventory")]
    public class InventoryData : ScriptableObject
    {
        public struct ItemTypeStackData
        {
            public ItemType Type;
            public int MaxStackCount;
        }

        public struct ItemStackData
        {
            public ItemData Data;
            public int MaxStackCount;
        }

        [Range(0, 10)] public int protectionLevel;
        public int SlotSpace;

        [Tooltip("Items that are allowed to be stored in this inventory, if empty defaults to allow all items")]
        public ItemType[] AllowableItems;

        [Tooltip("For Item Specific type limiting")]
        public ItemTypeStackData[] TypeStackDatas;

        [Tooltip("For Item Specific stack limiting")]
        public ItemStackData[] ItemStackDatas;

        public static int DEFAULT_STACK_AMOUNT = 99;
        public static int DEFAULT_SLOT_COUNT = 30;
    }
}
