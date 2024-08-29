using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjectBase : MonoBehaviour
{
    [SerializeField] private new Collider collider;
    private List<string> enemyTagList;
    private List<IHurt> enemyList = new List<IHurt>(); // 一次攻击的对象存起来
    private Action<IHurt, Vector3> onHitAction;
    public virtual void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        this.enemyTagList = enemyTagList;
        this.onHitAction = onHitAction;
        collider.enabled = false;
    }

    public virtual void StartSkillHit()
    {
        collider.enabled = true;
    }

    public virtual void StopSkillHit()
    {
        collider.enabled = false;
        enemyList.Clear();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (enemyTagList == null) return;
        // 检测打击对象
        if (enemyTagList.Contains(other.tag))
        {
            IHurt enemey = other.GetComponentInParent<IHurt>();
            // 之前没攻击
            if (enemey != null && !enemyList.Contains(enemey))
            {
                // 通知上级处理攻击
                onHitAction?.Invoke(enemey, other.ClosestPoint(transform.position));
                enemyList.Add(enemey);
            }
        }
    }
}
