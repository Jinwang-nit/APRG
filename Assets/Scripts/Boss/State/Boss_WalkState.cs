﻿using UnityEngine;
using System.Collections;
public class Boss_WalkState: BossStateBase
{
    private bool isVigilant; // 警惕
    public override void Enter()
    {
        boss.PlayAnimation("Walk");
        boss.navMeshAgent.enabled = true;

        if (boss.anger) isVigilant = false;
        else isVigilant = Random.Range(0, 3) >= 1; // 2 / 3的概率进行对峙
        
        if (isVigilant)
        {
            boss.navMeshAgent.updateRotation = true;    
            boss.navMeshAgent.speed = boss.vigilantSpeed;
            stopVigilantCoroutine = MonoManager.Instance.StartCoroutine(StopVigilant());
        }
        else
        {
            boss.navMeshAgent.speed = boss.walkSpeed;
        }
    }

    Coroutine stopVigilantCoroutine;
    IEnumerator StopVigilant()
    {
        yield return new WaitForSeconds(Random.Range(0, boss.vigilantTime));
        boss.navMeshAgent.updateRotation = false;
        boss.navMeshAgent.speed = boss.walkSpeed;
        stopVigilantCoroutine = null;
        boss.PlayAnimation("Walk", false);
    }
    

    public override void Update()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.targetPlayer.transform.position);
        if (distance >= boss.WalkRange)
        {
            boss.ChangeState(BossState.Run);
            return;
        }
    
        if (isVigilant)
        {
            Vector3 playerPos = boss.targetPlayer.transform.position;
            boss.transform.LookAt(new Vector3(playerPos.x, boss.transform.position.y, playerPos.z));
            Vector3 targetPos = (boss.transform.position - playerPos).normalized * boss.vigilantRange + playerPos;
            if (Vector3.Distance(boss.transform.position, playerPos) < 0.5f)
            {
                boss.PlayAnimation("Idle", false);
            }
            else
            {
                boss.PlayAnimation("Walk", false);
                boss.navMeshAgent.SetDestination(targetPos);
            }
        }
        else
        {
            if (distance < boss.standAttackRange)
            {
                boss.ChangeState(BossState.Attack);

            }
            else
            {
                boss.navMeshAgent.SetDestination(boss.targetPlayer.transform.position);
            }
            
        }
    }

    public override void Exit()
    {
        boss.navMeshAgent.enabled = false;
        if (stopVigilantCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(stopVigilantCoroutine);
            stopVigilantCoroutine = null;
        }
    }
}