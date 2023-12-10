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
        public SlotSpace(int stacks, int maxStacks)
        {
            Stacks = stacks;
            MaxStacks = maxStacks;
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
        public IEnumerable<Item> Items { get { return m_items.Keys; } }
        public ItemAcquiring EvtItemAcquiring = new ItemAcquiring();
        public ItemAcquired EvtItemAcquired = new ItemAcquired();
        public ItemRemoved EvtItemRemoved = new ItemRemoved();
        public ItemUsed EvtItemUsed = new ItemUsed();
        public ItemOverflowed EvtItemOverflow = new ItemOverflowed();

        protected Dictionary<Item, SlotSpace> m_items = new Dictionary<Item, SlotSpace>();
        private InventoryData m_data;

        [SerializeField]
        protected int inventorySlotSize;

        [SerializeField]
        protected ItemType[] allowableItems;

        [SerializeField]
        protected bool canHaveMultipleStacks = true;

        public void Initialize(InventoryData data)
        {
            m_data = Instantiate(data, transform);
            allowableItems = data.AllowableItems;
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
            return m_items.Any(s => s.Key.Id == id);
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
            List<SlotSpace> amounts = m_items.Values.Where(i => i.Item.Id == id).ToList();
            foreach (SlotSpace s in amounts)
            {
                total += s.Stacks;
            }
            return total;
        }

        public void UseItem(Item item, GameObject target = null)
        {
            if (!m_items.ContainsKey(item))
            {
                Debug.LogError("Key not found in items.");
                return;
            }

            SlotSpace requestedItem = m_items[item];
            requestedItem.Item.Use();

            EvtItemUsed.Invoke(requestedItem.Item, 1);

            requestedItem.ReduceStack(1);
            if (requestedItem.Stacks <= 0)
            {
                m_items.Remove(item);
            }
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
            if (canHaveMultipleStacks)
            {
                if (!m_items.Keys.Any(x => x.Id == item.Id))
                {
                    Debug.LogError(string.Format("Inventory key with item id [%s] not found", item.Id));
                }
                else
                {
                    item = m_items.Keys.Where(x => x.Id == item.Id).ToList()[0];
                }

                if (!m_items.ContainsKey(item))
                {
                    int maxStack = 0;
                    if (GetTypeLimit(item) != 0)
                    {
                        maxStack = GetTypeLimit(item);
                    }
                    else if (GetItemLimit(item) > 0)
                    {
                        maxStack = InventoryData.DEFAULT_STACK_AMOUNT;
                    }
                    else
                    {
                        maxStack = InventoryData.DEFAULT_STACK_AMOUNT;
                    }
                    m_items.Add(item, new SlotSpace(amount, maxStack));
                    added = true;
#if UNITY_EDITOR
                    Debug.LogWarning("New Item Added");
#endif
                }
                else
                {
                    int remaining = m_items[item].AddStack(amount);
                    added = true;
                    if (remaining == 0)
                    {
#if UNITY_EDITOR
                        Debug.LogWarning(string.Format("Stack Added: %s", amount));
#endif
                    }
                    else
                    {
                        HandleStackOverflow(item, remaining, m_items[item].MaxStacks);
                    }
                }
            }
            else
            {
                if (!m_items.ContainsKey(item))
                {
                    int maxStack = 0;
                    int typeLimit = GetTypeLimit(item);
                    if (typeLimit > 0)
                    {
                        maxStack = typeLimit;
                    }
                    else
                    {
                        maxStack = InventoryData.DEFAULT_STACK_AMOUNT;
                    }
                    m_items.Add(item, new SlotSpace(0, maxStack));
                }
                int remaining = m_items[item].AddStack(amount);
                if (remaining == 0)
                {
#if UNITY_EDITOR
                    Debug.LogWarning(string.Format("Stack Added: %s", amount));
#endif
                }
                else
                {
                    HandleStackOverflow(item, remaining, m_items[item].MaxStacks);
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
            Assert.IsTrue(m_items.ContainsKey(item), "Item not found in inventory");

            SlotSpace itemSlot = m_items[item];

            Assert.IsTrue(amount <= itemSlot.Stacks, "Amount to be removed is greater than amount in inventory");

            if (itemSlot.Stacks == amount)
            {
                m_items.Remove(item);
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
                    HandleAmountDeficiency(item, remaining);
                }
            }

            EvtItemRemoved.Invoke(item, amount);
        }

//        /// <summary>
//        /// Remove method for stackable items.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="amount"></param>
//        public void RemoveItem(Item item, int amount)
//        {
//            Assert.IsTrue(m_items.ContainsKey(item), "Item not found in inventory");

//            if (canHaveMultipleStacks)
//            {
//                SlotSpace space = m_items[item];

//                SlotSpace itemSlot = (SlotSpace)m_items[key];

//                Assert.IsTrue(amount <= itemSlot.Stacks, "Amount to be removed is greater than amount in inventory");

//                if (itemSlot.Stacks == amount)
//                {
//                    m_items.Remove(key);
//                }
//                else
//                {
//                    int remaining = itemSlot.ReduceStack(amount);
//                    if (remaining == 0)
//                    {
//#if UNITY_EDITOR
//                        Debug.LogWarning(string.Format("Stack Reduced: %s", amount));
//#endif
//                    }
//                    else if (remaining > 0)
//                    {
//                        HandleAmountDeficiency(key, m_items[key].Item, remaining);
//                    }
//                }
//            }
//            else
//            {

//            }

//            EvtItemRemoved.Invoke(m_items[key].Item, amount);
//        }

        void HandleStackOverflow(Item item, int amount, int maxStack)
        {
            int remaining = amount;
            if (canHaveMultipleStacks)
            {
                Item newItem = Instantiate(item, transform);
                if (m_items.Values.Count <= inventorySlotSize)
                {
                    m_items.Add(newItem, new SlotSpace(0, maxStack));
                    remaining = m_items[item].AddStack(remaining);
                }
                if(remaining > 0)
                {
                    HandleStackOverflow(newItem, remaining, maxStack);
                }
            }
            if(remaining > 0) 
            { 
                EvtItemOverflow.Invoke(item, remaining);
            }
        }

        void HandleAmountDeficiency(Item item, int amount)
        {
            int remaining = amount;
            if (canHaveMultipleStacks)
            {
                List<Item> items = m_items.Keys.Where(x => x.Id == item.Id).ToList();

                item = items[items.Count - 1];

                remaining = m_items[item].ReduceStack(remaining);

                if(m_items[item].Stacks <= 0)
                {
                    m_items.Remove(item);
                    items.Remove(item);

                    Item newItem = m_items.Last(x => x.Key.Id == item.Id).Key;
                    if(newItem != null)
                    {
                        item = newItem;
                    }
                    else
                    {
                        HandleAmountDeficiency(item, remaining);
                    }
                }
            }
            if (remaining > 0)
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

        public int GetItemCount(string id)
        {
            int count = 0;
            if (canHaveMultipleStacks)
            {
                IEnumerable<SlotSpace> spaces = m_items.Values.Where(x => x.Item.Id == id);
                foreach (SlotSpace space in spaces)
                {
                    count += space.Stacks;
                }
            }
            else
            {
                SlotSpace space = m_items.FirstOrDefault(x => x.Value.Item.Id == id).Value;
                count = space.Stacks;
            }
            return count;
        }

        public SlotSpace GetSpace(Item key)
        {
            return m_items[key];
        }

        public IEnumerable<Item> GetItems()
        {
            return m_items.Keys;
        }

        public IEnumerable<SlotSpace> GetAllSlots()
        {
            return m_items.Values;
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
            foreach (SlotSpace slot in m_items.Values)
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
