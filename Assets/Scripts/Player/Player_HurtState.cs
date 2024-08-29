using System.Collections;
using UnityEngine;

public class Player_HurtState: PlayerStateBase
{
    private Skill_HurtDate hurtDate => player.hurtDate;
    private ISkillOwner source => player.hurtSource;
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
                    player.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    Vector3 pos = source.Transform.position;
                    player.transform.LookAt(new Vector3(pos.x, player.transform.position.y, pos.z));
                    player.PlayAnimation("Down");
                    break;
                case HurtChildState.Rise:
                    player.PlayAnimation("Rise");
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
            player.CharacterController.Move(new Vector3(0, player.gravity * Time.deltaTime, 0));
        }
        currHardTime += Time.deltaTime;
        switch (HurtState)
        {
            case HurtChildState.NormalHurt:
                // 硬直检测
                if (currHardTime >= hurtDate.HardTime && coroutine == null)
                {
                    player.ChangeState(PlayerState.Idle);
                }
                break;
            case HurtChildState.Down:
                Vector3 pos = source.Transform.position;
                player.transform.LookAt(new Vector3(pos.x, player.transform.position.y, pos.z));
                // 硬直检测
                if (currHardTime >= hurtDate.HardTime && coroutine == null)
                {
                    HurtState = HurtChildState.Rise;
                }
                break;
            case HurtChildState.Rise:
                if (CheckAnimatorStateName("Rise", out float time) && time >= 0.99f)
                {
                    player.ChangeState(PlayerState.Idle);
                }
                break;
        }
    }

    private IEnumerator DoRepel(float time, Vector3 velocity)
    {
        float currTime = 0;
        time = time == 0 ? 0.0001f : time;
        // 将位移方向修改为相对对手的方向
        // Vector3 veloFromSource = source.Transform.TransformDirection(velocity);
        Vector3 point = source.Transform.TransformPoint(velocity);
        Vector3 dir = point - player.Transform.position;
        while (currHardTime < time)
        {
            player.CharacterController.Move(dir / time * Time.deltaTime);
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