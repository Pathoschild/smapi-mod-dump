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

using CritterRings.Models;

using HarmonyLib;

using StardewModdingAPI.Events;

using StardewValley.BellsAndWhistles;

using AtraUtils = AtraShared.Utils.Utils;

namespace CritterRings;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private static IJsonAssetsAPI? jsonAssets;
    private MigrationManager? migrator;

    private const string SAVEKEY = "item_ids";

    internal static ModConfig Config { get; private set; } = null!;

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

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

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
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

        GMCMHelper gmcmHelper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (gmcmHelper.TryGetAPI())
        {
            gmcmHelper.Register(
                reset: static () => Config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, Config))
            .AddParagraph(I18n.Mod_Description)
            .GenerateDefaultGMCM(static () => Config);
        }
    }

    /// <inheritdoc cref="IPlayerEvents.Warped"/>
    private void OnWarp(object? sender, WarpedEventArgs e)
    {
        if (!e.IsLocalPlayer)
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
                for (int i = 0; i < 3; i++)
                {
                    critters.Add(new Firefly(Game1.player.getTileLocation()));
                }
            }
        }
        else if (Config.ButterfliesSpawnInRain || !Game1.IsRainingHere(Game1.currentLocation))
        {
            if (ButterflyRing > 0 && Game1.player.isWearingRing(ButterflyRing))
            {
                for (int i = 0; i < 3; i++)
                {
                    critters.Add(new Butterfly(Game1.player.getTileLocation(), Game1.random.Next(2) == 0).setStayInbounds(true));
                }
            }
        }
    }

    /// <inheritdoc cref="IGameLoopEvents.TimeChanged"/>
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        Game1.currentLocation?.instantiateCrittersList();
        if (Game1.currentLocation?.critters is not List<Critter> critters)
        {
            return;
        }
        if (Game1.isDarkOut())
        {
            if (FireFlyRing > 0)
            {
                int count = Game1.player.GetEffectsOfRingMultiplier(FireFlyRing);
                for (int i = 0; i < count; i++)
                {
                    critters.Add(new Firefly(Game1.player.getTileLocation()));
                }
            }
        }
        else if (Config.ButterfliesSpawnInRain || !Game1.IsRainingHere(Game1.currentLocation))
        {
            if (ButterflyRing > 0)
            {
                int count = Game1.player.GetEffectsOfRingMultiplier(ButterflyRing);
                for (int i = 0; i < count; i++)
                {
                    critters.Add(new Butterfly(Game1.player.getTileLocation(), Game1.random.Next(2) == 0).setStayInbounds(true));
                }
            }
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
        butterflyRing = -1;
        fireflyRing = -1;
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

    #region migration

    /// <inheritdoc cref="IGameLoopEvents.Saving"/>
    private void OnSaving(object? sender, SavingEventArgs e)
    {
        this.Helper.Events.GameLoop.Saving -= this.OnSaving;
        if (Context.IsMainPlayer)
        {
            DataModel data = this.Helper.Data.ReadSaveData<DataModel>(SAVEKEY) ?? new();
            bool changed = false;

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
