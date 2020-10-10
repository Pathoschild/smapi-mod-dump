/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/loe2run/FamilyPlanningMod
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using Harmony;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using StardewValley.Characters;

/* TODO:
 * -> Check if multiplayer works
 * -> Implement multiplayer birthing event/naming menu stuff
 */
 
namespace FamilyPlanning
{
    /* Family Planning: allows players to customize the number of children they have and their genders,
     *                  modify the chance of a spouse asking for a child, with or without console messages,
     *                  and allows them to adopt children with a roommate. (In Vanilla, it's Krobus.)
     *                  
     * -> The player enters the number of children they want through a console command.
     *   -> If 0, they never get the question.
     *   -> If 1, they stop after 1.
     *   -> The default is 2, vanilla behavior. (if they don't already have more than 2 kids)
     *   -> If more than 2, then they get the event even after 2 children.
     *   
     * -> The player is given the option to customize the gender of the child at birth.
     * 
     * -> The player changes the chance of their spouse asking for a child through a console command.
     * -> They enter a whole number from 1 to 100, representing the percentage chance of the question happening.
     * -> The value defaults to 5%, the same as vanilla.
     * 
     * -> There's a config option, AdoptChildrenWithRoommate, which defaults to false.
     * -> If you set this value to true, then your roommate will prompt you to adopt a child.
     * -> (You could potentially stop this by setting TotalChildren to 0,
     *     but I'm not sure why you'd want to after setting the config option.)    
     * -> (If you're using a mod that turns Krobus into a normal marriage candidate, like Krobus Marriage Mod,
     *     then you don't need this setting, things will work out normally.)
     *     
     * -> There's another config option, BabyQuestionMessages, which defaults to false.
     * -> When true, you will get messages in the SMAPI console that give you information
     *    about whether your spouse is able to ask you for a child, and their chance of doing so.
     * 
     * This version of Family Planning is compatible with SMAPI 3.0 and Stardew Valley 1.4.
     */

    /* Harmony patches:
     *  -> StardewValley.NPC.canGetPregnant() -> determines the number of children you can have
     *  -> StardewValley.Characters.Child.reloadSprite() -> determines the sprite for a child
     *  -> StardewValley.Characters.Child.tenMinuteUpdate() -> tells the child where their bed is
     *  -> StardewValley.NPC.isGaySpouse() -> makes sure that roommates are given adoption dialogue
     *  -> StardewValley.Utility.pickPersonalFarmEvent() -> controls chance of baby question & creates console messages
     */

    /* Content Packs:
     * Instructions for how to make a Content Pack are in the README.md on GitHub 
     */

    /* Content Patcher:
     * I try to load the sprite from the value "Characters\\Child_<Child Name>".
     * CP Mods can get access to the child name through the custom CP tokens, then patch that value.
     * There is also a token, IsToddler, which returns:
     * -> "true" when the child is toddler age (3), and
     * -> "false" when the child is younger (0 to 2).
     * (I'm considering adding a gender token, but haven't gotten confirmation that it's necessary.)
     */

    class ModEntry : Mod
    {
        private static FamilyData data;
        private static List<IContentPack> contentPacks;
        public static IMonitor monitor;
        public static IModHelper helper;
        private ModConfig config;
        private static bool AdoptChildrenWithRoommate;
        private static bool BabyQuestionMessages;
        private bool firstTick = true;

