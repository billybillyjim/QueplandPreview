using System;
using System.Text.Json.Serialization;

public class Skill
{

    public string Name { get; set; }

    public int Level { get; set; }
    public int startExperience { get; set; }
    private long _experience;
    public bool IsBlocked { get; set; }

    public long Experience {
        get { return _experience; }
        set {
            if (value >= 0)
            {
                _experience = Math.Min(value, long.MaxValue - 20000000);
            }
            else
            {
                _experience = long.MaxValue - 20000000;
            }
        }
    }
    public string Description { get; set; }
    public int Boost { get; set; }
    public int BedBoost { get; set; }

    /// <summary>
    /// Returns a skill's level including boost and bed boost. Use GetSkillLevelUnboosted() for the real level.
    /// </summary>
    /// <returns></returns>
    public int GetSkillLevel()
    {
        return Level + Boost + BedBoost;
    }
    public int GetSkillLevelUnboosted()
    {
        return Level;
    }
    public void SetSkillLevel(int level)
    {
        Level = level;
    }
}
