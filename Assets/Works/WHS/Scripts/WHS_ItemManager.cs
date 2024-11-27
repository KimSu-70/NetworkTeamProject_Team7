using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Pun.Demo.Cockpit;
using static UnityEditor.Progress;
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

    [SerializeField] private GameObject[] hpPotionPrefabs;
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

        if (Input.GetKeyDown(KeyCode.O))
        {
            UpgradePotion();
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
                        if(statusModel.HP + itemValue <= statusModel.MaxHP)
                        {
                            statusModel.HP += itemValue;
                            Debug.Log($"ü�� {itemValue} ȸ��");
                        }
                        else if(statusModel.HP + itemValue > statusModel.MaxHP)
                        {
                            statusModel.HP = statusModel.MaxHP;
                            Debug.Log($"ü�� {itemValue} ȸ��");
                        }
                        break;

                    // TODO : ü�� �� �ٸ� ���� ����?
                    case ItemType.MaxHP:
                        statusModel.MaxHP += itemValue;
                        // statusModel.HP += itemValue;
                        Debug.Log($"�ִ� ü�� {itemValue} ����");
                        break;
                    case ItemType.Attack:
                        Debug.Log($"���ݷ� {itemValue} ����");
                        break;
                }
            }
        }
    }


    // ������ Ŭ���̾�Ʈ������ ���� ����
    private void SpawnChest(Vector3 position)
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
        // WHS_Chest chest = chestObj.GetComponent<WHS_Chest>();
        // chests.Add(chest);
    }

    /*
    // ���� �μ� ���� �� �ٸ� ���� ����
    public void DestroyAllChests(WHS_Chest destroyedChest)
    {
        foreach (WHS_Chest chest in chests)
        {
            if (chest != destroyedChest)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(chest.gameObject);
                }
            }
        }
        chests.Clear();
    }
    */

    // �κ��丮�� �� ������ ���� �ʱ�ȭ
    private void InitItemData()
    {
        foreach(ItemPrefab itemPrefab in itemPrefabs)
        {
            string itemPath = "GameObject/Items/" + itemPrefab.prefab.name;
            GameObject prefab = Resources.Load<GameObject>(itemPath);

            if(prefab != null)
            {
                WHS_Item item = prefab.GetComponent<WHS_Item>();
                if(item != null)
                {
                    itemData[item.type] = item;
                }
            }
        }
    }

    public void UpgradePotion()
    {
        hpPotionGrade++;
        if (hpPotionGrade > hpPotionPrefabs.Length)
        {
            Debug.Log("�̹� ������ �ִ����̴�");
            return;
        }
        Debug.Log($"���� ���׷��̵� {hpPotionGrade}");

        UpdatePotionPrefab(hpPotionGrade);
    }

    public void UpdatePotionPrefab(int grade)
    {
        for(int i=0; i<itemPrefabs.Length; i++)
        {
            if (itemPrefabs[i].type == ItemType.HP)
            {
                itemPrefabs[i].prefab = hpPotionPrefabs[grade - 1];
                if(itemData.TryGetValue(ItemType.HP, out WHS_Item item))
                {
                    if(item is WHS_HPPotion hpPotion)
                    {
                        hpPotion.UpdateGrade(grade);
                    }
                }

                OnPotionGradeChanged?.Invoke(grade);
                break;
            }
        }
    }

    public int GetPotionGrade()
    {
        return hpPotionGrade;
    }
}
