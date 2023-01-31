/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit;
using AtraCore.Framework.DialogueManagement;
using AtraShared;
using AtraShared.Utils.Extensions;
using SpecialOrdersExtended.DataModels;
using StardewModdingAPI.Utilities;

namespace SpecialOrdersExtended.Managers;

/// <summary>
/// Static. Handles logic, patches, and console commands related to the special order dialogues.
/// </summary>
internal class DialogueManager
{
    /// <summary>
    /// Backing field for PerScreened Dialogue Logs.
    /// </summary>
    private static readonly PerScreen<DialogueLog> InternalDialogueLog = new();

    /// <summary>
    /// Gets the current perscreened dialogue log.
    /// </summary>
    public static DialogueLog? PerscreenedDialogueLog
        => InternalDialogueLog.Value;

    /// <summary>
    /// Load the PerScreened Dialogue log.
    /// </summary>
    /// <param name="multiplayerID">The player's unique ID.</param>
    internal static void Load(long multiplayerID)
    {
        ModEntry.ModMonitor.DebugLog($"Loading dialogue log for {multiplayerID:X8}");
        InternalDialogueLog.Value = DialogueLog.Load(multiplayerID);
    }

    /// <summary>
    /// Loads the PerScreened Dialogue log from a temporary file if available.
    /// Else loads from the usual file.
    /// </summary>
    /// <param name="multiplayerID">The player's unique ID.</param>
    internal static void LoadTemp()
    {
        if (Context.IsMainPlayer && Context.IsSplitScreen)
        { // also will need to lad for farmhands.
            foreach (IMultiplayerPeer? multi in ModEntry.MultiplayerHelper.GetConnectedPlayers())
            {
                long multiplayerID = multi.PlayerID;
                int? screenid = multi.ScreenID;
                if (screenid is not null && DialogueLog.LoadTempIfAvailable(multiplayerID) is DialogueLog log)
                {
                    ModEntry.ModMonitor.Log($"Loading temp dialogue log for {multiplayerID:X8}");
                    InternalDialogueLog.SetValueForScreen(screenid.Value, log);
                }
            }
        }
        else
        {
            long multiplayerID = Game1.player.UniqueMultiplayerID;
            if (DialogueLog.LoadTempIfAvailable(multiplayerID) is DialogueLog log)
            {
                ModEntry.ModMonitor.Log($"Loading temp dialogue log for {multiplayerID:X8}");
                InternalDialogueLog.Value = log;
            }
            else
            {
                ModEntry.ModMonitor.Log($"No temp dialogue log found for {multiplayerID:X8}");
            }
        }
    }

    /// <summary>
    /// Save a dialoguelog for a specific player.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    internal static void Save()
    {
        if (PerscreenedDialogueLog is null)
        {
            throw new SaveNotLoadedError();
        }
        PerscreenedDialogueLog.Save();
    }

    /// <summary>
    /// Save temporary Dialogue Log file.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">Save not loaded.</exception>
    internal static void SaveTemp()
    {
        foreach ((int _, DialogueLog log) in InternalDialogueLog.GetActiveValues())
        {
            log.SaveTemp();
        }
    }

