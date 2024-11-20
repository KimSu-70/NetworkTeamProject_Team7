using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class ChangeAccountInfoPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField passwordConfirmInputField;

    public void Change()
    {

        FirebaseUser user = BackendManager.Auth.CurrentUser;
        UserProfile profile = new UserProfile();



        string name = nameInputField.text;

        string pass = passwordInputField.text;
        string confirm = passwordConfirmInputField.text;

  
        if (name.IsNullOrEmpty())
        {

            Debug.LogWarning("�̸��� �Է����ּ���");
            return;
        }


        if (pass != confirm)
        {
            Debug.LogWarning("�н����� ����ġ");
            return;
        }

        if (user.DisplayName == name)
        {
            Debug.LogWarning("�̸��� �����ϴ�");
            return;
        }

        if (PhotonNetwork.LocalPlayer.GetPassWord() == pass)
        {
            Debug.LogWarning("�н����尡 �����ϴ�");
            return;
        }

        user.UpdateUserProfileAsync(profile)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                return;
            }


        });


        user.UpdatePasswordAsync(pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdatePasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Password updated successfully.");
        });

        gameObject.SetActive(false);
    }
}
