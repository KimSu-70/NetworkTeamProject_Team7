using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MKH_MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        // ó������ �游��� �г� ����� ����
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        PhotonNetwork.LeaveRoom();

        // �� ����� ���� ����
        createRoomPanel.SetActive(true);

        // �� �̸� ����
        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        // �÷��̾� ��
        maxPlayerInputField.text = "1 ~ 4";
    }

    // �� �����
    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        // �� �̸��� ���� ��
        if (roomName == "")
        {
            Debug.LogWarning("�� �̸��� �����ؾ� ���� ������ �� �ֽ��ϴ�.");
            return;
        }

        int maxPlayer = int.Parse(maxPlayerInputField.text);        // �ִ� �ο��� ����
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 4);                   // �÷��̾� �� ����

        RoomOptions options = new RoomOptions();                    // �� ����⿡ ���� �ɼ� ����
        options.MaxPlayers = maxPlayer;                             // �ִ� �÷��̾� ��

        PhotonNetwork.CreateRoom(roomName, options);                // �游���(���̸�, �ɼ�)
    }

    // �� ����� ���
    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);
    }

    // ���� ��Ī
    public void RandomMatching()
    {
        Debug.Log("���� ��Ī ��û");
        PhotonNetwork.LeaveRoom();

        // ��� �ִ� ���� ������ ���� ���� ���� ���� ���
        string name = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    // �κ� ����
    public void JoinLobby()
    {
        Debug.Log("�κ� ���� ��û");
        PhotonNetwork.JoinLobby();
    }

    public void Server()
    {
        Debug.Log("������������ �̵�");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    // �α׾ƿ�
    public void Logout()
    {
        Debug.Log("�α׾ƿ� ��û");
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    // ������
    public void Exit()
    {
        Application.Quit();
    }

    // ���� ������ ����
    public void DeleteUser()
    {
        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        user.DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
                PhotonNetwork.Disconnect();
            });
    }
}
