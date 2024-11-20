using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;
using UnityEngine.Networking;

public class KSH_NetworkChat : MonoBehaviour,IChatClientListener
{
    private ChatClient _chatClient;  // Photon Chat Ŭ���̾�Ʈ ��ü

    private string _userName;        // ����� �̸�
    private string _currentChannelName;  // ���� ä�� ä�� �̸�
    public TMP_InputField InputField { get; private set; }  // ������ �޽����� �Է��ϴ� InputField UI ���
    public Text OutputText { get; private set; }    // �޽����� ��µǴ� Text UI ���

    public string ChatID { get; private set; }

    private void Start()
    {
        // ��׶��忡�� ���� ���
        Application.runInBackground = true;

        // ����� �̸��� �����÷��̾� �̸����� ����
        _userName = PhotonNetwork.LocalPlayer.NickName;

        // ChatID ���� �� ����
        ChatID = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;

        // �⺻ ä�� ����
        _currentChannelName = "Channel 001";

        // ChatClient ����, IChatClientListener�� `this`�� ����
        _chatClient = new ChatClient(this);

        //// ä�� ������ ���� ���� EU, US, ASIA
        // chatClient.ChatRegion = "ASIA";

        // ���� ������ �õ�
        Debug.Log(_chatClient.AppId);
        _chatClient.Connect(ChatID, "1.0", new AuthenticationValues(_userName));  // Photon Chat ������ ���� �õ�

        // ���� �õ� �޽��� ��� {0}�� �κп� nserName ǥ��
        AddLine(string.Format("����õ� : {0}", _userName));
    }

    public void AddLine(string lineString)
    {
        OutputText.text += lineString + "\r\n";  // outputText�� �޽����� �߰�
    }

    private void Update()
    {
        // ������ ������ �����ϰ� ���������� ȣ���Ͽ� ���� �޼����� �޽��ϴ�.
        _chatClient.Service();
    }


    // �޽����� �Է��� �� ���� Ű�� ������ �� ȣ��Ǵ� �Լ�
    public void Input_OnEndEdit(string text)
    {
        // ���� ���°� 'ConnectedToFrontEnd'�� ���� �޽��� ���� ����
        if (_chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            // ä�ο� �޽����� ����
            _chatClient.PublishMessage(_currentChannelName, InputField.text);

            // �޽��� �Է� �ʵ� �ʱ�ȭ
            InputField.text = "";
        }
    }


    // ���α׷� ���� ��, �������� ������ ����
    public void OnApplicationQuit()
    {
        if (_chatClient != null)
        {
            _chatClient.Disconnect();  // ���ø����̼� ���� ��, �������� ������ �����ϴ�.
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
        
    }



    // ������ ����Ǿ��� �� ȣ��Ǵ� �ݹ�
    public void OnConnected()
    {
        Debug.Log("����");
        AddLine("������ ����Ǿ����ϴ�.");  // ���� ���� �޽��� ���

        // ����� ��, ä�ο� ����
        _chatClient.Subscribe(new string[] { _currentChannelName }, 10);  // ä�ο� ����
    }



    // ������ ������ �������� �� ȣ��Ǵ� �ݹ�
    public void OnDisconnected()
    {
        
    }



    // ä�ο��� �޽����� ���ŵǾ��� �� ȣ��Ǵ� �ݹ� channelName ä���̸�, senders ����� �̸�, messages �޽��� ����
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }



    // ���� �޽����� ���ŵǾ��� �� ȣ��Ǵ� �ݹ�
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }



    // ����� ���� ������Ʈ�� ���� �� ȣ��Ǵ� �ݹ�
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }



    // ä�� ������ �Ϸ�Ǿ��� �� ȣ��Ǵ� �ݹ� channels ������ ä�� �̸� results�� ���� ���� ����
    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }



    // ä�� ������ ��ҵǾ��� �� ȣ��Ǵ� �ݹ�
    public void OnUnsubscribed(string[] channels)
    {
        
    }



    // ����ڰ� ä�ο� �������� �� ȣ��Ǵ� �ݹ�
    public void OnUserSubscribed(string channel, string user)
    {
        
    }



    // ����ڰ� ä�ο��� �������� �� ȣ��Ǵ� �ݹ�
    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}
