using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Assertions;
using Newtonsoft.Json.Linq;
using StoreyGame.Items;

namespace KenScripts.Game.Items
{
    public class ItemAcquiring : UnityEvent<Item, int> { }
    public class ItemAcquired : UnityEvent<Item, bool> { }
    public class ItemRemoved : UnityEvent<Item, int> { }
    public class ItemUsed : UnityEvent<Item, int> { }
    public class ItemOverflowed : UnityEvent<Item, int> { }

    public class SlotSpace
    {
        public SlotSpace(Item item, int stacks, int maxStacks)
        {
            Item = item;
            Stacks = stacks;
        }

        public Item Item { get; private set; }
        public int Stacks { get; private set; }
        public int MaxStacks { get; private set; }
        /// <summary>
        /// Adds the given amount to the stack of the slot.
        /// </summary>
        /// <param name="stackToAdd">Amount to be added.</param>
        /// <returns>It returns the leftover amount if there are any.</returns>
        public int AddStack(int stackToAdd)
        {
            int tempStack = Stacks + stackToAdd;
            if (tempStack > MaxStacks)
            {
                Stacks = MaxStacks;
                return (tempStack - MaxStacks);
            }

            Stacks += stackToAdd;
            return 0;
        }

        /// <summary>
        /// Reduces the given amount to the stack of the slot.
        /// </summary>
        /// <param name="stackToReduce">Amount to be reduced</param>
        /// <returns>It returns the lacking amount if the amount to reduce is bigger than the stack of the slot. Normally must not be possible </returns>
        public int ReduceStack(int stackToReduce)
        {
            int tempStack = Stacks - stackToReduce;
            if (tempStack < 0)
            {
                Stacks = 0;
                return Mathf.Abs(tempStack);
            }
            Stacks -= stackToReduce;
            return 0;
        }
    }

    public class Inventory : MonoBehaviour//, IJsonSerializable
    {
        public struct SlotKey
        {
            public string ItemId;
            public int index;
        }

        public IEnumerable<SlotSpace> Items { get { return items.Values; } }
        public ItemAcquiring EvtItemAcquiring = new ItemAcquiring();
        public ItemAcquired EvtItemAcquired = new ItemAcquired();
        public ItemRemoved EvtItemRemoved = new ItemRemoved();
        public ItemUsed EvtItemUsed = new ItemUsed();
        public ItemOverflowed EvtItemOverflow = new ItemOverflowed();

        protected Dictionary<SlotKey, SlotSpace> items = new Dictionary<SlotKey, SlotSpace>();
        private InventoryData m_data;

        [SerializeField]
        protected int inventorySlotSize;

        [SerializeField]
        ItemType[] allowableItems;

        [SerializeField]
        protected bool canHaveMultipleStacks = true;

        public void Initialize(InventoryData data)
        {
            m_data = Instantiate(data, transform);
        }

        public void IncreaseInventorySize(int value)
        {
            inventorySlotSize += value;
        }

        public void DecreaseInventorySize(int value)
        {
            inventorySlotSize -= value;
        }

        public bool IsItemAvailable(string id)
        {
            return items.Any(s => s.Key.ItemId == id);
        }

        public int GetAvailableAmount(string id)
        {
            if (!IsItemAvailable(id))
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("Item not found", id));
#endif
                return -1;
            }

