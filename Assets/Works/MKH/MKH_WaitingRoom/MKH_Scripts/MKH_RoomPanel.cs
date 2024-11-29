using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class MKH_RoomPanel : MonoBehaviourPun
{
    [SerializeField] MKH_PlayerEntry[] playerEntries;
    [SerializeField] Button startButton;

    // �濡 ������ ��
    private void OnEnable()
    {
        UpdatePlayers();
        // �÷��̾� �ѹ��� ������Ʈ
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
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
        foreach (MKH_PlayerEntry entry in playerEntries)
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

        // ���� �����̾��� ���
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // ��� �÷��̾ ���� �Ǿ������� ��ŸƮ��ư Ȱ��ȭ
            startButton.interactable = CheckAllReady();
        }
        // ���� ������ �ƴ� ���
        else
        {
            startButton.interactable = false;
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

    // �÷��̾� ������Ʈ
    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties)
    {
        // ���� Ŀ���� ������Ƽ�� ������ ���� READY Ű�� ����
        if (properties.ContainsKey(CustomProperty.READY))
        {
            UpdatePlayers();
        }
    }

    // ���� üũ
    private bool CheckAllReady()
    {
        // �÷��̾� ����Ʈ�� �ִ� �÷��̾���� ���� ������ ��� ��Ȱ��ȭ
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady() == false)
                return false;
        }

        return true;
    }

    // ���� ����
    public void StartGame()
    {
        // TODO : ���� ���� ����
        PhotonNetwork.LoadLevel("GameScene");
        // ���� ���� �� ������ ����
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    // �� ����
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
