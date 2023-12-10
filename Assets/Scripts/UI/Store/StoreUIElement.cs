using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using StoreyGame.Items;
using StoreyGame.Items.Store;

public class StoreUIElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] TextMeshProUGUI itemAmount;
    [SerializeField] TextMeshProUGUI itemPrice;
    [SerializeField] Image itemDisplay;

    private StoreUI m_controller;
    private ItemData m_itemData;

    public void Initialize(Store.StoreStockData data, StoreUI controller)
    {
        m_controller = controller;
        m_itemData = data.Data;

        if(itemName != null)
        {
            itemName.text = m_itemData.Name;
        }
        if (itemDescription != null)
        {
            itemDescription.text = m_itemData.Description;
        }
        if (itemAmount != null)
        {
            itemAmount.text = string.Format("x{0}", data.Stock.ToString());
        }
        if (itemPrice != null)
        {
            itemPrice.text = string.Format("{0} {1}", data.CurrentPrice.ToString("F2"), data.CurrencyType.ToString());
        }
        if(itemDisplay != null)
        {
            itemDisplay.sprite = m_itemData.DisplaySprite;
        }
    }
}
