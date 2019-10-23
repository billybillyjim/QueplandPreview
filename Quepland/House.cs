using System.Collections.Generic;
using System.Linq;

public class House
{
    public List<Furniture> furniture = new List<Furniture>();
    public Skill HouseSize = new Skill();
    public MessageManager messageManager;
    private bool notice25;
    private bool notice50;
    private bool notice75;

    public House()
    {
        HouseSize.Name = "House Size";
        HouseSize.Level = 10;
        HouseSize.Experience = Extensions.GetExperienceRequired(10);
        HouseSize.Description = "The amount of space your house has. You can increase this by building more room with planks and nails.";
    }

    public void BuildUp(GameItem plank)
    {
        HouseSize.Experience += System.Math.Max(plank.Value / 20, 1);
        if (HouseSize.Experience >= Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted()))
        {
            LevelUp(HouseSize);
        }
        else if(((double)HouseSize.Experience - Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted() - 1)) / (Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted()) - Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted() - 1)) > .25 && notice25 == false)
        {
            messageManager.AddMessage("Your house is now 25% of the way to a size increase.");
            notice25 = true;
        }
        else if (((double)HouseSize.Experience - Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted() - 1)) / (Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted()) - Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted() - 1)) > .50 && notice50 == false)
        {
            messageManager.AddMessage("Your house is now 50% of the way to a size increase.");
            notice50 = true;
        }
        else if (((double)HouseSize.Experience - Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted() - 1)) / (Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted()) - Extensions.GetExperienceRequired(HouseSize.GetSkillLevelUnboosted() - 1)) > .75 && notice75 == false)
        {
            messageManager.AddMessage("Your house is now 75% of the way to a size increase.");
            notice75 = true;
        }
    }
    private void LevelUp(Skill skill)
    {
        skill.SetSkillLevel(skill.GetSkillLevelUnboosted() + 1);
        messageManager.AddMessage("You've increased your house size! Your " + skill.Name + " is now " + skill.GetSkillLevelUnboosted() + ".");
        notice25 = false;
        notice50 = false;
        notice75 = false;
        if (skill.Experience >= Extensions.GetExperienceRequired(skill.GetSkillLevelUnboosted()))
        {
            LevelUp(skill);

        }
    }

}
