using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Locations;
using NewGamePlus.Util;

namespace NewGamePlus
{
    public class NewGamePlus : Mod
    {
        public static new IModHelper Helper;
        public static NewGamePlusConfig ModConfig { get; private set; }

        /*********
        ** Public methods
        *********/
        /// <summary>Initialise the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper)
        {
            Helper = helper;
            ModConfig = helper.ReadConfig<NewGamePlusConfig>();

            RegisterGameEvents();
        }

        #region Events
        /// <summary> Registers event handlers for mod functions. </summary>
        private void RegisterGameEvents()
        {
            //Monitor.Log("Registering game events...", LogLevel.Trace);

            TimeEvents.AfterDayStarted += AwardPlayer; // When game is loaded, check if something is missing.
            TimeEvents.AfterDayStarted += SaveAwards; // Save achievements upon load, this way load and quit would be enough to update awards.
            TimeEvents.AfterDayStarted += TravelingMerchantBonus; // If junimo way is done, travelling merchant always have at least one item required for community center

            SaveEvents.AfterSave += SaveAwards; // When day is saved, save achievements.

            MenuEvents.MenuClosed += MenuClosed; // Level up menu neds fix to work correctly with this.

            //Monitor.Log("Game events registered.", LogLevel.Trace);
        }

        /// <summary> If player has finished junimo way of refurbishing the Community Center, at future playthroughs Travelling Merchant would always have at least one item fitting for some bundle, if any. </summary>
        // Due to network nature this way does not work anymore.
        private void TravelingMerchantBonus(object sender, EventArgs args)
        {
            //if (!ModConfig.GetFlagBoolean("ccJunimo") || Game1.player.mailReceived.Contains("JojaMember")) return; // requires full cc completeon record and not being a joja member
            //if (!(Game1.getLocationFromName("Forest") as Forest).travelingMerchantDay) return; // merchant is away
            //CommunityCenter communityCenter = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
            //if (communityCenter.areAllAreasComplete()) return; // cc is completed

            //// format:
            //// key: Pantry/0 
            //// (area_name)/(area_id)
            //// value: Spring Crops/O 465 20/24 1 0 188 1 0 190 1 0 192 1 0/0
            //// (bundle_name)/(? reward_id reward_amount)/([[item_id amount quality][...]])/(?)
            //List<int> neededItems = new List<int>();
            //Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\Bundles");
            //foreach (KeyValuePair<string, string> KeyValue in dictionary)
            //{
            //    int area_id = Convert.ToInt32(KeyValue.Key.Split('/')[1]);
            //    string[] items = KeyValue.Value.Split('/')[2].Split(' ');
            //    for (int itm = 0; itm < items.Length / 3; itm++)
            //    {
            //        int item_id = Convert.ToInt32(items[itm * 3]);
            //        if (item_id > 0 && Game1.objectInformation.ContainsKey(item_id) && items[itm * 3 + 2] == "0" && !communityCenter.bundles[area_id][itm])
            //        {
            //            neededItems.Add(item_id);
            //        }
            //    }
            //}
            //Monitor.Log($"Found {neededItems.Count} items required for bundles...", LogLevel.Trace);
            //if (neededItems.Count > 0)
            //{
            //    Random r = new Random((int)((long)Game1.uniqueIDForThisGame + (long)Game1.stats.DaysPlayed));
            //    int item_id = neededItems[r.Next(neededItems.Count)];
            //    string[] itemInfo = Game1.objectInformation[item_id].Split('/');
            //    Monitor.Log($"Adding {itemInfo[0]} to Travelling Merchant stock.", LogLevel.Trace);
            //    (Game1.getLocationFromName("Forest") as Forest).travelingMerchantStock.Add((Item)new StardewValley.Object(item_id, 1, false, -1, 0), new int[2]
            //        {
            //          Math.Max(r.Next(1, 11) * 100, Convert.ToInt32(itemInfo[1]) * r.Next(3, 6)),
            //          r.NextDouble() < 0.1 ? 5 : 1
            //        });
            //}
        }

        /// <summary>Check what player is missing.</summary>
        private void AwardPlayer(object sender, EventArgs args)
        {
            if (Game1.stats.DaysPlayed == 0) return; // freshly created game before init cutscene - it would be called once again afterwards

            if (ModConfig.GetConfig("professions")) AwardProfessions();
            if (ModConfig.GetConfig("stardrops")) AwardStardrops();
            if (ModConfig.GetConfig("crafting_recipes")) AwardCraftingRecipes();
            if (ModConfig.GetConfig("cooking_recipes")) AwardCookingRecipes();
            if (ModConfig.GetConfig("experience")) AwardExperience();
            
            // Some bonuses only can be granted on a new game start. Mostly matherial rewards.
            if (Game1.stats.DaysPlayed == 1)
                AwardNewGame();
        }

