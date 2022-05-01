/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NormanPCN/StardewValleyMods
**
*************************************************/

using System;
using System.Text;

using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using GenericModConfigMenu;
using Helpers;
using HarmonyLib;


namespace EasierMonsterEradication
{
    public class ApiImplementation : IEasierMonsterEradicationApi
    {
        private ModEntry _instance;

        public ApiImplementation(ModEntry instance)
        {
            _instance = instance;
        }

        public int GetMonsterGoal(string nameOfMonster)
        {
            return ModEntry.DoGetMonsterGoal(nameOfMonster);
        }
    }

    public class ModEntry : Mod
    {
        public const float MinPercent = 0.2f;
        public const float MaxPercent = 1.5f;

        public static ModConfig Config;

        internal static ModEntry Instance;
        internal static IModHelper MyHelper;
        internal static Logger Log;

        internal bool Debug;

        private struct MonsterRec
        {
            public string GroupName;// must match game internal string used by "Gil". showMonsterKillList.
            public int KillsNeededOld;//vanilla kill amount
            public int KillsNeededNew;
            public string RewardName;
            public string[] Monsters;// string must match game internal monster names.

            public MonsterRec(string groupName, int killsNeeded, string rewardName, string[] monsterNames)
            {
                GroupName = groupName;
                KillsNeededOld = killsNeeded;
                KillsNeededNew = killsNeeded;
                RewardName = rewardName;
                Monsters = monsterNames;
            }
        }

        // the enum and lookup table must match order
        private enum MonsterType { Slime, DustSprite, Bat, Serpent, VoidSpirit, MagmaSprite, Insect, Mummy, RockCrab, Skeleton, Dino, Duggy };
        private static MonsterRec[] MonsterTable = new MonsterRec[(int)MonsterType.Duggy + 1]
        {
            new MonsterRec("Slimes",      1000, "Gil_Slime Charmer Ring",    new string[4] { "Green Slime", "Frost Jelly", "Sludge", "Tiger Slime" }),
            new MonsterRec("DustSprites", 500,  "Gil_Burglar's Ring",        new string[1] { "Dust Spirit" }),
            new MonsterRec("Bats",        200,  "Gil_Vampire Ring",          new string[4] { "Bat", "Frost Bat", "Lava Bat", "Iridium Bat" }),
            new MonsterRec("Serpent",     250,  "Gil_Napalm Ring",           new string[2] { "Serpent", "Royal Serpent" }),
            new MonsterRec("VoidSpirits", 150,  "Gil_Savage Ring",           new string[4] { "Shadow Guy", "Shadow Shaman", "Shadow Brute", "Shadow Sniper" }),
            new MonsterRec("MagmaSprite", 150,  "Gil_Telephone",             new string[2] { "Magma Sprite", "Magma Sparker" }),
            new MonsterRec("CaveInsects", 125,  "Gil_Insect Head",           new string[3] { "Grub", "Fly", "Bug" }),
            new MonsterRec("Mummies",     100,  "Gil_Arcane Hat",            new string[1] { "Mummy" }),
            new MonsterRec("RockCrabs",   60,   "Gil_Crabshell Ring",        new string[3] { "Rock Crab", "Lava Crab", "Iridium Crab" }),
            new MonsterRec("Skeletons",   50,   "Gil_Skeleton Mask",         new string[2] { "Skeleton", "Skeleton Mage" }),
            new MonsterRec("PepperRex",   50,   "Gil_Knight's Helmet",       new string[1] { "Pepper Rex" }),
            new MonsterRec("Duggies",     30,   "Gil_Hard Hat",              new string[2] { "Duggy", "Magma Duggy" })
        };

        public String I18nGet(String str)
        {
            return MyHelper.Translation.Get(str);
        }

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            MyHelper = helper;
            Log = new Logger(this.Monitor);

            MyHelper.Events.GameLoop.GameLaunched += OnGameLaunched;
            MyHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            MyHelper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        public static int DoGetMonsterGoal(string nameOfMonster)
        {
            for (int i = 0; i < MonsterTable.Length; i++)
            {
                var group = MonsterTable[i];

                if (nameOfMonster.Equals(group.GroupName, StringComparison.Ordinal))
                {
                    return group.KillsNeededNew;
                }

                foreach (string monster in group.Monsters)
                {
                    if (nameOfMonster.Equals(monster, StringComparison.Ordinal))
                    {
                        return group.KillsNeededNew;
                    }
                }
            }
            return -1;
        }

