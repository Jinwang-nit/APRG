using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player_MoveState: PlayerStateBase
{
    private enum MoveChildState
    {
        Move,
        Stop
    }


    private float runTransition; // 走路和奔跑的过渡阈值
    private MoveChildState moveState;

    private MoveChildState MoveState
    {
        get { return moveState; }
        set
        {
            moveState = value;
            switch (moveState)  
            {
                case MoveChildState.Move:
                    player.PlayAnimation("Move");
                    break;
                case MoveChildState.Stop:
                    player.PlayAnimation("RunStop");
                    break;
            }
        }
    }
    public override void Enter()
    {
        MoveState = MoveChildState.Move;
        player.Model.SetRootMotionAction(OnRootMotion); // 订阅事件
    }

    public override void Update()
    {
        // 检测攻击
        if (Input.GetMouseButtonDown(0))
        {
            player.ChangeState(PlayerState.StandAttack);
            return;
        }

        // 检测技能
        if (player.CheckAndEnterSkillState())
        {
            return;
        }

        // 检测翻滚
        if (Input.GetKeyDown(KeyCode.C))
        {
            player.ChangeState(PlayerState.Roll);
            return;
        }

        // 检测跳跃
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpPower = runTransition + 1;
            player.ChangeState(PlayerState.Jump);
            return;
        }

        // 检测格挡
        if (Input.GetKeyDown(KeyCode.F))
        {
            player.ChangeState(PlayerState.Defence);
            return;
        }

        if (player.CharacterController.isGrounded == false) // 检测下落
        {
            player.ChangeState(PlayerState.AirDown);
            return;
        }

        switch (moveState)
        {
            case MoveChildState.Move:
                MoveOnUpdate();
                break;
            case MoveChildState.Stop:
                StopOnUpdate();
                break;
        }
    }

    private void StopOnUpdate()
    {
        AnimatorStateInfo stateInfo = player.Model.Animator.GetCurrentAnimatorStateInfo(0);
        // 检测急停动画的播放进度，播完了就切回待机
        if (stateInfo.IsName("RunStop"))
        {
            if (stateInfo.normalizedTime >= 1) player.ChangeState(PlayerState.Idle);
        }
    }

    private void MoveOnUpdate()
    {
        // 判断是否移动和待机
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // 判断急停
        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");
        if (rawH == 0 && rawV == 0) // 没有输入就待机
        {
            if (runTransition > 0.4f)
            {
                MoveState = MoveChildState.Stop;
            }
            else if (h == 0 && v == 0)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
        else
        {
            // walk 到 run 的过渡
            if (Input.GetKey(KeyCode.LeftShift))
            {
                runTransition = Mathf.Clamp(runTransition + Time.deltaTime * player.runTransition, 0, 1);
            }
            else //run 到 walk
            {
                runTransition = Mathf.Clamp(runTransition - Time.deltaTime * player.runTransition, 0, 1);
            }
            player.Model.Animator.SetFloat("Move", runTransition);
            // 通过影响动画的播放速度，来实现位移速度的改变
            player.Model.Animator.speed = Mathf.Lerp(player.walkSpeed, player.runSpeed, runTransition);

            if (h != 0 || v != 0)
            {
                // 旋转，相对相机方向
                Vector3 input = new Vector3(h, 0, v);
                float y = Camera.main.transform.rotation.eulerAngles.y; // 相机的y
                Vector3 targetDir = Quaternion.Euler(0, y, 0) * input; // 四元数 * 向量 = 向量按照四元数的角度旋转

                player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
            }
        }
    }

    public override void Exit()
    {
        runTransition = 0;
        player.Model.ClearRootMotionAction();
        player.Model.Animator.speed = 1;
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}
