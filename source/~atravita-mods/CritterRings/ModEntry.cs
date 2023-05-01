/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore.Utilities;

using AtraShared.ConstantsAndEnums;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;

using CritterRings.Framework;
using CritterRings.Framework.Managers;
using CritterRings.Models;

using HarmonyLib;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

using StardewValley.BellsAndWhistles;
using StardewValley.Locations;

using AtraUtils = AtraShared.Utils.Utils;

namespace CritterRings;

/// <inheritdoc />
[HarmonyPatch]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Reviewed.")]
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// A buff corresponding to the bunny ring.
    /// </summary>
    internal const int BunnyBuffId = 2731247;

    private const string SAVEKEY = "item_ids";

    private static IJsonAssetsAPI? jsonAssets;
    private MigrationManager? migrator;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    internal static ICameraAPI? cameraAPI { get; private set; } = null;

    #region managers

    private static readonly PerScreen<BunnySpawnManager?> BunnyManagers = new(() => null);

    private static readonly PerScreen<JumpManager?> JumpManagers = new(() => null);

    /// <summary>
    /// Gets a reference to the current jumpManager, if applicable.
    /// </summary>
    internal static JumpManager? CurrentJumper => JumpManagers.Value;
    #endregion

    #region JA ids

    private static int bunnyRing = -1;

    /// <summary>
    /// Gets the integer Id of the Bunny Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int BunnyRing
    {
        get
        {
            if (bunnyRing == -1)
            {
                bunnyRing = jsonAssets?.GetObjectId("atravita.BunnyRing") ?? -1;
            }
            return bunnyRing;
        }
    }

    private static int butterflyRing = -1;

    /// <summary>
    /// Gets the integer Id of the Butterfly Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int ButterflyRing
    {
        get
        {
            if (butterflyRing == -1)
            {
                butterflyRing = jsonAssets?.GetObjectId("atravita.ButterflyRing") ?? -1;
            }
            return butterflyRing;
        }
    }

    private static int fireflyRing = -1;

    /// <summary>
    /// Gets the integer Id of the FireFly Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FireFlyRing
    {
        get
        {
            if (fireflyRing == -1)
            {
                fireflyRing = jsonAssets?.GetObjectId("atravita.FireFlyRing") ?? -1;
            }
            return fireflyRing;
        }
    }

    private static int frogRing = -1;

    /// <summary>
    /// Gets the integer Id of the Frog Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int FrogRing
    {
        get
        {
            if (frogRing == -1)
            {
                frogRing = jsonAssets?.GetObjectId("atravita.FrogRing") ?? -1;
            }
            return frogRing;
        }
    }

    private static int owlRing = -1;

    /// <summary>
    /// Gets the integer Id of the Owl Ring. -1 if not found/not loaded yet.
    /// </summary>
    internal static int OwlRing
    {
        get
        {
            if (owlRing == -1)
            {
                owlRing = jsonAssets?.GetObjectId("atravita.OwlRing") ?? -1;
            }
            return owlRing;
        }
    }

    #endregion

    #region initialization

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
        AssetManager.Initialize(helper.GameContent);
    }

    /// <inheritdoc cref="IGameLoopEvents.GameLaunched"/>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Warn);
            if (!helper.TryGetAPI("spacechase0.JsonAssets", "1.10.3", out jsonAssets))
            {
                this.Monitor.Log("Packs could not be loaded! This mod will probably not function.", LogLevel.Error);
                return;
            }
            jsonAssets.LoadAssets(Path.Combine(this.Helper.DirectoryPath, "assets", "json-assets"), this.Helper.Translation);
        }

        this.Helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        this.Helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        this.Helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        this.Helper.Events.Player.Warped += this.OnWarp;
        this.Helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;

        this.Helper.Events.Content.AssetRequested += static (_, e) => AssetManager.Apply(e);
        this.Helper.Events.Content.AssetsInvalidated += static (_, e) => AssetManager.Reset(e.NamesWithoutLocale);

        JumpManager.Initialize(this.Helper.ModRegistry);

        GMCMHelper gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (gmcmHelper.TryGetAPI())
        {
            gmcmHelper.Register(
                reset: static () => Config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.Mod_Description)
            .GenerateDefaultGMCM(static () => Config);
        }

        {
            IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Trace);
            if (helper.TryGetAPI("atravita.CameraPan", "0.1.1", out ICameraAPI? api))
            {
                cameraAPI = api;
            }
        }
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            // handle patches from annotations.
            harmony.PatchAll(typeof(ModEntry).Assembly);
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }

        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    #endregion

    /// <inheritdoc cref="IInputEvents.ButtonsChanged"/>
    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }
        if (!Game1.player.UsingTool && !Game1.player.isEating && Game1.player.yJumpOffset == 0
            && Config.MaxFrogJumpDistance > 0 && Config.FrogRingButton.Keybinds.FirstOrDefault(k => k.GetState() == SButtonState.Pressed) is Keybind keybind
            && FrogRing > 0 && Game1.player.isWearingRing(FrogRing))
        {
            if (JumpManagers.Value?.IsValid(out _) == true)
            {
                ModMonitor.Log($"Jump already in progress for this player, skipping.");
            }
            else if (Game1.player.isRidingHorse())
            {
                Game1.showRedMessage(I18n.FrogRing_Horse());
            }
            else if (Game1.player.exhausted.Value || (Game1.player.Stamina < Config.MaxFrogJumpDistance && Config.JumpCostsStamina))
            {
                Game1.showRedMessage(I18n.BunnyBuff_Tired());
            }
            else
            {
                JumpManagers.Value?.Dispose();
                JumpManagers.Value = new(Game1.player, this.Helper.Events.GameLoop, this.Helper.Events.Display, keybind);
            }
        }

        if (Config.BunnyRingBoost > 0 && Config.BunnyRingButton.JustPressed() && BunnyRing > 0
            && Game1.player.isWearingRing(BunnyRing) && !Game1.player.hasBuff(BunnyBuffId))
        {
            if (Game1.player.Stamina >= Config.BunnyRingStamina && !Game1.player.exhausted.Value)
            {
                Buff buff = BuffEnum.Speed.GetBuffOf(Config.BunnyRingBoost, 20, "atravita.BunnyRing", I18n.BunnyRing_Name());
                buff.which = BunnyBuffId;
                buff.description = I18n.BunnyBuff_Description(Config.BunnyRingBoost);
                buff.sheetIndex = 1;

                Game1.buffsDisplay.addOtherBuff(buff);
                Game1.player.Stamina -= Config.BunnyRingStamina;
            }
            else
            {
                Game1.showRedMessage(I18n.BunnyBuff_Tired());
            }
        }
    }

    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    [EventPriority(EventPriority.Low)]
    private void OnWarp(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
        {
            return;
        }

        // forcibly end the jump if the player was in one.
        JumpManagers.Value?.Dispose();
        JumpManagers.Value = null;

        if (Config.CritterSpawnMultiplier == 0)
        {
            return;
        }

        e.NewLocation?.instantiateCrittersList();
        if (e.NewLocation?.critters is not List<Critter> critters)
        {
            return;
        }
        if (Game1.isDarkOut())
        {
            if (FireFlyRing > 0 && Game1.player.isWearingRing(FireFlyRing))
            {
                CRUtils.SpawnFirefly(critters, 5);
            }
        }
        else if (e.NewLocation.ShouldSpawnButterflies() && ButterflyRing > 0 && Game1.player.isWearingRing(ButterflyRing))
        {
            CRUtils.SpawnButterfly(critters, 3);
        }
        if (BunnyRing > 0 && e.NewLocation is not Caldera && Game1.player.isWearingRing(BunnyRing))
        {
            if (BunnyManagers.Value?.IsValid() == false)
            {
                BunnyManagers.Value.Dispose();
                BunnyManagers.Value = null;
            }
            BunnyManagers.Value ??= new(this.Monitor, Game1.player, this.Helper.Events.Player);
            CRUtils.AddBunnies(critters, 5, BunnyManagers.Value.GetTrackedBushes());
        }
        if (FrogRing > 0 && Game1.player.isWearingRing(FrogRing) && e.NewLocation.ShouldSpawnFrogs())
        {
            CRUtils.SpawnFrogs(e.NewLocation, critters, 5);
        }

        if (OwlRing > 0 && Game1.player.isWearingRing(OwlRing) && e.NewLocation.ShouldSpawnOwls())
        {
            CRUtils.SpawnOwls(e.NewLocation, critters, 1);
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (Config.CritterSpawnMultiplier == 0)
        {
            return;
        }
        Game1.currentLocation?.instantiateCrittersList();
        if (Game1.currentLocation?.critters is not List<Critter> critters)
        {
            return;
        }
        if (Game1.isDarkOut())
        {
            if (FireFlyRing > 0)
            {
                CRUtils.SpawnFirefly(critters, Game1.player.GetEffectsOfRingMultiplier(FireFlyRing));
            }
        }
        else if (ButterflyRing > 0 && Game1.currentLocation.ShouldSpawnButterflies())
        {
            CRUtils.SpawnButterfly(critters, Game1.player.GetEffectsOfRingMultiplier(ButterflyRing));
        }
        if (FrogRing > 0 && Game1.currentLocation.ShouldSpawnFrogs())
        {
            CRUtils.SpawnFrogs(Game1.currentLocation, critters, Game1.player.GetEffectsOfRingMultiplier(FrogRing));
        }

        if (OwlRing > 0 && Game1.player.isWearingRing(OwlRing) && Game1.currentLocation.ShouldSpawnOwls())
        {
            CRUtils.SpawnOwls(Game1.currentLocation, critters, Game1.player.GetEffectsOfRingMultiplier(OwlRing));
        }

        if (BunnyRing > 0 && Game1.currentLocation is not Caldera)
        {
            if (BunnyManagers.Value?.IsValid() == false)
            {
                BunnyManagers.Value.Dispose();
                BunnyManagers.Value = null;
            }
            BunnyManagers.Value ??= new(this.Monitor, Game1.player, this.Helper.Events.Player);
            CRUtils.AddBunnies(critters, Game1.player.GetEffectsOfRingMultiplier(BunnyRing), BunnyManagers.Value.GetTrackedBushes());
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);

        this.migrator = new (this.ModManifest, this.Helper, this.Monitor);
        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        }
        else
        {
            this.migrator = null;
        }

        if (Context.IsMainPlayer)
        {
            // hook event to save Ids so future migrations are possible.
            this.Helper.Events.GameLoop.Saving -= this.OnSaving;
            this.Helper.Events.GameLoop.Saving += this.OnSaving;
        }
    }

    /// <summary>
    /// Resets the IDs when returning to the title.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">Event args.</param>
    [EventPriority(EventPriority.High)]
    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        // reset JA ids. No clue why but JA does this.
        bunnyRing = -1;
        butterflyRing = -1;
        fireflyRing = -1;
        frogRing = -1;
        owlRing = -1;

        // reset and yeet managers.
        foreach ((_, JumpManager? value) in JumpManagers.GetActiveValues())
        {
            value?.Dispose();
        }
        JumpManagers.ResetAllScreens();
        foreach ((_, BunnySpawnManager? value) in BunnyManagers.GetActiveValues())
        {
            value?.Dispose();
        }
        BunnyManagers.ResetAllScreens();
    }

    #region migration

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
        if (Context.IsMainPlayer)
        {
            DataModel data = this.Helper.Data.ReadSaveData<DataModel>(SAVEKEY) ?? new();
            bool changed = false;

            if (data.BunnyRing != BunnyRing)
            {
                data.BunnyRing = BunnyRing;
                changed = true;
            }

            if (data.ButterflyRing != ButterflyRing)
            {
                data.ButterflyRing = ButterflyRing;
                changed = true;
            }

            if (data.FireFlyRing != FireFlyRing)
            {
                data.FireFlyRing = FireFlyRing;
                changed = true;
            }

            if (data.FrogRing != FrogRing)
            {
                data.FrogRing = FrogRing;
                changed = true;
            }

            if (data.OwlRing != OwlRing)
            {
                data.OwlRing = OwlRing;
                changed = true;
            }

            if (changed)
            {
                ModMonitor.Log("Writing ids into save.");
                this.Helper.Data.WriteSaveData("item_ids", data);
            }
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.Saved"/>
    /// <remarks>
    /// Writes migration data then detaches the migrator.
    /// </remarks>
    private void WriteMigrationData(object? sender, SavedEventArgs e)
    {
        if (this.migrator is not null)
        {
            this.migrator.SaveVersionInfo();
            this.migrator = null;
        }

        this.Helper.Events.GameLoop.Saved -= this.WriteMigrationData;
    }
    #endregion
}
