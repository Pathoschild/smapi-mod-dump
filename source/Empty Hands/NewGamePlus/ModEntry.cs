/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/quicksilverfox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using NewGamePlus.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace NewGamePlus
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The mod configuration.</summary>
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saved += OnSaved;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }


        /*********
        ** Private methods
        *********/
        #region Events
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.AwardPlayer(); // check if something is missing
            this.SaveAwards(); // save achievements on load, so load & quit is enough to update awards
            this.TravelingMerchantBonus(); // if junimo way is done, travelling merchant always have at least one item required for community center
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaved(object sender, SavedEventArgs e)
        {
            this.SaveAwards(); // When day is saved, save achievements.
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // clean up possible duplicate professions, since we don't filter what player can pick up
            if (e.OldMenu is LevelUpMenu && e.NewMenu == null)
            {
                List<int> cleaned = new List<int>();
                foreach (int profession in Game1.player.professions)
                {
                    if (!cleaned.Contains(profession))
                        cleaned.Add(profession);
                }
                Game1.player.professions.Set(cleaned);
            }
        }

        /// <summary> If player has finished junimo way of refurbishing the Community Center, at future playthroughs Travelling Merchant would always have at least one item fitting for some bundle, if any. </summary>
        // Due to network nature this way does not work anymore.
        private void TravelingMerchantBonus()
        {
            //if (!Config.GetFlagBoolean("ccJunimo") || Game1.player.mailReceived.Contains("JojaMember")) return; // requires full cc completeon record and not being a joja member
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
        private void AwardPlayer()
        {
            if (Game1.stats.DaysPlayed == 0) return; // freshly created game before init cutscene - it would be called once again afterwards

            if (Config.GetConfig("professions")) AwardProfessions();
            if (Config.GetConfig("stardrops")) AwardStardrops();
            if (Config.GetConfig("crafting_recipes")) AwardCraftingRecipes();
            if (Config.GetConfig("cooking_recipes")) AwardCookingRecipes();
            if (Config.GetConfig("experience")) AwardExperience();

            // Some bonuses only can be granted on a new game start. Mostly matherial rewards.
            if (Game1.stats.DaysPlayed == 1)
                AwardNewGame();
        }

        /// <summary>Update known achievements and save them into config.</summary>
        private void SaveAwards()
        {
            SaveProfessions();
            SaveStardrops();
            SaveCraftingRecipes();
            SaveCookingRecipes();
            SaveExperience();

            if (Helper.Reflection.GetMethod((Game1.getLocationFromName("Town") as Town), "checkJojaCompletePrerequisite").Invoke<Boolean>()) Config.SetFlag("ccJoja", true);
            if (Game1.player.hasCompletedCommunityCenter()) Config.SetFlag("ccJunimo", true);

            Config.SetFlagIfGreater("grandpaScore", ((Farm)Game1.getLocationFromName("Farm")).grandpaScore.Get());
            Config.SetFlagIfGreater("money", Game1.player.Money);
            if (Game1.player.achievements.Contains(34)) Config.SetFlag("fullShipment", true);
            if (Game1.player.mailReceived.Contains("QiChallengeComplete")) Config.SetFlag("QiChallengeComplete", true);
            if (AdventureGuild.areAllMonsterSlayerQuestsComplete()) Config.SetFlag("areAllMonsterSlayerQuestsComplete", true);

            Helper.WriteConfig(Config);
        }
        #endregion

        #region Professions
        /// <summary>Iterate over known professions and award ones player does not have yet.</summary>
        private void AwardProfessions()
        {
            foreach (int profession in Config.Professions)
            {
                if (!Game1.player.professions.Contains(profession) &&
                    (profession % 6 > 1
                        || Game1.player.getEffectiveSkillLevel(Professions.GetSkillForProfession(profession)) >= 10
                        || Professions.HasAllProfessionsForSkill(Game1.player, Professions.GetSkillForProfession(profession)))) // only award first professions after second, due to UI limitations
                {
                    Game1.player.professions.Add(profession);

                    // hardcode... technically, this is from LevelUpMenu.getImmediateProfessionPerk
                    if (profession == Farmer.fighter)
                        Game1.player.maxHealth += 15;
                    if (profession == Farmer.defender)
                        Game1.player.maxHealth += 25;

                    Game1.player.health = Game1.player.maxHealth;
                    Monitor.Log("NG+: Awarding profession: " + profession, LogLevel.Info);
                }
            }

        }

        /// <summary>Iterate over current professions and save new ones as known.</summary>
        private void SaveProfessions()
        {
            foreach (int profession in Game1.player.professions)
            {
                if (!Config.Professions.Contains(profession))
                {
                    Config.Professions.Add(profession);
                    Monitor.Log("Saving profession: " + profession, LogLevel.Trace);
                }
            }
        }
        #endregion

        #region Experience
        /// <summary>Iterate over skills and award experience.</summary>
        private void AwardExperience()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Game1.player.experiencePoints[i] < Config.Experience[i])
                {
                    Game1.player.gainExperience(i, Config.Experience[i] - Game1.player.experiencePoints[i]);
                }
            }
        }

        /// <summary>Iterate over current cooking recipes and save new ones as known.</summary>
        private void SaveExperience()
        {
            for (int i = 0; i < 6; i++)
            {
                Config.Experience[i] = Math.Max(Game1.player.experiencePoints[i], Config.Experience[i]);
            }
        }
        #endregion

        #region Stardrops
        /// <summary>List of all game stardrops.</summary>
        private static String[] Stardrops { get; } = new String[] { "CF_Fair", "CF_Fish", "CF_Mines", "CF_Sewer", "museumComplete", "CF_Spouse", "CF_Statue" };

        /// <summary>Give all known stardrops.</summary>
        private void AwardStardrops()
        {
            foreach (String stardrop in Config.Stardrops)
            {
                if (Array.IndexOf(Stardrops, stardrop) != -1 && !Game1.player.hasOrWillReceiveMail(stardrop))
                {
                    Game1.player.mailReceived.Add(stardrop);
                    Game1.player.MaxStamina += 34;
                }
            }

            Game1.player.Stamina = Game1.player.MaxStamina;
        }

        /// <summary>Save known stardrop list.</summary>
        private void SaveStardrops()
        {
            foreach (String stardrop in Stardrops)
            {
                if (Game1.player.mailReceived.Contains(stardrop) && !Config.Stardrops.Contains(stardrop))
                {
                    Config.Stardrops.Add(stardrop);
                }
            }
        }
        #endregion

        #region Blueprints
        /// <summary>Iterate over known crafting recipes and award ones player does not have yet.</summary>
        private void AwardCraftingRecipes()
        {
            foreach (String recipe in Config.CraftingRecipes)
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
            int counter = 0;
            foreach (String recipe in Game1.player.craftingRecipes.Keys)
            {
                if (!Config.CraftingRecipes.Contains(recipe))
                {
                    Config.CraftingRecipes.Add(recipe);
                    counter++;
                }
            }
            if (counter > 0)
                Monitor.Log($"NG+: awarding {counter} crafting recipes.", LogLevel.Info);
        }
        #endregion

        #region Recipes
        /// <summary>Iterate over known cooking recipes and award ones player does not have yet.</summary>
        private void AwardCookingRecipes()
        {
            int counter = 0;
            foreach (String recipe in Config.CookingRecipes)
            {
                if (!Game1.player.cookingRecipes.ContainsKey(recipe))
                {
                    Game1.player.cookingRecipes.Add(recipe, 0);
                    counter++;
                }
            }
            if (counter > 0)
                Monitor.Log($"NG+: awarding {counter} cooking recipes.", LogLevel.Info);
        }

        /// <summary>Iterate over current cooking recipes and save new ones as known.</summary>
        private void SaveCookingRecipes()
        {
            foreach (String recipe in Game1.player.cookingRecipes.Keys)
            {
                if (!Config.CookingRecipes.Contains(recipe))
                {
                    Config.CookingRecipes.Add(recipe);
                }
            }
        }
        #endregion

        #region NewGame
        /// <summary> Give player some starting stuff. Only used for freshly started games. </summary>
        private void AwardNewGame()
        {
            //Monitor.Log("Awarding new game stuff...", LogLevel.Trace);
            if (Config.GetConfig("newgame_tools"))
            {
                if (Config.GetFlagBoolean("ccJunimo"))
                {
                    UpgradeTool("Watering Can");
                    UpgradeTool("Hoe");
                    Monitor.Log("NG+: Local Legend - awarding farming tools upgrade for a new game.", LogLevel.Info);
                }
                if (Config.GetFlagBoolean("ccJoja"))
                {
                    UpgradeTool("Axe");
                    UpgradeTool("Pickaxe");
                    Monitor.Log("NG+: Joja Co. Member Of The Year - awarding gathering tools upgrade for a new game.", LogLevel.Info);
                }
                if (Config.GetFlagBoolean("fullShipment"))
                {
                    Game1.player.increaseBackpackSize(12);
                    Monitor.Log("NG+: Full Shipment - awarding backpack upgrade for a new game.", LogLevel.Info);
                }
            }
            if (Config.GetConfig("newgame_assets"))
            {
                // 0.1% of maximum held money is awarded
                int moneyBonus = decimal.ToInt32(Config.GetFlagDecimal("money") / 1000);
                if (moneyBonus > 0)
                {
                    Game1.player.Money += moneyBonus;
                    Monitor.Log($"NG+: Hoarder - awarding {moneyBonus}g for a new game.", LogLevel.Info);
                }
                if (decimal.ToInt32(Config.GetFlagDecimal("grandpaScore")) == 4)
                {
                    Game1.player.addItemToInventory(new StardewValley.Object(628, 1));
                    Game1.player.addItemToInventory(new StardewValley.Object(629, 1));
                    Game1.player.addItemToInventory(new StardewValley.Object(630, 1));
                    Game1.player.addItemToInventory(new StardewValley.Object(631, 1));
                    Game1.player.addItemToInventory(new StardewValley.Object(632, 1));
                    Game1.player.addItemToInventory(new StardewValley.Object(633, 1));
                    Monitor.Log("NG+: Perfection - awarding a set of fruit tree saplings for a new game.", LogLevel.Info);
                }
                // Burglar's Ring for "Qi's Challenge"
                if (Config.GetFlagBoolean("QiChallengeComplete"))
                {
                    Game1.player.leftRing.Value = new Ring(526); // Burglar's Ring
                    Game1.player.leftRing.Value.onEquip(Game1.player, Game1.player.currentLocation);
                    Monitor.Log("NG+: Qi's Challenge - awarding the Burglar's Ring for a new game.", LogLevel.Info);
                }
                // Iridium Band for "Protector Of The Valley"
                if (Config.GetFlagBoolean("areAllMonsterSlayerQuestsComplete"))
                {
                    Game1.player.leftRing.Value = new Ring(527); // Iridium Band
                    Game1.player.leftRing.Value.onEquip(Game1.player, Game1.player.currentLocation);
                    Monitor.Log("NG+: Protector Of The Valley - awarding the Iridium Band for a new game.", LogLevel.Info);
                }
                // TODO:
                // Bonus 2 starting hearts with everyone for "Popular"?
                // Statue of Perfection for "Fector's Challenge"?
            }
            Monitor.Log("NG+: Awarding new game stuff completed.", LogLevel.Trace);
        }

        private void UpgradeTool(string tool)
        {
            Game1.player.getToolFromName(tool).UpgradeLevel++;
            Game1.player.getToolFromName(tool).setNewTileIndexForUpgradeLevel();
        }
        #endregion
    }
}
