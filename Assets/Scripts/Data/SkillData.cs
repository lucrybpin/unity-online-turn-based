using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public SkillID Id;
    public ElementType Element;
    [TextArea] public string Description;
    [TextArea] public string FlavorText;
    public List<SkillLevelInfo> SkillLevelInfo;
}
