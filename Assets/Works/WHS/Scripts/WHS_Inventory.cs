using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Inventory : MonoBehaviour
{
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
    private StatusModel statusModel;

    private void Awake()
    {
        statusModel = GetComponent<StatusModel>();
        if(statusModel == null)
        {
            Debug.LogError("statusModel�� ã������");
        }
    }

    public void AddItem(ItemType type, int amount)
    {
        if (items.ContainsKey(type))
        {
            items[type] += amount;
        }
        else
        {
            items[type] = amount;
        }
    }

    public void UseItem(ItemType type)
    {
        if (items.ContainsKey(type) && items[type] > 0)
        {
            items[type]--;

            if (WHS_ItemManager.Instance.itemData.TryGetValue(type, out WHS_Item item))
            {
                Debug.Log($"������ {item.type}, {item.value}");

                WHS_ItemManager.Instance.ApplyItem(statusModel, item);
            }
        }
    }

    public int GetItemCount(ItemType type)
    {
        return items.ContainsKey(type) ? items[type] : 0;
    }
}