        /// <summary>Update known achievements and save them into config.</summary>
        private void SaveAwards(object sender, EventArgs args)
        {
            SaveProfessions();
            SaveStardrops();
            SaveCraftingRecipes();
            SaveCookingRecipes();
            SaveExperience();

            if (Helper.Reflection.GetMethod((Game1.getLocationFromName("Town") as Town), "checkJojaCompletePrerequisite").Invoke<Boolean>()) ModConfig.SetFlag("ccJoja", true);
            if (Game1.player.hasCompletedCommunityCenter()) ModConfig.SetFlag("ccJunimo", true);

            ModConfig.SetFlagIfGreater("grandpaScore", (Game1.getLocationFromName("Farm") as Farm).grandpaScore);
            ModConfig.SetFlagIfGreater("money", Game1.player.money);
            if (Game1.player.achievements.Contains(34)) ModConfig.SetFlag("fullShipment", true);
            if (Game1.player.mailReceived.Contains("QiChallengeComplete")) ModConfig.SetFlag("QiChallengeComplete", true);
            if (StardewValley.Locations.AdventureGuild.areAllMonsterSlayerQuestsComplete()) ModConfig.SetFlag("areAllMonsterSlayerQuestsComplete", true);

            Helper.WriteConfig(ModConfig);
        }

        /// <summary>Clean up possible duplicate professions, since we don't filter what player can pick up.</summary>
        private void MenuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (args.PriorMenu is LevelUpMenu)
            {
                List<int> cleaned = new List<int>();
                foreach (int profession in Game1.player.professions)
                {
                    if (!cleaned.Contains(profession))
                        cleaned.Add(profession);
                }
                Game1.player.professions = cleaned;
            }
        }
        #endregion

        #region Professions
        /// <summary>Iterate over known professions and award ones player does not have yet.</summary>
        private void AwardProfessions()
        {
            foreach (int profession in ModConfig.Professions)
            {
                if (!Game1.player.professions.Contains(profession) &&
                    (profession % 6 > 1
                        || Game1.player.getEffectiveSkillLevel(Professions.GetSkillForProfession(profession)) >= 10
                        || Professions.HasAllProfessionsForSkill(Game1.player, Professions.GetSkillForProfession(profession)))) // only award first professions after second, due to UI limitations
                {
                    Game1.player.professions.Add(profession);

                    // hardcode... technically, this is from LevelUpMenu.getImmediateProfessionPerk
                    if (profession == StardewValley.Farmer.fighter)
                    {
                        Game1.player.maxHealth += 15;
                    }
                    if (profession == StardewValley.Farmer.defender)
                    {
                        Game1.player.maxHealth += 25;
                    }
                    Game1.player.health = Game1.player.maxHealth;
                    Monitor.Log("Awarding profession: " + profession, LogLevel.Trace);
                }
            }

        }

        /// <summary>Iterate over current professions and save new ones as known.</summary>
        private void SaveProfessions()
        {
            foreach (int profession in Game1.player.professions)
            {
                if (!ModConfig.Professions.Contains(profession))
                {
                    ModConfig.Professions.Add(profession);
                    Monitor.Log("Saving profession: " + profession, LogLevel.Trace);
                }
            }
        }
        #endregion

