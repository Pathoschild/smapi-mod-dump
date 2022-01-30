/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using System.Reflection;
using HarmonyLib;

using StardewValley.GameData;

namespace SpecialOrdersExtended;

/// <inheritdoc />
internal class ModEntry : Mod
{
    // The following fields are set in the Entry method, which is about as close to the constructor as I can get
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Logger for SMAPI.
    /// </summary>
    private static IMonitor modMonitor;

    /// <summary>
    /// SMAPI's data writer.
    /// </summary>
    private static IDataHelper dataHelper;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor => modMonitor;

    /// <summary>
    /// Gets SMAPI's data helper for this mod.
    /// </summary>
    internal static IDataHelper DataHelper => dataHelper;

    /// <inheritdoc/>
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        modMonitor = this.Monitor;
        dataHelper = helper.Data;

        Harmony harmony = new(this.ModManifest.UniqueID);

        harmony.Patch(
            original: typeof(SpecialOrder).GetMethod("CheckTag", BindingFlags.NonPublic | BindingFlags.Static),
            prefix: new HarmonyMethod(typeof(TagManager), nameof(TagManager.PrefixCheckTag)));
        ModMonitor.Log("Patching SpecialOrder::CheckTag for Special Orders Extended", LogLevel.Trace);

        try
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkForNewCurrentDialogue)),
                postfix: new HarmonyMethod(typeof(DialogueManager), nameof(DialogueManager.PostfixCheckDialogue)));
            ModMonitor.Log("Patching NPC::checkForNewCurrentDialogue for Special Orders Dialogue", LogLevel.Trace);
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Failed to patch NPC::checkForNewCurrentDialogue for Special Orders Dialogue. Dialogue will be disabled\n\n{ex}", LogLevel.Error);
        }

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
            documentation: $"{I18n.SpecialOrdersDialogue_Description()}\n\n{I18n.SpecialOrdersDialogue_Example()}\n    {I18n.SpecialOrdersDialogue_Usage()}",
            callback: DialogueManager.ConsoleSpecialOrderDialogue);
        helper.Events.GameLoop.GameLaunched += this.RegisterTokens;
        helper.Events.GameLoop.SaveLoaded += this.SaveLoaded;
        helper.Events.GameLoop.Saving += this.Saving;
        helper.Events.GameLoop.OneSecondUpdateTicking += this.OneSecondUpdateTicking;
    }

    private void OneSecondUpdateTicking(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickingEventArgs e)
    {
        RecentSOManager.GrabNewRecentlyCompletedOrders();
    }

    private void RegisterTokens(object? sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
    {
        Tokens.IContentPatcherAPI? api = this.Helper.ModRegistry.GetApi<Tokens.IContentPatcherAPI>("Pathoschild.ContentPatcher");
        if (api is null)
        {
            ModMonitor.Log(I18n.CpNotInstalled(), LogLevel.Warn);
            return;
        }

        api.RegisterToken(this.ModManifest, "Current", new Tokens.CurrentSpecialOrders());
        api.RegisterToken(this.ModManifest, "Available", new Tokens.AvailableSpecialOrders());
        api.RegisterToken(this.ModManifest, "Completed", new Tokens.CompletedSpecialOrders());
        api.RegisterToken(this.ModManifest, "CurrentRules", new Tokens.CurrentSpecialOrderRule());
        api.RegisterToken(this.ModManifest, "RecentCompleted", new Tokens.RecentCompletedSO());
    }

    private void Saving(object? sender, StardewModdingAPI.Events.SavingEventArgs e)
    {
        this.Monitor.DebugLog("Event Saving raised");

        DialogueManager.Save(); // Save dialogue

        if (Context.IsSplitScreen && Context.ScreenId != 0)
        {// Some properties only make sense for a single player to handle in splitscreen.
            return;
        }

        StatsManager.ClearProperties(); // clear property cache, repopulate at next use
        RecentSOManager.GrabNewRecentlyCompletedOrders();
        RecentSOManager.DayUpdate(Game1.stats.daysPlayed);
        RecentSOManager.Save();
    }

    private void SaveLoaded(object? sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
    {
        this.Monitor.DebugLog("Event SaveLoaded raised");
        DialogueManager.Load(Game1.player.UniqueMultiplayerID);
        RecentSOManager.Load();
    }

    private void ConsoleCheckTag(string command, string[] args)
    {
        if (!Context.IsWorldReady)
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
            bool result = match == this.Helper.Reflection.GetMethod(typeof(SpecialOrder), "CheckTag").Invoke<bool>(base_tag);
            ModMonitor.Log($"{tag}: {(result ? I18n.True() : I18n.False())}", LogLevel.Debug);
        }
    }

    private void GetAvailableOrders(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModMonitor.Log(I18n.LoadSaveFirst(), LogLevel.Warn);
        }
        Dictionary<string, SpecialOrderData> order_data = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
        List<string> keys = Utilities.ContextSort(order_data.Keys);
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

                if (!(match == this.Helper.Reflection.GetMethod(typeof(SpecialOrder), "CheckTag").Invoke<bool>(trimmed_tag)))
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
