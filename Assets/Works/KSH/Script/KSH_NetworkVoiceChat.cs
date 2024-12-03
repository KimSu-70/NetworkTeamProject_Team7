using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSH_NetworkVoiceChat : MonoBehaviour
{
    private PunVoiceClient _punVoiceClient;
    private Photon.Realtime.ClientState _previousState;
    private Recorder _recorder;

    [SerializeField] Image _voiceChatImage;
    [SerializeField] Sprite[] _voiceimage = new Sprite[2];

    [SerializeField] Image _mikeImage;
    [SerializeField] Sprite[] _image = new Sprite[2];
    private bool _isTransmitEnabled = false;

    private void Awake()
    {
        // PunVoiceClient�� �̱��� �ν��Ͻ� ��������
        this._punVoiceClient = PunVoiceClient.Instance;
    }

    void Start()
    {
        // ���� ���� �����ϴ� Recorder �ν��Ͻ� ã��
        if (this._recorder == null)
        {
            // Voice ������Ʈ�� �ִ� Recorder �ν��Ͻ� ��������
            this._recorder = this._punVoiceClient.PrimaryRecorder;

            if (this._recorder != null)
            {
                Debug.Log("Recorder �ν��Ͻ��� ���������� �ҷ��Խ��ϴ�.");
                this._recorder.RecordingEnabled = true; // ���� Ȱ��ȭ
                this._recorder.TransmitEnabled = true;  // ������ ���� Ȱ��ȭ
            }
            else
            {
                Debug.LogError("Recorder�� ã�� �� �����ϴ�. Voice ������Ʈ�� ���� �����ϴ��� Ȯ���ϼ���.");
            }
        }

    }

    void Update()
    {
        if (_punVoiceClient.ClientState != _previousState)
        {
            _previousState = _punVoiceClient.ClientState;

            if (_punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
            {
                VoiceChatImage(0); // ���̽� ê Ȱ��ȭ �̹���
            }
            else if (_punVoiceClient.ClientState == Photon.Realtime.ClientState.PeerCreated ||
                     _punVoiceClient.ClientState == Photon.Realtime.ClientState.Disconnected)
            {
                VoiceChatImage(1); // ���̽� ê ��Ȱ��ȭ �̹���
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            VoiceSwitchOnClick();
        }

        // 'T' Ű�� ����ũ ON/OFF ��ȯ
        if (this._punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (this._recorder != null)
                {
                    // TransmitEnabled ���¸� ���
                    this._recorder.TransmitEnabled = !this._recorder.TransmitEnabled;
                }
            }
        }

        if (_recorder != null && _recorder.TransmitEnabled != _isTransmitEnabled)
        {
            _isTransmitEnabled = _recorder.TransmitEnabled;
            // _isTransmitEnabled ? 0 : 1 = true �϶� 0��, flase �� �� 1�� *���� ������
            MikeImage(_isTransmitEnabled ? 0 : 1);
        }
    }

    private void VoiceSwitchOnClick()
    {
        // ���� Photon Voice�� Ŭ���̾�Ʈ ���� Ȯ��
        if (this._punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
        {
            // Voice Ŭ���̾�Ʈ�� ���� �뿡 ����� ���¶�� ���� ����
            this._punVoiceClient.Disconnect();
        }
        else if (this._punVoiceClient.ClientState == Photon.Realtime.ClientState.PeerCreated ||
                 this._punVoiceClient.ClientState == Photon.Realtime.ClientState.Disconnected)
        {
            // Voice Ŭ���̾�Ʈ�� �ʱ�ȭ �����̰ų� ������� �ʾҴٸ� ������ �����ϰ� �뿡 ����
            this._punVoiceClient.ConnectAndJoinRoom(); // ���� ���� �� �뿡 �ڵ����� ����
        }
    }

    public void VoiceChatOff()
    {
        // ���� Photon Voice�� Ŭ���̾�Ʈ ���� Ȯ��
        if (this._punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
        {
            // Voice Ŭ���̾�Ʈ�� ���� �뿡 ����� ���¶�� ���� ����
            this._punVoiceClient.Disconnect();
        }
    }

    private void VoiceChatImage(int mike)
    {
        _voiceChatImage.sprite = _voiceimage[mike];
    }

    private void MikeImage(int mike)
    {
        _mikeImage.sprite = _image[mike];
    }
}
