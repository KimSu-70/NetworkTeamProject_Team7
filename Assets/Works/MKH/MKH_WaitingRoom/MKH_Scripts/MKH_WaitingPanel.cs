using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class MKH_WaitingPanel : MonoBehaviour
{
    [SerializeField] MKH_WaitingPlayerEntry[] playerEntries;

    // �濡 ������ ��
    private void OnEnable()
    {
        UpdatePlayers();

        // �÷��̾� �ѹ��� ������Ʈ
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

    }

    // �濡 ������ ��
    private void OnDisable()
    {
        // �÷��̾� �ѹ��� ������Ʈ ���ϱ�
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    public void UpdatePlayers()
    {
        // �÷��̾� �������� ���� ����
        foreach (MKH_WaitingPlayerEntry entry in playerEntries)
        {
            entry.SetEmpty();
        }
        // ��� �÷��̾� Ȯ��
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // ���� ��ȣ �Ҵ� �ȹ޾��� �� ������Ʈ ���� ����
            if (player.GetPlayerNumber() == -1)
                continue;
            // �÷��̾� �ѹ� ������ ����
            int number = player.GetPlayerNumber();
            // �÷��̾� ��Ʈ������ ���� �ѹ� �÷��̾� ���� ����
            playerEntries[number].SetPlayer(player);
        }
    }

    // �÷��̾� ����
    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} ����!");
        UpdatePlayers();
    }

    // �÷��̾� ����
    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} ����!");
        UpdatePlayers();
    }

    // �� ����
    public void LeaveRoom()
    {
        PhotonNetwork.LoadLevel("MKH_LobbyScene");
    }
}

