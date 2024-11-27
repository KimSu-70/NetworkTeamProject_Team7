using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public string FriendName { get; private set; }
    [SerializeField] TMP_Text _friendNameText; // ģ�� �̸��� ǥ���ϴ� �ؽ�Ʈ
    [SerializeField] TMP_Text _statusText;      // ģ�� ���¸� ǥ���ϴ� �ؽ�Ʈ
    [SerializeField] Button _removeButton;       // ģ�� ���� ��ư

    [SerializeField] KSH_NetworkLobbyChat _lobbyChat;

    // ģ�� �׸� �ʱ�ȭ
    public void Setup(string friendName, string initialStatus, KSH_NetworkLobbyChat lobbyChat)
    {
        FriendName = friendName; // �̸� ����
        _friendNameText.text = friendName; // UI�� �̸� ǥ��
        UpdateStatus(initialStatus); // �ʱ� ���� ����

        _lobbyChat = lobbyChat;
        _removeButton.onClick.AddListener(OnRemoveButtonClicked);
    }

    // ���� ������Ʈ
    public void UpdateStatus(string status)
    {
        _statusText.text = status; // ���� �ؽ�Ʈ ������Ʈ
    }

    // ���� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    private void OnRemoveButtonClicked()
    {
        _lobbyChat.RemoveFriend(FriendName); // ���� ��ũ��Ʈ�� RemoveFriend ȣ��
    }
}
