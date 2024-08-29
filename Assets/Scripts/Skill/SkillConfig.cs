using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill")]
public class SkillConfig : ScriptableObject
{
    // ���ܶ�������
    public string AnimationName;
    public Skill_ReleaseData ReleaseData;
    public Skill_AttackData[] AttackData;
}

[Serializable] // �����ͷ�����
public class Skill_ReleaseData
{
    public Skill_SpawnObj skill_SpawnObj; // ����������Ч
    public AudioClip AudioClip; // �����ͷ���Ч
    public bool CanRotate; // �ͷż��ܵ�ʱ���Ƿ������ת
    public Skill_AttackData AttackData;
}

[Serializable] // ���ܹ�������
public class Skill_AttackData
{
    public Skill_SpawnObj skill_SpawnObj; // ����������Ч
    public AudioClip AudioClip; // �����ͷ���Ч

    /**** �������� ****/
    public Skill_HurtDate hurtDate;
    // ��Ļ��
    public float ScreeImpulseValue;
    // ɫ��Ч��
    public float ChromaticAberrationValue;
    // ����Ч��
    public SkillHitEFConfig SkillHitEFConfig;
    // ����Ч��
    public float FreezeFrameTime;
}
[Serializable]
public class Skill_HurtDate
{
    // �˺���ֵ
    public float DamageValue;
    // ���˵�Ӳֱʱ��
    public float HardTime;
    // ���ɣ����˵ĳ̶�
    public Vector3 RepelVeloctiy;
    // ���ɣ����˵Ĺ���ʱ��
    public float RepelTime;
    // �Ƿ���Ҫ����
    public bool Down;
    // �Ʒ�
    public bool Break;
}


[Serializable] // ���ܲ�������
public class Skill_SpawnObj
{
    // ����Ԥ����
    public GameObject Prefab;

    // ������Ч
    public AudioClip AudioClip;

    // λ��
    public Vector3 Position;

    // ��ת
    public Vector3 Rotation;
    // ����
    public Vector3 Scale = Vector3.one;

    // �ӳ��¼�
    public float Time;
}
