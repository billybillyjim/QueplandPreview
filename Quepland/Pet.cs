using System;
using System.Collections.Generic;
using System.Linq;

public class Pet
{
	public string Name { get; set; }
    public string Description { get; set; }
    public string Nickname { get; set; }
    public int Cost { get; set; }
    public int MinLevel { get; set; }
    public string Affinity { get; set; }
    public string Identifier { get; set; }
    public bool IsBuyable { get; set; }
    public MessageManager messageManager;
    public List<Skill> skills = new List<Skill>();

    public string GetSaveString()
    {
        string data = "";
        data += Name + (char)14;
        data += " " + (char)14;
        data += Nickname + (char)14;
        data += MinLevel.ToString() + (char)14;
        data += Affinity + (char)14;
        data += Identifier + (char)14;
        string skillString = "";
        foreach (Skill skill in skills)
        {
            skillString += skill.Name + "," + skill.Experience + "," + skill.GetSkillLevelUnboosted() + "," + skill.IsBlocked.ToString() +  "/";
        }
        skillString = skillString.Remove(skillString.Length - 1);
        return data + (char)15 + skillString;
    }
    public void SetSkills(List<Skill> skillList, List<Skill> playerSkills)
    {
        skills = new List<Skill>();
        skills.AddRange(skillList);
        foreach(Skill s in playerSkills)
        {
            if(skillList.Find(x => x.Name == s.Name) == null)
            {
                skills.Add(s);
            }
        }

    }
    public void SetMessageManager(MessageManager m)
    {
        messageManager = m;
    }
    public void GainExperience(string skill, long amount)
    {
        if (skills.Find(x => x.Name == skill) != null)
        {
            GainExperience(skills.Find(x => x.Name == skill), amount);
        }
    }
    public void GainExperience(string skill)
    {
        if (skill == null || skill == "")
        {
            return;
        }
        if (int.TryParse(skill.Split(':')[1], out int amount))
        {
            GainExperience(skills.Find(x => x.Name == skill.Split(':')[0]), amount);
        }
    }
    public void GainExperienceFromMultipleItems(string skill, int amount)
    {
        if (skill == null || skill == "")
        {
            return;
        }
        if (int.TryParse(skill.Split(':')[1], out int multi))
        {
            GainExperience(skills.Find(x => x.Name == skill.Split(':')[0]), multi * amount);
        }
    }
    private void GainExperience(Skill skill, long amount)
    {
        if (skill == null || skill.IsBlocked)
        {
            if(skill == null)
            {
                Console.WriteLine("Pet gained " + amount + " experience in unfound skill.");
            }
            
            return;
        }
        if (IsInSpecialty(skill))
        {
            amount = (long)(amount * 1.25d);
        }
        skill.Experience += amount;
        if (skill.Experience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            LevelUp(skill);
        }
    }
    public void LevelUp(Skill skill)
    {
        skill.SetSkillLevel(skill.GetSkillLevelUnboosted() + 1);
        messageManager.AddMessage(Name + " has levelled up! (" + skill.Name + " " + skill.GetSkillLevelUnboosted() + ")");
        if (skill.Experience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            LevelUp(skill);

        }
    }
    public string GetSpecialty()
    {
        float combat = GetCombatLevels() / 8f;
        float gather = GetGatherLevels() / 4f;
        float craft = GetCraftingLevels() / 5f;
        if(combat >= gather && combat >= craft)
        {
            return "Combat";
        }
        else if(gather >= combat && gather >= craft)
        {
            return "Gathering";
        }
        else
        {
            return "Crafting";
        }
    }
    
    public Skill GetHighestSkill()
    {
        if(GetTotalLevels() == skills.Count)
        {
            return null;
        }
        return skills.Aggregate((i, j) => i.GetSkillLevelUnboosted() > j.GetSkillLevelUnboosted() ? i : j);    
    }
    public int GetHighestSkillLevel()
    {
        if (GetTotalLevels() == skills.Count)
        {
            return 1;
        }
        return skills.Aggregate((i, j) => i.GetSkillLevelUnboosted() > j.GetSkillLevelUnboosted() ? i : j).GetSkillLevelUnboosted();
    }
    public float GetSkillBoost(Skill skill)
    {
        float extraBoost = 1;
        if(GetHighestSkill() != null && GetHighestSkill().Name == skill.Name)
        {
            extraBoost += 0.5f;
        }
        if(Affinity == skill.Name)
        {
            extraBoost += 0.8f;
        }
        return Math.Max((skills.Find(x => x.Name == skill.Name).GetSkillLevel() / 10f), 1) * Math.Max(1, (MinLevel / 15f)) * extraBoost;
    }
    private bool IsInSpecialty(Skill skill)
    {
        string specialty = GetSpecialty();
        if(specialty == "Combat")
        {
            if(skill.Name == "HP" ||
                skill.Name == "Knifesmanship" ||
                skill.Name == "Swordsmanship" ||
                skill.Name == "Axemanship" ||
                skill.Name == "Hammermanship" ||
                skill.Name == "Deftness" ||
                skill.Name == "Strength" ||
                skill.Name == "Archery")
            {
                return true;
            }
        }
        else if(specialty == "Gathering")
        {
            if (skill.Name == "Mining" ||
               skill.Name == "Fishing" ||
               skill.Name == "Woodcutting" ||
               skill.Name == "Hunting")
            {
                return true;
            }
        }
        else if(specialty == "Crafting")
        {
            if (skill.Name == "Smithing" ||
               skill.Name == "Alchemy" ||
               skill.Name == "Woodworking" ||
               skill.Name == "Culinary Arts" ||
               skill.Name == "Leatherworking" ||
               skill.Name == "Construction")
            {
                return true;
            }
        }
        return false;
    }
    private int GetCombatLevels()
    {
        int total = 0;
        total += skills.Find(x => x.Name == "HP").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Knifesmanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Swordsmanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Axemanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Hammermanship").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Deftness").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Strength").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Archery").GetSkillLevelUnboosted();
        return total;
    }
    private int GetGatherLevels()
    {
        int total = 0;
        total += skills.Find(x => x.Name == "Mining").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Fishing").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Woodcutting").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Hunting").GetSkillLevelUnboosted();
        return total;
    }
    private int GetCraftingLevels()
    {
        int total = 0;
        total += skills.Find(x => x.Name == "Smithing").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Alchemy").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Woodworking").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Culinary Arts").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Leatherworking").GetSkillLevelUnboosted();
        total += skills.Find(x => x.Name == "Construction").GetSkillLevelUnboosted();
        return total;
    }
    public int GetTotalLevels()
    {
        return skills.Sum(x => x.GetSkillLevelUnboosted());
    }
    public long GetTotalExp()
    {
        return skills.Sum(x => x.Experience);
    }

}
