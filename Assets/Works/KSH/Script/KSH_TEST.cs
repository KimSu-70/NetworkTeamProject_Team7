using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_TEST : MonoBehaviour
{
    private ChatClient _chatClient;  // Photon Chat Ŭ���̾�Ʈ ��ü

    private string _userName;        // ����� �̸�
    private string _currentChannelName;  // ���� ä�� ä�� �̸�
    private string _privateReceiver = "";    // ���� �޽��� ������

    [SerializeField] GameObject _speechBubble;  // ��ǳ�� ���� ������Ʈ
    private Coroutine _speechBubbleCoroutine;   // ��ǳ�� �ڸ�ƾ;

    [SerializeField] TMP_InputField _inputField;  // ������ �޽����� �Է��ϴ� InputField UI ���
    [SerializeField] Text _outputText;        // �޽����� ��µǴ� Text UI ���
    [SerializeField] Text _speechBubbleText;        // ��ǳ���� �޽��� ���


    [PunRPC]
    // �� ���� ��ǳ�� ȿ��
    public void SpeechBubble(string lineString)
    {
        if (_speechBubbleCoroutine != null)
        {
            StopCoroutine(_speechBubbleCoroutine);
        }

        // ���� Coroutine ����
        _speechBubbleCoroutine = StartCoroutine(DisplaySpeechBubble(lineString));
    }

    private IEnumerator DisplaySpeechBubble(string lineString)
    {
        // ��ǳ�� ǥ��
        _speechBubble.SetActive(true);

        // ��ǳ�� �ؽ�Ʈ ����
        _speechBubbleText.text += lineString + "\r\n";

        // 3�� ���� ���
        yield return new WaitForSeconds(3f);

        // 3�� �Ŀ� ��ǳ�� �����
        _speechBubble.SetActive(false);
    }
}
