using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreyGame.Items
{
    [CreateAssetMenu(fileName = "Item Settings", menuName = "Data/Settings")]
    public class ItemSettings : ScriptableObject
    {
        public List<ItemData> allItemData;

        public float DefaultItemPrice = 1.0f;
    }
}