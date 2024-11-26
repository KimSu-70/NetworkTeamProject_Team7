using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Menu, Lobby, Room, WaitingRoom}

    [SerializeField] MKH_MainPanel mainPanel;
    [SerializeField] MKH_RoomPanel roomPanel;
    [SerializeField] MKH_LobbyPanel lobbyPanel;
    [SerializeField] MKH_WaitingPanel waitingPanel;

    private void Start()
    {
        // ����� ���� ������ �̵�
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.InRoom)
        {
            SetActivePanel(Panel.Room);
        }
        else if (PhotonNetwork.InLobby)
        {
            SetActivePanel(Panel.Lobby);
        }
        else if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.WaitingRoom);
            SetActivePanel(Panel.Menu);
        }
    }

    // ���� ����
    public override void OnConnectedToMaster()
    {
        Debug.Log("���ӿ� �����ߴ�!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Menu);
    }

    // ���� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"������ ���״�. cause : {cause}");
        PhotonNetwork.LoadLevel("MKH_LobbyScene");
    }


    #region �� ����
    // �� ���� ����
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����");
    }

    // �� ���� ����
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"�� ���� ����, ���� : {message}");
        SetActivePanel(Panel.Menu);
    }
    #endregion

    #region �� (����, ����, �÷��̾� ������Ʈ)
    // �� ���� ����
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� ����");
        SetActivePanel(Panel.Room);
    }

    // �� ���� ����
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"�� ���� ����, ���� : {message}");
    }

    // ���� �� ���� ����
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"���� ��Ī ����, ���� : {message}");
    }

    // �濡�� ����
    public override void OnLeftRoom()
    {
        Debug.Log("�� ���� ����");
        SetActivePanel(Panel.Menu);
    }

    // �÷��̾� ����
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        waitingPanel.EnterPlayer(newPlayer);
        roomPanel.EnterPlayer(newPlayer);
    }

    // �÷��̾� ������Ʈ
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        roomPanel.UpdatePlayerProperty(targetPlayer, changedProps);
    }

    // �÷��̾� ����
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        waitingPanel.EnterPlayer(otherPlayer);
        roomPanel.ExitPlayer(otherPlayer);
    }
    #endregion

    #region �κ�
    // �κ� ����
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");
        SetActivePanel(Panel.Lobby);
    }

    // �κ� ����
    public override void OnLeftLobby()
    {
        Debug.Log("�κ� ���� ����");
        // ��ųʸ� �� ��� ����
        lobbyPanel.ClearRoomEntries();
        SetActivePanel(Panel.Menu);
    }

    // �� ����Ʈ ������Ʈ
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }
    #endregion

    // �г� ����
    private void SetActivePanel(Panel panel)
    {
        mainPanel.gameObject.SetActive(panel == Panel.Menu);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
        waitingPanel.gameObject.SetActive(panel == Panel.WaitingRoom);
    }

    private void Menu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (mainPanel.gameObject.activeSelf == false)
            {
                Debug.Log("1");
                mainPanel.gameObject.SetActive(true);
                SetActivePanel(Panel.Menu);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (mainPanel.gameObject.activeSelf == true)
            {
                Debug.Log("2");
                mainPanel.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
