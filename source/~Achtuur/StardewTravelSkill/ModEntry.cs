/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using SpaceCore;
using System.Runtime.CompilerServices;
using HarmonyLib;
using StardewTravelSkill.Patches;
using StardewValley.Menus;
using ContentPatcher.Framework;
using ContentPatcher;
using AchtuurCore.Patches;

namespace StardewTravelSkill
{
    internal sealed class ModEntry : Mod
    {
        public static Mod Instance;
        public static IContentPatcherAPI ContentAPI;
        public static TravelSkill TravelSkill;

        public ContentPackHelper contentPackHelper;
        public ModConfig Config;

        /// <summary>
        /// Whether player is currently under effects of sprint profession
        /// </summary>
        public static bool SprintActive { get; set; }

        /// <summary>
        /// Amount of steps taken the previous time it was checked
        /// </summary>
        private uint m_previousSteps;

        /// <summary>
        /// Amount of steps taken since last moving
        /// </summary>
        private uint m_consecutiveSteps;

        /// <summary>
        /// Whether totem recipe has been changed as result of profession
        /// </summary>
        private bool totemRecipeChanged;

        /// <summary>
        /// Whether obelisk recipe has been changed as result of profession
        /// </summary>
        private bool obeliskRecipeChanged;

        /// <summary>
        /// Returns movespeed multiplier farmer should receive
        /// </summary>
        /// <returns>
        /// <c>
        ///     <see cref="TravelSkill.LevelMovespeedBonus"/> * level + [<see cref="ModConfig.MovespeedProfessionBonus"/>] + [<see cref="ModConfig.SprintMovespeedBonus"/>]
        /// </c>
        /// </returns>
        public static float GetMovespeedMultiplier()
        {
            float professionbonus = Game1.player.HasCustomProfession(TravelSkill.ProfessionMovespeed) ? ModConfig.MovespeedProfessionBonus : 0.0f;
            float sprintbonus = SprintActive ? ModConfig.SprintMovespeedBonus : 0.0f;

            float multiplier = Game1.player.GetCustomSkillLevel(TravelSkill) * ModConfig.LevelMovespeedBonus + professionbonus + sprintbonus;
            return 1 + multiplier;
        }

        public static double GetWarpTotemConsumeChance()
        {
            return Game1.player.HasCustomProfession(TravelSkill.ProfessionTotemReuse)
                ? ModConfig.TotemUseChance
                : 1.0;
        }

        /// <summary>
        /// Returns stamina that should be restored every 10 minutes
        /// </summary>
        /// <returns></returns>
        public static double GetStaminaRestoreAmount()
        {
            return Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina)
                ? ModConfig.RestoreStaminaPercentage
                : 0.0;
        }

        /// <summary>
        /// Mod entry point, called after mod is first loaded
        /// </summary>
        /// <param name="helper">Simplified API for writing mods</param>
        public override void Entry(IModHelper helper)
        {
            HarmonyPatcher.ApplyPatches(this,
                new MoveSpeedPatch(),
                new ReduceActiveItemPatch()
            );

            // Init references to mod api
            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            this.Config = helper.ReadConfig<ModConfig>();

            Skills.RegisterSkill(ModEntry.TravelSkill = new TravelSkill());


            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.SaveCreated += this.OnSaveCreate;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoad;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.Input.ButtonReleased += this.OnButtonReleased;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += this.OnDayStart;

            ConsoleCommands.Initialize(helper);
        }


        /*********
        ** Event Listeners
        *********/
        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            // Can only assign content api on game launch as it takes a few ticks before api is available
            this.contentPackHelper = new ContentPackHelper(this);
            this.contentPackHelper.CreateTokens();

