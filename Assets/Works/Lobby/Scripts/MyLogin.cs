using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyLogin : MonoBehaviour
{
    [SerializeField] TMP_InputField idText;
    [SerializeField] TMP_InputField nickText;
    private void Start()
    {
        idText.text = $"ID {Random.Range(1000, 10000)}";
        nickText.text = $"NickName";
    }

    public void SetLogin()
    {
        if (idText.text == "")
        {
            Debug.LogWarning("���̵� �Է��ؾ� ������ �����մϴ�.");
            return;
        }
        if (nickText.text == "")
        {
            Debug.LogWarning("�г����� �Է��ؾ� ������ �����մϴ�.");
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = idText.text;
        PhotonNetwork.LocalPlayer.SetNickName(nickText.text);
      
        PhotonNetwork.ConnectUsingSettings();
        
    }
}
