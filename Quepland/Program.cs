﻿using Microsoft.AspNetCore.Blazor.Hosting;

namespace Quepland
{
    public class Program
    {
        public static ItemDatabase itemDatabase = new ItemDatabase();
        public static GatherManager gatherManager = new GatherManager();
        public static AreaManager areaManager = new AreaManager();
        public static BattleManager battleManager = new BattleManager();
        public static HuntingManager huntingManager = new HuntingManager();
        public static BuildingManager buildingManager = new BuildingManager();
        public static FollowerManager followerManager = new FollowerManager();
        public static NPCManager npcManager = new NPCManager();
        public static PlayfabManager playfabManager = new PlayfabManager();
        public static PetManager petManager = new PetManager();
        public static HouseManager houseManager = new HouseManager();

        public static SaveManager saveManager = new SaveManager(areaManager, followerManager, npcManager, buildingManager, battleManager, itemDatabase, huntingManager, playfabManager, petManager, houseManager);
        
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
