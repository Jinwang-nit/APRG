using System.Collections;
using UnityEngine;

public class Boss_HurtState: BossStateBase
{
    private Skill_HurtDate hurtDate => boss.hurtDate;
    private ISkillOwner source => boss.hurtSource;
    private Coroutine coroutine;

    enum HurtChildState
    {
        // 普通受伤
        NormalHurt,
        // down
        Down,
        // rise
        Rise
    }
    private HurtChildState hurtState;
    private HurtChildState HurtState 
    { 
        get => hurtState; 
        set
        {
            hurtState = value;
            switch (hurtState)
            {
                case HurtChildState.NormalHurt:
                    boss.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    Vector3 pos = source.Transform.position;
                    boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
                    boss.PlayAnimation("Down");
                    break;
                case HurtChildState.Rise:
                    boss.PlayAnimation("Rise");
                    break;
            }
        }
    }

    public override void Enter()
    {
        currHardTime = 0;

        if (hurtDate.Down) HurtState = HurtChildState.Down;
        else HurtState = HurtChildState.NormalHurt;

        if (hurtDate.RepelVeloctiy != Vector3.zero)
        {
            coroutine = MonoManager.Instance.StartCoroutine(DoRepel(hurtDate.RepelTime, hurtDate.RepelVeloctiy));
        }
    }

    private float currHardTime = 0;

    public override void Update()
    {
        if (coroutine == null)
        {
            boss.CharacterController.Move(new Vector3(0, boss.gravity * Time.deltaTime, 0));
        }
        currHardTime += Time.deltaTime;
        switch (HurtState)
        {
            case HurtChildState.NormalHurt:
                // 硬直检测
                if (currHardTime >= hurtDate.HardTime && coroutine == null)
                {
                    boss.ChangeState(BossState.Idle);
                }
                break;
            case HurtChildState.Down:
                Vector3 pos = source.Transform.position;
                boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
                // 硬直检测
                if (currHardTime >= hurtDate.HardTime && coroutine == null)
                {
                    HurtState = HurtChildState.Rise;
                }
                break;
            case HurtChildState.Rise:
                if (CheckAnimatorStateName("Rise", out float time) && time >= 0.99f)
                {
                    boss.ChangeState(BossState.Idle);
                }
                break;
        }
    }

    private IEnumerator DoRepel(float time, Vector3 velocity)
    {
        float currTime = 0;
        time = time == 0 ? 0.0001f : time;
        // 将位移方向修改为相对对手的方向
        Vector3 point = source.Transform.TransformPoint(velocity);
        Vector3 dir = point - boss.Transform.position;
        while (currHardTime < time)
        {
            boss.CharacterController.Move(dir / time * Time.deltaTime);
            currTime += Time.deltaTime;
            yield return null;
        }

        coroutine = null;
    }

    public override void Exit()
    {
        if (coroutine != null)
        {
            MonoManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}