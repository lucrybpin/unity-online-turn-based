using System.Collections.Generic;

[System.Serializable]
public struct SkillLevelInfo 
{
    public int Level;
    public int ApCost;

    // Effect
    public List<SkillEffect> Effects;

    // Critical
    public int CriticalBase;
    
    // Range
    public int RangeMin;
    public int RangeMax;
    public RangeType RangeType;
    public bool IsLockedRange;

    // Requirements
    public int RequiredLevel;
    public int TurnCap;
}
