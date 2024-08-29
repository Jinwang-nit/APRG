using System;
using UnityEngine;

public class Player_StandAttackState : PlayerStateBase
{
    // 当前是第几段普攻
    private int currentAttackIndex;
    private int CurrentAttackIndex
    {
        get => currentAttackIndex;
        set
        {
            if (value >= player.standAttackConfigs.Length) currentAttackIndex = 0;
            else currentAttackIndex = value;
        }
    }

    public override void Enter()
    {
        // 注册根运动
        player.Model.SetRootMotionAction(OnRootMotion); // 订阅事件
        CurrentAttackIndex = -1;
        // 播放技能
        StandAttack();
    }
    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
        player.OnSkillOver();
    }

    private void StandAttack()
    {
        CurrentAttackIndex += 1;
        player.StartAttack(player.standAttackConfigs[CurrentAttackIndex]);
    }

    public override void Update()
    {
        if (CheckAnimatorStateName(player.standAttackConfigs[CurrentAttackIndex].AnimationName, out float animationTime) && animationTime >= 1f)
        {
            // 动画播放结束，切回待机
            player.ChangeState(PlayerState.Idle);
        }

        // 是否连段攻击
        if (CheckStandAttack())
        {
            StandAttack();
            return;
        }

        // 检测技能
        if (player.CheckAndEnterSkillState())
        {
            return;
        }

        // 旋转逻辑
        if (player.CurrentSkillConfig.ReleaseData.CanRotate)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h != 0 || v != 0)
            {
                // 旋转，相对相机方向
                Vector3 input = new Vector3(h, 0, v);
                float y = Camera.main.transform.rotation.eulerAngles.y; // 相机的y
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input; // 四元数 * 向量 = 向量按照四元数的角度旋转

                player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotationSpeedForAttack);
            }
        }

        if (player.CanSwithSkill)
        {
            // 检测跳跃
            if (Input.GetKeyDown(KeyCode.Space))
            {
                jumpPower = 0;
                player.ChangeState(PlayerState.Jump);
                return;
            }

            // 检测翻滚
            if (Input.GetKeyDown(KeyCode.C))
            {
                player.ChangeState(PlayerState.Roll);
                return;
            }
        }
    }
    private bool CheckStandAttack()
    {
        return Input.GetMouseButtonDown(0) && player.CanSwithSkill;
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }

}
