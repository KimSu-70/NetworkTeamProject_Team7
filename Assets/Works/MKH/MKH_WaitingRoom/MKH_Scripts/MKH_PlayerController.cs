using Photon.Pun;
using UnityEngine;

public class MKH_PlayerController : MonoBehaviourPun
{
    [SerializeField] Camera _camera;
    [SerializeField] CharacterController controller;
    [SerializeField] float moveSpeed;


    private Vector3 inputDir;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        inputDir.x = Input.GetAxisRaw("Horizontal");
        inputDir.z = Input.GetAxisRaw("Vertical");
    }

    private  void FixedUpdate()
    {
        // �� �������� ��Ʈ��ũ ������Ʈ�� �ƴ� ���
        if (photonView.IsMine == false)
            return;

        if (inputDir == Vector3.zero)
            return;

        inputDir *= moveSpeed;

        controller.Move(inputDir * Time.fixedDeltaTime);
        transform.forward = _camera.transform.forward;
    }

}
