/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Models.Result;

using AtraShared.Utils.Extensions;

using HarmonyLib;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Fixes the secret note spawning code.
/// </summary>
[HarmonyPatch(typeof(GameLocation))]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
internal static class FixSecretNotes
{
    // we're doing this as an internal optimization and intentionally not saving it in ModData
    // so it's refreshed every time the game is launched.
    private static readonly PerScreen<bool> HasSeenAllSecretNotes = new(() => false);
    private static readonly PerScreen<bool> HasSeenAllJournalScraps = new(() => false);

    #region asset fussing

    private static IAssetName noteLoc = null!;

    /// <summary>
    /// Initializes the IAssetNames.
    /// </summary>
    /// <param name="parser">Game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
        => noteLoc = parser.ParseAssetName("Data/SecretNotes");

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if (assets is null || assets.Contains(noteLoc))
        {
            HasSeenAllSecretNotes.ResetAllScreens();
            HasSeenAllJournalScraps.ResetAllScreens();
        }
    }

    #endregion

    #region override vanilla note creation

    [HarmonyPatch(nameof(GameLocation.tryToCreateUnseenSecretNote))]
    private static bool Prefix(GameLocation __instance, Farmer who, ref SObject? __result)
    {
        if (!ModEntry.Config.OverrideSecretNotes)
        {
            return true;
        }

        try
        {
            __result = null;
            Option<SObject?> option = __instance.GetLocationContext() == GameLocation.LocationContext.Island
                       ? TryGenerateJournalScrap(who)
                       : TryGenerateSecretNote(who);

            if (option.IsNone)
            {
                return true;
            }
            else
            {
                __result = option.Unwrap_Or_Default();
                return false;
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while trying to generate a secret note:\n\n{ex}", LogLevel.Error);
        }

        return true;
    }

    private static Option<SObject?> TryGenerateSecretNote(Farmer who)
    {
        if (!who.hasMagnifyingGlass || HasSeenAllSecretNotes.Value)
        {
            return Option<SObject?>.None;
        }

        const string secreteNoteName = "Secret Note #";
        Dictionary<int, string> secretNoteData = Game1.content.Load<Dictionary<int, string>>(noteLoc.BaseName);

        // Get list of seen notes and add notes in inventory.
        HashSet<int> seenNotes = who.secretNotesSeen.Where(id => id < GameLocation.JOURNAL_INDEX).ToHashSet();
        foreach (Item? item in who.Items)
        {
            if (item.Name.StartsWith(secreteNoteName) && int.TryParse(item.Name.AsSpan(secreteNoteName.Length).Trim(), out int idx))
            {
                seenNotes.Add(idx);
            }
        }

        // find a note that the farmer has not seen.
        HashSet<int> unseenNotes = secretNoteData.Keys.Where(id => id < GameLocation.JOURNAL_INDEX && !seenNotes.Contains(id)).ToHashSet();

        ModEntry.ModMonitor.DebugOnlyLog($"{unseenNotes.Count} notes unseen: {string.Join(", ", unseenNotes.Select(x => x.ToString()))}", LogLevel.Info);
        if (unseenNotes.Count == 0)
        {
            HasSeenAllSecretNotes.Value = true;
            return Option<SObject?>.None;
        }

        // copied from game code.
        double fractionOfNotesRemaining = (unseenNotes.Count - 1) / Math.Max(1f, unseenNotes.Count + seenNotes.Count - 1);
        double chanceForNewNote = ModEntry.Config.MinNoteChance + ((ModEntry.Config.MaxNoteChance - ModEntry.Config.MinNoteChance) * fractionOfNotesRemaining);
        if (Game1.random.NextDouble() >= chanceForNewNote)
        {
            return new(null);
        }

        int noteID = unseenNotes.ElementAt(Game1.random.Next(unseenNotes.Count));
        SObject note = new(79, 1);
        note.Name += " #" + noteID;

        return new(note);
    }

    private static Option<SObject?> TryGenerateJournalScrap(Farmer who)
    {
        if (HasSeenAllJournalScraps.Value)
        {
            return Option<SObject?>.None;
        }

        const string journalName = "Journal Scrap #";
        Dictionary<int, string> secretNoteData = Game1.content.Load<Dictionary<int, string>>(noteLoc.BaseName);

        // get seen notes and add any note the farmer has in their inventory.
        HashSet<int> seenScraps = who.secretNotesSeen.Where(id => id >= GameLocation.JOURNAL_INDEX).ToHashSet();
        foreach (Item? item in who.Items)
        {
            if (item.Name.StartsWith(journalName) && int.TryParse(item.Name.AsSpan(journalName.Length).Trim(), out int idx))
            {
                seenScraps.Add(idx + GameLocation.JOURNAL_INDEX);
            }
        }

        // find a scrap that the farmer has not seen.
        HashSet<int> unseenScraps = secretNoteData.Keys.Where(id => id >= GameLocation.JOURNAL_INDEX && !seenScraps.Contains(id)).ToHashSet();

        ModEntry.ModMonitor.DebugOnlyLog($"{unseenScraps.Count} scraps unseen: {string.Join(", ", unseenScraps.Select(x => x.ToString()))}", LogLevel.Info);
        if (unseenScraps.Count == 0)
        {
            HasSeenAllJournalScraps.Value = true;
            return Option<SObject?>.None;
        }

        // copied from game code.
        double fractionOfNotesRemaining = (unseenScraps.Count - 1) / Math.Max(1f, unseenScraps.Count + seenScraps.Count - 1);
        double chanceForNewNote = ModEntry.Config.MinNoteChance + ((ModEntry.Config.MaxNoteChance - ModEntry.Config.MinNoteChance) * fractionOfNotesRemaining);
        if (Game1.random.NextDouble() >= chanceForNewNote)
        {
            return new (null);
        }

        int scrapID = unseenScraps.Min();
        SObject note = new(842, 1);
        note.Name += " #" + (scrapID - GameLocation.JOURNAL_INDEX);

        return new(note);
    }

    #endregion
}
