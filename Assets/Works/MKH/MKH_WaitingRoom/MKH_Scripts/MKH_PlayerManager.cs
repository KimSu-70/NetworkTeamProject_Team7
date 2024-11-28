using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MKH_PlayerManager : MonoBehaviourPun
{
    // ���� ī�޶�
    public Camera camera;

    // �������� �г���
    public TMP_Text nickName;

    // ���� Player�� ����ִ� �θ� gameobject
    public GameObject playerList;

    private void Start()
    {
        // �� ĳ���Ͱ� �ƴϸ� ī�޶� ����
        if (!photonView.IsMine)
        {
            camera.enabled = false;
        }

        // �г��� ����
        nickName.text = photonView.Owner.NickName; // connection manager�� join room���� ��������

    }

    public void SelectModel(string characterName)
    {
        // rpc �Լ��� ĳ���͸� ����
        photonView.RPC(nameof(RpcSelectModel), RpcTarget.AllBuffered, characterName);
    }

    [PunRPC]
    void RpcSelectModel(string characterName)
    {
        // Player ������ �ȿ� ����ִ� ĳ���� ��
        foreach (Transform t in playerList.transform)
        {
            if (t.name == characterName)
            {
                t.gameObject.SetActive(true);
            }
        }
    }
}
