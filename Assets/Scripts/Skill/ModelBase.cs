using System.Collections.Generic;
using System;
using UnityEngine;

public class ModelBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected WeaponController[] weapons;
    protected ISkillOwner skillOwner;

    public Animator Animator { get => animator; }


    public void Init(ISkillOwner skillOwner, List<string> enemyTagList)
    {
        this.skillOwner = skillOwner;  // player

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init(enemyTagList, skillOwner.OnHit);
        }
    }


    // 根运动
    protected Action<Vector3, Quaternion> rootMotionAction;
    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }

    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }

    protected void OnAnimatorMove() // 每帧都会调用
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }

    // 动画事件
    protected void FootStep()
    {
        skillOwner.OnFootStep();
    }

    protected void StartSkillHit(int weaponIndex)
    {
        skillOwner.StartSkillHit(weaponIndex); // playerController
        weapons[weaponIndex].StartSkillHit();
    }

    protected void StopSkillHit(int weaponIndex)
    {
        skillOwner.StopSkillHit(weaponIndex);
        weapons[weaponIndex].StopSkillHit();
    }

    protected void SkillCanSwitch()
    {
        skillOwner.SkillCanSwitch();
    }
}