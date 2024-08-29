using UnityEngine;

[CreateAssetMenu(menuName = "Config/SkillHitEFConfig")]
public class SkillHitEFConfig : ScriptableObject
{
    // ��������������
    public Skill_SpawnObj SpawnObj;
    // ͨ����Ч
    public AudioClip AudioClip;
    // ����������ס��
    public Skill_SpawnObj FailSpawnObj;
}
