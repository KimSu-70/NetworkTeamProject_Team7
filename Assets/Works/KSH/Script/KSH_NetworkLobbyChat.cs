using ExitGames.Client.Photon;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_NetworkLobbyChat : MonoBehaviour, IChatClientListener
{
    private ChatClient _chatClient;  // Photon Chat Ŭ���̾�Ʈ ��ü

    private string _userName;        // ����� �̸�
    private string _currentChannelName;  // ���� ä�� ä�� �̸�
    private string _privateReceiver = "";    // ���� �޽��� ������
    [SerializeField] TMP_InputField _inputField;  // ������ �޽����� �Է��ϴ� InputField UI ���
    [SerializeField] Text _outputText;        // �޽����� ��µǴ� Text UI ���

    private string _friendInputField; // ģ�� �̸� �Է� �ʵ�
    [SerializeField] GameObject _friendListContent; // ģ�� ����� ǥ�õ� Scroll View�� Content
    [SerializeField] GameObject _friendItemPrefab; // ģ�� �׸� ����� Prefab


    DatabaseReference _userDataRef;
    DatabaseReference _friend;

    // ģ�� ���¸� �����ϴ� ��ųʸ� (ģ�� �̸� -> ����)
    private Dictionary<string, string> _friendStatuses = new Dictionary<string, string>();

    private string _chatID;
    public string ChatID { get { return _chatID; } }
    public TMP_InputField InputField { get { return _inputField; } }
    public Text OutputText { get { return _outputText; } }


    private void OnEnable()
    {
        _outputText.text = "";

        // ��׶��忡�� ���� ���
        Application.runInBackground = true;

        // ����� �̸��� �����÷��̾� �̸����� ����
        _userName = PhotonNetwork.LocalPlayer.NickName;
        
        // ���� �� ����
        _chatID = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;

        // �⺻ ä�� ����
        _currentChannelName = "Channel 001";

        // ChatClient ����, IChatClientListener�� `this`�� ����
        _chatClient = new ChatClient(this);

        //// ä�� ������ ���� ���� EU, US, ASIA
        // chatClient.ChatRegion = "ASIA";

        // ���� ������ �õ�
        Debug.Log(_chatClient.AppId);
        _chatClient.Connect(_chatID, "1.0", new AuthenticationValues(_userName));  // Photon Chat ������ ���� �õ�

        // ���� �õ� �޽��� ��� {0}�� �κп� nserName ǥ��
        AddLine(string.Format(" ����õ� : {0}", _userName));

        // ����� ID
        string uid = BackendManager.Auth.CurrentUser.UserId;

        // �����ͺ��̽� �ڷ� ��ġ ����
        _userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);
        _friend = _userDataRef.Child("Friend");

        LoadFriendsFromFirebase();
    }

    void OnDisable()
    {
        // UI ������Ʈ�� �ı��� �� ä�ο��� ����
        if (_chatClient != null && _chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            _chatClient.Unsubscribe(new string[] { _currentChannelName });
            Debug.Log("ä�ο��� �������ϴ�: " + _currentChannelName);
        }
    }

    private void Update()
    {
        // ������ ������ �����ϰ� ���������� ȣ���Ͽ� ���� �޼����� �޽��ϴ�.
        _chatClient.Service();
    }


    // ���α׷� ���� ��, �������� ������ ����
    public void OnApplicationQuit()
    {
        if (_chatClient != null)
        {
            _chatClient.Disconnect();  // ���ø����̼� ���� ��, �������� ������ �����ϴ�.
        }
    }

    // ģ�� �߰� �޼���
    public void AddFriend()
    {
        // �Էµ� ģ�� �̸� ��������
        string friendName = _friendInputField;

        // �̸��� ��� ������ ��ȯ
        if (string.IsNullOrEmpty(friendName)) return;

        // ģ�� ��ȿ�� �˻� �� �߰�
        //if ( TODO ���� ê�� ����ڵ� ���� �ҷ����� �ʿ�)
        //{
        //    AddLine($"ģ�� '{friendName}'��(��) �������� �ʰų� ���������Դϴ�.");
        //    return;
        //}

        // Photon Chat�� ģ�� �߰� ��û
        _chatClient.AddFriends(new string[] { friendName });

        // UI�� ģ�� �׸� �߰�
        AddFriendToUI(friendName, "��� ��");
        
        // ���¸� ��ųʸ��� ����
        _friendStatuses[friendName] = "��� ��";

        // Firebase�� ģ�� ��� ����
        SaveFriendsToFirebase();

        // �Է� �ʵ� �ʱ�ȭ
        _friendInputField = "";
    }

    // ģ�� ���� �޼���
    public void RemoveFriend(string friendName)
    {
        // Photon Chat���� ģ�� ���� ��û
        _chatClient.RemoveFriends(new string[] { friendName });

        // UI���� ģ�� �׸� ����
        foreach (Transform child in _friendListContent.transform)
        {
            FriendItem friendItem = child.GetComponent<FriendItem>();
            if (friendItem != null && friendItem.FriendName == friendName)
            {
                Destroy(child.gameObject); // UI �׸� ����
                break;
            }
        }

        // ���� ���� ����
        _friendStatuses.Remove(friendName);

        // Firebase���� ģ�� ������ ����
        _friend.Child(friendName).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"ģ�� {friendName} ������ ���� ����!");
            }
            else
            {
                Debug.LogError($"ģ�� {friendName} ������ ���� ����: {task.Exception}");
            }
        });
    }

    // Firebase�� ģ�� ��� ����
    private void SaveFriendsToFirebase()
    {
        foreach (var friend in _friendStatuses)
        {
            // ģ�� �̸��� Ű�� ����Ͽ� ���¸� ����
            _friend.Child(friend.Key).SetValueAsync(friend.Value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"ģ�� {friend.Key} ���� ���� ����!");
                }
                else
                {
                    Debug.LogError($"ģ�� {friend.Key} ���� ���� ����: {task.Exception}");
                }
            });
        }
    }

    // Firebase���� ģ�� ��� �ҷ�����
    private void LoadFriendsFromFirebase()
    {
        _friend.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            // ������ �����Ͱ� �������� �ʴ� ��� ó��.
            if (!snapshot.Exists)
            {
                Debug.Log("�����Ͱ� �������� �ʽ��ϴ�.");
                return;
            }

            // �����Ͱ� �����ϸ� �� �ڽ��� ��ȸ�Ͽ� ó��.
            foreach (DataSnapshot child in snapshot.Children)
            {
                string friendName = child.Key;       // ģ�� �̸�(Key).
                string friendStatus = child.Value.ToString(); // ģ�� ����(Value).

                // �̹� UI�� ���� ��ųʸ��� ģ���� �����ϴ��� Ȯ��
                if (_friendStatuses.ContainsKey(friendName))
                {
                    Debug.Log($"�̹� ���ÿ� �����ϴ� ģ��: {friendName}, ����: {friendStatus}");
                    continue; // �̹� �����ϴ� ��� �ǳʶ�
                }

                // UI�� ģ�� �׸� �߰�.
                AddFriendToUI(friendName, friendStatus);

                // ���� ��ųʸ��� ģ�� ���� ����.
                _friendStatuses[friendName] = friendStatus;
            }

            Debug.Log("������ �ҷ����� ����");
        });
    }

    // UI�� ģ�� �߰�
    private void AddFriendToUI(string friendName, string status)
    {
        GameObject friendItemObject = Instantiate(_friendItemPrefab, _friendListContent.transform);
        FriendItem friendItem = friendItemObject.GetComponent<FriendItem>();
        friendItem.Setup(friendName, status, this); // ���� ��ũ��Ʈ(this)�� ����
    }

    public void AddLine(string lineString)
    {
        _outputText.text += lineString + "\r\n";  // outputText�� �޽����� �߰�
    }

    // ���� �޽����� ���� �����ڸ� ����
    public void ReceiverOnValueChange(string valueIn)
    {
        _privateReceiver = valueIn; // �Էµ� ���� privateReceiver�� ����
    }


    // ģ�� �߰� �̸� ����
    public void FriendName(string valueIn)
    {
        _friendInputField = valueIn; // �Էµ� ���� privateReceiver�� ����
    }


    // �޽����� �Է��� �� ���� Ű�� ������ �� ȣ��Ǵ� �Լ�
    public void Input_OnEndEdit(string text)
    {
        // ���� ���°� 'ConnectedToFrontEnd'�� ���� �޽��� ���� ����
        if (_chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
        }
    }


    // ��ü ä���� �����ϴ� �Լ�
    public void SubmitPublicChatOnClick()
    {

        // ���� �޽����� �ƴ϶�� ���� ä������ ó��
        if (_privateReceiver == "")
        {
            // ä�ο� �޽����� ����
            _chatClient.PublishMessage(_currentChannelName, _inputField.text);

            // �޽��� �Է� �ʵ� �ʱ�ȭ
            _inputField.text = "";
        }

    }


    // ���� ä���� �����ϴ� �Լ�
    public void SubmitPrivateChatOnClick()
    {

        if (_privateReceiver != "") // �����ڰ� �����Ǿ� ������
        {
            _chatClient.SendPrivateMessage(_privateReceiver, _inputField.text); // �ش� �����ڿ��� ���� �޽��� ����
            _inputField.text = ""; // �Է� �ʵ� �ʱ�ȭ
        }
    }

    // ����� �޽����� ����ϴ� �Լ�
    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.Log(message); // ���� �޽����� ��� ���� �α׷� ���
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);  // ��� �޽����� ��� ��� �α׷� ���
        }
        else
        {
            Debug.Log(message);  // �� �� �޽����� �Ϲ� �α׷� ���
        }
    }

    // ä�� ���°� ����� �� ȣ��Ǵ� �ݹ�
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);  // ä�� ���°� ����� �� ���¸� �α׷� ���
    }



    // ������ ����Ǿ��� �� ȣ��Ǵ� �ݹ�
    public void OnConnected()
    {
        AddLine("������ ����Ǿ����ϴ�.");  // ���� ���� �޽��� ���

        //// ä�� ä���� Photon �� �̸����� ����
        //_currentChannelName = PhotonNetwork.CurrentRoom.Name;

        // ����� ��, ä�ο� ����
        _chatClient.Subscribe(new string[] { _currentChannelName }, 0);  // ä�ο� ����
        _chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }



    // ������ ������ �������� �� ȣ��Ǵ� �ݹ�
    public void OnDisconnected()
    {
        Debug.Log("���� �ȵ�");
        // ���� ������ ���� Ȯ���� ���� �α� �߰�
        Debug.Log("��Ŀ��Ʈ ����: " + _chatClient.DebugOut.ToString());
        AddLine("������ ������ ���������ϴ�.");  // ���� ������ �޽��� ���
        _chatClient.SetOnlineStatus(ChatUserStatus.Offline);
    }



    // ä�ο��� �޽����� ���ŵǾ��� �� ȣ��Ǵ� �ݹ� channelName ä���̸�, senders ����� �̸�, messages �޽��� ����
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            // �� �޽����� ���� ����� ���
            AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
        }
    }



    // ���� �޽����� ���ŵǾ��� �� ȣ��Ǵ� �ݹ�
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);  // ���� �޽��� ���� �α׷� ���
        AddLine(string.Format("<color=magenta>{0} : {1}</color>", sender, message.ToString()));
    }



    // ����� ���� ������Ʈ�� ���� �� ȣ��Ǵ� �ݹ�
    // string user (����� ID), int status(���� ���� �ڵ�), bool gotMessage(����� ���� �޼��� ������ ����), object message(���� ���� �� ����� ���� �޼���)
    // ���� ���� �ڵ� 1 = �¶���, 0 = ��������
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        string statusMessage; // ���� �޽����� ������ ����

        // ���� �ڵ忡 ���� ���� �޽��� ����
        switch (status)
        {
            case ChatUserStatus.Online:
                statusMessage = "�¶���";
                break;
            case ChatUserStatus.Offline:
                statusMessage = "��������";
                break;
            default:
                statusMessage = $"���� �ڵ�: {status}";
                break;
        }

        // �߰� �޽����� ������ ���� �޽����� ����
        if (message != null)
        {
            statusMessage += $" (�޽���: {message})";
        }

        // UI���� ���� ������Ʈ
        foreach (Transform child in _friendListContent.transform)
        {
            FriendItem friendItem = child.GetComponent<FriendItem>();
            if (friendItem != null && friendItem.FriendName == user)
            {
                friendItem.UpdateStatus(statusMessage); // ���¸� UI�� �ݿ�
                break; // �ش� ģ���� ã�����Ƿ� ���� ����
            }
        }

        // ��ųʸ����� ���� ������Ʈ
        if (_friendStatuses.ContainsKey(user))
        {
            _friendStatuses[user] = statusMessage; // ���� ���¸� ������Ʈ
        }
        else
        {
            _friendStatuses.Add(user, statusMessage); // ���ο� ���¸� �߰�
        }

        // ����� ��� �Ǵ� �α� UI�� �߰�
        AddLine($"ģ�� {user} ���� ������Ʈ: {statusMessage}");
    }



    // ä�� ������ �Ϸ�Ǿ��� �� ȣ��Ǵ� �ݹ� channels ������ ä�� �̸� results�� ���� ���� ����
    public void OnSubscribed(string[] channels, bool[] results)
    {
        AddLine(string.Format("ä�� ���� ({0})", string.Join(",", channels)));  // ä�ο� ������ ��, ä�� �̸� ���
    }



    // ä�� ������ ��ҵǾ��� �� ȣ��Ǵ� �ݹ�
    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("ä�� ���� ({0})", string.Join(",", channels)));  // ä�ο��� ���� ��, ������ ä�� �̸� ���
    }



    // ����ڰ� ä�ο� �������� �� ȣ��Ǵ� �ݹ�
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user}���� ä�� '{channel}'�� �����߽��ϴ�!");
    }



    // ����ڰ� ä�ο��� �������� �� ȣ��Ǵ� �ݹ�
    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user}���� ä�� '{channel}'�� �����߽��ϴ�!");
    }
}