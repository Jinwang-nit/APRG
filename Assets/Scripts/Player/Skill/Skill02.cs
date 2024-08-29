using System;
using System.Collections.Generic;
using UnityEngine;

public class Skill02 : SkillObjectBase
{
    public AudioSource AudioSource;
    public override void Init(List<string> enemyTagList, Action<IHurt, Vector3> onHitAction)
    {
        base.Init(enemyTagList, onHitAction);
        Destroy(gameObject, 4f);
        Invoke(nameof(StartSkillHit), 0.5f);
        Invoke(nameof(PlayAudio), 0.6f);
    }

    private void PlayAudio()
    {
        AudioSource.enabled = true;
    }
}
