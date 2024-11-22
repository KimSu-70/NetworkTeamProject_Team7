using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_MainPanel menuPanel;
    [SerializeField] MKH_RoomPanel roomPanel;
    [SerializeField] MKH_LobbyPanel lobbyPanel;

    private void Start()
    {
        // ����� ���� ������ �̵�
        PhotonNetwork.AutomaticallySyncScene = true;

        if(PhotonNetwork.InRoom)
        {
            SetActivePanel(Panel.Room);
            //PhotonNetwork.LoadLevel("WaitingScene");
        }
        else if (PhotonNetwork.InLobby)
        {
            SetActivePanel(Panel.Lobby);
        }
        else if(PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Menu);
        }
        else
        {
            SetActivePanel(Panel.Login);
        }
    }

    #region ���� (����, ����)
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
        SetActivePanel(Panel.Login);
    }
    #endregion

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
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
        //SetActivePanel(Panel.Room);
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
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
