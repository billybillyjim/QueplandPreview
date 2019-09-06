using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class HouseManager
{

    public GameState gameState;
    public MessageManager messageManager;
    public ItemDatabase itemDatabase;

    public bool useLowestPlank;
    public bool useLowestBar;
    public GameItem currentPlank;
    public GameItem currentBar;
    public GameItem currentOtherItem;
    private static Random rand = new Random();

    public List<Furniture> furniture = new List<Furniture>();
    public List<FurnitureSlot> furnitureSlots = new List<FurnitureSlot>();
    public List<FurnitureSlot> emptySlots = new List<FurnitureSlot>();


    public void BeginBuilding(Furniture furniture, FurnitureSlot slot)
    {
        slot.SetFurniture(furniture);
        slot.hasSelected = true;
        gameState.UpdateState();
    }
    public void CompleteFurniture(FurnitureSlot slot)
    {
        slot.isFinished = true;
        slot.FurnitureToBuild.IsFinished = true;
        gameState.UpdateState();
    }

    public void Withdraw(FurnitureSlot slot)
    {
        if (gameState.GetPlayerInventory().AddMultipleOfItem(itemDatabase.GetItemByID(slot.FurnitureToBuild.WithdrawItemID), slot.FurnitureToBuild.WithdrawItemAmount))
        {
            slot.FurnitureToBuild.LastWithdrawn = DateTime.UtcNow;
            messageManager.AddMessage("You take " + slot.FurnitureToBuild.WithdrawItemAmount + " " + itemDatabase.GetItemByID(slot.FurnitureToBuild.WithdrawItemID).ItemName + "s.");
        }
        else
        {
            messageManager.AddMessage("You don't have enough inventory space.");
        }
        gameState.UpdateState();
    }
    public void SetCurrentPlank(GameItem item)
    {
        currentPlank = item;
        gameState.UpdateState();
    }
    public void SetCurrentBar(GameItem item)
    {
        currentBar = item;
        gameState.UpdateState();
    }
    public void SetCurrentOther(GameItem item)
    {
        currentOtherItem = itemDatabase.GetItemByID(item.Id);
        gameState.UpdateState();
    }
    public void DepositPlanks(FurnitureSlot slot)
    {

        if (currentPlank != null && slot.Inventory.GetAmountOfPlanks() < slot.FurnitureToBuild.PlanksRequired)
        {
            int amount = Math.Min(gameState.GetPlayerBank().GetInventory().GetAmountOfItem(currentPlank), (slot.FurnitureToBuild.PlanksRequired - slot.Inventory.GetAmountOfPlanks()));
            gameState.GetPlayerBank().GetInventory().RemoveAsManyItemsAsPossible(currentPlank, amount);
            slot.Inventory.AddMultipleOfItem(currentPlank, amount);
            messageManager.AddMessage("Added " + amount + " " + currentPlank.ItemName + "s to " + slot.Name);
            if (gameState.GetPlayerBank().GetInventory().GetAmountOfItem(currentPlank) == 0)
            {
                SetCurrentPlank(gameState.GetPlayerBank().GetInventory().GetLowestLevelPlank());
            }
        }

        gameState.UpdateState();
    }
    public void DepositBars(FurnitureSlot slot)
    {
        if (currentBar != null && slot.Inventory.GetAmountOfBars() < slot.FurnitureToBuild.BarsRequired)
        {
            int amount = Math.Min(gameState.GetPlayerBank().GetInventory().GetAmountOfItem(currentBar), (slot.FurnitureToBuild.BarsRequired - slot.Inventory.GetAmountOfBars()));
            gameState.GetPlayerBank().GetInventory().RemoveAsManyItemsAsPossible(currentBar, amount);
            slot.Inventory.AddMultipleOfItem(currentBar, amount);
            messageManager.AddMessage("Added " + amount + " " + currentBar.ItemName + "s to " + slot.Name);
            if (gameState.GetPlayerBank().GetInventory().GetAmountOfItem(currentBar) == 0)
            {
                SetCurrentBar(gameState.GetPlayerBank().GetInventory().GetLowestLevelBar());
            }

        }
        gameState.UpdateState();
    }
    public void ToggleUsePlankOption()
    {
        useLowestPlank = !useLowestPlank;
        if (useLowestPlank)
        {
            SetCurrentPlank(gameState.GetPlayerBank().GetInventory().GetLowestLevelPlank());
        }
        gameState.UpdateState();
    }
    public void ToggleUseBarOption()
    {
        useLowestBar = !useLowestBar;
        if (useLowestBar)
        {
            SetCurrentBar(gameState.GetPlayerBank().GetInventory().GetLowestLevelBar());
        }
        gameState.UpdateState();
    }
    public void DepositOther(FurnitureSlot slot)
    {
        if (currentOtherItem != null && slot.Inventory.GetAmountOfItem(currentOtherItem) < slot.FurnitureToBuild.GetCostOfItem(currentOtherItem.Id))
        {
            int amount = Math.Min(gameState.GetPlayerBank().GetInventory().GetAmountOfItem(currentOtherItem), (slot.FurnitureToBuild.GetCostOfItem(currentOtherItem.Id) - slot.Inventory.GetAmountOfItem(currentOtherItem)));
            gameState.GetPlayerBank().GetInventory().RemoveAsManyItemsAsPossible(currentOtherItem, amount);
            slot.Inventory.AddMultipleOfItem(currentOtherItem, amount);
            messageManager.AddMessage("Added " + amount + " " + currentOtherItem.ItemName + " to " + slot.Name);

        }

        gameState.UpdateState();
    }
    public void Construct(Furniture furniture)
    {
        gameState.StopActions();
        gameState.isGathering = true;
        gameState.gatherTimer = new Timer(new TimerCallback(_ =>
        {
            if (furniture.Progress < furniture.GetWorkRequired())
            {
                GameItem nail = gameState.GetPlayerInventory().GetBestNails();
                if (nail == null)
                {
                    messageManager.AddMessage("You have run out of nails.");
                    gameState.StopActions();
                    return;
                }
                int level = gameState.GetPlayer().GetLevel("Construction");
                int nailsUsed = Math.Min(rand.Next(1, 1 + Math.Max(100 - level, 1)), gameState.GetPlayerInventory().GetAmountOfItem(nail));
                //REMOVE MULTIPLIER BEFORE RELEASE
                furniture.Progress += nail.NailLevel * 100;
                gameState.GetPlayer().GainExperience("Construction", nail.NailLevel);
                nailsUsed = gameState.GetPlayerInventory().RemoveAsManyItemsAsPossible(nail, nailsUsed);
                messageManager.AddMessage("You hammer two planks together with " + nailsUsed + " " + nail.ItemName);
            }
            else
            {
                furniture.IsFinished = true;
                gameState.StopActions();
            }
            gameState.UpdateState();

        }), null, 500, 500);
        gameState.UpdateState();

    }

    public int GetSlotMinimumLevel(FurnitureSlot slot)
    {
        return 0;
    }
    public void CancelBuild(FurnitureSlot slot)
    {
        foreach (KeyValuePair<GameItem, int> pair in slot.Inventory.GetItems())
        {
            gameState.GetPlayerBank().GetInventory().AddMultipleOfItem(pair.Key, pair.Value);
        }
        if (emptySlots.Remove(slot))
        {
            messageManager.AddMessage("You have cancelled building the " + slot.Name + " and any materials you used have been returned to the bank.");
        }
        else
        {
            messageManager.AddMessage("Failed to remove slot because it wasnt event there.");
        }

        gameState.UpdateState();

    }

    public async Task LoadFurnitureAndSlots(HttpClient Http)
    {
        Furniture[] furnitureArray = await Http.GetJsonAsync<Furniture[]>("data/furniture.json");
        furniture.AddRange(furnitureArray);
        FurnitureSlot[] slotsArray = await Http.GetJsonAsync<FurnitureSlot[]>("data/furnitureSlots.json");
        furnitureSlots.AddRange(slotsArray);
    }

    public List<Furniture> GetAvailableFurnitureForSlot(FurnitureSlot slot)
    {
        List<Furniture> availableFurniture = new List<Furniture>();
        foreach(int id in slot.AvailableFurnitureIDs)
        {
            availableFurniture.Add(furniture[id]);
        }
        return availableFurniture;
    }
    public int GetSpaceUsed()
    {   
        return emptySlots.Sum(x => x.SpaceRequired);
    }
}

