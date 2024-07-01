/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore;
using AchtuurCore.Patches;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewTravelSkill.Patches;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewTravelSkill;

internal sealed class ModEntry : Mod
{
    internal static ModEntry Instance;
    public TravelSkill travelSkill;

    public ContentPackHelper contentPackHelper;
    public ModConfig Config;

    /// <summary>
    /// Whether player is currently under effects of sprint profession
    /// </summary>
    public PerScreen<bool> SprintActive { get; set; }

    /// <summary>
    /// Amount of steps taken the previous time it was checked
    /// 
    /// Uses PerScreen for splitscreen gameplay
    /// </summary>
    private readonly PerScreen<uint> m_previousSteps = new PerScreen<uint>();

    /// <summary>
    /// Amount of steps taken since last moving
    /// 
    /// Uses PerScreen for splitscreen gameplay
    /// </summary>
    private PerScreen<uint> m_consecutiveSteps = new PerScreen<uint>();

    private PerScreen<float> m_AccumulatedExp = new PerScreen<float>();

    private PerScreen<int> m_LastWalkTick = new PerScreen<int>();

    /// <summary>
    /// Whether totem recipe has been changed as result of profession
    /// </summary>
    private bool totemRecipeChanged;

    /// <summary>
    /// Whether obelisk recipe has been changed as result of profession
    /// </summary>
    private bool obeliskRecipeChanged;

    /// <summary>
    /// Returns true if the player is currently walking
    /// </summary>
    /// <returns></returns>
    public static bool IsWalking()
    {
        return Game1.player.movementDirections.Count > 0;
    }

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
        if (!Context.IsWorldReady)
            return 1.0f;

        float professionbonus = Game1.player.HasCustomProfession(TravelSkill.ProfessionMovespeed) ? Instance.Config.MovespeedProfessionBonus : 0.0f;
        float sprintbonus = (ModEntry.Instance.SprintActive.Value) ? Instance.Config.SprintMovespeedBonus : 0.0f;

