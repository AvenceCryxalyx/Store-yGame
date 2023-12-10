using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StoreyGame.Items;
using StoreyGame.Items.Store;
using TMPro;
using UnityEngine.UI;
public class StoreUI : UiController
{
    [SerializeField] private StoreUIElement PrefabElement;
    [SerializeField] private RectTransform m_elementContainer;

    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] TextMeshProUGUI itemPrice;
    [SerializeField] Image itemDisplay;
    [SerializeField] Button buyButton;

    private Store m_store;
    private Item m_currentItem;

    private List<StoreUIElement> m_elements;

    // Start is called before the first frame update
    void Start()
    {
        StoreManager.Instance.EvtOnHandlerRegistered.AddListener(OnHandlerRegister);
        StoreManager.Instance.EvtOnHandlerUnregistered.AddListener(OnHandlerUnregister);
    }

    void OnHandlerRegister(StoreHandler handler)
    {
        handler.EvtAccessStore.AddListener(OnStoreAccesed);
    }

    void OnHandlerUnregister(StoreHandler handler)
    {
        handler.EvtAccessStore.RemoveListener(OnStoreAccesed);
    }

    void OnStoreAccesed(StoreHandler handler)
    {
        m_store = StoreManager.Instance.GetStore(handler);

        Initialize();

        Show();
    }

    public override void OnInitialized()
    {
        base.OnInitialized();

        foreach(Store.StoreStockData data in m_store.GetCurrentItems())
        {

        }
    }
    
    public void OnSelectItem(Item item)
    {

    }
}
