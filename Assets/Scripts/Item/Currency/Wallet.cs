using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreyGame.Common
{
    public class Wallet : MonoBehaviour
    {
        private Dictionary<CurrencyData.CurrencyType,Currency> m_currencies;
        private WalletData m_data;

        public void Initialize(WalletData data)
        {
            m_data = Instantiate(data,transform);

            foreach(CurrencyData cData in m_data.InitialCurrencies)
            {
                Currency newCurrency = Instantiate(cData.CurrencyPrefab, transform);
                newCurrency.Initialize(cData);
            }
        }

        public Currency GetCurrency(CurrencyData.CurrencyType type)
        {
            if(!m_currencies.ContainsKey(type))
            {
                return null;
            }
            return m_currencies[type];
        }

        public IEnumerable<Currency> GetAllCurrencies()
        {
            return m_currencies.Values;
        }
    }
}