    /// <summary>
    /// Handle the console command to add, remove, or check a seen dialogue.
    /// </summary>
    /// <param name="command">Name of console command.</param>
    /// <param name="args">Arguments for the console coomand.</param>
    internal static void ConsoleSpecialOrderDialogue(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            ModEntry.ModMonitor.Log(I18n.LoadSaveFirst(), LogLevel.Warn);
            return;
        }
        if (args[0].Equals("save", StringComparison.OrdinalIgnoreCase))
        {
            foreach ((int _, DialogueLog log) in InternalDialogueLog.GetActiveValues())
            {
                log.Save();
            }
            return;
        }
        if (args.Length < 3)
        {
            ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleError(command), LogLevel.Warn);
            return;
        }
        switch (args[0])
        {
            case "add":
                foreach (string characterName in args[2..])
                {
                    if (TryAddSeenDialogue(args[1], characterName))
                    {
                        ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleAddSuccess(args[1], characterName), LogLevel.Info);
                    }
                    else
                    {
                        ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleAddFailure(args[1], characterName), LogLevel.Info);
                    }
                }
                break;
            case "remove":
                foreach (string characterName in args[2..])
                {
                    if (TryRemoveSeenDialogue(args[1], characterName))
                    {
                        ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleRemoveSuccess(args[1], characterName), LogLevel.Info);
                    }
                    else
                    {
                        ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleRemoveFailure(args[1], characterName), LogLevel.Info);
                    }
                }
                break;
            case "hasseen":
                foreach (string characterName in args[2..])
                {
                    if (HasSeenDialogue(args[1], characterName))
                    {
                        ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleDoesContain(args[1], characterName), LogLevel.Info);
                    }
                    else
                    {
                        ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleDoesNotContain(args[1], characterName), LogLevel.Info);
                    }
                }
                break;
            default:
                ModEntry.ModMonitor.Log(I18n.Dialogue_ConsoleActionInvalid(args[0]), LogLevel.Info);
                break;
        }
    }

    /// <summary>
    /// Whether or not a character has said the particular key.
    /// </summary>
    /// <param name="key">Exact dialogue key.</param>
    /// <param name="characterName">Name of character.</param>
    /// <returns>True if key has been said, false otherwise.</returns>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    internal static bool HasSeenDialogue(string key, string characterName)
    {
        if (!Context.IsWorldReady)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
        }
        return PerscreenedDialogueLog!.Contains(key, characterName);
    }

    /// <summary>
    /// Attempts to add a SeenDialogue.
    /// </summary>
    /// <param name="key">Dialogue key.</param>
    /// <param name="characterName">Character.</param>
    /// <returns>True if added succesfully, false otherwise.</returns>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    internal static bool TryAddSeenDialogue(string key, string characterName)
    {
        if (!Context.IsWorldReady)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
        }
        return PerscreenedDialogueLog!.TryAdd(key, characterName);
    }

    /// <summary>
    /// Attempts to remove a dialogue key from someone's SeenDialogue.
    /// </summary>
    /// <param name="key">Dialogue key.</param>
    /// <param name="characterName">Name of the NPC to check.</param>
    /// <returns>True if successful, false otherwise.</returns>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    internal static bool TryRemoveSeenDialogue(string key, string characterName)
    {
        if (!Context.IsWorldReady)
        {
            ASThrowHelper.ThrowSaveNotLoaded();
        }
        return PerscreenedDialogueLog!.TryRemove(key, characterName);
    }

    /// <summary>
    /// Clear any memory of RepeatOrder keys when needed.
    /// </summary>
    /// <param name="removedKeys">List of keys to remove.</param>
    internal static void ClearRepeated(List<string> removedKeys)
    {
        if (PerscreenedDialogueLog is null)
        {
            return;
        }
        foreach (string key in removedKeys)
        {
            if (key.Contains("_RepeatOrder"))
            {
                foreach (string dialogueKey in PerscreenedDialogueLog.SeenDialogues.Keys)
                {
                    if(dialogueKey.Contains(key))
                    {
                        ModEntry.ModMonitor.DebugOnlyLog($"Removing key {dialogueKey}");
                        PerscreenedDialogueLog.SeenDialogues.Remove(dialogueKey);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Clears dialogue related to a special order if failed.
    /// </summary>
    /// <param name="specialOrderKey">The key for the order failed.</param>
    internal static void ClearOnFail(string specialOrderKey)
    {
        if (PerscreenedDialogueLog is null)
        {
            return;
        }
        foreach (string dialogueKey in PerscreenedDialogueLog.SeenDialogues.Keys)
        {
            if (dialogueKey.Contains(specialOrderKey))
            {
                ModEntry.ModMonitor.DebugOnlyLog($"Removing key {dialogueKey}");
                PerscreenedDialogueLog.SeenDialogues.Remove(dialogueKey);
            }
        }
    }

    /// <summary>
    /// Harmony patch - shows the dialogue for special orders.
    /// </summary>
    /// <param name="__result">Result from the original function.</param>
    /// <param name="__instance">NPC in question.</param>
    /// <param name="__0">NPC heart level.</param>
    /// <param name="__1">NoPreface in vanilla code - to preface with season or not.</param>
    /// <exception cref="UnexpectedEnumValueException{SpecialOrder.QuestState}">Recieved unexpected enum value.</exception>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Convention used by Harmony")]
    internal static void PostfixCheckDialogue(ref bool __result, ref NPC __instance, int __0, bool __1)
    {
        try
        {
            // have already found a New Current Dialogue
            if (__result)
            {
                return;
            }

            // Handle dialogue for orders currently active (no matter their state)
            foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
            {
                if (specialOrder?.questKey is null)
                {
                    continue;
                }

                string baseKey = __1 ? specialOrder.questKey.Value : Game1.currentSeason + specialOrder.questKey.Value;
                baseKey += specialOrder.questState.Value switch
                {
                    SpecialOrder.QuestState.InProgress => "_InProgress",
                    SpecialOrder.QuestState.Failed => "_Failed",
                    SpecialOrder.QuestState.Complete => "_Completed",
                    _ => throw new UnexpectedEnumValueException<SpecialOrder.QuestState>(specialOrder.questState.Value),
                };
                __result = FindBestDialogue(baseKey, __instance, __0);
                if (__result)
                {
                    return;
                }

                // Handle repeat orders!
                if (specialOrder.questState.Value == SpecialOrder.QuestState.InProgress && Game1.player.team.completedSpecialOrders.ContainsKey(specialOrder.questKey.Value))
                {
                    __result = FindBestDialogue((__1 ? specialOrder.questKey.Value : Game1.currentSeason + specialOrder.questKey.Value) + "_RepeatOrder", __instance, __0);
                    if (__result)
                    {
                        return;
                    }
                }
            }

            // Handle dialogue for recently completed special orders.
            if (RecentSOManager.GetKeys(2u) is IEnumerable<string> cacheOrders)
            {
                foreach (string cacheOrder in cacheOrders)
                {
                    __result = FindBestDialogue(cacheOrder + "_Completed", __instance, __0);
                    if (__result)
                    {
                        return;
                    }
                }
            }

            // Handle available order dialogue.
            if (SpecialOrder.IsSpecialOrdersBoardUnlocked())
            {
                HashSet<string> currentOrders = Game1.player.team.specialOrders.Where(s => s?.questKey is not null).Select((SpecialOrder s) => s.questKey.Value).ToHashSet();
                foreach (SpecialOrder specialOrder in Game1.player.team.availableSpecialOrders)
                {
                    if (specialOrder?.questKey is null)
                    {
                        continue;
                    }

                    if (!currentOrders.Contains(specialOrder.questKey.Value))
                    {
                        __result = FindBestDialogue(specialOrder.questKey.Value + "_IsAvailable", __instance, __0);
                        if (__result)
                        {
                            return;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"{I18n.Dialogue_ErrorInPatchedFunction(__instance.Name)}\n{ex}", LogLevel.Error);
        }
    }

    /// <summary>
    /// Checks to see if a dialoguekey has been said already, and if not said, pushes the dialogue
    /// onto the dialogue stack.
    /// </summary>
    /// <param name="dialogueKey">Dialogue key to check.</param>
    /// <param name="npc">NPC that says the dialogue.</param>
    /// <returns>true if a dialogue is successfully pushed, false otherwise.</returns>
    private static bool PushAndSaveDialogue(string dialogueKey, NPC npc)
    {
        if (!TryAddSeenDialogue(dialogueKey, npc.Name))
        {// I have already said this dialogue
            return false;
        }

        QueuedDialogueManager.PushCurrentDialogueToQueue(npc);

        // Push my dialogue onto their stack.
        npc.CurrentDialogue.Push(new Dialogue(npc.Dialogue[dialogueKey], npc) { removeOnNextMove = true });
        if (ModEntry.Config.Verbose)
        {
            ModEntry.ModMonitor.Log(I18n.Dialogue_FoundKey(dialogueKey), LogLevel.Debug);
        }
        return true;
    }

    /// <summary>
    /// Locates the most specific dialogue, given a specific basekey, and pushes it to the NPC's dialogue stack.
    /// </summary>
    /// <param name="baseKey">The base key, without friendship or time modifiers.</param>
    /// <param name="npc">The NPC who is talking.</param>
    /// <param name="hearts">The current heart level of the NPC.</param>
    /// <returns>true if a dialogue was successfully found and pushed, false otherwise.</returns>
    private static bool FindBestDialogue(string baseKey, NPC npc, int hearts)
    {
        string dialogueKey = $"{baseKey}_{Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth)}";
        if (npc.Dialogue.ContainsKey(dialogueKey))
        {
            if (PushAndSaveDialogue(dialogueKey, npc))
            {
                return true;
            }
        }

        for (int heartLevel = Math.Max((hearts / 2) * 2, 0); heartLevel > 1; heartLevel -= 2)
        {
            dialogueKey = $"{baseKey}{heartLevel}";
            if (npc.Dialogue.ContainsKey(dialogueKey))
            {
                return PushAndSaveDialogue(dialogueKey, npc);
            }
        }

        if (npc.Dialogue.ContainsKey(baseKey))
        {
            return PushAndSaveDialogue(baseKey, npc);
        }

        if (ModEntry.Config.Verbose)
        {
            ModEntry.ModMonitor.Log(I18n.Dialogue_NoKey(baseKey, npc.Name), LogLevel.Trace);
        }
        return false;
    }
}