            this.Config.createMenu(this);
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.player.HasCustomProfession(TravelSkill.ProfessionSprint))
                CheckSprintActive();
        }

        private void OnSaveCreate(object sender, EventArgs e)
        {
            InitValueTrackers();
        }

        private void OnSaveLoad(object sender, EventArgs e)
        {
            InitValueTrackers();
            registerProfessionAssetEvents();
        }

        private void OnDayStart(object sender, DayStartedEventArgs e)
        {
            registerProfessionAssetEvents();
        }

        /// <summary>
        /// On button release, check the amount of steps taken and increase EXP based on that
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            // Exit early if no world loaded
            if (!Context.IsWorldReady)
                return;

            // Exit early if button pressed was not a movement command
            if (e.IsSuppressed() || !this.isMovementButton(e.Button))
                return;

            // Calcuate difference in steps, and if it exceeds 1 exp treshold, add it as exp. Hacky fix to get xp values between 0 and 1
            uint step_diff = Game1.player.stats.stepsTaken - this.m_previousSteps;
            if (step_diff > ModConfig.StepsPerExp)
            {
                Game1.player.AddCustomSkillExperience(TravelSkill, 1);
                // Set previous steps to current steps, with correction
                this.m_previousSteps = Game1.player.stats.stepsTaken + ((uint)ModConfig.StepsPerExp - step_diff);
            }

        }

        /// <summary>
        /// When time changes in game, restore a little bit of stamina
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimeChanged(object sender, EventArgs e)
        {
            if (Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina))
            {
                Game1.player.stamina += Game1.player.MaxStamina * ModConfig.RestoreStaminaPercentage;
            }
        }

        /*********
        ** Helper functions
        *********/

        /// <summary>
        /// Initialise variables that are used to track certain values, this should be called only at start of world load.
        /// </summary>
        private void InitValueTrackers()
        {
            if (!Context.IsWorldReady)
                return;

            this.m_previousSteps = Game1.player.stats.stepsTaken;
            this.totemRecipeChanged = false;
            this.obeliskRecipeChanged = false;
        }

        
        /// <summary>
        /// Register events for AssetRequested for the obelisk and totem recipe professions
        /// </summary>
        private void registerProfessionAssetEvents()
        {
            if (!Context.IsWorldReady)
                return;

            if (!totemRecipeChanged && Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem))
            {
                Instance.Helper.Events.Content.AssetRequested += updateTotemRecipe;
                Instance.Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
            }
            
            if (!obeliskRecipeChanged && Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapObelisk))
            {
                Instance.Helper.Events.Content.AssetRequested += updateObeliskRecipe;
                Instance.Helper.GameContent.InvalidateCache("Data/Blueprints");
            }
        }

        /// <summary>
        /// Updates totem recipe based on profession, listens to <see cref="IContentEvents.AssetRequested"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateTotemRecipe(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes") || !Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem))
                return;

            
            e.Edit(asset =>
            {
                IDictionary<string, string> assetDict = asset.AsDictionary<string, string>().Data;
                // Farm totem to 5 wood, 5 hay, 10 fiber
                ChangeTotemRecipeInDict(assetDict, "Warp Totem: Farm", "388 5 178 5 771 10");
                Instance.Monitor.Log("[StardewTravelSkill] Farm totem recipe updated!", LogLevel.Trace);

                // Moutain totem to 5 wood, 1 copper bar, 10 stone
                ChangeTotemRecipeInDict(assetDict, "Warp Totem: Mountains", "388 5 334 1 390 10");
                Instance.Monitor.Log("[StardewTravelSkill] Mountains totem recipe updated!", LogLevel.Trace);

                // Desert totem to 10 wood, coconut, 1 gold bar
                ChangeTotemRecipeInDict(assetDict, "Warp Totem: Desert", "388 10 88 1 336 1");
                Instance.Monitor.Log("[StardewTravelSkill] Desert totem recipe updated!", LogLevel.Trace);

                // Beach totem to 5 wood, 5 fiber, any 2 fish
                ChangeTotemRecipeInDict(assetDict, "Warp Totem: Beach", "388 5 771 5 -4 2");
                Instance.Monitor.Log("[StardewTravelSkill] Beach totem recipe updated!", LogLevel.Trace);
            });
            
            // Unsubscribe this method so asset isn't needlessly updated again
            totemRecipeChanged = true;
            Instance.Helper.Events.Content.AssetRequested -= updateTotemRecipe;
        }

        /// <summary>
        /// Update obelisk recipe based on profession, listens to <see cref="IContentEvents.AssetRequested"/>
        /// </summary>
        /// <param name="sender"><inheritdoc/></param>
        /// <param name="e"><inheritdoc/></param>
        private void updateObeliskRecipe(object sender, AssetRequestedEventArgs e)
        {
            if (!e.NameWithoutLocale.IsEquivalentTo("Data/Blueprints") || !Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapObelisk))
                return;

            e.Edit(asset =>
            {
                IDictionary<string, string> assetDict = asset.AsDictionary<string, string>().Data;

                ChangeObeliskCostInDict(assetDict, "Earth Obelisk", "250000");
                Instance.Monitor.Log("[StardewTravelSkill] Earth Obelisk cost changed!", LogLevel.Trace);

                ChangeObeliskCostInDict(assetDict, "Water Obelisk", "250000");
                Instance.Monitor.Log("[StardewTravelSkill] Water Obelisk cost changed!", LogLevel.Trace);

                ChangeObeliskCostInDict(assetDict, "Desert Obelisk", "500000");
                Instance.Monitor.Log("[StardewTravelSkill] Desert Obelisk cost changed!", LogLevel.Trace);

                ChangeObeliskCostInDict(assetDict, "Island Obelisk", "500000");
                Instance.Monitor.Log("[StardewTravelSkill] Island Obelisk cost changed!", LogLevel.Trace);
            });

            obeliskRecipeChanged = true;
            Instance.Helper.Events.Content.AssetRequested -= updateObeliskRecipe;
        }

        /// <summary>
        /// Returns true if <paramref name="button"/> is a movement control button
        /// </summary>
        /// <param name="button">Button to check</param>
        private bool isMovementButton(SButton button)
        {
            InputButton b_equiv;
            button.TryGetStardewInput(out b_equiv);
            return Game1.options.moveUpButton.Any(b => b.Equals(b_equiv)) ||
                Game1.options.moveDownButton.Any(b => b.Equals(b_equiv)) ||
                Game1.options.moveLeftButton.Any(b => b.Equals(b_equiv)) ||
                Game1.options.moveRightButton.Any(b => b.Equals(b_equiv));
        }

        /// <summary>
        /// Returns true if a button corresponding to movement is held
        /// </summary>
        private bool MovementButtonHeld()
        {
            return ButtonHeld(SButton.W) || ButtonHeld(SButton.S) || ButtonHeld(SButton.A) || ButtonHeld(SButton.D);
        }

        /// <summary>
        /// Checks whether <paramref name="button"/> is held this tick
        /// </summary>
        /// <param name="button">Button to check</param> 
        private bool ButtonHeld(SButton button)
        {
            SButtonState state = this.Helper.Input.GetState(button);
            return state == SButtonState.Held || state == SButtonState.Pressed;
        }

        /// <summary>
        /// <para>Counts consecutive steps and activates sprint while walking and having at least x consecutive steps.</para>
        /// <para>Should be called every tick</para>
        /// </summary>
        private void CheckSprintActive()
        {
            if (!this.MovementButtonHeld())
            {
                // "Reset" counter by setting it to current step count
                this.m_consecutiveSteps = Game1.player.stats.stepsTaken;
                ModEntry.SprintActive = false;
                return;
            }

            uint step_diff = Game1.player.stats.stepsTaken - this.m_consecutiveSteps;
            
            if (step_diff > ModConfig.SprintSteps && !ModEntry.SprintActive)
            {
                this.Monitor.Log("Now sprinting", LogLevel.Debug);
                ModEntry.SprintActive = true;
            }
        }

        private void ChangeTotemRecipeInDict(IDictionary<string, string> dict, string affected_item, string new_recipe)
        {
            var dictEntrySplit = dict[affected_item].Split('/');
            dictEntrySplit[0] = new_recipe;
            dict[affected_item] = String.Join<string>("/", dictEntrySplit);
        }

        private void ChangeObeliskCostInDict(IDictionary<string, string> dict, string affected_item, string new_cost)
        {
            var dictEntrySplit = dict[affected_item].Split('/');
            dictEntrySplit[17] = new_cost;
            dict[affected_item] = String.Join<string>("/", dictEntrySplit);
        }
    }
}
