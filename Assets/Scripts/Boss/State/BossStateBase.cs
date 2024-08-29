
using UnityEngine;

public class BossStateBase: StateBase
{
    protected Boss_Controller boss;
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        boss = (Boss_Controller)owner;
    }
    protected virtual bool CheckAnimatorStateName(string stateName, out float normalizedTmie)
    {
        AnimatorStateInfo nextInfo = boss.Model.Animator.GetNextAnimatorStateInfo(0);

        // 优先判断下一个状态是否合法
        if (nextInfo.IsName(stateName))
        {
            normalizedTmie = nextInfo.normalizedTime;
            return true;
        }

        AnimatorStateInfo currentInfo = boss.Model.Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTmie = currentInfo.normalizedTime;
        return currentInfo.IsName(stateName);
    }
}