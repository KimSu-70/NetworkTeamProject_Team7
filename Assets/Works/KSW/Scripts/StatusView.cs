using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StatusView : MonoBehaviour
{
    [SerializeField] TMP_Text statusText;
    StringBuilder sb = new StringBuilder(); 

    [SerializeField]StatusModel statusModel;

    public void SetModel(StatusModel model)
    {
        statusModel = model;
      
    }

    private void Update()
    {
        if (statusModel)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ChangeStatus();
            }
        }
    }

    public void ChangeStatus()
    {
        sb.Clear();

        sb.Append("���ݷ� : ");
        sb.Append(statusModel.Attack);
        sb.Append("\n");

        sb.Append("�ִ� ü�� : ");
        sb.Append(statusModel.MaxHP);
        sb.Append("\n");


        sb.Append("�ִ� ���¹̳� : ");
        sb.Append(statusModel.MaxStamina);
        sb.Append("\n");

        sb.Append("���¹̳� ȸ���� : ");
        sb.Append(statusModel.RecoveryStaminaMag);
        sb.Append("\n");

        sb.Append("ȸ�� �Һ� ���¹̳� : ");
        sb.Append(statusModel.ConsumeStamina);
        sb.Append("\n");

        sb.Append("���ݼӵ� : ");
        sb.Append(statusModel.AttackSpeed);
        sb.Append("\n");

        sb.Append("�̵��ӵ� : ");
        sb.Append(statusModel.MoveSpeed);
        sb.Append("\n");

        sb.Append("ġ��Ÿ Ȯ�� : ");
        sb.Append(statusModel.CriticalRate);
        sb.Append("\n");

        sb.Append("ġ��Ÿ ������ ���� : ");
        sb.Append(statusModel.CriticalDamageRate);
        sb.Append("\n");

        statusText.text = sb.ToString();
    }
}
