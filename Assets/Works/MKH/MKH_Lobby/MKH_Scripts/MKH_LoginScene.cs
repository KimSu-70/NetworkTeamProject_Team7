using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LoginScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Server }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_ServerPanel serverPanel;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Server);
        }
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

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)         // �� ���� ���� ��
    {
        Debug.Log($"�� ���� ����, ���� : {message}");
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        serverPanel.gameObject.SetActive(panel == Panel.Server);
    }
}
