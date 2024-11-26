using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WHS_InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI potionCountText;
    // TODO : ��ư �����̹���
    [SerializeField] Button potionButton;
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

    // �κ��丮�� ���� �÷��̾� ã��
    IEnumerator FindPlayer()
    {
        while (inventory == null)
        {
            yield return new WaitForSeconds(1f);

            foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
            {
                if (photonView.IsMine)
                {
                    GameObject player = photonView.gameObject;
                    inventory = player.GetComponent<WHS_Inventory>();

                    if (inventory != null)
                    {
                        Debug.Log($"�κ��丮 ��� �Ϸ� {photonView.ViewID}");
                        break;
                    }
                }
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
            inventory.UseItem(ItemType.HP);
        }
        else
        {
            Debug.Log("�������� ������ �����ϴ�.");
        }
    }
}
