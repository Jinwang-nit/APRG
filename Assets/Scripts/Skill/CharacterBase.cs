using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IStateMachineOwner, ISkillOwner, IHurt
{
    [SerializeField] protected ModelBase model;
    public ModelBase Model { get => model; }
    [SerializeField] protected CharacterController characterController;
    public CharacterController CharacterController { get => characterController; }
    public Transform Transform => Model.transform;
    public AudioClip[] footStepClip;
    public float gravity = -9.8f;
    protected StateMachine stateMachine;
    [SerializeField] protected AudioSource audioSource;

    public List<string> enemyTagList;
    public SkillConfig[] standAttackConfigs;
    public Skill_HurtDate hurtDate { get; protected set; }
    public ISkillOwner hurtSource { get; protected set; }
    public List<SkillInfo> skillInfoList = new List<SkillInfo>();
    public virtual void Init()
    {
        canSwithSkill = true;
        Model.Init(this, enemyTagList);
        stateMachine = new StateMachine();
        stateMachine.Init(this);
    }

    /*
 ****** 技能相关配置 *******
 */
    protected SkillConfig currentSkillConfig;
    public SkillConfig CurrentSkillConfig { get => currentSkillConfig; }
    protected int currentHitId = 0;
    protected bool canSwithSkill;
    public bool CanSwithSkill { get => canSwithSkill; }
    public void StartAttack(SkillConfig skillConfig)
    {
        canSwithSkill = false; // 防止立即切换其它技能
        currentSkillConfig = skillConfig;
        currentHitId = 0;
        // 播放技能动画
        PlayAnimation(currentSkillConfig.AnimationName);
        // 技能特效，音效
        SpawnSkillObject(currentSkillConfig.ReleaseData.skill_SpawnObj);
        PlayAudio(currentSkillConfig.ReleaseData.AudioClip);
    }

    void ISkillOwner.StartSkillHit(int weaponIndex)
    {
        // 技能音效
        SpawnSkillObject(currentSkillConfig.AttackData[currentHitId].skill_SpawnObj);
        // 技能特效
        PlayAudio(currentSkillConfig.AttackData[currentHitId].AudioClip);
    }

    void ISkillOwner.StopSkillHit(int weaponIndex)
    {
        currentHitId += 1;
    }

    void ISkillOwner.SkillCanSwitch()
    {
        canSwithSkill = true;
    }

    public virtual void OnHit(IHurt target, Vector3 hitPostion)
    {
        // 拿到这段攻击数据
        Skill_AttackData attackData = currentSkillConfig.AttackData[currentHitId];
        PlayAudio(attackData.SkillHitEFConfig.AudioClip);
        // 传递伤害
        if (target.Hurt(attackData.hurtDate, this))
        {
            // 生成基于命中的配置效果
            StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig.SpawnObj, hitPostion));
            StartFreezeFrame(attackData.FreezeFrameTime);
        }
        else
        {
            StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig.FailSpawnObj, hitPostion));
        }
    }

    /*
      ****** 协程实现暂停时间 ******
      *这里其实是暂停播放动画来达到暂停时间
    */
    protected void StartFreezeFrame(float time)
    {
        if (time > 0) StartCoroutine(DoFreezeFrame(time));
    }
    protected IEnumerator DoFreezeFrame(float time)
    {
        Model.Animator.speed = 0;
        yield return new WaitForSeconds(time);
        Model.Animator.speed = 1;
    }

    protected IEnumerator DoSkillHitEF(Skill_SpawnObj spawnObj, Vector3 sapwnPoint)
    {
        if (spawnObj == null) yield break;

        if (spawnObj != null && spawnObj.Prefab != null)
        {
            yield return new WaitForSeconds(spawnObj.Time);
            GameObject temp = Instantiate(spawnObj.Prefab);
            temp.transform.position = sapwnPoint + spawnObj.Position;
            temp.transform.localScale = sapwnPoint + spawnObj.Scale;

            temp.transform.LookAt(Camera.main.transform);
            temp.transform.eulerAngles += spawnObj.Rotation;
            PlayAudio(spawnObj.AudioClip);
        }
    }
    public void OnSkillOver()
    {
        canSwithSkill = true;
    }

    /*
      ****** 协程实现延迟生成物体 ******
      *生成刀光特效，挥刀和特效要同时出现
    */
    protected void SpawnSkillObject(Skill_SpawnObj spawnObj)
    {
        if (spawnObj != null && spawnObj.Prefab != null)
        {
            StartCoroutine(DoSpawnObject(spawnObj));
        }
    }
    protected IEnumerator DoSpawnObject(Skill_SpawnObj spawnObj)
    {
        // 延迟时间
        yield return new WaitForSeconds(spawnObj.Time);
        GameObject skillObj = GameObject.Instantiate(spawnObj.Prefab, null);
        // 要相对模型
        skillObj.transform.position = Model.transform.position + Model.transform.TransformDirection(spawnObj.Position);
        skillObj.transform.localScale = spawnObj.Scale;
        skillObj.transform.eulerAngles = Model.transform.eulerAngles + spawnObj.Rotation;
        PlayAudio(spawnObj.AudioClip);

        // 查找是否有技能物体
        if (skillObj.TryGetComponent<SkillObjectBase>(out SkillObjectBase skillObject))
        {
            skillObject.Init(enemyTagList, OnHitForRealseData);
        }
    }

    public virtual void OnHitForRealseData(IHurt target, Vector3 hitPostion)
    {
        // 拿到这段攻击数据
        Skill_AttackData attackData = currentSkillConfig.ReleaseData.AttackData;
        PlayAudio(attackData.SkillHitEFConfig.AudioClip);
        // 传递伤害
        if (target.Hurt(attackData.hurtDate, this))
        {
            // 生成基于命中的配置效果
            StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig.SpawnObj, hitPostion));
            StartFreezeFrame(attackData.FreezeFrameTime);
        }
        else
        {
            StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig.FailSpawnObj, hitPostion));
        }
    }

    private string currentAnimationName;
    public void PlayAnimation(string animationName, bool reState = false, float fixedTransitionDuration = 0.25f)
    {
        if (currentAnimationName == animationName && !reState)
        {
            return;
        }

        currentAnimationName = animationName;
        model.Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }

    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null) audioSource.PlayOneShot(audioClip);
    }


    public void OnFootStep()
    {
        audioSource.PlayOneShot(footStepClip[Random.Range(0, footStepClip.Length)]);
    }

    public virtual void SetHurtDate(Skill_HurtDate hurtDate, ISkillOwner hurtSource)
    {
        this.hurtDate = hurtDate;
        this.hurtSource = hurtSource;
    }

    public abstract bool Hurt(Skill_HurtDate hurtDate, ISkillOwner hurtSource);
}