        #region Experience
        /// <summary>Iterate over known cooking recipes and award ones player does not have yet.</summary>
        private void AwardExperience()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Game1.player.experiencePoints[i] < ModConfig.Experience[i])
                {
                    Game1.player.gainExperience(i, ModConfig.Experience[i] - Game1.player.experiencePoints[i]);
                }
            }
        }

        /// <summary>Iterate over current cooking recipes and save new ones as known.</summary>
        private void SaveExperience()
        {
            for (int i = 0; i < 6; i++)
            {
                ModConfig.Experience[i] = Math.Max(Game1.player.experiencePoints[i], ModConfig.Experience[i]);
            }
        }
        #endregion

        #region Stardrops
        /// <summary>List of all game stardrops.</summary>
        private static String[] Stardrops { get; } = new String[] { "CF_Fair", "CF_Fish", "CF_Mines", "CF_Sewer", "museumComplete", "CF_Spouse", "CF_Statue" };

        /// <summary>Give all known stardrops.</summary>
        private void AwardStardrops()
        {
            foreach (String stardrop in ModConfig.Stardrops)
            {
                if (Array.IndexOf(Stardrops, stardrop) != -1 && !Game1.player.hasOrWillReceiveMail(stardrop))
                {
                    Game1.player.mailReceived.Add(stardrop);
                    Game1.player.MaxStamina += 34;
                }
            }

            Game1.player.Stamina = (float)Game1.player.MaxStamina;
        }

        /// <summary>Save known stardrop list.</summary>
        private void SaveStardrops()
        {
            foreach (String stardrop in Stardrops)
            {
                if (Game1.player.mailReceived.Contains(stardrop) && !ModConfig.Stardrops.Contains(stardrop))
                {
                    ModConfig.Stardrops.Add(stardrop);
                }
            }
        }
        #endregion

        #region Blueprints
        /// <summary>Iterate over known crafting recipes and award ones player does not have yet.</summary>
        private void AwardCraftingRecipes()
        {
            foreach (String recipe in ModConfig.CraftingRecipes)
            {
                if (!Game1.player.craftingRecipes.ContainsKey(recipe))
                {
                    Game1.player.craftingRecipes.Add(recipe, 0);
                }
            }
        }

        /// <summary>Iterate over current crafting recipes and save new ones as known.</summary>
        private void SaveCraftingRecipes()
        {
            foreach (String recipe in Game1.player.craftingRecipes.Keys)
            {
                if (!ModConfig.CraftingRecipes.Contains(recipe))
                {
                    ModConfig.CraftingRecipes.Add(recipe);
                }
            }
        }
        #endregion

        #region Recipes
        /// <summary>Iterate over known cooking recipes and award ones player does not have yet.</summary>
        private void AwardCookingRecipes()
        {
            foreach (String recipe in ModConfig.CookingRecipes)
            {
                if (!Game1.player.cookingRecipes.ContainsKey(recipe))
                {
                    Game1.player.cookingRecipes.Add(recipe, 0);
                }
            }
        }

        /// <summary>Iterate over current cooking recipes and save new ones as known.</summary>
        private void SaveCookingRecipes()
        {
            foreach (String recipe in Game1.player.cookingRecipes.Keys)
            {
                if (!ModConfig.CookingRecipes.Contains(recipe))
                {
                    ModConfig.CookingRecipes.Add(recipe);
                }
            }
        }
        #endregion

        #region NewGame
        /// <summary> Five player some starting stuff. Only used for a freshly started games. </summary>
        private void AwardNewGame()
        {
            //Monitor.Log("Awarding new game stuff...", LogLevel.Trace);
            if (ModConfig.GetConfig("newgame_tools"))
            {
                if (ModConfig.GetFlagBoolean("ccJunimo"))
                {
                    UpgradeTool("Watering Can");
                    UpgradeTool("Hoe");
                    Monitor.Log("Local Legend: Awarding a farming tools upgrade for a new game.", LogLevel.Info);
                }
                if (ModConfig.GetFlagBoolean("ccJoja"))
                {
                    UpgradeTool("Axe");
                    UpgradeTool("Pickaxe");
                    Monitor.Log("Joja Co. Member Of The Year: Awarding a gathering tools upgrade for a new game.", LogLevel.Info);
                }
                if (ModConfig.GetFlagBoolean("fullShipment"))
                {
                    Game1.player.increaseBackpackSize(12);
                    Monitor.Log("Full Shipment: Awarding backpack upgrade for a new game.", LogLevel.Info);
                }
            }
            if (ModConfig.GetConfig("newgame_assets"))
            {
                int moneyBonus = Decimal.ToInt32(ModConfig.GetFlagDecimal("money", 0) / 1000);
                if (moneyBonus > 0)
                {
                    Game1.player.money += moneyBonus;
                    Monitor.Log($"Hoarder: Awarding {moneyBonus} money for a new game.", LogLevel.Info);
                }
                if (Decimal.ToInt32(ModConfig.GetFlagDecimal("grandpaScore", 0)) == 4)
                {
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(628, 1));
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(629, 1));
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(630, 1));
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(631, 1));
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(632, 1));
                    Game1.player.addItemToInventory((Item)new StardewValley.Object(633, 1));
                    Monitor.Log("Perfection: Awarding a set of the tree saplings for a new game.", LogLevel.Info);
                }
                // Burglar's Ring for "Qi's Challenge"
                if (ModConfig.GetFlagBoolean("QiChallengeComplete"))
                {
                    Game1.player.leftRing.Value = new Ring(526);
                    Game1.player.leftRing.Value.onEquip(Game1.player, Game1.player.currentLocation);
                    Monitor.Log("Qi's Challenge: Awarding the Burglar's Ring for a new game.", LogLevel.Info);
                }
                // Iridium Band for "Protector Of The Valley"
                if (ModConfig.GetFlagBoolean("areAllMonsterSlayerQuestsComplete"))
                {
                    Game1.player.leftRing.Value = new Ring(527);
                    Game1.player.leftRing.Value.onEquip(Game1.player, Game1.player.currentLocation);
                    Monitor.Log("Protector Of The Valley: Awarding the Iridium Band for a new game.", LogLevel.Info);
                }
                // TODO:
                // Bonus 2 starting hearts with everyone for "Popular"?
                // Statue of Perfection for "Fector's Challenge"?
            }
            //Monitor.Log("Awarding new game stuff - completed.", LogLevel.Trace);
        }

        private void UpgradeTool(string tool)
        {
            Game1.player.getToolFromName(tool).UpgradeLevel++;
            Game1.player.getToolFromName(tool).setNewTileIndexForUpgradeLevel();
        }
        #endregion
    }
}
