using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class TestGameScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoom";
    [SerializeField] GameObject characterSelectUI;
    [SerializeField] GameObject startPoint;

    [SerializeField] int monsterCount;
    [SerializeField] List<int> monsterPrefabsNumber;
    [SerializeField] Queue<int> monsterOrderQueue = new Queue<int>();

    [SerializeField] TimelineBoss timeline;

    GameObject currentBoss;
 
    public int readyPlayer = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            BossSpawn();
        }
      
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();

        for (int i = 0; i < monsterCount; i++) {
            monsterPrefabsNumber.Add(i+1);
        }



    }

    // ��� �÷��̾ �ε� �Ϸ� ������ ȣ�� �ϵ��� ����
    private void SetMonster()
    {
        // ���� ���� �ʱ�ȭ
       
        if (photonView.IsMine)
        {
            
            while (monsterPrefabsNumber.Count >0)
            {


                int ran = Random.Range(0, monsterPrefabsNumber.Count);
                
                photonView.RPC(nameof(SetMonsterOrder), RpcTarget.All, ran);



            }
        }

       
    }

    [PunRPC]
    public void SetMonsterOrder(int num)
    {
        monsterOrderQueue.Enqueue(monsterPrefabsNumber[num]);
        monsterPrefabsNumber.Remove(monsterPrefabsNumber[num]);
    }



    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }



    public override void OnJoinedRoom()
    {
        StartCoroutine(StartDelayRoutine());
    }

    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f); // ��Ʈ��ũ �غ� �ʿ��� �ð� ��¦ �ֱ�
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("���� ����");
        characterSelectUI.SetActive(true);
    

        // ���常 �����ϴ� �ڵ�

        if (PhotonNetwork.IsMasterClient == false)
            return;

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
           
        }
    }
  
   
    public void PlayerSpawn(int num)
    {
        AudioManager.GetInstance().PlayBGM();
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

   

        PhotonNetwork.Instantiate($"GameObject/Player{num}", randomPos, Quaternion.identity);

        characterSelectUI.SetActive(false);
        
    }

    private void BossSpawn()
    {
      

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));


        PhotonNetwork.InstantiateRoomObject("GameObject/Boss6", randomPos, Quaternion.identity);

    }

 
    public void StartStage()
    {
    
        if(monsterOrderQueue.Count > 0) 
             timeline.StartTimeline(monsterOrderQueue.Dequeue());
    }

    public void ClearBoss(GameObject obj)
    {
        currentBoss = obj;
        startPoint.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayer++;

            //�ӽ� ����
            if (monsterPrefabsNumber.Count > 0)
            {
                if(photonView.IsMine)
                    SetMonster();
            }
            Debug.Log($"�غ� {readyPlayer}/{PhotonNetwork.PlayerList.Count()} ");

            // �������� ���� ȣ��
            if(readyPlayer >= PhotonNetwork.PlayerList.Count())
            {
                StartStage();
                readyPlayer=0;
                startPoint.SetActive(false);
                if (currentBoss is not null)
                {
                    Destroy(currentBoss);
                    currentBoss = null;
                }
            }


        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            readyPlayer--;

       

            Debug.Log($"�غ� {readyPlayer}/{PhotonNetwork.PlayerList.Count()} ");
        }

       

    }

    private void OnDisable()
    {
        readyPlayer = 0;
    }


}
