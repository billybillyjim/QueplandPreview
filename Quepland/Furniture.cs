using System;
using System.Collections.Generic;

public class Furniture
{
	public string Name { get; set; }
    public int ID { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public int ConstructionLevelRequired { get; set; }
    public int MinimumPlankLevel { get; set; }
    public int PlanksRequired { get; set; }
    public int BarsRequired { get; set; }
    public int LevelRequired { get; set; }
    public int MinimumBarLevel { get; set; }
    public int[][] OtherItemCosts { get; set; }
    public int[,] OtherItemCurrentAmounts = new int[10,2];
    public int ExperienceGained { get; set; }
    public int[] UpgradeIDs { get; set; }
    public int WithdrawItemID { get; set; }
    public TimeSpan WithdrawEvery { get; set; }
    public string WithdrawEveryString { get; set; }
    public DateTime LastWithdrawn { get; set; }
    public int WithdrawItemAmount { get; set; }
    public int Progress { get; set; }
    public bool IsFinished { get; set; }
    public int BoostDuration { get; set; }
    public int BoostAmount { get; set; }

    public int GetWorkRequired()
    {
        return 1000 + (PlanksRequired + (BarsRequired * 7) * 4);
    }
    public int GetCurrentAmountOfItem(int itemID)
    {
        for(int i = 0; i < OtherItemCurrentAmounts.GetLength(0); i++)
        {
            if(OtherItemCurrentAmounts[i,0] == itemID)
            {
                return OtherItemCurrentAmounts[i,1];
            }
        }
        return 0;
    }
    public int GetCostOfItem(int itemID)
    {
        for (int i = 0; i < OtherItemCosts.GetLength(0); i++)
        {
            if (OtherItemCosts[i][0] == itemID)
            {
                return OtherItemCosts[i][1];
            }
        }
        return 0;
    }
    public TimeSpan GetWithdrawTime()
    {
        return TimeSpan.Parse(WithdrawEveryString);
    }
}
