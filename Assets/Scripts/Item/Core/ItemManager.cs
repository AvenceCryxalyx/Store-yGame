using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoreyGame.Items
{
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public Item GetItem(string id)
        {
            return null;
        }
    }
}
