using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// Hashtable ����� PhotonHashtable �̶�� �̸����� ����� �ϰ� ���� �� ���� �ڵ�(using ����)
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;


public static class MKH_CustomProperty
{
    // ������ �÷��͸� ���̱� ���� �ڵ�
    private static PhotonHashtable customProperty = new PhotonHashtable();

    public const string READY = "Ready";

    public static void SetReady(this Player player, bool ready)
    {
        // ��� �߰��� �����ϱ� �� Ŭ����
        customProperty.Clear();
        // ó�� �� ������ �� Ready�� false�� ����
        customProperty.Add(READY, ready);
        player.SetCustomProperties(customProperty);
    }

    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        // Ready ��ư ������ ��
        if(customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
            
        }
        else
        {
            return false;
        }
    }
}
