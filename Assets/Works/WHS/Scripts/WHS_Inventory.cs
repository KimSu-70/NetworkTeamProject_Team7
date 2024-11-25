using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Inventory : MonoBehaviour
{
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();    

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

    public bool UseItem(ItemType type)
    {
        if(items.ContainsKey(type) && items[type] > 0)
        {
            Debug.Log("�������� ����մϴ�.");
            items[type]--;
            return true;
        }

        Debug.Log($"{type} �������� �����ϴ�");
        return false;
    }

    public int GetItemCount(ItemType type)
    {
        return items.ContainsKey(type) ? items[type] : 0;
    }


}
