using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoreyGame.Common
{
    [CreateAssetMenu(fileName ="Currency Data", menuName = "Data/Currency")]
    public class CurrencyData : ScriptableObject
    {
        public enum CurrencyType
        {
            Cash,
            Credit
        }
        public Currency CurrencyPrefab;

        public string Id;

        [TextArea]
        public string Description;

        public CurrencyType Type;
        public float InitialValue;
    }
}