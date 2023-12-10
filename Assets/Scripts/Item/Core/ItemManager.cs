using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace StoreyGame.Items
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField]
        private ItemSettings itemSettings;
        public static ItemManager Instance { get; private set; }

        private Dictionary<string, Item> m_items = new Dictionary<string, Item>();

        private void Awake()
        {
            itemSettings = Instantiate(itemSettings, transform);

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            foreach(ItemData itd in itemSettings.allItemData)
            {
                Item newItem = new Item();
                newItem.Initialize(itd);
                m_items.Add(itd.Id, newItem);
            }
        }

        public Item GetItem(string id)
        {
            if(m_items.ContainsKey(id))
            {
                return m_items[id];
            }
            else
            {
                Debug.LogError("Item not found in ItemManager");
            }
            return null;
        }
    }
}
