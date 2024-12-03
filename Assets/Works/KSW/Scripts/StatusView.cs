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
        statusModel.OnChangedStatusEvent += ChangeStatus;

        ChangeStatus();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (statusModel)
        {
            statusModel.OnChangedStatusEvent += ChangeStatus;
            ChangeStatus();
        }
    }

    private void OnDisable()
    {
        if(statusModel)
            statusModel.OnChangedStatusEvent -= ChangeStatus;
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
        sb.Append(string.Format("{0:N2}", statusModel.RecoveryStaminaMag));
        sb.Append("\n");

        sb.Append($"ȸ�� �Һ� ���¹̳� : ");
        sb.Append(string.Format("{0:N2}", statusModel.ConsumeStamina));
        sb.Append("\n");

        sb.Append("���ݼӵ� : ");
        sb.Append(string.Format("{0:N2}", statusModel.AttackSpeed));
        sb.Append("\n");

        sb.Append("�̵��ӵ� : ");
        sb.Append(string.Format("{0:N2}", statusModel.MoveSpeed));
        sb.Append("\n");

        sb.Append("ġ��Ÿ Ȯ�� : ");
        sb.Append(string.Format("{0:N2}", statusModel.CriticalRate));
        sb.Append("\n");

        sb.Append("ġ��Ÿ ������ ���� : ");
        sb.Append(string.Format("{0:N2}", statusModel.CriticalDamageRate));
        sb.Append("\n");

        statusText.text = sb.ToString();
    }
}
