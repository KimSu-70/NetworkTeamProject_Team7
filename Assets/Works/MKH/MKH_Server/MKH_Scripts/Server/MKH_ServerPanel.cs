using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MKH_ServerPanel : MonoBehaviour
{
    [SerializeField] GameObject serverPanel;
    [SerializeField] GameObject createServerPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()         // ó������ �游��� �г� ����� ����
    {
        createServerPanel.SetActive(false);
    }

    public void CreateServerMenu()        // �� ����� ���� ����
    {
        createServerPanel.SetActive(true);

        //roomNameInputField.text = " ";          // �� �̸� ����
        //maxPlayerInputField.text = " ";                                         // �÷��̾� ��
    }

    public void CreateServerConfirm()     // �� �����
    {
        string serverName = roomNameInputField.text;
        if (serverName == "")      // �� �̸��� ���� ��
        {
            Debug.LogWarning("�� �̸��� �����ؾ� ���� ������ �� �ֽ��ϴ�.");
            return;
        }

        int maxPlayer = int.Parse(maxPlayerInputField.text);        // �ִ� �ο��� ����
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 20);                   // �÷��̾� �� ����

        RoomOptions options = new RoomOptions();                    // �� ����⿡ ���� �ɼ� ����
        options.MaxPlayers = maxPlayer;                             // �ִ� �÷��̾� ��

        PhotonNetwork.CreateRoom(serverName, options);                // �游���(���̸�, �ɼ�)
    }

    public void CreateServerCancel()      // �� ����� ���
    {
        createServerPanel.SetActive(false);
    }

    public void RandomMatching()        // ���� ��Ī
    {
        Debug.Log("���� ��Ī ��û");

        // ��� �ִ� ���� ������ ���� ���� ���� ���� ���
        string name = $"{Random.Range(1, 100)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void JoinLobby()             // �κ� ����
    {
        Debug.Log("���� �κ� ���� ��û");
        PhotonNetwork.JoinLobby();
    }

    public void Logout()                // ������
    {
        Debug.Log("�α׾ƿ� ��û");
        PhotonNetwork.Disconnect();
    }
}
