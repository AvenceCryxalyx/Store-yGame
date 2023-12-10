using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Common;
using KenScripts.Game.Items;

namespace StoreyGame.Items.Store
{
    public class Store : MonoBehaviour
    {
        public struct StoreStockData
        {
            public int Stock;
            public ItemData Data;
            public float CurrentPrice;
            public float SaleValue;
            public CurrencyData.CurrencyType CurrencyType;
        }

        private StoreData m_data;
        private Dictionary<string, StoreStockData> m_currentItems = new Dictionary<string, StoreStockData>();
        //private Inventory m_storeInventory;


        private Unit m_currentClient;
        private Wallet m_clientWallet;
        private Inventory m_clientInventory;

        public void Initialize(StoreData data)
        {
            m_data = Instantiate(data, transform);

            foreach(StoreData.StoreItemData sid in m_data.PossibleItemData)
            {
                StoreStockData newdata = new StoreStockData();
                newdata.Stock = sid.InitialStock;
                newdata.Data = sid.Data;
                newdata.SaleValue = 0.0f;
                newdata.CurrentPrice = sid.Data.BasePrice;
                m_currentItems[sid.Data.Id] = newdata;
            }
        }

        public IEnumerable<StoreStockData> GetCurrentItems()
        {
            return m_currentItems.Values;
        }

        public bool AccessStore(Unit client)
        {
            if (client.GetComponent<Wallet>() == null)
            {
                Debug.LogError("Client trying to access store without wallet");
                return false;
            }

            if (client.GetComponent<Inventory>() == null)
            {
                Debug.LogError("Client trying to access store without inventory");
                return false;
            }

            if (m_currentClient != null)
            {
                return false;
            }
            
            m_currentClient = client;
            m_clientWallet = m_currentClient.GetComponent<Wallet>();

            return true;
        }

        public void BuyItem(string id, int amount)
        {
            StoreStockData data = m_currentItems[id];
            Currency currency = m_clientWallet.GetCurrency(data.CurrencyType);
            if (data.Stock < amount)
            {
                Debug.LogError("Amount requested to buy is bigger than stock");
                return;
            }

            if(amount * data.CurrentPrice > currency.Amount)
            {
                Debug.LogErrorFormat("Client does not have enough funds - item price:{0}, {1}:{2}", amount, currency.Type.ToString(), currency.Amount);
                return;
            }

            currency.ReduceAmount(amount * data.CurrentPrice);
            m_clientInventory.AddItem(Instantiate(ItemManager.Instance.GetItem(id)), amount);
        }

        public void SellItem(Item item, int amount)
        {
            Currency currency = m_clientWallet.GetCurrency(m_data.DefaultSellingCurrency);
            if (m_clientInventory.GetItemCount(item.Id) < amount)
            {
                Debug.LogErrorFormat("Client doesn't have enough of the items in his inventory");
                return;
            }
            currency.AddAmount(amount * (item.BasePrice * m_data.SellingPercentageReduction));
            m_clientInventory.RemoveItem(item, amount);
        }
    }
}