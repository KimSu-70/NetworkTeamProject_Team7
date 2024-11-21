using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_VerifyPanel : MonoBehaviour
{
    [SerializeField] MKH_NickNamePanel nickNamePanel;

    private void OnEnable()
    {
        SendVerifyMail();
    }

    private void OnDisable()
    {
        if(checkVerifyRoutine != null)
        {
            StopCoroutine(checkVerifyRoutine);
        }
    }

    // �̸��� ������
    private void SendVerifyMail()
    {
        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        user.SendEmailVerificationAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendEmailVerificationAsync was canceled.");
                    gameObject.SetActive(false);
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                    gameObject.SetActive(false);
                    return;
                }

                Debug.Log("Email sent successfully.");
                checkVerifyRoutine = StartCoroutine(CheckVerifyRoutine());
            });
    }

    Coroutine checkVerifyRoutine;
    // �̸��� ���� Ȯ�� �ڷ�ƾ
    IEnumerator CheckVerifyRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(3f);

        while(true)
        {
            // �������� ���ΰ�ħ(���� ���� �������� ��)
            MKH_FirebaseManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(TaskExtension =>
            {
                if(TaskExtension.IsCanceled)
                {
                    Debug.LogError("ReloadAsync was canceled.");
                    return;
                }
                if(TaskExtension.IsFaulted)
                {
                    Debug.LogError($"ReloadAsync encountered an error : {TaskExtension.Exception.Message}");
                    return;
                }

                //���� Ȯ��
                if (MKH_FirebaseManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    Debug.Log("���� Ȯ��");
                    nickNamePanel.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            });

            yield return delay;
        }
    }
}
