
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;



public class AnimationEventReceiver : MonoBehaviourPunCallbacks
{
    [Header("필수 컴포넌트")]
    [SerializeField] StatusModel model;

    [SerializeField] Transform objectTransform;
    [SerializeField] AudioSource audioSource;

    [SerializeField] Rigidbody rigid;
    PlayerCamera playerCamera;


    [Header("히트 박스")]
    [SerializeField] GameObject[] hitboxes;
    [Header("피격 박스")]
    [SerializeField] GameObject hurtbox;
    [Header("투사체")]
    [SerializeField] GameObject[] projectiles;
    [Header("이펙트")]
    [SerializeField] ParticleSystem[] effects;
    [Header("범위 이펙트")]
    [SerializeField] ParticleSystem[] aoeEffects;

    [Header("범위 체크 마스크")]
    [SerializeField] LayerMask aoeRayMask;

    [Header("히트박스 레이어")]
    [SerializeField] LayerEnum hitboxLayerEnum;
    [SerializeField] LayerEnum hurtboxLayerEnum;
    //  [SerializeField] LayerMask hitboxLayerMask;

    // 히트박스 레이어
    int hitboxLayer;
    int hurtboxLayer;
    int colliderDisableLayer = (int)LayerEnum.DISABLE_BOX;



    [Header("애니메이션 이벤트")]
    [SerializeField] List<AnimationEvent> animationEvents = new();


    private void Awake()
    {
        //  hitboxLayer = Mathf.RoundToInt(Mathf.Log(hitboxLayerMask.value, 2));
        InitReceiver();


    }

    private void Start()
    {
        SetCamera();
        SetLayerAndTag();
    }

    private void SetCamera()
    {
        playerCamera = Camera.main.GetComponentInParent<PlayerCamera>();

    }

    private void InitReceiver()
    {
        audioSource = GetComponent<AudioSource>();
        hitboxLayer = (int)hitboxLayerEnum;
        hurtboxLayer = (int)hurtboxLayerEnum;
    }

    public void OnAnimationEventTriggered(string eventName)
    {

        AnimationEvent matchingEvent = animationEvents.Find(se => se.eventName == eventName);

        matchingEvent?.OnAnimationEvent?.Invoke();

    }

    public void OnAnimationEventTriggered(EventType eventType)
    {

        animationEvents[(int)eventType-1].OnAnimationEvent?.Invoke();

      

    }


    public void ControllMoveAnimation(float speed)
    {
        rigid.velocity = objectTransform.forward * speed * model.AttackSpeed;

    }

    public void ActiveHitboxAnimation(int num, bool active)
    {
        if (photonView.IsMine == false)
            return;

        hitboxes[num].layer = active ? hitboxLayer : colliderDisableLayer;

    }

    public void ActiveHurtboxAnimation(bool active)
    {
        if (photonView.IsMine == false)
            return;

        hurtbox.layer = active ? hurtboxLayer : colliderDisableLayer;

    }

    public void ActiveProjectileAnimation(int num)
    {
        projectiles[num].SetActive(true);
    }

    public void ResetColider()
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.layer = colliderDisableLayer;
        }
        if (model.ModelType == ModelType.ENEMY)
            hurtbox.layer =  hurtboxLayer;
    }

    private void SetLayerAndTag()
    {

        for (int i = 0; i < projectiles.Length; i++)
        {
            if (!photonView.IsMine)
            {
                if (model.ModelType == ModelType.PLAYER)
                {
                    projectiles[i].layer = (int)LayerEnum.OTHER_CLIENT_PLAYER_COLLIDER;
                    
                }
                else if (model.ModelType == ModelType.ENEMY)
                {
                    projectiles[i].layer = (int)LayerEnum.OTHER_CLIENT_MONSTER_COLLIDER;
                   
                }
                projectiles[i].tag = "Untagged";
            }
            else
            {
                if (model.ModelType == ModelType.PLAYER)
                {
                    projectiles[i].layer = (int)LayerEnum.PLAYER_PROJECTILE;
                    projectiles[i].tag = "Hitbox";
                }
                else if (model.ModelType == ModelType.ENEMY)
                {
                    projectiles[i].layer = (int)LayerEnum.MONSTER_PROJECTILE;
                    projectiles[i].tag = "Enemy";
                }
            }
        }
        for (int i = 0; i < hitboxes.Length; i++)
        {
            if (photonView.IsMine)
            {
                if (model.ModelType == ModelType.PLAYER)
                {
                    hitboxes[i].tag = "Hitbox";
                }
                else if (model.ModelType == ModelType.ENEMY)
                {
                    hitboxes[i].tag = "Enemy";
                   
                }
            }
            else
            {
                    hitboxes[i].tag = "Untagged";
                  
            }
        }
        if (model.ModelType == ModelType.ENEMY)
        {
            if (photonView.IsMine)
                hurtbox.tag = "Enemy";
            else
               hurtbox.tag = "Untagged";
        }
    }

    public void PlaySound(string str)
    {
        AudioClip clip = null;
        if (model.ModelType == ModelType.PLAYER)
        {
            clip = AudioManager.GetInstance().GetPlayerSoundDic(model.CharacterNumber, str);
        }
        else if (model.ModelType == ModelType.ENEMY)
        {
            clip = AudioManager.GetInstance().GetMonsterSoundDic(model.CharacterNumber, str);
        }
        audioSource.PlayOneShot(clip);
       
        
    }

    public void PlayCommonSound(string str)
    {
        AudioClip clip = null;
        clip = AudioManager.GetInstance().GetCommonSoundDic(str);
        audioSource.PlayOneShot(clip);
    }

    public void ActiveEffect(int num)
    {
        effects[num].gameObject.SetActive(true);
        effects[num].Play();
        //   effects[num].SetActive(true);
    }

    public void AOERayCast(int colliderNum)
    {



        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2, objectTransform.forward, out hit, 20f, aoeRayMask))
        {
            Vector3 vec;
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
            {
                vec = hit.transform.position;
            }
            else
            {


                vec = hit.point;
            }

            vec.y = 0;
            projectiles[colliderNum].transform.position = vec;
        }
        else
        {
            projectiles[colliderNum].transform.position = transform.position;
            projectiles[colliderNum].transform.rotation = objectTransform.rotation;
            projectiles[colliderNum].transform.Translate(Vector3.forward * 20);
        }

    }
    // 시전, 범위 표시용
    public void AOERayCast(int colliderNum, int effectNum)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2, objectTransform.forward, out hit, 20f, aoeRayMask))
        {
            Vector3 vec;
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
            {
                vec = hit.transform.position;
            }
            else
            {


                vec = hit.point;
            }

            vec.y = 0;
            projectiles[colliderNum].transform.position = vec;
            aoeEffects[effectNum].transform.position = vec;
        }
        else
        {
            projectiles[colliderNum].transform.position = transform.position;
            projectiles[colliderNum].transform.rotation = objectTransform.rotation;
            projectiles[colliderNum].transform.Translate(Vector3.forward * 20);

            aoeEffects[effectNum].transform.position = projectiles[colliderNum].transform.position;

        }
        aoeEffects[effectNum].gameObject.SetActive(true);
    }

    public void ShakeCamera(float time)
    {
        playerCamera.StartShake(time);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            SetLayerAndTag();
        }
    }
}
