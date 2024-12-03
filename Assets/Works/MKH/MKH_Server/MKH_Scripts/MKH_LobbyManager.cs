using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_MainPanel menuPanel;
    [SerializeField] MKH_LobbyPanel lobbyPanel;
    [SerializeField] GameObject cover;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        MKH_LoadingSceneController.Create();

        if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Menu);
        }
        else
        {
            SetActivePanel(Panel.Login);
            cover.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            cover.SetActive(false);
        }
    }

    public override void OnConnectedToMaster()      // ������ ������ ������ �Ϸ� ���� ��
    {
        Debug.Log("���ӿ� �����ߴ�!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Menu);
    }

    public override void OnDisconnected(DisconnectCause cause)      // ������ ������ ��
    {
        Debug.Log($"������ ���״�. cause : {cause}");
        SetActivePanel(Panel.Login);
    }

    public override void OnCreatedRoom()        // �� ���� ���� ��
    {
        Debug.Log("�� ���� ����");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� ����");
        MKH_LoadingSceneController.Instance.LoadScene("MKH_WaitingScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)         // �� ���� ���� ��
    {
        Debug.Log($"�� ���� ����, ���� : {message}");
        SetActivePanel(Panel.Menu);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)       // ���� �� ���� ���� ��
    {
        Debug.Log($"���� ��Ī ����, ���� : {message}");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("�κ� ���� ����");
        lobbyPanel.ClearRoomEntries();          // ��ųʸ� �� ��� ����
        SetActivePanel(Panel.Menu);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���� ����� ������ �ִ� ��� �������� ������ ������
        // ���� ����
        // 1. ó�� �κ� ���� �� : ��� �� ����� ����
        // 2. ���� �� �� ����� ����Ǵ� ��� : ����� �� ��ϸ� ����
        lobbyPanel.UpdateRoomList(roomList);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
