using UnityEngine;

public class Player_AirDownState : PlayerStateBase
{
    private enum AirDownChildState
    {
        Loop,
        End
    }

    private float playEndAnimationHeight = 2f; // 距离地面xx米的时候翻滚
    private float endAnimationHeight = 1.8f; // End动画播放需要的高度
    private bool needEndAnimation;
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
    private AirDownChildState airDownState;
    private AirDownChildState AirDownState 
    { 
        get { return airDownState; }
        set
        {
            airDownState = value;
            switch (airDownState)
            {
                case AirDownChildState.Loop:
                    player.PlayAnimation("JumpLoop");
                    break;
                case AirDownChildState.End:
                    player.PlayAnimation("JumpEnd");
                    break;
            }
        }
    }


    public override void Enter()
    {
        AirDownState = AirDownChildState.Loop;

        // 检测是否需要切换到end
        needEndAnimation = !Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.up * -1, playEndAnimationHeight + 0.5f, groundLayerMask);
    }

    public override void Update()
    {
        switch (airDownState)
        {
            case AirDownChildState.Loop:

                if (needEndAnimation)
                {
                    if (Physics.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), player.transform.up * -1, endAnimationHeight + 0.5f, groundLayerMask))
                    {
                        AirDownState = AirDownChildState.End;
                    }
                }
                else
                {
                    player.ChangeState(PlayerState.Idle);
                    return;
                }
                AirController();
                break;
            case AirDownChildState.End:
                if (CheckAnimatorStateName("JumpEnd", out float normalizedTime))
                {
                    if (normalizedTime >= 0.8f)
                    {
                        if (player.CharacterController.isGrounded == false)
                        {
                            AirDownState = AirDownChildState.Loop;
                        }
                        else
                        {
                            player.OnFootStep();
                            player.ChangeState(PlayerState.Idle);
                        }
                    }
                    else if (normalizedTime < 0.6f)
                    {
                        AirController();
                    }
                }
                break;
        }
    }

    private void AirController()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 motion = new Vector3(0, player.gravity * Time.deltaTime, 0);

        if (h != 0 || v != 0)
        {
            // 空中移动
            Vector3 input = new Vector3(h, 0, v);
            Vector3 dir = Camera.main.transform.TransformDirection(input);
            motion.x = player.airDownSpeed * Time.deltaTime * dir.x;
            motion.z = player.airDownSpeed * Time.deltaTime * dir.z;

            // 空中旋转
            float y = Camera.main.transform.rotation.eulerAngles.y; // 相机的y
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input; // 四元数 * 向量 = 向量按照四元数的角度旋转

            player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * player.rotateSpeed);
        }

        player.CharacterController.Move(motion);
    }
}
