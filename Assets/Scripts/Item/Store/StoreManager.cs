using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StoreyGame.Items.Store
{
    public class OnStoreRegistered : UnityEvent<StoreHandler> { }
    public class OnStoreUnregistered : UnityEvent<StoreHandler> { }

    public class StoreManager : MonoBehaviour
    {
        public Store Prefab;
        public static StoreManager Instance { get; private set; }
        public OnStoreRegistered EvtOnHandlerRegistered = new OnStoreRegistered();
        public OnStoreUnregistered EvtOnHandlerUnregistered = new OnStoreUnregistered();

        private Dictionary<StoreHandler, Store> m_allStores = new Dictionary<StoreHandler, Store>();

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

        public IEnumerable<StoreHandler> GetHandlers()
        {
            return m_allStores.Keys;
        }

        public Store GetStore(StoreHandler handler)
        {
            if (!m_allStores.ContainsKey(handler))
            {
                Debug.LogErrorFormat("Handler not found in store manager, {0}", handler.gameObject.name);
                return null;
            }
            return m_allStores[handler];
        }

        public void Register(StoreHandler handler, StoreData data)
        {
            if (m_allStores.ContainsKey(handler))
            {
                Debug.LogErrorFormat("Handler already registered in store manager, {0}", handler.gameObject.name);
                return;
            }
            m_allStores[handler] = Instantiate(Prefab, handler.transform);

            EvtOnHandlerRegistered.Invoke(handler);
        }

        public void UnRegister(StoreHandler handler)
        {
            if(!m_allStores.ContainsKey(handler))
            {
                Debug.LogErrorFormat("Handler not found in store manager, {0}", handler.gameObject.name);
                return;
            }
            Destroy(m_allStores[handler]);
            m_allStores.Remove(handler);

            EvtOnHandlerUnregistered.Invoke(handler);
        }
    }
}
