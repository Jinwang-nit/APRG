using UnityEngine;

public interface ISkillOwner
{
    Transform Transform { get; }

    void StartSkillHit(int weaponIndex);

    void StopSkillHit(int weaponIndex);

    void SkillCanSwitch();

    void OnHit(IHurt target, Vector3 hitPostion);

    void OnFootStep();
}