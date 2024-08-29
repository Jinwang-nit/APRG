using UnityEngine;

[CreateAssetMenu(menuName = "Config/SkillHitEFConfig")]
public class SkillHitEFConfig : ScriptableObject
{
    // 产生的粒子物体
    public Skill_SpawnObj SpawnObj;
    // 通用音效
    public AudioClip AudioClip;
    // 攻击被个挡住了
    public Skill_SpawnObj FailSpawnObj;
}
