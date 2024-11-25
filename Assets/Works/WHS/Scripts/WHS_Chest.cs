using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Chest : MonoBehaviourPun
{
    // ���� �浹 �� ������ ����
    // TODO : �÷��̾ ���� �����ؼ� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                WHS_ItemManager.Instance.DestroyAllChests(this);

                Vector3 spawnPos = transform.position;
                spawnPos.y += 1f;

                WHS_ItemManager.Instance.SpawnItem(spawnPos);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}