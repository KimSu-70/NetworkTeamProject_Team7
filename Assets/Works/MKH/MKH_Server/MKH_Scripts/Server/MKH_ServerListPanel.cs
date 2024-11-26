using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_ServerListPanel : MonoBehaviour
{
    [SerializeField] RectTransform serverContent;
    [SerializeField] MKH_ServerEntry serverEntryPrefab;

    private Dictionary<string, MKH_ServerEntry> serverDictionary = new Dictionary<string, MKH_ServerEntry>();

    public void LeaveLobby()
    {
        Debug.Log("���� �κ� ���� ��û");
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateServerList(List<RoomInfo> serverList)
    {
        foreach (RoomInfo info in serverList)
        {

            // ���� ����� ��� + ���� ������� ��� + ������ �Ұ����� ���� ���
            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
            {
                // ���� ��Ȳ : �κ� ���ڸ��� ������� ���� ���
                if (serverDictionary.ContainsKey(info.Name) == false)     // room��Ͽ� �߰��� ���� ���� ��
                    continue;                                           // �ٸ� �� ó���� ���� continue

                Destroy(serverDictionary[info.Name].gameObject);          // ����� ��
                serverDictionary.Remove(info.Name);                       // ��ųʿ��� ����
            }

            // ���ο� ���� ������ ���
            else if (serverDictionary.ContainsKey(info.Name) == false)
            {
                MKH_ServerEntry serverEntry = Instantiate(serverEntryPrefab, serverContent);    // �� ����
                serverDictionary.Add(info.Name, serverEntry);                           // ������ �� ��ųʸ��� �߰�
                serverEntry.SetServerInfo(info);                                        // TODO : �� ���� ����
            }

            // ���� ������ ����� ���
            else if (serverDictionary.ContainsKey((string)info.Name) == true)
            {
                MKH_ServerEntry roomEntry = serverDictionary[info.Name];                    // ��ųʸ��� �ִ� ���� �� ����
                roomEntry.SetServerInfo(info);                                        // TODO : �� ���� ����
            }
        }
    }

    public void ClearServerEntries()
    {
        foreach (string name in serverDictionary.Keys)
        {
            Destroy(serverDictionary[name].gameObject);
        }
        serverDictionary.Clear();
    }
}
