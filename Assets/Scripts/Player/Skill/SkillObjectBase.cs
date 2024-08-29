using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObjectBase : MonoBehaviour
{
    [SerializeField] private new Collider collider;
    private List<string> enemyTagList;
    private List<IHurt> enemyList = new List<IHurt>(); // һ�ι����Ķ��������
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
        // ���������
        if (enemyTagList.Contains(other.tag))
        {
            IHurt enemey = other.GetComponentInParent<IHurt>();
            // ֮ǰû����
            if (enemey != null && !enemyList.Contains(enemey))
            {
                // ֪ͨ�ϼ�������
                onHitAction?.Invoke(enemey, other.ClosestPoint(transform.position));
                enemyList.Add(enemey);
            }
        }
    }
}