        public override object GetApi()
        {
            return new ApiImplementation(this);
        }

        private string GetParagraphText()
        {
            //return MyHelper.Translation.Get("VanillaMonsters", new { slimes = MonstersTable[0].KillsNeededNew.ToString() });
            return I18nGet("VanillaMonsters");
        }

        private static void SetupNewGoalValue(float monsterPercentage)
        {
            Config.MonsterPercentage = monsterPercentage;

            for (int i = 0; i < MonsterTable.Length; i++)
                MonsterTable[i].KillsNeededNew = (int)((float)MonsterTable[i].KillsNeededOld * monsterPercentage);
        }

        /// <summary>Raised after the game has loaded and all Mods are loaded. Here we load the config.json file and setup GMCM </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = MyHelper.ReadConfig<ModConfig>();
            if (Config.MonsterPercentage < MinPercent)
                Config.MonsterPercentage = MinPercent;
            else if (Config.MonsterPercentage > MaxPercent)
                Config.MonsterPercentage = MaxPercent;

            SetupNewGoalValue(Config.MonsterPercentage);

            // use GMCM in an optional manner.

            //IGenericModConfigMenuApi gmcm = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            var gmcm = Helper.ModRegistry.GetGenericModConfigMenuApi(this.Monitor);
            if (gmcm != null)
            {
                gmcm.Register(ModManifest,
                              reset: () => Config = new ModConfig(),
                              save: () => Helper.WriteConfig(Config),
                              titleScreenOnly:true);

                //gmcm.AddBoolOption(ModManifest,
                //                   () => Config.xxx,
                //                   (bool value) => Config.xxx = value,
                //                   () => I18nGet("config.Label"),
                //                   () => I18nGet("config.Tooltip"));
                gmcm.AddNumberOption(ModManifest,
                                     () => Config.MonsterPercentage,
                                     (float value) => SetupNewGoalValue(value),
                                     () => I18nGet("monsterPercent.Label"),
                                     () => I18nGet("monsterPercent.tooltip"),
                                     min: MinPercent,
                                     max: MaxPercent,
                                     interval: 0.05f);
                gmcm.AddParagraph(ModManifest, () => GetParagraphText());
            }
            else
            {
                Log.LogOnce("Generic Mod Config Menu not available.");
            };

            Debug = Config.Debug;
#if DEBUG
            Debug = true;
#endif

            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.PatchAll();
        }

