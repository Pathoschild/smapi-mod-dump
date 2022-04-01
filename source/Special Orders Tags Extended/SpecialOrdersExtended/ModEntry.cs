/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using AtraBase.Toolkit.Reflection;
using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.MigrationManager;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using SpecialOrdersExtended.HarmonyPatches;
using SpecialOrdersExtended.Managers;
using StardewModdingAPI.Events;
using StardewValley.GameData;
using AtraUtils = AtraShared.Utils.Utils;

namespace SpecialOrdersExtended;

/// <inheritdoc />
internal class ModEntry : Mod
{
    /// <summary>
    /// Spacecore API handle.
    /// </summary>
    /// <remarks>If null, means API not loaded.</remarks>
    private static ISpaceCoreAPI? spaceCoreAPI;

    /// <summary>
    /// Gets the Spacecore API instance.
    /// </summary>
    /// <remarks>If null, was not able to be loaded.</remarks>
    internal static ISpaceCoreAPI? SpaceCoreAPI => spaceCoreAPI;

    private MigrationManager? migrator;

    // The following fields are set in the Entry method, which is about as close to the constructor as I can get
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }

    /// <summary>
    /// Gets SMAPI's data helper for this mod.
    /// </summary>
    internal static IDataHelper DataHelper { get; private set; }

    /// <summary>
    /// Gets the config class for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private static Func<string, bool>? CheckTagDelegate { get; set; } = null;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        // Bind useful SMAPI features.
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        DataHelper = helper.Data;

        // Read config file.
        try
        {
            Config = this.Helper.ReadConfig<ModConfig>();
        }
        catch
        {
            this.Monitor.Log(I18n.IllFormatedConfig(), LogLevel.Warn);
            Config = new();
        }
        Harmony harmony = new(this.ModManifest.UniqueID);

        harmony.Patch(
            original: typeof(SpecialOrder).StaticMethodNamed("CheckTag"),
            prefix: new HarmonyMethod(typeof(TagManager), nameof(TagManager.PrefixCheckTag)));

        harmony.Patch(
            original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.GetSpecialOrder)),
            finalizer: new HarmonyMethod(typeof(Finalizers), nameof(Finalizers.FinalizeGetSpecialOrder)));

        try
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkForNewCurrentDialogue)),
                postfix: new HarmonyMethod(typeof(DialogueManager), nameof(DialogueManager.PostfixCheckDialogue)));
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Failed to patch NPC::checkForNewCurrentDialogue for Special Orders Dialogue. Dialogue will be disabled\n\n{ex}", LogLevel.Error);
        }

        harmony.Snitch(this.Monitor, this.ModManifest.UniqueID);

        // Register console commands.
        helper.ConsoleCommands.Add(
            name: "special_order_pool",
            documentation: I18n.SpecialOrderPool_Description(),
            callback: this.GetAvailableOrders);
        helper.ConsoleCommands.Add(
            name: "check_tag",
            documentation: I18n.CheckTag_Description(),
            callback: this.ConsoleCheckTag);
        helper.ConsoleCommands.Add(
            name: "list_available_stats",
            documentation: I18n.ListAvailableStats_Description(),
            callback: StatsManager.ConsoleListProperties);
        helper.ConsoleCommands.Add(
            name: "special_orders_dialogue",
            documentation: $"{I18n.SpecialOrdersDialogue_Description()}\n\n{I18n.SpecialOrdersDialogue_Example()}\n    {I18n.SpecialOrdersDialogue_Usage()}\n    {I18n.SpecialOrdersDialogue_Save()}",
            callback: DialogueManager.ConsoleSpecialOrderDialogue);

        // Register event handlers.
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
        helper.Events.GameLoop.Saving += this.Saving;
        helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
        helper.Events.GameLoop.OneSecondUpdateTicking += this.OneSecondUpdateTicking;
    }

    /// <summary>
    /// Raised every 10 in game minutes.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">TimeChanged params.</param>
    /// <remarks>Currently handles: pushing delayed dialogue back onto the stack.</remarks>
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
    {
        DialogueManager.PushPossibleDelayedDialogues();
    }

    /// <summary>
    /// Raised every second.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">OneSecondUpdate params.</param>
    /// <remarks>Currently handles: grabbing new recently completed special orders.</remarks>
    private void OneSecondUpdateTicking(object? sender, OneSecondUpdateTickingEventArgs e)
    {
        RecentSOManager.GrabNewRecentlyCompletedOrders();
    }

    /// <summary>
    /// Raised on game launch.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">Game Launched arguments.</param>
    /// <remarks>Used to bind APIs and register CP tokens.</remarks>
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // Get a delegate on SpecialOrder.CheckTag.
        CheckTagDelegate = typeof(SpecialOrder).StaticMethodNamed("CheckTag").CreateDelegate<Func<string, bool>>();

        // Bind Spacecore API
        IntegrationHelper helper = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, LogLevel.Debug);
        helper.TryGetAPI("spacechase0.SpaceCore", "1.5.10", out spaceCoreAPI);

        if (helper.TryGetAPI("Pathoschild.ContentPatcher", "1.20.0", out IContentPatcherAPI? api))
        {
            api.RegisterToken(this.ModManifest, "Current", new Tokens.CurrentSpecialOrders());
            api.RegisterToken(this.ModManifest, "Available", new Tokens.AvailableSpecialOrders());
            api.RegisterToken(this.ModManifest, "Completed", new Tokens.CompletedSpecialOrders());
            api.RegisterToken(this.ModManifest, "CurrentRules", new Tokens.CurrentSpecialOrderRule());
            api.RegisterToken(this.ModManifest, "RecentCompleted", new Tokens.RecentCompletedSO());
        }
    }

    /// <summary>
    /// Raised right before the game is saved.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">Event arguments.</param>
    /// <remarks>Used to handle day-end events.</remarks>
    private void Saving(object? sender, SavingEventArgs e)
    {
        this.Monitor.DebugLog("Event Saving raised");

        DialogueManager.Save(); // Save dialogue
        DialogueManager.ClearDelayedDialogue();

        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {// Some properties only make sense for a single player to handle in splitscreen.
            return;
        }

        TagManager.ResetRandom();
        StatsManager.ClearProperties(); // clear property cache, repopulate at next use
        RecentSOManager.GrabNewRecentlyCompletedOrders();
        RecentSOManager.DayUpdate(Game1.stats.daysPlayed);
        RecentSOManager.Save();
    }

    /// <summary>
    /// Raised when save is loaded.
    /// </summary>
    /// <param name="sender">Unknown, used by SMAPI.</param>
    /// <param name="e">Parameters.</param>
    /// <remarks>Used to load in this mod's data models.</remarks>
    private void SaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.Monitor.DebugLog("Event SaveLoaded raised");
        DialogueManager.Load(Game1.player.UniqueMultiplayerID);
        MultiplayerHelpers.AssertMultiplayerVersions(this.Helper.Multiplayer, this.ModManifest, this.Monitor, this.Helper.Translation);

        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {
            return;
        }
        this.migrator = new(this.ModManifest, this.Helper, this.Monitor);
        this.migrator.ReadVersionInfo();

        this.Helper.Events.GameLoop.Saved += this.WriteMigrationData;
        RecentSOManager.Load();
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
    /// Console commands to check the value of a tag.
    /// </summary>
    /// <param name="command">Name of the command.</param>
    /// <param name="args">List of tags to check.</param>
    private void ConsoleCheckTag(string command, string[] args)
    {
        if (!Context.IsWorldReady || CheckTagDelegate is null)
        {
            ModMonitor.Log(I18n.LoadSaveFirst(), LogLevel.Debug);
            return;
        }
        foreach (string tag in args)
        {
            string base_tag;
            bool match = true;
            if (tag.StartsWith("!"))
            {
                match = false;
                base_tag = tag.Trim()[1..];
            }
            else
            {
                base_tag = tag.Trim();
            }
            ModMonitor.Log($"{tag}: {(match == CheckTagDelegate(base_tag) ? I18n.True() : I18n.False())}", LogLevel.Debug);
        }
    }

    /// <summary>
    /// Console command to get all available orders.
    /// </summary>
    /// <param name="command">Name of command.</param>
    /// <param name="args">Arguments for command.</param>
    private void GetAvailableOrders(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModMonitor.Log(I18n.LoadSaveFirst(), LogLevel.Warn);
        }
        Dictionary<string, SpecialOrderData> order_data = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
        List<string> keys = AtraUtils.ContextSort(order_data.Keys);
        ModMonitor.Log(I18n.NumberFound(count: keys.Count), LogLevel.Debug);

        List<string> validkeys = new();
        List<string> unseenkeys = new();

        foreach (string key in keys)
        {
            SpecialOrderData order = order_data[key];
            if (this.IsAvailableOrder(key, order))
            {
#if DEBUG
                ModMonitor.DebugLog($"    {key} is valid");
#endif
                validkeys.Add(key);
                if (!Game1.MasterPlayer.team.completedSpecialOrders.ContainsKey(key))
                {
                    unseenkeys.Add(key);
                }
            }
        }
        ModMonitor.Log($"{I18n.ValidKeys(count: validkeys.Count)}: {string.Join(", ", validkeys)}", LogLevel.Debug);
        ModMonitor.Log($"{I18n.UnseenKeys(count: unseenkeys.Count)}: {string.Join(", ", unseenkeys)}", LogLevel.Debug);
    }

    private bool IsAvailableOrder(string key, SpecialOrderData order)
    {
        ModMonitor.Log($"{I18n.Analyzing()} {key}", LogLevel.Debug);
        try
        {
            SpecialOrder.GetSpecialOrder(key, Game1.random.Next());
            ModMonitor.Log($"    {key} {I18n.Parsable()}", LogLevel.Debug);
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"    {key} {I18n.Unparsable()}\n{ex}", LogLevel.Error);
            return false;
        }

        bool seen = Game1.MasterPlayer.team.completedSpecialOrders.ContainsKey(key);
        if (order.Repeatable != "True" && seen)
        {
            ModMonitor.Log($"    {I18n.Nonrepeatable()}", LogLevel.Debug);
            return false;
        }
        else if (seen)
        {
            ModMonitor.Log($"    {I18n.RepeatableSeen()}", LogLevel.Debug);
        }
        if (Game1.dayOfMonth >= 16 && order.Duration == "Month")
        {
            ModMonitor.Log($"    {I18n.MonthLongLate(cutoff: 16)}");
            return false;
        }
        if (!SpecialOrder.CheckTags(order.RequiredTags))
        {
            ModMonitor.Log($"    {I18n.HasInvalidTags()}:", LogLevel.Debug);
            string[] tags = order.RequiredTags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (string tag in tags)
            {
                bool match = true;
                if (tag.Length == 0)
                {
                    continue;
                }
                string trimmed_tag;
                if (tag.StartsWith("!"))
                {
                    match = false;
                    trimmed_tag = tag[1..];
                }
                else
                {
                    trimmed_tag = tag;
                }

                if (!(CheckTagDelegate?.Invoke(trimmed_tag) == match))
                {
                    ModMonitor.Log($"         {I18n.TagFailed()}: {tag}", LogLevel.Debug);
                }
            }
            return false;
        }
        foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
        {
            if (specialOrder.questKey.Value == key)
            {
                ModMonitor.Log($"    {I18n.Active()}", LogLevel.Debug);
                return false;
            }
        }
        return true;
    }
}
