using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_RoomManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Room }

    [SerializeField] MKH_RoomPanel roomPanel;

    private void Start()
    {
        // ����� ���� ������ �̵�
        PhotonNetwork.AutomaticallySyncScene = true;

        SetActivePanel(Panel.Room);
    }

    // ���� ����
    public override void OnConnectedToMaster()
    {
        Debug.Log("���ӿ� �����ߴ�!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Room);
    }

    // ���� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"������ ���״�. cause : {cause}");
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }


   

    #region �� (����, ����, �÷��̾� ������Ʈ)
    // �� ���� ����
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� ����");
        SetActivePanel(Panel.Room);
    }

    // �濡�� ����
    public override void OnLeftRoom()
    {
        Debug.Log("�� ���� ����");
        PhotonNetwork.LoadLevel("MKH_ServerScene");
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

    // �г� ����
    private void SetActivePanel(Panel panel)
    {
        roomPanel.gameObject.SetActive(panel == Panel.Room);
    }
}
