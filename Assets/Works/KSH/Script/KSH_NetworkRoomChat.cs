using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;

public class KSH_NetworkRoomChat : MonoBehaviour, IChatClientListener
{
    private ChatClient _chatClient;  // Photon Chat Ŭ���̾�Ʈ ��ü

    private string _userName;        // ����� �̸�
    private string _currentChannelName;  // ���� ä�� ä�� �̸�
    private string _privateReceiver = "";    // ���� �޽��� ������
    [SerializeField] TMP_InputField _inputField;  // ������ �޽����� �Է��ϴ� InputField UI ���
    [SerializeField] Text _outputText;        // �޽����� ��µǴ� Text UI ���

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
        _currentChannelName = "Channel 002";

        // ChatClient ����, IChatClientListener�� `this`�� ����
        _chatClient = new ChatClient(this);

        //// ä�� ������ ���� ���� EU, US, ASIA
        // chatClient.ChatRegion = "ASIA";

        // ���� ������ �õ�
        Debug.Log(_chatClient.AppId);
        _chatClient.Connect(_chatID, "1.0", new AuthenticationValues(_userName));  // Photon Chat ������ ���� �õ�

        // ���� �õ� �޽��� ��� {0}�� �κп� nserName ǥ��
        AddLine(string.Format(" ����õ� : {0}", _userName));
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


    public void AddLine(string lineString)
    {
        _outputText.text += lineString + "\r\n";  // outputText�� �޽����� �߰�
    }

    // ���� �޽����� ���� �����ڸ� ����
    public void ReceiverOnValueChange(string valueIn)
    {
        _privateReceiver = valueIn; // �Էµ� ���� privateReceiver�� ����
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

        // ä�� ä���� Photon �� �̸����� ����
        _currentChannelName = PhotonNetwork.CurrentRoom.Name;

        // ����� ��, ä�ο� ����
        _chatClient.Subscribe(new string[] { _currentChannelName }, 0);  // ä�ο� ����
    }



    // ������ ������ �������� �� ȣ��Ǵ� �ݹ�
    public void OnDisconnected()
    {
        Debug.Log("���� �ȵ�");
        // ���� ������ ���� Ȯ���� ���� �α� �߰�
        Debug.Log("��Ŀ��Ʈ ����: " + _chatClient.DebugOut.ToString());
        AddLine("������ ������ ���������ϴ�.");  // ���� ������ �޽��� ���
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
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
        // ���� ���濡 ���� �α� ���
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