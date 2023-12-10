using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StoreyGame.Common
{
    public class OnCurrencyValueUpdated : UnityEvent<Currency, float> { }
    public class Currency : MonoBehaviour
    {
        private CurrencyData m_data;
        public string Id { get { return m_data.Id; } }
        public string Description { get { return m_data.Description; } }
        public float Amount { get; private set; }
        public CurrencyData.CurrencyType Type { get { return m_data.Type; } }
        public OnCurrencyValueUpdated EvtValueUpdated = new OnCurrencyValueUpdated();

        public bool IsInitialized { get; private set; }

        public void Initialize(CurrencyData data)
        {
            m_data = data;
            Amount = m_data.InitialValue;
            IsInitialized = true;
        }

        public bool HasEnoughAmount(float amountNeeded)
        {
            return amountNeeded < Amount;
        }

        public bool AddAmount(float amount)
        {
            if(!IsInitialized)
            {
                Debug.LogError("Currency not yet initialized");
                return false;
            }

            float prevAmount = Amount;

            Amount += amount;

            EvtValueUpdated.Invoke(this, prevAmount);

            return true;
        }

        public bool ReduceAmount(float amount)
        {
            if (!IsInitialized)
            {
                Debug.LogError("Currency not yet initialized");
                return false;
            }

            if (!HasEnoughAmount(amount))
            {
                Debug.LogErrorFormat("Not enough {0}, My amount {1}, amount needed{2}", Type.ToString(), Amount, amount);
                return false;
            }
            float prevAmount = Amount;

            Amount -= amount;

            EvtValueUpdated.Invoke(this, prevAmount);

            return true;
        }
    }
}
