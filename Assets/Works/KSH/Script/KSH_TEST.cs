using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_TEST : MonoBehaviourPun, IPunObservable
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

    private IEnumerator DisplaySpeechBubble()
    {
        // 3�� ���� ���
        yield return new WaitForSeconds(3);

        // 3�� �Ŀ� ��ǳ�� �����
        myView.RPC("CloseChatBox", RpcTarget.AllBuffered);
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
