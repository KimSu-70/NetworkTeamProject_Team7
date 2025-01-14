using Firebase.Analytics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[System.Serializable]
public class MonsterPattern
{
    public string pattern;
    public float range;
}

public class MonsterController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Animator animator;
    [SerializeField] List<PlayerController> pc_s;
    [SerializeField] Rigidbody rigid;

    [SerializeField] StatusModel model;

    [SerializeField] float aniSpeed;
    [SerializeField] public float atk;
    [SerializeField] float speed;
    [SerializeField] float range;

    [SerializeField] bool isFixed;
    [SerializeField] bool isMoveAni;

    float lag;
    float time;
    float aniStateTime;
 
    Transform target;

    [SerializeField] MonsterPattern[] patterns;

    int[] animtionHash;
    [SerializeField] int[] animatorParameterHash;

    [SerializeField] int runParameterHash;
    [SerializeField] int waitParameterHash;
    [SerializeField] int atkEndParameterHash;


    WaitForSeconds cooldown = new WaitForSeconds(0.5f);
    Coroutine cooldownCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pc_s = new List<PlayerController>();
        rigid = GetComponent<Rigidbody>();
        model = GetComponent<StatusModel>();
        FindPlayers();

        animtionHash = new int[patterns.Length];
        animatorParameterHash = new int[patterns.Length];
        for (int i = 0; i < patterns.Length; i++)
        {
            animtionHash[i] = Animator.StringToHash(patterns[i].pattern);

            AnimatorControllerParameter animatorControllerParameter = animator.parameters[i];
            animatorParameterHash[i] = animatorControllerParameter.nameHash;
        }

        for (int i = 0; i < animator.parameterCount; i++)
        {
           
            AnimatorControllerParameter animatorControllerParameter = animator.parameters[i];

            if (animatorControllerParameter.name == "Run")
            {
                runParameterHash = animatorControllerParameter.nameHash;
            }
            if (animatorControllerParameter.name == "Wait")
            {
                waitParameterHash = animatorControllerParameter.nameHash;
            }
            if (animatorControllerParameter.name == "AtkEnd")
            {
                atkEndParameterHash = animatorControllerParameter.nameHash;
            }

        }


        animator.speed = aniSpeed;
    }

    public void FindPlayers()
    {
        pc_s.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in objects)
        {
            pc_s.Add(obj.GetComponent<PlayerController>());
        }
        TargetChange();


    }

    public void TargetChange()
    {
        if (photonView.IsMine == false)
            return;
        int ran = Random.Range(0, pc_s.Count);
        if (pc_s[ran] == null)
        {
            return;
        }
        else
        {
            target = pc_s[ran].transform;
        }

      
    }

    private void Start()
    {

        if (photonView.IsMine == false)
            StartCoroutine(CheckAniLag());
    }

    private void Update()
    {

        TraceMonster();


        SetAniTime();
    }

    IEnumerator CheckAniLag()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);




            if (Mathf.Abs(lag) > 0.5f)
            {

            }
            else if (Mathf.Abs(lag) > 0.05f)
            {
                Debug.Log("렉 발생"); 
                for (int i = 0; i < patterns.Length; i++)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName(patterns[i].pattern))
                    {
                        animator.Play(animtionHash[i], 0, aniStateTime);
                    }
                  
                }

               

            }




        }
    }

    public void SetAniTime()
    {
       






        if (photonView.IsMine == false)
        {
            time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            lag = time - aniStateTime;
            return;
        }
        
        aniStateTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;




    }

    public void TraceMonster()
    {

        if (isFixed)
        {
            if (isMoveAni)
            {
                return;
            }
            rigid.velocity = Vector3.zero;
            return;
        }

        if (photonView.IsMine == false)
            return;

        if (target == null)
        {
            FindPlayers();
        }

        
        
        if ((target.position - transform.position).sqrMagnitude < range)
        {
            rigid.velocity = Vector3.zero;


            animator.SetBool(runParameterHash, false);
            animator.SetBool(waitParameterHash, true);
            MonsterAttack();

            return;
        }


        animator.SetBool(runParameterHash, true);
        animator.SetBool(waitParameterHash, false);
        // Look



        transform.LookAt(target);
        //   if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))

        // Trace

        rigid.velocity = transform.forward * speed;

    }

    public void MonsterAttack()
    {
       
        transform.LookAt(target);
        isFixed = true;
       

        int ran = Random.Range(0, patterns.Length);
       
        animator.SetBool(animatorParameterHash[ran], true);

        animator.SetBool(atkEndParameterHash, false);
    }

    public void PatternReset()
    {
        if(cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(AttackCooldown());
       

        for (int i = 0; i < animatorParameterHash.Length; i++)
        {

            animator.SetBool(animatorParameterHash[i], false);
        }

        animator.SetBool(atkEndParameterHash, true);

        TargetChange();
        Debug.Log("패턴 끝");
    }


    public void MoveAni()
    {
        isMoveAni = true;
    }

    public void EndMoveAni()
    {
        isMoveAni = false;
        rigid.velocity = Vector3.zero;
    }

    IEnumerator AttackCooldown()
    {
        yield return cooldown;
        isFixed = false;
    }

    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFixed);
            stream.SendNext(aniStateTime);

        }

        else if (stream.IsReading)
        {

            isFixed = (bool)stream.ReceiveNext();
            aniStateTime = (float)stream.ReceiveNext();

        }

    }

    public void TakeDamage(float damage)
    {

        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.AllViaServer, damage);
    }
    [PunRPC]
    public void TakeDamageRPC(float damage)
    {

        Debug.Log("HIT!!!!!!!!!!!!!!" + damage);
        model.HP -= (float)damage;
    }
}
