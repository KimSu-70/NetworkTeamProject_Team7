using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class MKH_RoomManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Room }

    [SerializeField] MKH_RoomPanel roomPanel;


    private void Start()
    {
        // ����� ���� ������ �̵�
        PhotonNetwork.AutomaticallySyncScene = true;

        StartCoroutine(PlayerSpawns());
    }

    // ���� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"������ ���״�. cause : {cause}");
        MKH_LoadingSceneController.Instance.LoadScene("MKH_ServerScene");
    }


    #region �� (����, ����, �÷��̾� ������Ʈ)
    // �濡�� ����
    public override void OnLeftRoom()
    {
        Debug.Log("�� ���� ����");
        MKH_LoadingSceneController.Instance.LoadScene("MKH_ServerScene");
    }

    // �÷��̾� ����
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.EnterPlayer(newPlayer);
    }

    // �÷��̾� ������Ʈ
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
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
        Vector3 randPos = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player", randPos, Quaternion.identity);
    }

    IEnumerator PlayerSpawns()
    {
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            PlayerSpawn();
            SetActivePanel(Panel.Room);
        }
    }
}
