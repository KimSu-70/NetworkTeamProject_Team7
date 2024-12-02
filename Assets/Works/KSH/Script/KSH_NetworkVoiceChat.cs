using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSH_NetworkVoiceChat : MonoBehaviour
{
    private PunVoiceClient _punVoiceClient;
    private Recorder _recorder;

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
}
