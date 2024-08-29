using UnityEngine;

public class PlayerStateBase: StateBase
{
    protected Player_Controller player;
    protected static float jumpPower;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        player = (Player_Controller)owner;
    }

    protected virtual bool CheckAnimatorStateName(string stateName, out float normalizedTmie)
    {
        AnimatorStateInfo nextInfo = player.Model.Animator.GetNextAnimatorStateInfo(0);

        // 优先判断下一个状态是否合法
        if (nextInfo.IsName(stateName))
        {
            normalizedTmie = nextInfo.normalizedTime;
            return true;
        }

        AnimatorStateInfo currentInfo = player.Model.Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTmie = currentInfo.normalizedTime;
        return currentInfo.IsName(stateName);
    }
}