using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class MyRoomPanel : MonoBehaviour
{
    [SerializeField] MyPlayerEntry[] playerEntries;
    [SerializeField] Button startButton;

    // �濡 ���Ծ��� ��
    private void OnEnable()
    {
        UpdatePlayers();

        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
    }
    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    public void UpdatePlayers()
    {

        foreach (MyPlayerEntry entry in playerEntries)
        {
            entry.SetEmpty();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetPlayerNumber() == -1)
            {
                continue;
            }

            int number = player.GetPlayerNumber();
            playerEntries[number].SetPlayer(player);

           
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startButton.interactable = CheckAllReady();
        }
        else
        {
            startButton.interactable = false;
        }

    }



    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} ����!");
        UpdatePlayers();
    }

    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} ����!");
        UpdatePlayers();

    }

    public void UpdatePlayerProperty(Player targetPlayer, PhotonHashtable properties)
    {
        // ���� Ŀ���� ������Ƽ�� ������ ���� READY Ű�� ����
        if (properties.ContainsKey(CustomProperty.READY))
        {
            UpdatePlayers();
        }
    }

    private bool CheckAllReady()
    {

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady() == false)
                return false;


        }

        return true;
    }


    public void StartGame()
    {



        PhotonNetwork.LoadLevel("GameScene");          // ��Ʈ��ũ�� ���� ���� �̵��ϵ��� > ��û��
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
