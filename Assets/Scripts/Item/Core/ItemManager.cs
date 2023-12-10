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
        private Dictionary<ItemType, Item> m_itemPrefabs;
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


            foreach(Item item in itemSettings.Prefabs)
            {
                ItemType type = GetPrefabType(item);
                m_itemPrefabs[type] = item;
            }

            foreach(ItemData itd in itemSettings.allItemData)
            {
                Item newItem = Instantiate(m_itemPrefabs[itd.Type], transform);
                newItem.Initialize(itd);
                m_items.Add(itd.Id, newItem);
            }
        }

        public Item GetItem(string id)
        {
            if(m_items.ContainsKey(id))
            {
                return Instantiate(m_items[id]);
            }
            else
            {
                Debug.LogError("Item not found in ItemManager");
            }
            return null;
        }

        private ItemType GetPrefabType(Item item)
        {
            if(item is Clothes)
            {
                return ItemType.Clothes;
            }
            return ItemType.Materials;
        }

        public T GetInstance<T>(ItemData data)  where T : Item
        {
            if(data.Type == ItemType.Clothes)
            {
                return Instantiate<Clothes>(m_itemPrefabs[data.Type] as Clothes) as T;
            }
            else
            {
                return Instantiate<Item>(m_itemPrefabs[ItemType.Materials] as Item) as T;
            }
        }
    }
}
