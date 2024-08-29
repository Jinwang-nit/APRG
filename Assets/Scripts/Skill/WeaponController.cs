using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]private new Collider collider;
    [SerializeField] private  MeleeWeaponTrail weaponTrail;
    private List<string> enemyTagList;
    private List<IHurt> enemyList = new List<IHurt>(); // 一次攻击的对象存起来
    private Action<IHurt, Vector3> onHitAction;
    public void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        this.enemyTagList = enemyTagList;
        this.onHitAction = onHitAction;
        weaponTrail.Emit = false;
        collider.enabled = false;
    }

    public void StartSkillHit()
    {
        collider.enabled = true;
        weaponTrail.Emit = true;
    }

    public void StopSkillHit()
    {
        collider.enabled = false;
        weaponTrail.Emit = false;
        enemyList.Clear();
    }

    private void OnTriggerStay(Collider other)
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
