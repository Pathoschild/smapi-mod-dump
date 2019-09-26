using System;
using System.Reflection;
using StardewModdingAPI;
using Harmony;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using System.Collections.Generic;

namespace FamilyPlanning
{
    /* Family Planning: allow players to customize the number of children they have.
     * -> The player enters the number of children they want, for now it's a console command.
     *   -> If 0, they never get the question.
     *   -> If 1, they stop after 1.
     *   -> The default is 2, vanilla behavior. (if they don't already have more than 2 kids)
     *   -> If more than 2, then they get the event even after 2 children.
     * -> Also, this mods allows the player to customize the gender of the child at birth.
     */

    /* Documented issues:
     * -> I haven't comfirmed whether every language works with my message editing.
     * -> If you have more than 2 kids, kids will have to share beds.
     * -> You don't get new dialogue from your spouse for the kids after 2 (unless you're gay)
     * -> Your spouse can't get pregnant until the previous child is a toddler. (not vanilla?)
     * -> The way children choose beds is by gender and age. If you have two or fewer kids, they each get their own bed.
     *    Once you get more than 2, they need to double up, so they'll try first to double up with a sibling of the same gender.
     * -> Currently disabled for multiplayer
     * -> (not so much an issue: you still get the Fullhouse achievment at 2 kids)
     */

    /* Harmony patches needed:
     *  -> StardewValley.NPC.canGetPregnant() -> determines the number of children you can have
     *  -> StardewValley.Events.BirthingEvent.setUp() -> determines baby gender based on previous sibling
     *  -> StardewValley.Events.BirthingEvent.tickUpdate(GameTime time) -> spouse dialogue based on number of children
     */

    /* Content Packs:
     * I've added support for Content Packs to this mod.
     * I'll add a pre-made Content Pack on Nexus that only needs your to add the existing png's.
     * Instructions for how to make a Content Pack are in the README.md on GitHub and to a lesser extent the ChildSpriteData class.
     */

    /*
     * For now, I'm just going to be patching over the single player
     */

    class ModEntry : Mod
    {
        private static FamilyData data;
        private static List<IContentPack> contentPacks;
        private static IMonitor monitor;
        public static IModHelper helper;
        private readonly int maxChildren = 4;

        public override void Entry(IModHelper helper)
        {
            //Console commands
            helper.ConsoleCommands.Add("get_number_of_children", "Returns the number of children you can have.", GetTotalChildrenConsole);
            helper.ConsoleCommands.Add("set_number_of_children", "Sets the value for how many children you can have. (Currently, the limit is " + maxChildren + ".) \nUsage: set_number_of_children <value>\n- value: the number of children you can have.", SetTotalChildrenConsole);
            //Event handlers
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            //Harmony
            HarmonyInstance harmony = HarmonyInstance.Create("Loe2run.FamilyPlanning");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            //create variables
            monitor = Monitor;
            ModEntry.helper = helper;
            //Load content packs
            contentPacks = new List<IContentPack>();
            foreach (IContentPack contentPack in helper.ContentPacks.GetOwned())
            {
                Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}");
                contentPacks.Add(contentPack);
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;
            
            data = Helper.Data.ReadSaveData<FamilyData>(Constants.SaveFolderName) ?? new FamilyData();
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            if (Game1.farmEvent != null && Game1.farmEvent is BirthingEvent)
            {
                Game1.farmEvent = new CustomBirthingEvent();
                Game1.farmEvent.setUp();
            }
        }

        public void GetTotalChildrenConsole(string command, string[] args)
        {
            if (!Context.IsMainPlayer)
                return;

            Monitor.Log("The number of children you can have is: " + GetFamilyData().TotalChildren);
        }

        public void SetTotalChildrenConsole(string command, string[] args)
        {
            if (!Context.IsMainPlayer)
                return;

            int input;
            try
            {
                input = int.Parse(args[0]);

                if (input > maxChildren)
                    Monitor.Log("This mod currently limits your maximum to " + maxChildren + ", so your input won't be accepted.");
                else if (input >= 0)
                {
                    data.TotalChildren = input;
                    Helper.Data.WriteSaveData(Constants.SaveFolderName, data);
                    Monitor.Log("The number of children you can have has been set to " + input + ".");
                }
                else
                    Monitor.Log("Input value is out of bounds.");
            }
            catch (Exception e)
            {
                Monitor.Log(e.Message);
            }
        }

        public static FamilyData GetFamilyData()
        {
            return data;
        }

        public static Tuple<string, string> GetChildSpriteData(string childName)
        {
            foreach(IContentPack contentPack in contentPacks)
            {
                try
                {
                    ChildSpriteData childData = contentPack.ReadJsonFile<ChildSpriteData>("assets/data.json");
                    foreach (string key in childData.ChildSpriteID.Keys)
                    {
                        if (key.Equals(childName))
                        {
                            childData.ChildSpriteID.TryGetValue(key, out Tuple<string, string> pair);
                            string assetName1 = contentPack.GetActualAssetKey("assets/" + pair.Item1);
                            string assetName2 = contentPack.GetActualAssetKey("assets/" + pair.Item2);
                            return new Tuple<string, string>(assetName1, assetName2);
                        }
                    }
                }
                catch (Exception e)
                {
                    monitor.Log("An exception occurred in Loe.FamilyPlanning while loading the child sprite.");
                    monitor.Log(e.Message);
                }
            }
            return null;
        }
    }
}