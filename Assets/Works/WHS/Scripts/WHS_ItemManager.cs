using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WHS_ItemManager : MonoBehaviourPun
{
    [System.Serializable]
    public class ItemPrefab
    {
        public ItemType type;
        public GameObject prefab;
    }

    [SerializeField] ItemPrefab[] itemPrefabs;
    [SerializeField] GameObject chestPrefab;
    [SerializeField] Vector3 chestPos;
    //[SerializeField] float chestDistance;
    // private List<WHS_Chest> chests = new List<WHS_Chest>();

    [SerializeField] public GameObject[] hpPotionPrefabs;
    public UnityAction<int> OnPotionGradeChanged;
    private int hpPotionGrade = 1;


    public Dictionary<ItemType, WHS_Item> itemData = new Dictionary<ItemType, WHS_Item>();

    public static WHS_ItemManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            InitItemData();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        // TODO : ���� ��� �� ������ ����
        if (Input.GetKeyDown(KeyCode.I))
        {
            Vector3 spawnPos = new Vector3(Random.Range(0, 5), 1, Random.Range(0, 5));
            SpawnItem(spawnPos);
        }

        // TODO : ���� ��� �� ���� ����
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnChest(chestPos);
        }
    }

    // ������ Ŭ���̾�Ʈ������ ������ ���� ȣ��
    public void SpawnItem(Vector3 position)
    {
        photonView.RPC(nameof(SpawnItemRPC), RpcTarget.MasterClient, position);
    }

    // ������ Ÿ�� �� ������ ������ ����
    [PunRPC]
    private void SpawnItemRPC(Vector3 position)
    {
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        ItemPrefab selectedItem = itemPrefabs[randomIndex];

        string itemPath = "GameObject/Items/" + selectedItem.prefab.name;
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        GameObject itemObj = PhotonNetwork.Instantiate(itemPath, position, rotation);
        // WHS_Item item = itemObj.GetComponent<WHS_Item>();
    }

    // ȹ���� ������ ���� ���� ȣ��
    public void ApplyItem(StatusModel statusModel, WHS_Item item)
    {
        photonView.RPC(nameof(ApplyItemRPC), RpcTarget.All, statusModel.photonView.ViewID, (int)item.type, item.value);

        for (int i = 0; i < item.additionalItemValue.Length; i++)
        {

            photonView.RPC(nameof(ApplyItemRPC), RpcTarget.All, statusModel.photonView.ViewID, (int)item.additionalItemValue[i].type, item.additionalItemValue[i].value);
        }
    }

    // �� �÷��̾� ȹ���� ������ ���� ����
    [PunRPC]
    public void ApplyItemRPC(int playerViewID, int itemTypeIndex, float itemValue)
    {
        ItemType itemType = (ItemType)itemTypeIndex;
        PhotonView playerPV = PhotonView.Find(playerViewID);

        if (playerPV != null)
        {
            StatusModel statusModel = playerPV.GetComponent<StatusModel>();

            if (statusModel != null)
            {

                switch (itemType)

                {
                    case ItemType.HP:
                        if (statusModel.HP + itemValue <= statusModel.MaxHP)
                        {
                            statusModel.HP += itemValue;
                            Debug.Log($"ü�� {itemValue} ȸ��");
                        }
                        else if (statusModel.HP + itemValue > statusModel.MaxHP)
                        {
                            statusModel.HP = statusModel.MaxHP;
                            Debug.Log($"ü�� {itemValue} ȸ��");
                        }

                        break;
                    case ItemType.MaxHP:
                        statusModel.MaxHP += itemValue;
                        // statusModel.HP += itemValue;
                        Debug.Log($"�ִ� ü�� {itemValue} ����");
                        break;
                    case ItemType.Attack:
                        statusModel.Attack += itemValue;
                        Debug.Log($"���ݷ� {itemValue} ����");
                        break;
                    case ItemType.AtkSpeed:
                        statusModel.AttackSpeed += itemValue;
                        Debug.Log($"���� �ӵ� {itemValue} ����");
                        break;
                    case ItemType.MoveSpeed:
                        statusModel.MoveSpeed += itemValue;
                        Debug.Log($"�̵� �ӵ� {itemValue} ����");
                        break;
                    case ItemType.MaxStamina:
                        statusModel.MaxStamina += itemValue;
                        Debug.Log($"�ִ� ���¹̳� {itemValue} ����");
                        break;
                    case ItemType.ConsumeStamina:
                        statusModel.ConsumeStamina -= itemValue;
                        Debug.Log($"���¹̳� �Һ� {itemValue} ����");
                        break;
                    case ItemType.RecoveryStaminaMag:
                        statusModel.RecoveryStaminaMag += itemValue;
                        Debug.Log($"���¹̳� ȸ�� �ӵ� {itemValue} ����");
                        break;
                    case ItemType.CriticalRate:
                        statusModel.CriticalRate += itemValue;
                        Debug.Log($"ġ��Ÿ Ȯ�� {itemValue} ����");
                        break;
                    case ItemType.CriticalDamageRate:
                        statusModel.CriticalDamageRate += itemValue;
                        Debug.Log($"ġ��Ÿ ������ {itemValue} ����");
                        break;
                    case ItemType.SkillCoolTime:
                        statusModel.SetSkillCoolTime(itemValue);
                        Debug.Log($"��ų ��Ÿ�� {itemValue} ����");
                        break;

                }


            }
        }
    }


    // ������ Ŭ���̾�Ʈ������ ���� ����
    public void SpawnChest(Vector3 position)
    {
        /*
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = position + Vector3.right * (i - 1) * chestDistance;
            photonView.RPC(nameof(SpawnChestRPC), RpcTarget.MasterClient, spawnPos);
        }
        */
        photonView.RPC(nameof(SpawnChestRPC), RpcTarget.MasterClient, position);
    }

    // ���� ����
    [PunRPC]
    private void SpawnChestRPC(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        string chestPath = "GameObject/Items/" + chestPrefab.name;
        GameObject chestObj = PhotonNetwork.Instantiate(chestPath, position, rotation);
    }

    // �κ��丮�� �� ������ ���� �ʱ�ȭ
    private void InitItemData()
    {
        foreach (ItemPrefab itemPrefab in itemPrefabs)
        {
            string itemPath = "GameObject/Items/" + itemPrefab.prefab.name;
            GameObject prefab = Resources.Load<GameObject>(itemPath);

            if (prefab != null)
            {
                WHS_Item item = prefab.GetComponent<WHS_Item>();
                if (item != null)
                {
                    itemData[item.type] = item;
                }
            }
        }
    }

    // HP���� ���׷��̵�
    public void UpgradePotion()
    {
        int newGrade = hpPotionGrade + 1;
        if (newGrade <= hpPotionPrefabs.Length)
        {
            photonView.RPC(nameof(UpdatePotionRPC), RpcTarget.All, newGrade);
        }
    }

    // HP���� ���׷��̵� ȣ��
    [PunRPC]
    private void UpdatePotionRPC(int newGrade)
    {
        hpPotionGrade = newGrade;
        UpdatePotion(hpPotionGrade);
        OnPotionGradeChanged?.Invoke(hpPotionGrade);
    }

    // ���׷��̵��� hp���� ������ ����
    public void UpdatePotion(int grade)
    {
        if (grade - 1 < hpPotionPrefabs.Length)
        {
            for (int i = 0; i < itemPrefabs.Length; i++)
            {
                if (itemPrefabs[i].type == ItemType.HP)
                {
                    itemPrefabs[i].prefab = hpPotionPrefabs[grade - 1];
                    if (itemData.TryGetValue(ItemType.HP, out WHS_Item item))
                    {
                        if (item is WHS_HPPotion hpPotion)
                        {
                            hpPotion.UpdateGrade(grade);
                        }
                    }
                    break;
                }
            }
            OnPotionGradeChanged?.Invoke(grade);
        }
    }

    public int GetPotionGrade()
    {
        return hpPotionGrade;
    }
}
