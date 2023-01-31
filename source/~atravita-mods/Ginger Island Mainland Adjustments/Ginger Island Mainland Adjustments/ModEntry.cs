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
using AtraShared.Menuing;
using AtraShared.MigrationManager;
using AtraShared.Utils.Extensions;
using GingerIslandMainlandAdjustments.AssetManagers;
using GingerIslandMainlandAdjustments.CustomConsoleCommands;
using GingerIslandMainlandAdjustments.DialogueChanges;
using GingerIslandMainlandAdjustments.Integrations;
using GingerIslandMainlandAdjustments.MultiplayerHandler;
using GingerIslandMainlandAdjustments.Niceties;
using GingerIslandMainlandAdjustments.ScheduleManager;
using HarmonyLib;
using StardewModdingAPI.Events;

namespace GingerIslandMainlandAdjustments;

/// <inheritdoc />
[UsedImplicitly]
internal sealed class ModEntry : Mod
{
    private bool haveFixedSchedulesToday = false;

    private MigrationManager? migrator;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        Globals.Initialize(helper, this.Monitor, this.ModManifest);
        AssetEditor.Initialize(helper.GameContent);
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");

        ConsoleCommands.Register(this.Helper.ConsoleCommands);

        // Register events
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        helper.Events.GameLoop.DayEnding += this.OnDayEnding;
        helper.Events.GameLoop.ReturnedToTitle += this.ReturnedToTitle;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.Player.Warped += this.OnPlayerWarped;

        helper.Events.Multiplayer.PeerConnected += this.PeerConnected;
        helper.Events.Multiplayer.ModMessageReceived += this.ModMessageReceived;

        helper.Events.Content.AssetRequested += this.OnAssetRequested;

        AssetLoader.Init(helper.GameContent);
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        AssetLoader.Load(e);
        AssetEditor.Edit(e);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, Globals.Manifest, Globals.ModMonitor, this.Helper.Translation);
        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }

        GenerateGMCM.BuildNPCDictionary();

        Globals.LoadDataFromSave();

        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        if (!this.migrator.CheckVersionInfo())
        {
            this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        }
        else
        {
            this.migrator = null;
        }
    }

    /// <summary>
    /// Writes migration data then detaches the migrator.
    /// </summary>
    /// <param name="sender">Smapi thing.</param>
    /// <param name="e">Arguments for just-before-saving.</param>
    private void WriteMigrationData(object? sender, SavedEventArgs e)
    {
        if (this.migrator is not null)
        {
            this.migrator.SaveVersionInfo();
            this.migrator = null;
        }
        this.Helper.Events.GameLoop.Saved -= this.WriteMigrationData;
    }

    /// <summary>
    /// Clear all caches at the end of the day and if the player exits to menu.
    /// </summary>
    private void ClearCaches()
    {
        DialoguePatches.ClearTalkRecord();
        DialogueUtilities.ClearDialogueLog();

        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }
        this.haveFixedSchedulesToday = false;
        MidDayScheduleEditor.Reset();
        IslandSouthPatches.ClearCache();
        GIScheduler.ClearCache();
        GIScheduler.DayEndReset();
        ConsoleCommands.ClearCache();
        ScheduleUtilities.ClearCache();
    }

    /// <summary>
    /// Clear caches when returning to title.
    /// </summary>
    /// <param name="sender">Unknown, never used.</param>
    /// <param name="e">Possible parameters.</param>
    private void ReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        this.ClearCaches();
        GenerateGMCM.Build();
    }

    /// <summary>
    /// Clear cache at day end.
    /// </summary>
    /// <param name="sender">Unknown, never used.</param>
    /// <param name="e">Possible parameters.</param>
    private void OnDayEnding(object? sender, DayEndingEventArgs e)
    {
        this.ClearCaches();
        NPCPatches.ResetAllFishers();

        if (Context.IsMainPlayer)
        {
            Game1.netWorldState.Value.IslandVisitors.Clear();
            Globals.SaveCustomData();
        }
    }

    /// <summary>
    /// Applies and logs this mod's harmony patches.
    /// </summary>
    /// <param name="harmony">My harmony instance.</param>
    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            // handle patches from annotations.
            harmony.PatchAll(typeof(ModEntry).Assembly);
            if (Globals.Config.DebugMode)
            {
                ScheduleDebugPatches.ApplyPatches(harmony);
            }
        }
        catch (Exception ex)
        {
            Globals.ModMonitor.Log($"{I18n.HarmonyCrash()} {Globals.GithubLocation}{Environment.NewLine}{ex}", LogLevel.Error);
        }

        harmony.Snitch(Globals.ModMonitor, this.ModManifest.UniqueID, transpilersOnly: true);
    }

    /// <summary>
    /// Initialization after other mods have started.
    /// </summary>
    /// <param name="sender">Unknown, never used.</param>
    /// <param name="e">Possible parameters.</param>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // Applies harmony patches.
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));

        // Generate the GMCM for this mod.
        GenerateGMCM.Initialize(this.ModManifest, this.Helper.Translation);
        GenerateGMCM.Build();

        // Add CP tokens for this mod.
        GenerateCPTokens.AddTokens(this.ModManifest);

        // Bind Child2NPC's IsChildNPC method
        if (Globals.GetIsChildToNPC())
        {
            Globals.ModMonitor.Log("Successfully grabbed Child2NPC for integration", LogLevel.Debug);
        }
    }

    private void OnPlayerWarped(object? sender, WarpedEventArgs e)
        => ShopHandler.AddBoxToShop(e);

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (e.Button.IsActionButton() && MenuingExtensions.IsNormalGameplay())
        {
            ShopHandler.HandleWillyShop(e);
            ShopHandler.HandleSandyShop(e);
        }
    }

    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }
        MidDayScheduleEditor.AttemptAdjustGISchedule(e);
        if (!this.haveFixedSchedulesToday && e.NewTime > 615)
        {
            // No longer need the exclusions cache.
            IslandSouthPatches.ClearCache();

            ScheduleUtilities.FixUpSchedules();
            if (Globals.Config.DebugMode)
            {
                ScheduleDebugPatches.FixNPCs();
            }
            this.haveFixedSchedulesToday = true;
        }
    }

    private void PeerConnected(object? sender, PeerConnectedEventArgs e)
        => MultiplayerSharedState.ReSendMultiplayerMessage(e);

    private void ModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        => MultiplayerSharedState.UpdateFromMessage(e);
}
