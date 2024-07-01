/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enteligenz/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
//using QuestFramework.Api;
//using QuestFramework.Quests;
//using QuestFramework;
using System.IO;

namespace FoodCravings
{
    internal sealed class ModEntry : Mod
    {
        Random rnd = new Random();
        string DailyCravingKey;
        string DailyCravingName;
        bool CravingFulfilled;
        Buff cravingBuff;
        Buff cravingDebuff;
        //Dictionary<string, string> recipeDict = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
        private ModConfig Config;
        bool isHangryMode;
        string[] recipeBlacklist;

        //IQuestApi api;
        //IManagedQuestApi managedApi;
        //CustomQuest quest;

        string fulfilledText, unfulfilledText, HUDText;//, questDescription, questObjective;


        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.isHangryMode = this.Config.useHangryMode;
            this.recipeBlacklist = this.Config.recipeBlacklist;

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            // Read translations
            // I18n.Init(helper.Translation); // ??
            this.fulfilledText = helper.Translation.Get("fulfilled");
            this.unfulfilledText = helper.Translation.Get("unfulfilled");
            this.HUDText = helper.Translation.Get("hud-msg");
            //this.questDescription = helper.Translation.Get("quest-desc");
            //this.questObjective = helper.Translation.Get("quest-obj");

            this.cravingBuff = new Buff(
                id: "Hexenentendrache.FoodCravings_Buff",
                displayName: this.fulfilledText,
                iconTexture: this.Helper.ModContent.Load<Texture2D>("assets/food_craving_icon.png"),
                duration: 540000,
                effects: new StardewValley.Buffs.BuffEffects()
                { // TODO change values if necessary
                    Attack = { this.Config.attackBuff },
                    Defense = { this.Config.defenseBuff },
                    Speed = { this.Config.speedBuff }
                }
            );

            this.cravingDebuff = new Buff(
                id: "Hexenentendrache.FoodCravings_Debuff",
                displayName: this.unfulfilledText,
                iconTexture: this.Helper.ModContent.Load<Texture2D>("assets/food_craving_debuff_icon.png"),
                duration: Buff.ENDLESS,
                effects: new StardewValley.Buffs.BuffEffects()
                { // TODO change values if necessary
                    Attack = { this.Config.attackDebuff },
                    Defense = { this.Config.defenseDebuff },
                    Speed = { this.Config.speedDebuff }
                }
            );

            helper.Events.GameLoop.GameLaunched += this.OnGameStarted;
        }


        private void OnGameStarted(object sender, GameLaunchedEventArgs e)
        {
            //bool isLoaded = this.Helper.ModRegistry.IsLoaded("PurrplingCat.QuestFramework");
            //this.api = this.Helper.ModRegistry.GetApi<IQuestApi>("PurrplingCat.QuestFramework");
            //this.managedApi = api.GetManagedApi(this.ModManifest);

            //this.api.Events.GettingReady += (_sender, _e) => {
                //this.quest = new CustomQuest();
                //this.quest.Name = "food_craving";
                //this.quest.BaseType = QuestType.Basic;
                //this.quest.Title = "Food Craving";
                //this.managedApi.RegisterQuest(this.quest);
            //};
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Get list of all known recipes
            List<string> knownRecipes = Game1.player.cookingRecipes.Keys.ToList();

            // Randomize food craving until non-blacklisted food is found
            if (this.Config.seededRandom)
            {
                this.rnd = new Random(Game1.Date.ToString().GetHashCode());
            }
            while (true)
            {
                this.DailyCravingKey = knownRecipes.ElementAt(this.rnd.Next(0, knownRecipes.Count));

                // Find the proper display name of the food
                this.DailyCravingName = this.DailyCravingKey; // For vanilla food (and some older mods) the key name will be the same as the display name
                if (CraftingRecipe.cookingRecipes.TryGetValue(this.DailyCravingKey, out string recipe))
                {
                    string[] recipeParts = recipe.Split('/');
                    if (recipeParts.Length == 6) // afaik modded food will follow this format, where the last part of the recipe is the name we want
                    {
                        this.DailyCravingName = recipeParts[5]; // Modded food might use i18n format as key, so we need to replace it with sth more readable
                    }
                }

                if (!this.recipeBlacklist.Contains(this.DailyCravingName))
                {
                    break;
                }
            }
            
            foreach (string rec in this.recipeBlacklist)
            {
                this.Monitor.Log($"recipe blacklist: {rec}.", LogLevel.Debug);
            }

            // Add quest for craving into quest tab
            Game1.addHUDMessage(new HUDMessage(this.HUDText + this.DailyCravingName, 2));
            //if (!this.CravingFulfilled) this.managedApi.CompleteQuest("food_craving"); // Remove old food craving quest in case it was not fulfilled
            //this.quest.Description = this.questDescription + this.DailyCravingName + "...";
            //this.quest.Objective = this.questObjective + this.DailyCravingName + ".";
            //this.managedApi.AcceptQuest("food_craving", true);

            // Reset flag (buffs seem to automatically reset on next day)
            this.CravingFulfilled = false;

            // Apply craving debuff if necessary
            if (this.isHangryMode)
            {
                // Game1.buffsDisplay.addOtherBuff(this.cravingDebuff);
                Game1.player.applyBuff(this.cravingDebuff);
            }
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (!Game1.player.isEating || this.CravingFulfilled) // Player is not eating or craving has already been fulfilled before
            {
                return;
            }

            Item CurrentFood = Game1.player.itemToEat;

            if (!this.DailyCravingKey.Equals(CurrentFood.Name)) // Player is eating food that is not craved
            {
                return;
            }

            this.CravingFulfilled = true;
            //this.managedApi.CompleteQuest("food_craving"); // Mark quest for craving as completed

            // Game1.buffsDisplay.addOtherBuff(this.cravingBuff); // Add buff for fulfilled craving
            Game1.player.applyBuff(this.cravingBuff);
            if (this.isHangryMode)
            {
                    Game1.player.buffs.Remove("Hexenentendrache.FoodCravings_Debuff"); // Remove debuff
            }
        }
    }
}