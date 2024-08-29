using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : CharacterBase
{ 
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("配置")]
    public float rotateSpeed;
    public float rotationSpeedForAttack = 2;
    public float runTransition = 1;
    public float runSpeed = 1;
    public float walkSpeed = 1;
    public float jumpPower = 1; 
    public float jumpingSpeed;
    public float airDownSpeed;
    public float waitCounterattackTime;
    public SkillConfig counterattackSkillConfig;
    public SkillConfig jumpAttackConfig;
    public bool isDefence { get => currentState == PlayerState.Defence; }

    private void Start()
    {
        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Init();
        ChangeState(PlayerState.Idle); // 默认是Idle
    }

    public void ScreenImpulse(float value)
    {
        impulseSource.GenerateImpulse(value);
    }
    private PlayerState currentState;
    public void ChangeState(PlayerState playerState, bool reCurrstate = false)
    {
        currentState = playerState;
        switch (playerState)
        {
            case PlayerState.Idle:
                stateMachine.ChangeState<Player_IdleState>(reCurrstate);
                break;
            case PlayerState.Move:
                stateMachine.ChangeState<Player_MoveState>(reCurrstate);
                break;
            case PlayerState.Jump:
                stateMachine.ChangeState<Player_JumpState>(reCurrstate);
                break;
            case PlayerState.AirDown:
                stateMachine.ChangeState<Player_AirDownState>(reCurrstate);
                break;
            case PlayerState.Roll:
                stateMachine.ChangeState<Player_RollState>(reCurrstate);
                break;
            case PlayerState.StandAttack:
                stateMachine.ChangeState<Player_StandAttackState>(reCurrstate);
                break;
            case PlayerState.Hurt:
                stateMachine.ChangeState<Player_HurtState>(reCurrstate);
                break;
            case PlayerState.Defence:
                stateMachine.ChangeState<Player_DefenceState>(reCurrstate);
                break;
            case PlayerState.SkillAttack:
                stateMachine.ChangeState<Player_SkillAttackState>(reCurrstate);
                break;
        }
    }

    private void Update()
    {
        UpdateCDTime();
    }

    public override void OnHit(IHurt target, Vector3 hitPostion)
    {
        // 拿到这段攻击数据
        Skill_AttackData attackData = currentSkillConfig.AttackData[currentHitId];
        PlayAudio(attackData.SkillHitEFConfig.AudioClip);
        // 传递伤害
        if (target.Hurt(attackData.hurtDate, this))
        {
            // 生成基于命中的配置效果
            StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig.SpawnObj, hitPostion));
            // 播放效果类, 屏幕效果
            if (attackData.ScreeImpulseValue != 0) ScreenImpulse(attackData.ScreeImpulseValue);
            if (attackData.ChromaticAberrationValue != 0) PostProcessManager.Instance.ChromaticAberrationEF(attackData.ScreeImpulseValue);
            StartFreezeFrame(attackData.FreezeFrameTime);
        }
        else
        {
            StartCoroutine(DoSkillHitEF(attackData.SkillHitEFConfig.FailSpawnObj, hitPostion));
        }
    }

    public override bool Hurt(Skill_HurtDate hurtDate, ISkillOwner hurtSource)
    {
        SetHurtDate(hurtDate, hurtSource);
        bool isDefence = currentState == PlayerState.Defence;

        if (isDefence && hurtDate.Break)
        {
            isDefence = false;
        }
        if (isDefence)
        {
           Transform enemyTransform = ((CharacterBase)hurtSource).Transform;
            Vector3 dir = (enemyTransform.position - Transform.position).normalized;

            if (Vector3.Dot(dir, Transform.forward) < 0) isDefence = false; // 敌人在我后面攻击
            else
            {
                Player_DefenceState defenceState = (Player_DefenceState)stateMachine.CurrentState;
                defenceState.Hurt();
            }
        }
        if (!isDefence)
        {
            ChangeState(PlayerState.Hurt, true);
        }
        return !isDefence;
    }

    // 检测并进入技能状态
    public bool CheckAndEnterSkillState()
    {
        if (!CanSwithSkill) return false;

        // 检测技能CD，和玩家输入
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            if (skillInfoList[i].currentTime == 0 && Input.GetKeyDown(skillInfoList[i].KeyCode))
            {
                // 释放技能
                ChangeState(PlayerState.SkillAttack, true);
                Player_SkillAttackState skillAttackState = (Player_SkillAttackState)stateMachine.CurrentState;
                skillAttackState.InitDate(skillInfoList[i].Config);
                // 技能CD
                skillInfoList[i].currentTime = skillInfoList[i].cdTime;
                return true;
            }
        }
        return false;
    }

    private void UpdateCDTime()
    {
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            skillInfoList[i].currentTime = Mathf.Clamp(skillInfoList[i].currentTime - Time.deltaTime, 0, skillInfoList[i].cdTime);
            skillInfoList[i].cdMaskImage.fillAmount = skillInfoList[i].currentTime / skillInfoList[i].cdTime;
        }
    }
}
