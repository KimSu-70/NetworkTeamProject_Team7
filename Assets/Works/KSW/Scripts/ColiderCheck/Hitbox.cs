using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // ������ �׽�Ʈ
    [SerializeField] Animator animator;

    [SerializeField] StatusModel model;
    [SerializeField] bool down;
    [SerializeField] GameObject effectPrefab;
    [SerializeField] AudioClip hitSound;
    private void Awake()
    {
        if (model == null)
            model = GetComponentInParent<StatusModel>();

       
    }

    public void HitEffect()
    {
        Instantiate(effectPrefab, transform.position, transform.rotation);

        // ������ �׽�Ʈ
        animator.speed -= 0.9f;
        StartCoroutine(SlowSpeed());
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
    }

    // ������ �׽�Ʈ
    IEnumerator SlowSpeed()
    {
        yield return new WaitForSeconds(0.2f);
        animator.speed += 0.9f;
    }

    public float GetAtk()
    {
        return model.Attack;
    }
    public bool GetDown()
    {
        return down;
    }

    public void ChangeLayer()
    {
        gameObject.layer = (int)LayerEnum.DISABLE_BOX;
    }
}
