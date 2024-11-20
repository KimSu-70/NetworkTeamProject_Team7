using UnityEngine;
using Photon.Pun;

using Photon.Realtime;
using System.Collections.Generic;
public class MyLobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] MyRoomEntry roomEntryPrefab;

    private Dictionary<string, MyRoomEntry> roomDictionary = new Dictionary<string, MyRoomEntry>();



    public void LeaveLobby()
    {
        Debug.Log("�κ� ���� ��û");
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // ���� ����� ��� + ���� ������� ���
            if (info.RemovedFromList || info.IsVisible == false || info.IsOpen == false)
            {
                // ���� ��Ȳ : �κ� ���ڸ��� ������� ���� ���
                if (roomDictionary.ContainsKey(info.Name) == false)
                    continue;

                Destroy(roomDictionary[info.Name].gameObject);
                roomDictionary.Remove(info.Name);
            }

            //  ���ο� ���� ������ ���

            else if (roomDictionary.ContainsKey(info.Name) == false)
            {
                MyRoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomDictionary.Add(info.Name, roomEntry);
                roomEntry.SetRoomInfo(info);
            }

            //  ���� ������ ����� ���
            else if (roomDictionary.ContainsKey(info.Name) == true)
            {
                MyRoomEntry roomEntry = roomDictionary[info.Name];
                roomEntry.SetRoomInfo(info);

            }

        }
    }

    public void ClearRoomEntries()
    {
        foreach (string name in roomDictionary.Keys)
        {
            Destroy(roomDictionary[name].gameObject);
        }
        roomDictionary.Clear();
    }
}
