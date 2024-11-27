using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_ServerManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Server, List }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_ServerPanel serverPanel;
    [SerializeField] MKH_ServerListPanel listPanel;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        SetActivePanel(Panel.Login);
    }

    public override void OnConnectedToMaster()      // ������ ������ ������ �Ϸ� ���� ��
    {
        Debug.Log("���ӿ� �����ߴ�!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Server);
    }

    public override void OnDisconnected(DisconnectCause cause)      // ������ ������ ��
    {
        Debug.Log($"������ ���״�. cause : {cause}");
        SetActivePanel(Panel.Login);
    }

    public override void OnCreatedRoom()        // �� ���� ���� ��
    {
        Debug.Log("���� ���� ����");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("���� ���� ����");
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)         // �� ���� ���� ��
    {
        Debug.Log($"���� ���� ����, ���� : {message}");
        SetActivePanel(Panel.Server);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)       // ���� �� ���� ���� ��
    {
        Debug.Log($"���� ��Ī ����, ���� : {message}");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("�κ� ���� ����");
        SetActivePanel(Panel.List);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("�κ� ���� ����");
        listPanel.ClearServerEntries();          // ��ųʸ� �� ��� ����
        SetActivePanel(Panel.Server);
    }

    public override void OnRoomListUpdate(List<RoomInfo> serverList)
    {
        // ���� ����� ������ �ִ� ��� �������� ������ ������
        // ���� ����
        // 1. ó�� �κ� ���� �� : ��� �� ����� ����
        // 2. ���� �� �� ����� ����Ǵ� ��� : ����� �� ��ϸ� ����
        listPanel.UpdateServerList(serverList);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        serverPanel.gameObject.SetActive(panel == Panel.Server);
        listPanel.gameObject.SetActive(panel == Panel.List);
    }
}