        public override void Entry(IModHelper helper)
        {
            //create variables
            monitor = Monitor;
            ModEntry.helper = helper;
            //Load Config
            config = helper.ReadConfig<ModConfig>();
            AdoptChildrenWithRoommate = config.AdoptChildrenWithRoommate;
            BabyQuestionMessages = config.BabyQuestionMessages;
            //Console commands
            helper.ConsoleCommands.Add("get_max_children", "Returns the number of children you can have.", GetTotalChildrenConsole);
            helper.ConsoleCommands.Add("set_max_children", "Sets the value for how many children you can have. (If you set the value to more than 4, children will overlap in bed and Content Patcher mods may not work.)\nUsage: set_max_children <value>\n- value: the number of children you can have.", SetTotalChildrenConsole);
            helper.ConsoleCommands.Add("get_question_chance", "Returns the percentage chance that your spouse will ask to have a child.", GetBabyQuestionChance);
            helper.ConsoleCommands.Add("set_question_chance", "Sets the probability that your spouse will ask to have a child. The default chance is 5%.\nUsage: set_question_chance <value>\n- value: the percentage chance that your spouse will ask you for a baby (when it's possible for them to do so). Enter this as a whole number between 1 and 100, representing the percentage chance.\nExamples: set_question_chance 100 => 100% chance.\n          set_question_chance 5 => 5% chance.", SetBabyQuestionChance);
            //Event handlers
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;
            //Harmony
            HarmonyInstance harmony = HarmonyInstance.Create("Loe2run.FamilyPlanning");
            
            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.canGetPregnant)),
               postfix: new HarmonyMethod(typeof(Patches.CanGetPregnantPatch), nameof(Patches.CanGetPregnantPatch.Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Child), nameof(Child.reloadSprite)),
               postfix: new HarmonyMethod(typeof(Patches.ChildReloadSpritePatch), nameof(Patches.ChildReloadSpritePatch.Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Child), nameof(Child.tenMinuteUpdate)),
               postfix: new HarmonyMethod(typeof(Patches.ChildTenMinuteUpdatePatch), nameof(Patches.ChildTenMinuteUpdatePatch.Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.isGaySpouse)),
                postfix: new HarmonyMethod(typeof(Patches.IsGaySpousePatch), nameof(Patches.IsGaySpousePatch.Postfix))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.pickPersonalFarmEvent)),
                prefix: new HarmonyMethod(typeof(Patches.PickPersonalFarmEventPatch), nameof(Patches.PickPersonalFarmEventPatch.Prefix))
            );

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
            if (!Context.IsWorldReady)
                return;

            try
            {
                data = Helper.Data.ReadJsonFile<FamilyData>("data/" + Constants.SaveFolderName + ".json");

                if(data == null)
                {
                    data = new FamilyData();
                    Helper.Data.WriteJsonFile("data/" + Constants.SaveFolderName + ".json", data);
                }
            }
            catch (Exception)
            {
                data = new FamilyData();
                Helper.Data.WriteJsonFile("data/" + Constants.SaveFolderName + ".json", data);
            }
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Game1.farmEvent != null && Game1.farmEvent is BirthingEvent)
            {
                Game1.farmEvent = new CustomBirthingEvent();
                Game1.farmEvent.setUp();
            }
        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (firstTick)
            {
                try
                {
                    foreach (Child child in Game1.player.getChildren())
                    {
                        child.reloadSprite();
                    }
                    firstTick = false;
                }
                catch (Exception) { }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            IContentPatcherAPI api = Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            if (api == null)
                return;

            ChildToken token = new ChildToken(1);
            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FirstChildIsToddler",
                updateContext: token.AgeUpdateContext,
                isReady: token.IsReady,
                getValue: token.AgeGetValue,
                allowsInput: false,
                requiresInput: false
            );

            token = new ChildToken(2);
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "SecondChildIsToddler",
                updateContext: token.AgeUpdateContext,
                isReady: token.IsReady,
                getValue: token.AgeGetValue,
                allowsInput: false,
                requiresInput: false
            );

            token = new ChildToken(3);
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "ThirdChildIsToddler",
                updateContext: token.AgeUpdateContext,
                isReady: token.IsReady,
                getValue: token.AgeGetValue,
                allowsInput: false,
                requiresInput: false
            );

            token = new ChildToken(4);
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildName",
                updateContext: token.NameUpdateContext,
                isReady: token.IsReady,
                getValue: token.NameGetValue,
                allowsInput: false,
                requiresInput: false
            );
            api.RegisterToken(
                mod: ModManifest,
                name: "FourthChildIsToddler",
                updateContext: token.AgeUpdateContext,
                isReady: token.IsReady,
                getValue: token.AgeGetValue,
                allowsInput: false,
                requiresInput: false
            );
        }

        public void GetTotalChildrenConsole(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            Monitor.Log("The number of children you can have is: " + GetFamilyData().TotalChildren, LogLevel.Info);
        }

        public void SetTotalChildrenConsole(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            int input;
            try
            {
                input = int.Parse(args[0]);

                if (input >= 0)
                {
                    data.TotalChildren = input;
                    Helper.Data.WriteJsonFile("data/" + Constants.SaveFolderName + ".json", data);
                    Monitor.Log("The number of children you can have has been set to " + input + ".", LogLevel.Info);
                }
                else
                    Monitor.Log("Input value is out of bounds.", LogLevel.Info);
            }
            catch (Exception e)
            {
                Monitor.Log(e.Message, LogLevel.Trace);
            }
        }

        public void GetBabyQuestionChance(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            Monitor.Log("The percentage chance that your spouse will ask to have children is: " + GetFamilyData().BabyQuestionChance + "%", LogLevel.Info);
        }

        public void SetBabyQuestionChance(string command, string[] args)
        {
            if (!Context.IsWorldReady)
                return;

            int input;
            try
            {
                input = int.Parse(args[0]);

                if(input >= 1 && input <= 100)
                {
                    data.BabyQuestionChance = input;
                    Helper.Data.WriteJsonFile("data/" + Constants.SaveFolderName + ".json", data);
                    Monitor.Log("The percentage chance that your spouse will ask about having a baby (when possible) is now " + input + "%.", LogLevel.Info);
                    Monitor.Log("BabyQuestionDouble is now " + (input / 100.0), LogLevel.Trace);
                }
                else if(input == 0)
                {
                    Monitor.Log("You can't set the percentage chance to 0. If you don't want your spouse to ask about having children at all, you should use the set_max_children command.", LogLevel.Info);
                }
                else
                {
                    Monitor.Log("That value is out of bounds. The value can be between 1 to 100, representing the percentage chance. (I.e. 100 -> 100%, 50 -> 50%, 5 -> 5%).", LogLevel.Info);
                }
            }
            catch(Exception e)
            {
                Monitor.Log(e.Message, LogLevel.Trace);
            }
        }

        public static FamilyData GetFamilyData()
        {
            return data;
        }

        public static bool RoommateConfig()
        {
            return AdoptChildrenWithRoommate;
        }

        public static bool MessagesConfig()
        {
            return BabyQuestionMessages;
        }

        public static Tuple<string, string> GetChildSpriteData(string childName)
        {
            foreach (IContentPack contentPack in contentPacks)
            {
                try
                {
                    ContentPackData cpdata = contentPack.ReadJsonFile<ContentPackData>("assets/data.json");
                    if (cpdata.ChildSpriteID == null)
                        return null;
                    foreach (string key in cpdata.ChildSpriteID.Keys)
                    {
                        if (key.Equals(childName))
                        {
                            cpdata.ChildSpriteID.TryGetValue(key, out Tuple<string, string> pair);
                            string assetName1 = contentPack.GetActualAssetKey("assets/" + pair.Item1);
                            string assetName2 = contentPack.GetActualAssetKey("assets/" + pair.Item2);
                            return new Tuple<string, string>(assetName1, assetName2);
                        }
                    }
                }
                catch (Exception e)
                {
                    monitor.Log("An exception occurred in Loe2run.FamilyPlanning while loading the child sprite.");
                    monitor.Log(e.Message);
                }
            }
            return null;
        }

        public static List<Tuple<int, string>> GetSpouseDialogueData(string spouseName)
        {
            foreach (IContentPack contentPack in contentPacks)
            {
                try
                {
                    ContentPackData cpdata = contentPack.ReadJsonFile<ContentPackData>("assets/data.json");
                    if (cpdata.SpouseDialogue == null)
                        return null;
                    foreach (string key in cpdata.SpouseDialogue.Keys)
                    {
                        if (key.Equals(spouseName))
                        {
                            cpdata.SpouseDialogue.TryGetValue(key, out List<Tuple<int, string>> spouseDialogue);
                            return spouseDialogue;
                        }
                    }
                }
                catch (Exception e)
                {
                    monitor.Log("An exception occurred in Loe2run.FamilyPlanning while loading spouse dialogue.");
                    monitor.Log(e.Message);
                }
            }
            return null;
        }
    }
}