        float multiplier = Game1.player.GetCustomSkillLevel(ModEntry.Instance.travelSkill) * Instance.Config.LevelMovespeedBonus + professionbonus + sprintbonus;
        return 1 + multiplier;
    }

    public static double GetWarpTotemConsumeChance()
    {
        return Game1.player.HasCustomProfession(TravelSkill.ProfessionTotemReuse)
            ? Instance.Config.TotemUseChance
            : 1.0;
    }

    /// <summary>
    /// Returns stamina that should be restored every 10 minutes
    /// </summary>
    /// <returns></returns>
    public static double GetStaminaRestoreAmount()
    {
        return Game1.player.HasCustomProfession(TravelSkill.ProfessionRestoreStamina)
            ? Instance.Config.RestoreStaminaPercentage
            : 0.0;
    }

    /// <summary>
    /// Mod entry point, called after mod is first loaded
    /// </summary>
    /// <param name="helper">Simplified API for writing mods</param>
    public override void Entry(IModHelper helper)
    {
        // Init references to mod api
        I18n.Init(helper.Translation);
        ModEntry.Instance = this;

        // Apply harmony patches
        HarmonyPatcher.ApplyPatches(this,
            new MoveSpeedPatch(),
            new ReduceActiveItemPatch()
        );

        // Setup config
        this.Config = helper.ReadConfig<ModConfig>();


        // Setup Spacecore skill
        this.travelSkill = new TravelSkill();
        Skills.RegisterSkill(this.travelSkill);
        this.SprintActive = new PerScreen<bool>();


        // Setup event listeners
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        helper.Events.GameLoop.SaveCreated += this.OnSaveLoad;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoad;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
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

        this.Config.createMenu();
    }

    private void OnUpdateTicked(object sender, EventArgs e)
    {
        if (!Context.IsWorldReady || !Context.IsPlayerFree)
            return;

        if (IsWalking())
            m_LastWalkTick.Value = Game1.ticks;

        CheckSprintActive();

        UpdateExp();
    }

    private void OnSaveLoad(object sender, EventArgs e)
    {
        InitValueTrackers();
        RegisterProfessionAssetEvents();
    }

    private void OnDayStart(object sender, DayStartedEventArgs e)
    {
        RegisterProfessionAssetEvents();
    }
    private void UpdateExp()
    {
        uint step_diff = Game1.player.stats.StepsTaken - this.m_previousSteps.Value;
        // If steps are less than the number of steps than the increment, early return
        if (step_diff < Config.StepsPerExp * Config.ExpGainStepThreshold)
            return;

        m_AccumulatedExp.Value += (float)step_diff / (float)Config.StepsPerExp;
        int exp_to_add = (int)m_AccumulatedExp.Value; // take integer part of exp
        m_AccumulatedExp.Value -= exp_to_add;
        Game1.player.AddCustomSkillExperience(travelSkill, exp_to_add);

        m_previousSteps.Value = Game1.player.stats.StepsTaken;
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
            float new_stamina = Game1.player.stamina + Game1.player.MaxStamina * Instance.Config.RestoreStaminaPercentage;
            Game1.player.stamina = Math.Min(new_stamina, Game1.player.MaxStamina);
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

        this.m_previousSteps.Value = Game1.player.stats.StepsTaken;
        this.totemRecipeChanged = false;
        this.obeliskRecipeChanged = false;
    }


    /// <summary>
    /// Register events for AssetRequested for the obelisk and totem recipe professions
    /// </summary>
    private void RegisterProfessionAssetEvents()
    {
        if (!Context.IsWorldReady)
            return;

        if (!totemRecipeChanged && Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem))
        {
            Instance.Helper.Events.Content.AssetRequested += UpdateTotemRecipe;
            Instance.Helper.GameContent.InvalidateCache("Data/CraftingRecipes");
        }

        if (!obeliskRecipeChanged && Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapObelisk))
        {
            Instance.Helper.Events.Content.AssetRequested += UpdateObeliskRecipe;
            Instance.Helper.GameContent.InvalidateCache("Data/Blueprints");
        }
    }

    /// <summary>
    /// Updates totem recipe based on profession, listens to <see cref="IContentEvents.AssetRequested"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateTotemRecipe(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/CraftingRecipes") || !Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapWarpTotem))
            return;


        e.Edit(asset =>
        {
            IDictionary<string, string> assetDict = asset.AsDictionary<string, string>().Data;
            // Farm totem to 5 wood, 5 hay, 10 fiber
            ChangeTotemRecipeInDict(assetDict, "Warp Totem: Farm", "388 5 178 5 771 10");
            Instance.Monitor.Log("Farm totem recipe updated!", LogLevel.Trace);

            // Moutain totem to 5 wood, 1 copper bar, 10 stone
            ChangeTotemRecipeInDict(assetDict, "Warp Totem: Mountains", "388 5 334 1 390 10");
            Instance.Monitor.Log("Mountains totem recipe updated!", LogLevel.Trace);

            // Desert totem to 10 wood, coconut, 1 gold bar
            ChangeTotemRecipeInDict(assetDict, "Warp Totem: Desert", "388 10 88 1 336 1");
            Instance.Monitor.Log("Desert totem recipe updated!", LogLevel.Trace);

            // Beach totem to 5 wood, 5 fiber, any 2 fish
            ChangeTotemRecipeInDict(assetDict, "Warp Totem: Beach", "388 5 771 5 -4 2");
            Instance.Monitor.Log("Beach totem recipe updated!", LogLevel.Trace);
        });

        // Unsubscribe this method so asset isn't needlessly updated again
        totemRecipeChanged = true;
        Instance.Helper.Events.Content.AssetRequested -= UpdateTotemRecipe;
    }

    /// <summary>
    /// Update obelisk recipe based on profession, listens to <see cref="IContentEvents.AssetRequested"/>
    /// </summary>
    /// <param name="sender"><inheritdoc/></param>
    /// <param name="e"><inheritdoc/></param>
    private void UpdateObeliskRecipe(object sender, AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.IsEquivalentTo("Data/Blueprints") || !Game1.player.HasCustomProfession(TravelSkill.ProfessionCheapObelisk))
            return;

        e.Edit(asset =>
        {
            IDictionary<string, string> assetDict = asset.AsDictionary<string, string>().Data;

            ChangeObeliskCostInDict(assetDict, "Earth Obelisk", "250000");
            Instance.Monitor.Log("Earth Obelisk cost changed!", LogLevel.Trace);

            ChangeObeliskCostInDict(assetDict, "Water Obelisk", "250000");
            Instance.Monitor.Log("Water Obelisk cost changed!", LogLevel.Trace);

            ChangeObeliskCostInDict(assetDict, "Desert Obelisk", "500000");
            Instance.Monitor.Log("Desert Obelisk cost changed!", LogLevel.Trace);

            ChangeObeliskCostInDict(assetDict, "Island Obelisk", "500000");
            Instance.Monitor.Log("Island Obelisk cost changed!", LogLevel.Trace);
        });

        obeliskRecipeChanged = true;
        Instance.Helper.Events.Content.AssetRequested -= UpdateObeliskRecipe;
    }

    /// <summary>
    /// Returns true if the player is walking or has walked within the last 10 ticks
    /// </summary>
    /// <returns></returns>
    public bool IsWalkingWithMargin()
    {
        return IsWalking() || Game1.ticks - m_LastWalkTick.Value < 20;
    }

    /// <summary>
    /// <para>Counts consecutive steps and activates sprint while walking and having at least x consecutive steps.</para>
    /// <para>Should be called every tick</para>
    /// </summary>
    private void CheckSprintActive()
    {
        if (!Game1.player.HasCustomProfession(TravelSkill.ProfessionSprint))
            return;

        if (!IsWalkingWithMargin() && this.SprintActive.Value)
        {
            // "Reset" counter by setting it to current step count
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, "No longer sprinting");
            this.m_consecutiveSteps.Value = Game1.player.stats.StepsTaken;
            this.SprintActive.Value = false;
            return;
        }

        uint step_diff = Game1.player.stats.StepsTaken - this.m_consecutiveSteps.Value;

        if (step_diff > Instance.Config.SprintSteps && !this.SprintActive.Value)
        {
            AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, "Now sprinting");
            this.SprintActive.Value = true;
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

    private int GetScreenId()
    {
        if (!Context.IsSplitScreen)
            return 0;

        return Context.ScreenId;
    }
}
