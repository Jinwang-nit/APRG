using System;
using UnityEngine;

public class Boss_StandAttackState : BossStateBase
{
    // 当前是第几段普攻
    private int currentAttackIndex;
    private int CurrentAttackIndex
    {
        get => currentAttackIndex;
        set
        {
            if (value >= boss.standAttackConfigs.Length) currentAttackIndex = 0;
            else currentAttackIndex = value;
        }
    }

    private float currentAttackTime = 0;
    private SkillConfig currentSkillConfig;
    public override void Enter()
    {
        // 注册根运动
        boss.Model.SetRootMotionAction(OnRootMotion); // 订阅事件
        CurrentAttackIndex = -1;
        // 播放技能
        StandAttack();

        currentAttackTime = 0;
    }
    public override void Exit()
    {
        boss.Model.ClearRootMotionAction();
        boss.OnSkillOver();
        currentSkillConfig = null;
    }

    private void StandAttack()
    {
        Vector3 pos = boss.targetPlayer.transform.position;
        boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
        CurrentAttackIndex += 1;
        boss.StartAttack(boss.standAttackConfigs[CurrentAttackIndex]);
        currentSkillConfig = boss.standAttackConfigs[CurrentAttackIndex];
    }

    public override void Update()
    {
        currentAttackTime += Time.deltaTime;
        if (boss.CanSwithSkill)
        {
            float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
            if (distance <= boss.standAttackRange && currentAttackTime < boss.attackTime)
            {
                // 先检测有没有破防技能
                if (boss.targetPlayer.isDefence)
                {
                    for (int i = 0; i < boss.skillInfoList.Count; i++)
                    {
                        if (boss.skillInfoList[i].currentTime == 0 && boss.skillInfoList[i].Config.AttackData.Length > 0)
                        {
                            if (boss.skillInfoList[i].Config.AttackData[0].hurtDate.Break)
                            {
                                StartSkill(i);
                                return;
                            }
                        }
                    }
                }
                // 监测其他技能
                for (int i = 0; i < boss.skillInfoList.Count; i++)
                {
                    if (boss.skillInfoList[i].currentTime == 0)
                    {
                        StartSkill(i);
                        return;
                    }
                }
                // 普通攻击
                StandAttack();
            }
            else
            {
                boss.ChangeState(BossState.Walk);
            }
        }
        else if (currentSkillConfig != null && CheckAnimatorStateName(currentSkillConfig.AnimationName, out float animationTime) && animationTime >= 1f)
        {
            // 动画播放结束，切回待机
            boss.ChangeState(BossState.Walk);
        }



    }

    private void StartSkill(int index)
    {
        Vector3 pos = boss.targetPlayer.transform.position;
        boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
        boss.StartSkill(index);
        currentSkillConfig = boss.skillInfoList[index].Config;
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y = boss.gravity * Time.deltaTime;
        boss.CharacterController.Move(deltaPosition);
    }

}
