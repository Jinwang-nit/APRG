using UnityEngine;
public class Player_JumpState : PlayerStateBase
{
    public override void Enter()
    {
        player.PlayAnimation("JumpStart");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        // 第一次执行的时候，动画可能不是jump

        if (CheckAnimatorStateName("JumpStart", out float normalizedTime) && normalizedTime >= 0.9f)
        {
           player.ChangeState(PlayerState.AirDown);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            player.StartAttack(player.jumpAttackConfig);
        }
    }


    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y *= player.jumpPower;

        // 跳的时候向前偏移一点
        Vector3 offset = jumpPower * Time.deltaTime * player.jumpingSpeed * player.Model.transform.forward;
        player.CharacterController.Move(deltaPosition + offset);
    }
}
