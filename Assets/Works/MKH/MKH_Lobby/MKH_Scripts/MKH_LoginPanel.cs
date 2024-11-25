using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
using Photon.Pun.Demo.Cockpit;

public class MKH_LoginPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordInputField;

    [SerializeField] MKH_NickNamePanel nickNamePanel;
    [SerializeField] MKH_VerifyPanel verifyPanel;


    
    // �α���
    public void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        MKH_FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                AuthResult result = task.Result;
                Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
                CheckUserInfo();
                OnConnectedToMaster();
            });
    }

    // ���� ���� üũ
    private void CheckUserInfo()
    {
        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        if (user == null)
            return;

        if (user.IsEmailVerified == false)
        {
            // �̸��� ���� ����
            verifyPanel.gameObject.SetActive(true);
        }
        else if (user.DisplayName == "")
        {
            // �г��� ���� ����
            nickNamePanel.gameObject.SetActive(true);
        }
        else
        {
            // ���� ����
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("���ӿ� �����ߴ�!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }
}
