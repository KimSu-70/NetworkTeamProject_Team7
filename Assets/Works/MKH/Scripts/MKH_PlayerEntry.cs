using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MKH_PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text readyText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button readyButton;

    public void SetPlayer(Player player)
    {
        // �÷��̾� �г���
        // ������ �÷��̾�
        if (player.IsMasterClient)
        {
            // �̸� �� ���� �� ����(�ٸ��ɷ� ���� ����)
            nameText.text = $"Master \n {player.NickName}";
        }
        // ������ �ƴ� �÷��̾�
        else
        {
            nameText.text = player.NickName;
        }
        // �����ư Ȱ��ȭ
        readyButton.gameObject.SetActive(true);
        // �� �ڽ��̸� �����ư Ŭ�� ����
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;
        if(player.GetReady())
        {
            readyText.text = "Ready";
        }
        else
        {
            readyText.text = "";
        }
    }

    public void SetEmpty()
    {
        readyText.text = "";
        nameText.text = "None";
        readyButton.gameObject.SetActive(false);
    }

    public void Ready()
    {
        // ���� Ǯ�������� �ٽ� Ŭ�����ְ�
        // Ŭ�� �Ǿ������� �ٽ� Ǯ���ִ� �۾��� ���� �ڵ�
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }

    }
}
