using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;

public class MKH_PlayerManager : MonoBehaviourPun
{
    // ���� ī�޶�
    [SerializeField] Camera camera;

    // �������� �г���
    [SerializeField] TMP_Text nickName;

    private void Start()
    {
        // �� ĳ���Ͱ� �ƴϸ� ī�޶� ����
        if (!photonView.IsMine)
        {
            camera.gameObject.SetActive(false);
        }

        camera = Camera.main;

        // �г��� ����
        nickName.text = photonView.Owner.NickName;
    }


}
