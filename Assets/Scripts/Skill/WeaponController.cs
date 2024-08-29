using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]private new Collider collider;
    [SerializeField] private  MeleeWeaponTrail weaponTrail;
    private List<string> enemyTagList;
    private List<IHurt> enemyList = new List<IHurt>(); // һ�ι����Ķ��������
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
