using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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

        if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
        {
            PlayerSpawn();
        }
        else if(PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient)
        {
            return;
        }

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
        if (PhotonNetwork.LocalPlayer != null)
        {
            PlayerSpawn();
            Debug.Log("1");
        }
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
        if (PhotonNetwork.LocalPlayer == newPlayer)
        {
            PlayerSpawn();
            Debug.Log("1");
        }
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

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player", randomPos, Quaternion.identity);
    }
}
