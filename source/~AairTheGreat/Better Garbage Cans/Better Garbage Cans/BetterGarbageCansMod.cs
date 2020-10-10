/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AairTheGreat/StardewValleyMods
**
*************************************************/

using BetterGarbageCans.Config;
using BetterGarbageCans.Data;
using BetterGarbageCans.GamePatch;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace BetterGarbageCans
{
    public class BetterGarbageCansMod : Mod
    {
        public static BetterGarbageCansMod Instance { get; private set; }
        internal static Multiplayer multiplayer;

        internal HarmonyInstance harmony { get; private set; }

        internal ModConfig config;
        internal Dictionary<GARBAGE_CANS, GarbageCan> garbageCans;

        // Below are the variables handling birthday trash.
        internal List<NPC> allNPCCharacters = new List<NPC>();
        internal List<NPC> allNPCCharactersWithBirthdaysThisSeason = new List<NPC>();
        internal Item birthdayItem = null;
        internal int preBirthdayStartTime;
        internal int preBirthdayEndTime;
        internal GARBAGE_CANS birthdayCan;

        public override void Entry(IModHelper helper)
        {
            Instance = this;
            config = helper.Data.ReadJsonFile<ModConfig>("config.json") ?? ModConfigDefaultConfig.CreateDefaultConfig("config.json");
            
            if (config.enableMod)
            {
                harmony = HarmonyInstance.Create("com.aairthegreat.mod.garbagecan");
                harmony.Patch(typeof(Town).GetMethod("checkAction"), new HarmonyMethod(typeof(GarbageCanOverrider).GetMethod("prefix_betterGarbageCans")));

                string garbageCanFile = Path.Combine("DataFiles", "garbage_cans.json");
                garbageCans = helper.Data.ReadJsonFile<Dictionary<GARBAGE_CANS, GarbageCan>>(garbageCanFile) ?? GarbageCanDefaultConfig.CreateGarbageCans(garbageCanFile);
                if (garbageCans.Count < 8)
                {
                    garbageCans = GarbageCanDefaultConfig.UpdateConfigToLatest(garbageCans, garbageCanFile);
                }
                AddTrashToCans(config.baseTrashChancePercent);
                helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
                helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
                Type type = typeof(Game1);
                FieldInfo info = type.GetField("multiplayer", BindingFlags.NonPublic | BindingFlags.Static);
                multiplayer = info.GetValue(null) as Multiplayer;
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            ResetGarbageCanLastTimes();
            if (config.enableBirthdayGiftTrash)
            {
                SetupTheBirthdayTrash();
            }
            if (!Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater")
                   && !Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheaterJoja"))
            {
                foreach (var treasure in garbageCans[GARBAGE_CANS.JOJA_MART].treasureList)
                {
                    if (treasure.Id == 270 || treasure.Id == 809)
                    {
                        treasure.Enabled = false;
                    }
                }
            }
        }

        private void GameLoop_SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Context.IsWorldReady)
            {
                if (config.enableBirthdayGiftTrash)
                {
                    allNPCCharacters = new List<NPC>();
                    allNPCCharactersWithBirthdaysThisSeason = new List<NPC>();
                    Utility.getAllCharacters(allNPCCharacters);
                }

                if (Helper.ModRegistry.IsLoaded("Pathoschild.Automate"))
                {
                    this.Monitor.Log("Found the Automate Mod, this mod is not fully compatible with this mod.");
                }                
            }
        }

        private void AddTrashToCans(double trashChance)
        {
            foreach (int i in Enum.GetValues(typeof(GARBAGE_CANS)))
            {
                garbageCans[(GARBAGE_CANS)i].treasureList.Add(new TrashTreasure(168, "Trash", trashChance));
            }
        }

        private void ResetGarbageCanLastTimes()
        {
            // Update garbage can settings.
            foreach (int i in Enum.GetValues(typeof(GARBAGE_CANS)))
            {
                garbageCans[(GARBAGE_CANS)i].LastTimeChecked = -1;
                garbageCans[(GARBAGE_CANS)i].LastTimeFoundItem = -1;
            }

            if (config.enableBirthdayGiftTrash && birthdayItem != null)
            {
                foreach (TrashTreasure item in garbageCans[birthdayCan].treasureList)
                {
                    if (item.Id == birthdayItem.ParentSheetIndex)
                    {
                        item.Chance -= config.birthdayGiftChancePercent;
                        item.AvailableStartTime = preBirthdayStartTime;
                        item.AvailableStartTime = preBirthdayEndTime;
                    }
                }
                birthdayItem = null;
            }
        }

        private void SetupTheBirthdayTrash()
        {
            allNPCCharactersWithBirthdaysThisSeason = allNPCCharacters
                .Where(npc => (npc.CanSocialize && npc.Birthday_Season == Game1.CurrentSeasonDisplayName)
                || (npc.Name == "Dwarf" && npc.Birthday_Season == Game1.CurrentSeasonDisplayName)).ToList();

            foreach (NPC npc in allNPCCharactersWithBirthdaysThisSeason)
            {
                if (npc.Birthday_Day == Game1.dayOfMonth + 1 || npc.Birthday_Day == Game1.dayOfMonth)
                {
                    //Found a birthday.

                    birthdayItem = (Item)npc.getFavoriteItem();                    
                                        
                    switch (npc.Name)
                    {
                        case "George":
                        case "Evelyn":
                        case "Alex":
                            SetBirthdayGift(GARBAGE_CANS.EVELYN_GEORGE, birthdayItem);
                            break;
                        case "Haley":
                        case "Emily":
                            SetBirthdayGift(GARBAGE_CANS.EMILY_HALEY, birthdayItem);
                            break;
                        case "Kent":
                        case "Vincent":
                        case "Jodi":
                        case "Sam":
                            SetBirthdayGift(GARBAGE_CANS.JODI_SAM, birthdayItem);
                            break;
                        case "Clint":
                            SetBirthdayGift(GARBAGE_CANS.CLINT, birthdayItem);
                            break;
                        case "Gus":
                            SetBirthdayGift(GARBAGE_CANS.STARDROP_SALOON, birthdayItem);
                            break;
                        default:
                            SetBirthdayGift(GARBAGE_CANS.MAYOR_LEWIS, birthdayItem);
                            break;
                    }                    
                }
            }
        }

        private void SetBirthdayGift(GARBAGE_CANS can, Item favItem)
        {
            birthdayCan = can;
            bool foundItem = false;
            foreach (TrashTreasure item in garbageCans[can].treasureList)
            {
                if (item.Id == favItem.ParentSheetIndex)
                {
                    item.Chance += config.birthdayGiftChancePercent;
                    preBirthdayStartTime = item.AvailableStartTime;
                    preBirthdayEndTime = item.AvailableEndTime;
                    item.AvailableStartTime = 600;
                    item.AvailableEndTime = 2600;
                    foundItem = true;
                    break;
                }
            }

            if(!foundItem)
            {
                garbageCans[can].treasureList.Add(new TrashTreasure(favItem.ParentSheetIndex, favItem.Name, config.birthdayGiftChancePercent));
                preBirthdayStartTime = 600;
                preBirthdayEndTime = 2600;
            }
        }

        internal static void SendMulitplayerMessage(string mesageType, string playerName = null, string npcName = null)
        {
            if (multiplayer != null && playerName != null)
            {
                multiplayer.globalChatInfoMessage(mesageType, playerName, npcName);
            }
            else
            {
                multiplayer.globalChatInfoMessage(mesageType);
            }
        }
    }
}