            return GetAllSlotAmounts(id);
        }

        int GetAllSlotAmounts(string id)
        {
            int total = 0;
            List<SlotSpace> amounts = items.Values.Where(i => i.Item.Id == id).ToList();
            foreach (SlotSpace s in amounts)
            {
                total += s.Stacks;
            }
            return total;
        }

        public void UseItem(SlotKey key, GameObject target = null)
        {
            if (!items.Keys.Contains(key))
            {
                Debug.LogError("Key not found in items.");
                return;
            }

            SlotSpace requestedItem = items[key];
            requestedItem.Item.Use();

            EvtItemUsed.Invoke(requestedItem.Item, 1);

            requestedItem.ReduceStack(1);
            if (requestedItem.Stacks <= 0) items.Remove(key);
        }

        public void AddItem(Item item, int amount)
        {
            bool isItemAllowed = true;
            if (allowableItems.Length > 0)
            {
                isItemAllowed = false;
                foreach (ItemType type in allowableItems)
                {
                    if (item.Type == type)
                    {
                        isItemAllowed = true;
                        break;
                    }
                }

                if (!isItemAllowed)
                {
                    Debug.LogError("Item is not allowed in this storage");
                }
            }

            bool added = false;
            EvtItemAcquiring.Invoke(item, amount);
            SlotKey key;

            if (!items.Keys.Any(x => x.ItemId == item.Id))
            {
                Debug.LogError(string.Format("Inventory key with item id [%s] not found", item.Id));
                key.ItemId = item.Id;
                key.index = 0;
            }
            else
            {
                key = items.Keys.Where(x => x.ItemId == item.Id)
                                             .OrderByDescending(x => x.index).ToList()[0];
            }

            if (!items.ContainsKey(key))
            {
                int maxStack = 0;
                if (GetTypeLimit(item) != 0)
                {
                    maxStack = GetTypeLimit(item);
                }
                else if (GetItemLimit(item) != 0)
                {

                }
                else
                {
                    maxStack = InventoryData.DEFAULT_STACK_AMOUNT;
                }
                items.Add(key, new SlotSpace(item, amount, maxStack));
                added = true;
#if UNITY_EDITOR
                Debug.LogWarning("New Item Added");
#endif
            }
            else
            {
                int remaining = items[key].AddStack(amount);
                if (remaining == 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(string.Format("Stack Added: %s", amount));
#endif
                }
                else
                {
                    HandleStackOverflow(key, item,remaining, items[key].MaxStacks);
                }
            }
            EvtItemAcquired.Invoke(item, added);
        }

        /// <summary>
        /// Remove method for non stackable items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void RemoveItem(Item item, int amount)
        {
            SlotKey key;

            key = items.Keys.Where(x => x.ItemId == item.Id)
                            .OrderByDescending(x => x.index).ToList()[0];

            Assert.IsTrue(items.ContainsKey(key), "Item not found in inventory");

            SlotSpace itemSlot = (SlotSpace)items[key];

            Assert.IsTrue(amount <= itemSlot.Stacks, "Amount to be removed is greater than amount in inventory");

            if (itemSlot.Stacks == amount)
            {
                items.Remove(key);
            }
            else
            {
                int remaining = itemSlot.ReduceStack(amount);
                if (remaining == 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(string.Format("Stack Reduced: %s", amount));
#endif
                }
                else if (remaining > 0)
                {
                    HandleAmountDeficiency(key, item, remaining);
                }
            }

            EvtItemRemoved.Invoke(item, amount);
        }

        /// <summary>
        /// Remove method for stackable items.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        public void RemoveItem(string id, int amount)
        {
            SlotKey key;

            key = items.Keys.Where(x => x.ItemId == id)
                            .OrderByDescending(x => x.index).ToList()[0];

            Assert.IsTrue(items.ContainsKey(key), "Item not found in inventory");

            SlotSpace itemSlot = (SlotSpace)items[key];

            Assert.IsTrue(amount <= itemSlot.Stacks, "Amount to be removed is greater than amount in inventory");

            if (itemSlot.Stacks == amount)
            {
                items.Remove(key);
            }
            else
            {
                int remaining = itemSlot.ReduceStack(amount);
                if (remaining == 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(string.Format("Stack Reduced: %s", amount));
#endif
                }
                else if (remaining > 0)
                {
                    HandleAmountDeficiency(key, items[key].Item, remaining);
                }
            }

            EvtItemRemoved.Invoke(items[key].Item, amount);
        }

        void HandleStackOverflow(SlotKey key, Item item, int amount, int maxStack)
        {
            int remaining = amount;
            if (canHaveMultipleStacks)
            {
                while (remaining != 0)
                {
                    SlotKey newKey;
                    newKey.ItemId = item.Id;
                    newKey.index = key.index + 1;
                    items.Add(newKey, new SlotSpace(item, 0, maxStack));
                    remaining = items[newKey].AddStack(remaining);
                    key = newKey;
                }
            }
            else
            {
                EvtItemOverflow.Invoke(item, amount);
            }
        }

        void HandleAmountDeficiency(SlotKey key, Item item, int amount)
        {
            int remaining = amount;
            if (canHaveMultipleStacks)
            {
                while (remaining != 0)
                {
                    if (key.index == 0)
                    {
                        Debug.LogError("Stacks to be removed bigger than actual amount of items.");
                        break;
                    }

                    key = items.Keys.Where(x => x.ItemId == item.Id)
                        .OrderByDescending(y => y.index).ToList()[0];
                    remaining = items[key].ReduceStack(remaining);
                }
            }
            else
            {
                Debug.LogError("Stacks to be removed bigger than actual amount of items.");
            }
        }

        public int GetItemLimit(Item item)
        {
            if (HasItemLimit(item.Id))
            {
                return m_data.ItemStackDatas.First(x => x.Data.Id == item.Id).MaxStackCount;
            }
            return -1;
        }    

        public int GetTypeLimit(Item item)
        {
            if(HasTypeLimit(item.Type))
            {
                return m_data.TypeStackDatas.First(x => x.Type == item.Type).MaxStackCount;
            }
            return -1;
        }

        public bool HasItemLimit(string id)
        {
            return m_data.ItemStackDatas.Any(x => x.Data.Id == id);
        }

        public bool HasTypeLimit(ItemType type)
        {
            return m_data.TypeStackDatas.Any(x => x.Type == type);
        }

        public JToken Serialize()
        {
            JObject json = new JObject();
            JArray inventory = new JArray();
            json["inventory-size"] = inventorySlotSize;
            foreach (SlotSpace slot in items.Values)
            {
                JObject item = new JObject();
                item["item-id"] = slot.Item.Id;
                item["stacks"] = slot.Stacks;
                inventory.Add(item);
            }

            json["items"] = inventory;
            return json;
        }

        public void Deserialize(JToken json)
        {
            inventorySlotSize = (int)json["inventory-size"];

            foreach (JToken item in json["items"])
            {
                AddItem(ItemManager.Instance.GetItem((string)item["item-id"]), (int)item["stacks"]);
            }
        }
    }
}
