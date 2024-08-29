using UnityEngine;
using System.Collections;
public class Player_DefenceState: PlayerStateBase
{
    enum DefenceChildState
    {
        Enter,
        Hold,
        WaitCounterattack,
        Counterattack,
        Exit
    }

    private DefenceChildState currentState;
    private Coroutine coroutine;

    private DefenceChildState CurrentState 
    { 
        get => currentState; 
        set
        {
            currentState = value;
            switch (currentState)
            {
                case DefenceChildState.Enter:
                    player.PlayAnimation("EnterDefence");
                    break;
                case DefenceChildState.Hold:
                    break;
                case DefenceChildState.WaitCounterattack:
                    coroutine = MonoManager.Instance.StartCoroutine(WaitCounterattackTime());
                    break;
                case DefenceChildState.Counterattack:
                    player.StartAttack(player.counterattackSkillConfig);
                    break;
                case DefenceChildState.Exit:
                    player.PlayAnimation("ExitDefence");
                    break;
            }
        }
    }

    public override void Enter()
    {
        player.Model.SetRootMotionAction(OnRootMotion);
        CurrentState = DefenceChildState.Enter;
    }

    public override void Update()
    {

        switch (CurrentState)
        {
            case DefenceChildState.Enter:
                if (CheckAnimatorStateName("EnterDefence", out float animationTime) && animationTime >= 1f)
                {
                    CurrentState = DefenceChildState.Hold;
                }
                break;
            case DefenceChildState.Hold:
                if (Input.GetKeyUp(KeyCode.F))
                {
                    CurrentState = DefenceChildState.Exit;
                }
                break;
            case DefenceChildState.WaitCounterattack:
                // 反击检测
                if (Input.GetMouseButtonDown(0))
                {
                    MonoManager.Instance.StopCoroutine(coroutine);
                    coroutine = null;
                    CurrentState = DefenceChildState.Counterattack;
                }
                // 退出检测
                else if (Input.GetKeyUp(KeyCode.F))
                {
                    CurrentState = DefenceChildState.Exit;
                    MonoManager.Instance.StopCoroutine(coroutine);
                    coroutine = null;
                    return;
                } 
                break;
            case DefenceChildState.Counterattack: 
                if (CheckAnimatorStateName(player.counterattackSkillConfig.AnimationName, out float attackTime) && attackTime >= 1f)
                {
                    Debug.Log("wawwwwww!");
                    player.ChangeState(PlayerState.Idle);
                }
                else if (player.CanSwithSkill && Input.GetMouseButton(0))
                {
                    player.ChangeState(PlayerState.StandAttack);
                }
                break;
            case DefenceChildState.Exit:
                if (CheckAnimatorStateName("ExitDefence", out float ExitanimationTime) && ExitanimationTime >= 1f)
                {
                    player.ChangeState(PlayerState.Idle);
                }
                break;
        }
    }

    public void Hurt()
    {
        if (currentState == DefenceChildState.Hold)
        {
            CurrentState = DefenceChildState.WaitCounterattack;
        }
    }

    public override void Exit()
    {
        player.Model.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaQuaternion)
    {
        deltaPosition.y = player.gravity * Time.deltaTime;
        player.CharacterController.Move(deltaPosition);
    }
    private IEnumerator WaitCounterattackTime()
    {
        yield return new WaitForSeconds(player.waitCounterattackTime);

        CurrentState = DefenceChildState.Hold;
        coroutine = null;
    }
}
