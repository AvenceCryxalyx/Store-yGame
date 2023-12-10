using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StoreyGame.Items.Store
{
    public class OnInteractStore : UnityEvent<StoreHandler> { }
    public class OnAccessStore : UnityEvent<StoreHandler> { }
    public class StoreHandler : MonoBehaviour, IInteractable
    {
        public OnAccessStore EvtAccessStore = new OnAccessStore();
        public OnInteractStore EvtInteractStore = new OnInteractStore();
        public string Id;

        [SerializeField] 
        private StoreData data;

        public void Interact()
        {
            EvtInteractStore.Invoke(this);
        }

        public void AccessStore()
        {

            EvtAccessStore.Invoke(this);
        }
    }
}