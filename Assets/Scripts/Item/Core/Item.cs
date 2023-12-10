using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KenScripts.Game.Items;

namespace StoreyGame.Items
{
    public class ItemUsed : UnityEvent<Item> { }

    public class Item : MonoBehaviour
    {
        public ItemUsed EvtOnUse = new ItemUsed();

        public string Id { get { return m_data.Id; } }
        public string Description { get { return m_data.Description; } }
        public int Price { get { return m_data.Price; } }
        public ItemType Type { get { return m_data.Type; } }

        private ItemData m_data;
        public virtual bool Initialize(ItemData data)
        {
            //creates a duplicate of the data to prevent manipulation during runtime.
            m_data = Instantiate(data, transform);
            return true;
        }

        public void Use()
        {
            OnUse();
            EvtOnUse.Invoke(this);
        }

        public virtual void OnUse() { }
    }
}