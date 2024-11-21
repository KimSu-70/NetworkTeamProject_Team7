using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class MKH_LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] MKH_RoomEntry roomEntryPrefab;

    private Dictionary<string, MKH_RoomEntry> roomDictionary = new Dictionary<string, MKH_RoomEntry>();

    /*
    private void OnEnable()     // ��� ���� ��
    {
        ClearRoomEntries();
    }

    private void OnDisable()    // ������ ��
    {
        ClearRoomEntries();
    }
    */

    public void LeaveLobby()
    {
        Debug.Log("�κ� ���� ��û");
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {

            // ���� ����� ��� + ���� ������� ��� + ������ �Ұ����� ���� ���
            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
            {
                // ���� ��Ȳ : �κ� ���ڸ��� ������� ���� ���
                if (roomDictionary.ContainsKey(info.Name) == false)     // room��Ͽ� �߰��� ���� ���� ��
                    continue;                                           // �ٸ� �� ó���� ���� continue

                Destroy(roomDictionary[info.Name].gameObject);          // ����� ��
                roomDictionary.Remove(info.Name);                       // ��ųʿ��� ����
            }

            // ���ο� ���� ������ ���
            else if (roomDictionary.ContainsKey(info.Name) == false)
            {
                MKH_RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);    // �� ����
                roomDictionary.Add(info.Name, roomEntry);                           // ������ �� ��ųʸ��� �߰�
                roomEntry.SetRoomInfo(info);                                        // TODO : �� ���� ����
            }

            // ���� ������ ����� ���
            else if (roomDictionary.ContainsKey((string)info.Name) == true)
            {
                MKH_RoomEntry roomEntry = roomDictionary[info.Name];                    // ��ųʸ��� �ִ� ���� �� ����
                roomEntry.SetRoomInfo(info);                                        // TODO : �� ���� ����
            }
        }
    }

    public void ClearRoomEntries()
    {
        foreach(string name in roomDictionary.Keys)
        {
            Destroy(roomDictionary[name].gameObject);
        }
        roomDictionary.Clear();
    }
}
