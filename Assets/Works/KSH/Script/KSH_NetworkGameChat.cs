using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_NetworkGameChat : MonoBehaviourPun, IChatClientListener, IPunObservable
{
    private ChatClient _chatClient;  // Photon Chat Ŭ���̾�Ʈ ��ü

    private string _userName;        // ����� �̸�
    private string _currentChannelName;  // ���� ä�� ä�� �̸�
    private string _privateReceiver = "";    // ���� �޽��� ������
    private bool isWhispering = false;  // �Ӹ� ������� ����
    [SerializeField] TMP_InputField _privateInputField; // ���� �޽��� ����� �̸� �Է�

    private PhotonView myView;

    [SerializeField] GameObject _speechBubble;  // ��ǳ�� ���� ������Ʈ
    private Coroutine _speechBubbleCoroutine;   // ��ǳ�� �ڸ�ƾ;

    [SerializeField] GameObject _networkChat;       // ��Ʈ��ũ ä�� ������Ʈ
    [SerializeField] TMP_InputField _inputField;  // ������ �޽����� �Է��ϴ� InputField UI ���
    [SerializeField] Text _outputText;        // �޽����� ��µǴ� Text UI ���
    [SerializeField] Text _speechBubbleText;        // ��ǳ���� �޽��� ���

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

    private void Start()
    {
        myView = GetComponent<PhotonView>();    // 
        _speechBubble.SetActive(false);         // ������Ʈ ��Ȱ��ȭ
        _networkChat.SetActive(false);
    }

    private void Update()
    {
        // ������ ������ �����ϰ� ���������� ȣ���Ͽ� ���� �޼����� �޽��ϴ�.
        _chatClient.Service();

        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Return))
        {
            if (_networkChat.activeSelf)
            {
                // ä��â�� �ݱ�
                _networkChat.SetActive(false);
                if (_inputField.text.Length > 0)
                {
                    if (_chatClient.State == ChatState.ConnectedToFrontEnd)
                    {
                        SubmitPublicChatOnClick();
                        SubmitPrivateChatOnClick();

                        // ä�� �޽����� �ٸ� ������� ����
                        if (_privateReceiver == "")
                        {
                            myView.RPC("OpenChatBox", RpcTarget.AllBuffered);
                            _inputField.text = "";
                            if (_speechBubbleCoroutine != null)
                            {
                                StopCoroutine(_speechBubbleCoroutine);
                            }
                            // ���� Coroutine ����
                            _speechBubbleCoroutine = StartCoroutine(DisplaySpeechBubble());
                        }
                    }
                }
            }
            else
            {
                // ä��â Ȱ��ȭ
                _networkChat.SetActive(true);
                _inputField.ActivateInputField(); // �Է� �ʵ� Ȱ��ȭ
                _inputField.Select(); // �Է� �ʵ忡 ��Ŀ�� ����
            }
        }

        // �� Ű�� ���� �Է¶� ��ȯ
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInputField();
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
        }

    }


    // ���� ä���� �����ϴ� �Լ�
    public void SubmitPrivateChatOnClick()
    {

        if (_privateReceiver != "") // �����ڰ� �����Ǿ� ������
        {
            _chatClient.SendPrivateMessage(_privateReceiver, _inputField.text); // �ش� �����ڿ��� ���� �޽��� ����
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


    private IEnumerator DisplaySpeechBubble()
    {
        // 3�� ���� ���
        yield return new WaitForSeconds(3);

        // 3�� �Ŀ� ��ǳ�� �����
        myView.RPC("CloseChatBox", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void OpenChatBox()
    {
        // ��ǳ�� ǥ��
        _speechBubble.SetActive(true);

        // ��ǳ�� �޽��� ���� �ʱ�ȭ
        _speechBubbleText.text = "";

        // ��ǳ�� �ؽ�Ʈ ����
        _speechBubbleText.text = _inputField.text;
    }

    [PunRPC]
    public void CloseChatBox()
    {
        _speechBubble.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // ���� �����͸� ���� ��
        {
            stream.SendNext(_speechBubbleText.text);  // ä�� �޽��� ����
        }
        else if (stream.IsReading) // ���� �����͸� ���� ��
        {
            _speechBubbleText.text = (string)stream.ReceiveNext();  // ä�� �޽��� ����
        }
    }

    // ä�� �Է¶��� �Ӹ� �Է¶��� ��ȯ
    private void ToggleInputField()
    {
        if (isWhispering)
        {
            // �Ӹ� ��忡�� ä�� ���� ��ȯ
            _inputField.Select(); // ä�� �Է¶� ��Ŀ��
        }
        else
        {
            // ä�� ��忡�� �Ӹ� ���� ��ȯ
            _privateInputField.Select(); // �Ӹ� ����� �Է¶� ��Ŀ��
        }

        // �Ӹ� ��� ���� ���
        isWhispering = !isWhispering;
    }
}