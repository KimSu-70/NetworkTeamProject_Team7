using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_ServerStateLogger : MonoBehaviourPunCallbacks 
{
    [SerializeField] ClientState state;

    private void Update()
    {
        if (state == PhotonNetwork.NetworkClientState)      // ���� ���¿� ���� ���¸� ���� ����
            return;

        state = PhotonNetwork.NetworkClientState;       // ���� ���� Ȯ��(�����ߴ��� �ƴ����� ���� ��Ȳ)
        Debug.Log($"[Pun] {state}");
    }
}