        /// <summary>Raised after a game save is loaded. Here we hook into necessary events for gameplay.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (Debug)
                MyHelper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        /// <summary>Raised after a game has exited a game/save to the title screen.  Here we unhook our gameplay events.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            if (Debug)
                MyHelper.Events.Input.ButtonPressed -= Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree)
            {
                if (Debug)
                {
                    var stats = Game1.player.stats;

                    if (e.Button == SButton.F5) //set monsters just below threshold
                    {
                        for (int i = 0; i < MonsterTable.Length; i++)
                        {
                            var group = MonsterTable[i];

                            int killed = 0;
                            foreach (string monster in group.Monsters)
                            {
                                if (stats.specificMonstersKilled.TryGetValue(monster, out int thisKill))
                                {
                                    killed += thisKill;
                                }
                            }
                            int needed = group.KillsNeededNew;

                            // make sure the first monster exists
                            if (!stats.specificMonstersKilled.ContainsKey(group.Monsters[0]))
                                stats.specificMonstersKilled.Add(group.Monsters[0], 0);

                            needed = needed - killed - 2;
                            if (needed > 1)
                                stats.specificMonstersKilled[group.Monsters[0]] += needed;
                        }
                    }
                    else if (e.Button == SButton.F6) //complete one monster slayer goal
                    {
                        for (int i = 0; i < MonsterTable.Length; i++)
                        {
                            var group = MonsterTable[i];

                            // make sure the first monster exists
                            if (!stats.specificMonstersKilled.ContainsKey(group.Monsters[0]))
                                stats.specificMonstersKilled.Add(group.Monsters[0], 0);

                            int killed = 0;
                            foreach (string monster in group.Monsters)
                            {
                                if (stats.specificMonstersKilled.TryGetValue(monster, out int thisKill))
                                {
                                    killed += thisKill;
                                }
                            }
                            int needed = group.KillsNeededNew;

                            if (killed < needed)
                            {
                                do
                                {
                                    killed++;
                                    stats.monsterKilled(group.Monsters[0]);
                                }
                                while (killed < needed);
                                return;
                            }
                        }
                    }
                }
            }
        }

        // the game code has embedded literal constants for the monster goals spread across numerous methods.
        // rather than try to transpile every literal, I largely just copy the game method code function here and replace the methods with Harmony.
        // for the monster calculations, I did substitute a lookup table setup.

        //different from game code due to lookup table
        private static bool willThisKillCompleteAMonsterSlayerQuest(string nameOfMonster)
        {
            for (int i = 0; i < MonsterTable.Length; i++)
            {
                var player = Game1.player;

                var group = MonsterTable[i];
                foreach (string monster in group.Monsters)
                {
                    if (nameOfMonster.Equals(monster, StringComparison.Ordinal))
                    {
                        if (!player.mailReceived.Contains(group.RewardName))
                        {
                            var stats = player.stats;

                            int killed = 0;
                            foreach (string specificMonster in group.Monsters)
                            {
                                if (stats.specificMonstersKilled.TryGetValue(specificMonster, out int thisKill))
                                {
                                    killed += thisKill;
                                }
                            }

                            if ((killed < group.KillsNeededNew) && (killed+1 >= group.KillsNeededNew))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        //different from game code due to lookup table
        private static bool areAllMonsterSlayerQuestsComplete()
        {
            for (int i = 0; i < MonsterTable.Length; i++)
            {
                var stats = Game1.player.stats;
                var group = MonsterTable[i];

                int killed = 0;
                foreach (string monster in group.Monsters)
                {
                    if (stats.specificMonstersKilled.TryGetValue(monster, out int thisKill))
                    {
                        killed += thisKill;
                    }
                }

                if (killed < group.KillsNeededNew)
                    return false;
            }
            return true;
        }

        // verbatum copy of game code
        private static string killListLine(string monsterType, int killCount, int target)
        {
            string monsterNamePlural = Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_" + monsterType);
            if (killCount == 0)
            {
                return Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_None", killCount, target, monsterNamePlural) + "^";
            }
            if (killCount >= target)
            {
                return Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat_OverTarget", killCount, target, monsterNamePlural) + "^";
            }
            return Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_LineFormat", killCount, target, monsterNamePlural) + "^";
        }

        //mostly a copy of the game code
        private static void showMonsterKillList()
        {
            var player = Game1.player;

            StringBuilder stringBuilder = new StringBuilder();

            if (!player.mailReceived.Contains("checkedMonsterBoard"))
            {
                player.mailReceived.Add("checkedMonsterBoard");
            }

            stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Header").Replace('\n', '^') + "^");

            for (int i = 0; i < MonsterTable.Length; i++)
            {
                var group = MonsterTable[i];

                int killed = 0;
                foreach (string monster in group.Monsters)
                {
                    if (player.stats.specificMonstersKilled.TryGetValue(monster, out int thisKill))
                    {
                        killed += thisKill;
                    }
                }

                stringBuilder.Append(killListLine(group.GroupName, killed, group.KillsNeededNew));
            }

            stringBuilder.Append(Game1.content.LoadString("Strings\\Locations:AdventureGuild_KillList_Footer").Replace('\n', '^'));
            Game1.drawLetterMessage(stringBuilder.ToString());
        }

        //verbatum copy of game code
        public void onRewardCollected(Item item, Farmer who)
        {
            if (item != null && !who.hasOrWillReceiveMail("Gil_" + item.Name))
            {
                who.mailReceived.Add("Gil_" + item.Name);
            }
        }

        //significant copy of game code
        public void GilRewards(StardewValley.Locations.AdventureGuild __instance)
        {
            List<Item> rewards = new List<Item>();

            for (int i = 0; i < MonsterTable.Length; i++)
            {
                var player = Game1.player;
                var group = MonsterTable[i];

                int killed = 0;
                foreach (string specificMonster in group.Monsters)
                {
                    if (player.stats.specificMonstersKilled.TryGetValue(specificMonster, out int thisKill))
                    {
                        killed += thisKill;
                    }
                }

                if ((killed >= group.KillsNeededNew) && !player.mailReceived.Contains(group.RewardName))
                {
                    switch ((MonsterType)i)
                    {
                        case MonsterType.Slime:
                            rewards.Add(new StardewValley.Objects.Ring(520));
                            break;
                        case MonsterType.VoidSpirit:
                            rewards.Add(new StardewValley.Objects.Ring(523));
                            break;
                        case MonsterType.Skeleton:
                            rewards.Add(new StardewValley.Objects.Hat(8));
                            break;
                        case MonsterType.DustSprite:
                            rewards.Add(new StardewValley.Objects.Ring(526));
                            break;
                        case MonsterType.Bat:
                            rewards.Add(new StardewValley.Objects.Ring(522));
                            break;
                        case MonsterType.Serpent:
                            rewards.Add(new StardewValley.Objects.Ring(811));
                            break;
                        case MonsterType.MagmaSprite:
                            var gilNpc = MyHelper.Reflection.GetField<StardewValley.NPC>(__instance, "Gil").GetValue();
                            Game1.addMail("Gil_Telephone", noLetter: true, sendToEveryone: true);
                            Game1.drawDialogue(gilNpc, Game1.content.LoadString("Strings\\Locations:Gil_Telephone"));
                            return;
                        case MonsterType.Insect:
                            rewards.Add(new StardewValley.Tools.MeleeWeapon(13));
                            break;
                        case MonsterType.Mummy:
                            rewards.Add(new StardewValley.Objects.Hat(60));
                            break;
                        case MonsterType.RockCrab:
                            rewards.Add(new StardewValley.Objects.Ring(810));
                            break;
                        case MonsterType.Dino:
                            rewards.Add(new StardewValley.Objects.Hat(50));
                            break;
                        case MonsterType.Duggy:
                            rewards.Add(new StardewValley.Objects.Hat(27));
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (Item i in rewards)
            {
                if (i is StardewValley.Object obj)
                {
                    obj.specialItem = true;
                }
            }
            if (rewards.Count > 0)
            {
                Game1.activeClickableMenu = new StardewValley.Menus.ItemGrabMenu(rewards, __instance)
                {
                    behaviorOnItemGrab = onRewardCollected
                };
                return;
            }

            var gil = MyHelper.Reflection.GetField<StardewValley.NPC>(__instance, "Gil").GetValue();
            var talkedToGil = MyHelper.Reflection.GetField<bool>(__instance, "talkedToGil");
            if (talkedToGil.GetValue())
            {
                Game1.drawDialogue(gil, Game1.content.LoadString("Characters\\Dialogue\\Gil:Snoring"));
            }
            else
            {
                Game1.drawDialogue(gil, Game1.content.LoadString("Characters\\Dialogue\\Gil:ComeBackLater"));
            }
            talkedToGil.SetValue(true);
        }

        [HarmonyPatch(typeof(StardewValley.Locations.AdventureGuild))]
        public class AdventureGuildPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(StardewValley.Locations.AdventureGuild.willThisKillCompleteAMonsterSlayerQuest))]
            [HarmonyPatch(new Type[] { typeof(string) })]
            public static bool willThisKillCompleteAMonsterSlayerQuest_Prefix(StardewValley.Locations.AdventureGuild __instance, ref bool __result, string nameOfMonster)
            {
                try
                {
                    __result = willThisKillCompleteAMonsterSlayerQuest(nameOfMonster);
                    return false;
                }
                catch (Exception ex)
                {
                    ModEntry.Log.Error($"Failed in {nameof(willThisKillCompleteAMonsterSlayerQuest_Prefix)}:\n{ex}");
                    return true;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(StardewValley.Locations.AdventureGuild.areAllMonsterSlayerQuestsComplete))]
            public static bool areAllMonsterSlayerQuestsComplete_Prefix(StardewValley.Locations.AdventureGuild __instance, ref bool __result)
            {
                try
                {
                    __result = areAllMonsterSlayerQuestsComplete();
                    return false;
                }
                catch (Exception ex)
                {
                    ModEntry.Log.Error($"Failed in {nameof(areAllMonsterSlayerQuestsComplete_Prefix)}:\n{ex}");
                    return true;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(nameof(StardewValley.Locations.AdventureGuild.showMonsterKillList))]
            public static bool showMonsterKillList_Prefix(StardewValley.Locations.AdventureGuild __instance)
            {
                try
                {
                    showMonsterKillList();
                    return false;
                }
                catch (Exception ex)
                {
                    ModEntry.Log.Error($"Failed in {nameof(showMonsterKillList_Prefix)}:\n{ex}");
                    return true;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("gil")] // private method
            public static bool gil_Prefix(StardewValley.Locations.AdventureGuild __instance)
            {
                try
                {
                    Instance.GilRewards(__instance);
                    return false;
                }
                catch (Exception ex)
                {
                    ModEntry.Log.Error($"Failed in {nameof(gil_Prefix)}:\n{ex}");
                    return true;
                }
            }
        }
    }
}

