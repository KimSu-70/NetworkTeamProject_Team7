using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WHS_InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI potionCountText;
    [SerializeField] Button potionButton;
    [SerializeField] GameObject hpPotionPrefab;
    private int potionCount;
    private WHS_Inventory inventory;

    private void Start()
    {
        potionButton.onClick.AddListener(UsePotion);
        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        if (inventory != null)
        {
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.H))
            {
                UsePotion();
            }
        }
    }

    // �κ��丮 ������Ʈ�� ���� �÷��̾� ã��
    IEnumerator FindPlayer()
    {
        while (inventory == null)
        {
            yield return new WaitForSeconds(1f);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                inventory = player.GetComponent<WHS_Inventory>();
                Debug.Log("�κ��丮 ���");
            }
        }
    }

    public void UpdateUI()
    {
        potionCount = inventory.GetItemCount(ItemType.HP);
        potionCountText.text = $"{potionCount}";
        potionButton.interactable = potionCount > 0;
    }

    private void UsePotion()
    {
        if (potionCount > 0)
        {
            Debug.Log("���� ���");
            inventory.UseItem(ItemType.HP);
        }
        else
        {
            Debug.Log("�������� ������ �����ϴ�.");
        }
    }
}
