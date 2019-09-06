using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FurnitureSlot
{
    public string Name { get; set; }
    public int ID { get; set; }
    public string Description { get; set; }
    public int[] AvailableFurnitureIDs { get; set; }
    public Furniture FurnitureToBuild { get; set; }
    public Inventory Inventory = new Inventory(int.MaxValue);
    public bool isFinished { get; set; }
    public bool hasSelected { get; set; }
    public int SpaceRequired { get; set; }

    public FurnitureSlot()
    {

    }
    public FurnitureSlot(FurnitureSlot slot)
    {
        Name = slot.Name;
        ID = slot.ID;
        Description = slot.Description;
        AvailableFurnitureIDs = slot.AvailableFurnitureIDs;
        SpaceRequired = slot.SpaceRequired;
    }
    public void SetFurniture(Furniture furniture)
    {
        FurnitureToBuild = new Furniture();
        FurnitureToBuild.BarsRequired = furniture.BarsRequired;
        FurnitureToBuild.ConstructionLevelRequired = furniture.ConstructionLevelRequired;
        FurnitureToBuild.Description = furniture.Description;
        FurnitureToBuild.ExperienceGained = furniture.ExperienceGained;
        FurnitureToBuild.ID = furniture.ID;
        FurnitureToBuild.MinimumBarLevel = furniture.MinimumBarLevel;
        FurnitureToBuild.MinimumPlankLevel = furniture.MinimumPlankLevel;
        FurnitureToBuild.Name = furniture.Name;
        FurnitureToBuild.OtherItemCosts = furniture.OtherItemCosts;
        FurnitureToBuild.OtherItemCurrentAmounts = Extensions.To2DArray(furniture.OtherItemCosts);
        FurnitureToBuild.PlanksRequired = furniture.PlanksRequired;
        FurnitureToBuild.UpgradeIDs = furniture.UpgradeIDs;
        FurnitureToBuild.WithdrawItemAmount = furniture.WithdrawItemAmount;
        FurnitureToBuild.WithdrawEveryString = furniture.WithdrawEveryString;
        FurnitureToBuild.WithdrawItemID = furniture.WithdrawItemID;

        if(furniture.OtherItemCosts != null)
        {
            for(int i = 0; i < furniture.OtherItemCosts.GetLength(0); i++)
            {
                FurnitureToBuild.OtherItemCurrentAmounts[i, 0] = furniture.OtherItemCosts[i][0];
                FurnitureToBuild.OtherItemCurrentAmounts[i, 1] = 0;
            }
        }
    }
}

