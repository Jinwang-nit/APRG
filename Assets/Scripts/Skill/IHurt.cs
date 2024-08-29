using UnityEngine;

public interface IHurt
{
    bool Hurt(Skill_HurtDate hurtDate, ISkillOwner hurtSource);
}
