using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class SaveManager
{
    public AreaManager areaManager;
    public FollowerManager followerManager;
    public NPCManager npcManager;
    public BuildingManager buildingManager;
    public BattleManager battleManager;
    public HuntingManager huntingManager;
    public ItemDatabase itemDatabase;
    public PlayfabManager playfabManager;
    public Blazored.LocalStorage.ISyncLocalStorageService localStorage;

    public MessageManager messageManager;
    public GameState gameState;
    private static readonly SimpleAES Encryptor = new SimpleAES();
    public IJSRuntime JSRuntime;

    public SaveManager(AreaManager areaManager, FollowerManager followerManager, 
        NPCManager npcManager, BuildingManager buildingManager, BattleManager battleManager, 
        ItemDatabase itemDatabase, HuntingManager huntingManager, PlayfabManager playfabManager)
    {
        this.areaManager = areaManager;
        this.followerManager = followerManager;
        this.buildingManager = buildingManager;
        this.battleManager = battleManager;
        this.itemDatabase = itemDatabase;
        this.npcManager = npcManager;
        this.huntingManager = huntingManager;
        this.playfabManager = playfabManager;
    }

    public string GetSaveStringEncrypted(bool encrypt)
    {

        int pos = 0;
        string data = "";

        //Bank 0
        data += "" + gameState.GetPlayerBank().GetInventory().ToString();
        pos++;
        //Skills 1
        data += "#" + gameState.GetPlayer().GetSkillString();
        pos++;
        //Inventory 2
        data += "#" + gameState.GetPlayerInventory().ToStringSorted();
        pos++;
        //Areas 3
        data += "#" + areaManager.SaveAreas();
        pos++;
        //Followers 4
        data += "#" + followerManager.ToString();
        pos++;
        //HP 5
        data += "#" + gameState.GetPlayer().CurrentHP.ToString();
        pos++;
        //ActiveFollower 6
        if (gameState.GetPlayer().activeFollower != null)
        {
            data += "#" + gameState.GetPlayer().activeFollower.id;
        }
        else
        {
            data += "#";
        }
        pos++;
        //Recipes 7
        data += "#";
        /*foreach (string s in GetPlayer().GetRecipes())
        {
            data += s + "/";
        }*/
        pos++;
        //EquippedItems 8
        data += "#";
        foreach (KeyValuePair<GameItem, int> pair in gameState.GetPlayerInventory().GetEquippedItems())
        {
            data += pair.Key.Id + "/";
        }
        pos++;
        //Settings 9
        data += "#";
        data += gameState.isSplitView.ToString();
        data += ",";
        data += gameState.compactBankView.ToString();
        data += ",";
        data += gameState.expensiveItemThreshold;
        data += ",";
        data += gameState.totalKills;
        data += ",";
        data += gameState.PetShopUnlocked.ToString();
        data += ",";
        data += gameState.autoBuySushiSupplies.ToString();
        data += ",";
        data += gameState.totalCoinsEarned;
        data += ",";
        data += gameState.totalDeaths;
        data += ",";
        data += gameState.submitHighScores.ToString();
        pos++;
        //NPC data 10
        data += "#";
        data += npcManager.GetNPCData();
        pos++;
        //Sushi House Data 11
        data += "#";
        data += gameState.sushiHouseRice + "," + gameState.sushiHouseSeaweed;
        pos++;
        //Tannery Data 12
        data += "#";
        foreach (Building b in buildingManager.GetBuildings())
        {
            if (b.Salt > 0)
            {
                data += "" + b.ID + "," + b.Salt + "/";
            }
        }
        pos++;
        //Tannery Slot Data 13
        data += "#";
        foreach (Building b in buildingManager.GetBuildings())
        {
            if (b.IsTannery)
            {
                data += b.ID + ">";
                foreach (TanningSlot slot in b.TanneryItems)
                {
                    data += slot.GetString() + "_";
                }
                data += "@";
            }
        }
        pos++;
        //GameState.isHunting 14
        data += "#";
        data += gameState.isHunting.ToString() + ",";
        data += gameState.huntingAreaID + ",";
        data += gameState.huntingStartTime.ToString() + ",";
        data += gameState.huntingEndTime.ToString();
        pos++;
        //Bank Tabs 15
        data += "#";
        data += gameState.GetPlayerBank().GetTabsString();
        pos++;
        //Pets 16
        data += "#";
        data += gameState.GetPlayer().GetPetString();
        pos++;
        //KC 17
        data += "#";
        data += gameState.GetKCString();
        pos++;
        //Dojos 18
        data += "#";
        data += battleManager.GetDojoSaveData();
        pos++;
        if (encrypt)
        {
            data = Encryptor.EncryptToString(data);
        }

        return data;
    }
    public string GetSaveString()
    {
        return GetSaveStringEncrypted(true);
    }
    public void LoadDataFromString(string data)
    {
        string decryptedData = Encryptor.DecryptString(data);
        string[] lines = decryptedData.Split('#');

        Dictionary<GameItem, int> bankItems = Extensions.GetItemDicFromString(lines[0], itemDatabase);
        List<Skill> skills = Extensions.GetSkillsFromString(lines[1]);
        Dictionary<GameItem, int> invItems = Extensions.GetItemDicFromString(lines[2], itemDatabase);

        gameState.GetPlayerBank().GetInventory().LoadItems(bankItems);
        gameState.GetPlayer().SetSkills(skills);
        gameState.GetPlayerInventory().LoadItems(invItems);

        areaManager.LoadSaveData(lines[3]);
        followerManager.LoadSaveData(lines[4]);

        if (int.TryParse(lines[5], out int hp))
        {
            gameState.GetPlayer().CurrentHP = hp;
        }
        else
        {
            gameState.GetPlayer().CurrentHP = gameState.GetPlayer().MaxHP;
        }

        if (int.TryParse(lines[6], out int activeFollower))
        {
            gameState.GetPlayer().activeFollower = followerManager.GetFollowerByID(activeFollower);
        }

        List<string> recipes = lines[7].Split('/').ToList();
        gameState.GetPlayer().LoadRecipes(recipes);


        List<int> equippedItems = new List<int>();
        foreach (string s in lines[8].Split('/') ?? Enumerable.Empty<string>())
        {
            if (int.TryParse(s, out int id))
            {
                equippedItems.Add(id);
            }
        }
        if (equippedItems != null && equippedItems.Count > 0)
        {
            gameState.GetPlayer().EquipItems(equippedItems);
        }
        if (lines.Length > 9 && lines[9] != null)
        {
            string[] settings = lines[9].Split(',');
            gameState.isSplitView = bool.Parse(settings[0]);
            gameState.compactBankView = bool.Parse(settings[1]);
            if (settings.Length > 2 && settings[2] != null)
            {
                gameState.expensiveItemThreshold = int.Parse(settings[2]);
            }
            if (settings.Length > 3 && settings[3] != null)
            {
                gameState.totalKills = int.Parse(settings[3]);
            }
            if (settings.Length > 4 && settings[4] != null)
            {
                gameState.PetShopUnlocked = bool.Parse(settings[4]);
            }
            if (settings.Length > 5 && settings[5] != null)
            {
                gameState.autoBuySushiSupplies = bool.Parse(settings[5]);
            }
            if (settings.Length > 6 && settings[6] != null)
            {
                gameState.totalCoinsEarned = long.Parse(settings[6]);
            }
            if (settings.Length > 7 && settings[7] != null)
            {
                gameState.totalDeaths = int.Parse(settings[7]);
            }
            if (settings.Length > 8 && settings[8] != null)
            {
                gameState.submitHighScores = bool.Parse(settings[8]);
            }
        }
        if (lines.Length > 10 && lines[10] != null)
        {
            npcManager.LoadNPCData(lines[10]);
        }
        if (lines.Length > 11 && lines[11] != null)
        {
            gameState.sushiHouseRice = int.Parse(lines[11].Split(',')[0]);
            gameState.sushiHouseSeaweed = int.Parse(lines[11].Split(',')[1]);
        }
        if (lines.Length > 12 && lines[12] != null)
        {
            string[] salts = lines[12].Split('/');
            foreach (string salt in salts)
            {
                if (salt.Length > 0)
                {
                    int id = int.Parse(salt.Split(',')[0]);
                    int amount = int.Parse(salt.Split(',')[1]);
                    buildingManager.GetBuildingByID(id).Salt = amount;
                }
            }
        }
        if (lines.Length > 13 && lines[13] != null)
        {
            string[] tanneries = lines[13].Split('@');

            foreach (string tannery in tanneries)
            {
                if (tannery.Length > 0)
                {
                    Building t = buildingManager.GetBuildingByID(int.Parse(tannery.Split('>')[0]));
                    string[] tanneryData = tannery.Split('>')[1].Split('_');
                    int i = 0;
                    foreach (string s in tanneryData)
                    {
                        if (s.Length > 0 && i < t.TannerySlots)
                        {
                            TanningSlot slot = new TanningSlot();
                            slot.SetDataFromString(s);
                            t.TanneryItems.Add(slot);
                            i++;
                        }

                    }
                }
            }
        }
        if (lines.Length > 14 && lines[14] != null)
        {
            if (bool.TryParse(lines[14].Split(',')[0], out bool isHunting))
            {
                if (isHunting)
                {
                    gameState.isHunting = true;
                    gameState.huntingAreaID = int.Parse(lines[14].Split(',')[1]);
                    gameState.huntingStartTime = DateTime.Parse(lines[14].Split(',')[2]);
                    gameState.huntingEndTime = DateTime.Parse(lines[14].Split(',')[3]);
                    huntingManager.LoadHunt(gameState.huntingStartTime, gameState.huntingEndTime);

                }
            }
        }
        if (lines.Length > 15 && lines[15] != null)
        {
            gameState.GetPlayerBank().LoadTabsFromString(lines[15]);
        }
        if (lines.Length > 16 && lines[16] != null)
        {
            gameState.GetPlayer().LoadPetsFromString(lines[16]);
        }
        if (lines.Length > 17 && lines[17] != null)
        {
            gameState.LoadKC(lines[17]);
        }
        if (lines.Length > 18 && lines[18] != null)
        {
            battleManager.LoadDojoSaveData(lines[18]);
        }
        messageManager.AddMessage("Save game loaded.");
        gameState.saveDataLoaded = true;
        gameState.UpdateState();
    }
    public void StartAutosaving()
    {
        if (gameState.autoSaveTimer != null)
        {
            gameState.autoSaveTimer.Dispose();
            gameState.autoSaveTimer = null;
        }
        gameState.autoSaveTimer = new Timer(new TimerCallback(_ =>
        {
            try
            {
                SaveData();
            }
            catch
            {
                messageManager.AddMessage("Failed to auto save. Please make a local backup before closing the game.", "red");
            }
            gameState.saveToPlayFab++;
            if (gameState.saveToPlayFab > gameState.saveToPlayFabEvery)
            {
                SaveToCloud();
                gameState.saveToPlayFab = 0;
            }
        }), null, 60000, 60000);
    }
    public void SaveToCloud()
    {
        if (playfabManager.IsConnected == false && (gameState.userID == null || gameState.userID == ""))
        {
            ConnectToKongregate();
        }
        else if (playfabManager.IsConnected)
        {
            playfabManager.Save(GetSaveStringEncrypted(false));
        }
    }
    private async Task ConnectToKongregate()
    {
        try
        {
            gameState.userID = await JSRuntime.InvokeAsync<string>("kongregateFunctions.getUserID");
            gameState.token = await JSRuntime.InvokeAsync<string>("kongregateFunctions.getToken");
        }
        catch
        {

            gameState.userID = "";
            gameState.token = "";

        }
    }
    private void SaveData()
    {
        while (!gameState.safeToSave)
        {
            Thread.Sleep(5);
        }

        localStorage.SetItem("SaveGameExists", true);
        localStorage.SetItem("eSave", GetSaveStringEncrypted(true));
        UpdateHighScores();

        messageManager.AddMessage("Your game has been saved.");
        //gameHasBeenSaved = true;
        gameState.saveDataLoaded = true;
        gameState.saveGameExists = true;
        GC.Collect();
    }
    private async void UpdateHighScores()
    {
        if (playfabManager.IsConnected == false && (gameState.userID == null || gameState.userID == ""))
        {
            await ConnectToKongregate();
        }
        else
        {
            await JSRuntime.InvokeAsync<object>("kongregateFunctions.updateTotalLevelScore", gameState.GetPlayer().GetTotalLevel());
            await JSRuntime.InvokeAsync<object>("kongregateFunctions.updateTotalKills", gameState.totalKills);
        }


    }
}
