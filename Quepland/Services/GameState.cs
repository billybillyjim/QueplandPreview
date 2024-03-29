﻿using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public class GameState
{
    public event EventHandler StateChanged;
    public IJSRuntime JSRuntime;

    public bool isGathering;
    public bool isHunting;
    public bool isWorkingOut;
    public bool isRunning;
    public bool isSmithing;
    public bool isFighting;
    public bool canSell;
    public bool canBank;
    public bool sellGuard;
    public bool isUsing { get; set; }
    public bool PetShopUnlocked;

    public string userID;
    public string token;

    public int huntingAreaID;
    public DateTime huntingEndTime;
    public DateTime huntingStartTime;

    public DateTime bedBoostEndTime;
    public int bedBoostAmount;

    public bool isAutoCollecting;

    public bool isSplitView;
    public bool inventoryIsActiveView;
    public string activeView = "Skills";
    public List<string> activeButtons = new List<string>() { "Skills", "Inventory" };
    public bool compactBankView;
    public bool autoBuySushiSupplies;
    public bool hideLockedItems;
    public bool submitHighScores = true;

    public bool saveDataLoaded;
    public bool gameDataLoaded;
    public bool saveGameExists;
    public bool safeToSave = true;
    public bool safeToLoad = false;

    public string previousURL;
    public string updateVersionString = "1.18c";

    public string gatherItem;


    public GameItem currentUsedItem;
    public GameItem currentGatherItem;
    public GameItem currentBuffItem;

    public int sushiHouseRice;
    public int sushiHouseSeaweed;

    public int bankWithdrawAmount;

    public int buffSecondsLeft;

    public int bankSortStyle;

    public int expensiveItemThreshold = 10000;

    public int totalKills;
    public List<int> killCount = new List<int>();
    public int totalDeaths;
    public long totalCoinsEarned;

    private Player player = new Player();
    public House playerHouse = new House();

    public Area currentArea;
    public string currentRegion = "Quepland";
    public string previousArea;

    public Timer attackTimer;
    public Timer foodTimer;
    public Timer bedBoostTimer;
    public Timer UIRefreshTimer;

    //Area Menu Timers
    public Timer gatherTimer;
    public Timer huntCountdownTimer;
    public Timer autoCollectTimer;
    public Timer miningVeinSearchTimer;
    public Timer workoutTimer;

    //Fighting Menu Timers
    public Timer monsterAttackTimer;
    public Timer autoFightTimer;

    //Inventory Timer
    public Timer createRepeatTimer;

    //Smithing Timer
    public Timer smithingTimer;
    public Timer autoSmithingTimer;

    //Navbar Timer
    public Timer autoSaveTimer;
    public int saveToPlayFab = 0;
    public int saveToPlayFabEvery = 5;

    public Pet petToBuy;
    public List<Pet> buyablePets = new List<Pet>();
    public PetManager petManager;

    

    private static SimpleAES Encryptor = new SimpleAES();

    private void StateHasChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateState()
    {
        StateHasChanged();
    }

    public void RestorePet(string petID)
    {
        foreach (Pet pet in petManager.GetPets())
        {
            if (pet.Identifier == petID && GetPlayer().HasPet(pet) == false)
            {
                GetPlayer().Pets.Add(pet);
            }
        }
    }
    public void ToggleBankStyle()
    {
        compactBankView = !compactBankView;
    }
    public Player GetPlayer()
    {
        return player;
    }
    public Inventory GetPlayerInventory()
    {
        return player.inventory;
    }
    public Bank GetPlayerBank()
    {
        return player.bank;
    }
    public async Task LoadPlayerData(HttpClient Http)
    {
        await player.LoadSkills(Http);
    }
    public void LoadMonsters(List<Monster> monsters)
    {
        foreach (Monster m in monsters)
        {
            killCount.Add(0);
        }
    }
    public void SetBuffItem(GameItem item)
    {
        currentBuffItem = item;
        buffSecondsLeft = item.HealDuration;
    }
    public bool CanLeave()
    {
        if (!isGathering && !isHunting && !isWorkingOut && !isSmithing && !isRunning && !isFighting)
        {
            return true;
        }
        return false;
    }
    public void Sleep(Furniture furniture)
    {
        bedBoostEndTime = DateTime.UtcNow.AddHours(furniture.BoostDuration);
        player.Sleep(furniture.BoostAmount);
        bedBoostTimer = new Timer(new TimerCallback(_ =>
        {
            if(DateTime.UtcNow.CompareTo(bedBoostEndTime) > 0)
            {
                player.EndBedBoost();
            }
        }), null, 60000, 60000);
        UpdateState();
    }
    public void StopActions()
    {
        gatherItem = "";
        isWorkingOut = false;
        isRunning = false;
        isGathering = false;
        isHunting = false;
        isSmithing = false;
        isFighting = false;

        if (attackTimer != null)
        {
            attackTimer.Dispose();
            attackTimer = null;
        }
        if (gatherTimer != null)
        {
            gatherTimer.Dispose();
            gatherTimer = null;
        }
        if (huntCountdownTimer != null)
        {
            huntCountdownTimer.Dispose();
            huntCountdownTimer = null;
        }
        if (autoCollectTimer != null)
        {
            autoCollectTimer.Dispose();
            autoCollectTimer = null;
        }
        if (miningVeinSearchTimer != null)
        {
            miningVeinSearchTimer.Dispose();
            miningVeinSearchTimer = null;
        }
        if (workoutTimer != null)
        {
            workoutTimer.Dispose();
            workoutTimer = null;
        }
        if (monsterAttackTimer != null)
        {
            monsterAttackTimer.Dispose();
            monsterAttackTimer = null;
        }
        if (autoFightTimer != null)
        {
            autoFightTimer.Dispose();
            autoFightTimer = null;
        }
        if (createRepeatTimer != null)
        {
            createRepeatTimer.Dispose();
            createRepeatTimer = null;
        }
        if (smithingTimer != null)
        {
            smithingTimer.Dispose();
            smithingTimer = null;
        }
        if (autoSmithingTimer != null)
        {
            autoSmithingTimer.Dispose();
            autoSmithingTimer = null;
        }
        if (UIRefreshTimer != null)
        {
            UIRefreshTimer.Dispose();
            UIRefreshTimer = null;
        }
        UpdateState();
    }
    public void ToggleSplitView()
    {
        isSplitView = !isSplitView;
        activeButtons.Clear();
        if (isSplitView)
        {

            activeButtons.Add("Skills");
            activeButtons.Add("Inventory");
        }
        else
        {

            activeView = "Inventory";

        }
        UpdateState();
    }
    public void ToggleActiveView()
    {
        inventoryIsActiveView = !inventoryIsActiveView;
        UpdateState();
    }
    public void ToggleSubmitHighScores()
    {
        submitHighScores = !submitHighScores;
    }
    public void IncrementKillCount(int monsterID)
    {
        killCount[monsterID] += 1;
    }
    public async void GetKongregateLogin()
    {
        userID = await JSRuntime.InvokeAsync<string>("kongregateFunctions.getUserID");
        token = await JSRuntime.InvokeAsync<string>("kongregateFunctions.getToken");
    }
    
    public string GetKCString()
    {
        string data = "";
        foreach(int i in killCount)
        {
            data += i + ",";
        }
        return data;
    }
    public void LoadKC(string kcString)
    {
        string[] data = kcString.Split(',');
        int i = 0;
        foreach(string line in data)
        {
            if(line.Length > 0)
            {
                killCount[i] = int.Parse(line);
                i++;
            }
        }
    }
    public void TestLoadKC(string kcString)
    {
        string[] data = kcString.Split(',');
        int i = 0;
        foreach (string line in data)
        {
            if (line.Length > 0)
            {
                try
                {
                    int.Parse(line);
                }
                catch
                {
                    Console.WriteLine("Kill Count:Failed to parse kill count for:" + line + ", " + i);
                }
                i++;
            }
        }
    }
}
