using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Skill")]
public class SkillConfig : ScriptableObject
{
    // 技能动画名称
    public string AnimationName;
    public Skill_ReleaseData ReleaseData;
    public Skill_AttackData[] AttackData;
}

[Serializable] // 技能释放数据
public class Skill_ReleaseData
{
    public Skill_SpawnObj skill_SpawnObj; // 播放粒子特效
    public AudioClip AudioClip; // 技能释放音效
    public bool CanRotate; // 释放技能的时候是否可以旋转
    public Skill_AttackData AttackData;
}

[Serializable] // 技能攻击数据
public class Skill_AttackData
{
    public Skill_SpawnObj skill_SpawnObj; // 播放粒子特效
    public AudioClip AudioClip; // 技能释放音效

    /**** 命中数据 ****/
    public Skill_HurtDate hurtDate;
    // 屏幕震动
    public float ScreeImpulseValue;
    // 色差效果
    public float ChromaticAberrationValue;
    // 命中效果
    public SkillHitEFConfig SkillHitEFConfig;
    // 卡肉效果
    public float FreezeFrameTime;
}
[Serializable]
public class Skill_HurtDate
{
    // 伤害数值
    public float DamageValue;
    // 敌人的硬直时间
    public float HardTime;
    // 击飞，击退的程度
    public Vector3 RepelVeloctiy;
    // 击飞，击退的过渡时间
    public float RepelTime;
    // 是否需要击倒
    public bool Down;
    // 破防
    public bool Break;
}


[Serializable] // 技能产生物体
public class Skill_SpawnObj
{
    // 生产预制体
    public GameObject Prefab;

    // 生成音效
    public AudioClip AudioClip;

    // 位置
    public Vector3 Position;

    // 旋转
    public Vector3 Rotation;
    // 缩放
    public Vector3 Scale = Vector3.one;

    // 延迟事件
    public float Time;
}
