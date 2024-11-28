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

    private void Start()
    {
        camera = Camera.main;

        // �� ĳ���Ͱ� �ƴϸ� ī�޶� ����
        if (!photonView.IsMine)
        {
            camera.enabled = false;
        }

        // �г��� ����
        nickName.text = photonView.Owner.NickName;

    }

   
}
