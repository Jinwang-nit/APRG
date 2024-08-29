using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss_Controller : CharacterBase
{
    public NavMeshAgent navMeshAgent;
    public float WalkRange = 8;
    public Player_Controller targetPlayer;
    public float walkSpeed;
    public float runSpeed;
    public float standAttackRange;

    public float vigilantTime = 10; // 警戒的时间
    public float vigilantRange = 6; // 警戒的范围
    public float vigilantSpeed = 2; // 警戒的速度
    public float attackTime = 5; // 攻击时间
    public bool anger; // 愤怒状态
    private float currentSkillCDTimer;
    private void Start()
    {
        currentSkillCDTimer = 0;
        Init();
        ChangeState(BossState.Idle);
    }

    public void ChangeState(BossState bossState, bool reCurrstate = false)
    {
        switch (bossState)
        {
            case BossState.Idle:
                stateMachine.ChangeState<Boss_IdleState>(reCurrstate);
                break;
            case BossState.Hurt:
                stateMachine.ChangeState<Boss_HurtState>(reCurrstate);
                anger = true;
                break;
            case BossState.Attack:
                stateMachine.ChangeState<Boss_StandAttackState>(reCurrstate);
                anger = false;
                break;
            case BossState.Walk:
                stateMachine.ChangeState<Boss_WalkState>(reCurrstate);
                break;
            case BossState.Run:
                stateMachine.ChangeState<Boss_RunState>(reCurrstate);
                break;
        }
    }

    public override bool Hurt(Skill_HurtDate hurtDate, ISkillOwner hurtSource)
    {
        SetHurtDate(hurtDate, hurtSource);
        ChangeState(BossState.Hurt, true);
        return true;
    }

    private void Update()
    {
        UpdateCDTime();
    }

    public void StartSkill(int index)
    {
        currentSkillCDTimer = 2;
        skillInfoList[index].currentTime = skillInfoList[index].cdTime;
        StartAttack(skillInfoList[index].Config);
    }

    private void UpdateCDTime()
    {
        for (int i = 0; i < skillInfoList.Count; i++)
        {
            skillInfoList[i].currentTime = Mathf.Clamp(skillInfoList[i].currentTime - Time.deltaTime, 0, skillInfoList[i].cdTime);
        }
        if (currentSkillCDTimer > 0)
        {
            currentSkillCDTimer -= Time.deltaTime;
        }
    }

    /*
     * 将敌人所有的collider的层级设置为HurtCollider
     */
    [ContextMenu("SetHurtCollider")]
    private void SetHurtCollider()
    {
        // 设置为hurtcollider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<WeaponController>() == null) // 武器除外，因为武器是不会受伤的
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("HurtCollider");
            }
        }
        // 保存场景修改
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
}
