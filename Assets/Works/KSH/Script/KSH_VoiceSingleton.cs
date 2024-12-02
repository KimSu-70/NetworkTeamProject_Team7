using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSH_VoiceSingleton : MonoBehaviour
{
    void Awake()
    {
        // �̹� �����ϴ� PunVoiceClient�� �ִ� ��� ���� ��ü�� ����
        if (FindObjectsOfType<PunVoiceClient>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // �ߺ��� �ƴ϶�� �� ������Ʈ�� ����
        DontDestroyOnLoad(gameObject);
    }
}
