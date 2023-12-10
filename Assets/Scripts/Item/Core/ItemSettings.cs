using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreyGame.Items
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Data/Settings/Item Settings")]
    public class ItemSettings : ScriptableObject
    {
        public List<Item> Prefabs;
        public List<ItemData> allItemData;

        public float DefaultItemPrice = 1.0f;
    }
}