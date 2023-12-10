using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreyGame.Common
{
    [CreateAssetMenu(fileName = "Wallet Data", menuName = "Data/Wallet")]
    public class WalletData : ScriptableObject
    {
        public CurrencyData[] InitialCurrencies;
    }
}
