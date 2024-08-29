using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SkillInfo
{
    public KeyCode KeyCode;
    public SkillConfig Config;
    public float cdTime;
    [NonSerialized]public float currentTime;
    public Image cdMaskImage;
}
