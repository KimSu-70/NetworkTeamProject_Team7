//using Photon.Voice.PUN;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class KSH_NetworkVoiceChat : MonoBehaviour
//{
//    private PunVoiceClient punVoiceClient;

//    private void Awake()
//    {
//        this.punVoiceClient = PunVoiceClient.Instance;
//    }


//    private void VoiceSwitchOnClick()
//    {
//        // ���� Photon Voice�� Ŭ���̾�Ʈ ���� Ȯ��
//        if (this.punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
//        {
//            // Voice Ŭ���̾�Ʈ�� ���� �뿡 ����� ���¶�� ���� ����
//            this.punVoiceClient.Disconnect();
//        }
//        else if (this.punVoiceClient.ClientState == Photon.Realtime.ClientState.PeerCreated ||
//                 this.punVoiceClient.ClientState == Photon.Realtime.ClientState.Disconnected)
//        {
//            // Voice Ŭ���̾�Ʈ�� �ʱ�ȭ �����̰ų� ������� �ʾҴٸ� ������ �����ϰ� �뿡 ����
//            this.punVoiceClient.ConnectAndJoinRoom(); // ���� ���� �� �뿡 �ڵ����� ����
//        }
//    }
//}
