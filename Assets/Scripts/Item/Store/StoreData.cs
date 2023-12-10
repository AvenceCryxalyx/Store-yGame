using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Items;
using StoreyGame.Common;

namespace StoreyGame.Items.Store
{
    public class StoreData : MonoBehaviour
    {
        public struct StoreItemData
        {
            public ItemData Data;
            public int InitialStock;
            public CurrencyData.CurrencyType CurrencyType;
        }

        public struct ItemSellingData
        {
            public ItemData[] ItemDatas;
            public CurrencyData.CurrencyType SellingCurrency;
        }

        public string Id;
        public string OwnerId;
        [TextArea] public string Description;

        [Range(0, 10)] public int ProtectionLevel;
        [Range(0.1f, 1.0f)] public float SellingPercentageReduction;

        public bool RandomizeInventory = true;

        [Tooltip("Defaults to accept any ItemType for selling if empty.")]
        public ItemType[] AcceptedItemsForSale;
        [Tooltip("For specific selling data by items.")]
        public ItemSellingData[] ItemSellingDatas;

        public CurrencyData.CurrencyType DefaultSellingCurrency;

        public StoreItemData[] PossibleItemData;
    }
}
