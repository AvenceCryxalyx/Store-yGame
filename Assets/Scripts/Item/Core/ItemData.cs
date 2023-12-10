using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KenScripts.Game.Items;

namespace StoreyGame.Items
{
    [CreateAssetMenu(fileName = "Item Data", menuName = "Data/Items")]
    public class ItemData : ScriptableObject
    {
        public string Id;
        public string Name;
        public float Price;
        public ItemType Type;
        [TextArea] public string Description;
    }
}