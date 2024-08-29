using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RollState : PlayerStateBase
{
    private Coroutine coroutine;
    private bool isRote = false;
    public override void Enter()
    {
        // 检测玩家输入方向
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            Vector3 inputDir = new Vector3(h, 0, v).normalized;
            coroutine = MonoManager.Instance.StartCoroutine(DoRotate(inputDir));
        }
        else
        {
            player.PlayAnimation("Roll");
            player.Model.SetRootMotionAction(OnRootMotion);
        }
    }

    private IEnumerator DoRotate(Vector3 dir)
    {
        float y = Camera.main.transform.rotation.eulerAngles.y; // 相机的y
        Vector3 targetDir = Quaternion.Euler(0, y, 0) * dir; // 四元数 * 向量 = 向量按照四元数的角度旋转
        float rate = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        isRote = true;
        while (rate < 1)
        {
            rate += Time.deltaTime * 10;
            player.Model.transform.rotation = Quaternion.Slerp(player.Model.transform.rotation, targetRotation, rate);
            yield return null;
        }

        isRote = false;
        player.PlayAnimation("Roll");
        player.Model.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        if (isRote) return;
        if (CheckAnimatorStateName("Roll", out float normalizedTime))
        {
            if (normalizedTime > 0.8f)
            {
                player.ChangeState(PlayerState.Idle);
            }
        }
    }

    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
        if (coroutine != null) MonoManager.Instance.StopCoroutine(coroutine);   
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
}
