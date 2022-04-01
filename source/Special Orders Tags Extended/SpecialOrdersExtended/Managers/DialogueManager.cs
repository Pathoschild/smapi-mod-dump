/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/SpecialOrdersExtended
**
*************************************************/

using AtraShared;
using AtraShared.Utils.Extensions;
using SpecialOrdersExtended.DataModels;
using StardewModdingAPI.Utilities;

namespace SpecialOrdersExtended.Managers;

/// <summary>
/// A dialogue to delay.
/// </summary>
internal struct DelayedDialogue
{
    private readonly int time;
    private readonly Dialogue dialogue;
    private readonly NPC npc;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelayedDialogue"/> struct.
    /// </summary>
    /// <param name="time">Time to delay to.</param>
    /// <param name="dialogue">Dialogue to delay.</param>
    /// <param name="npc">Speaking NPC.</param>
    public DelayedDialogue(int time, Dialogue dialogue, NPC npc)
    {
        this.time = time;
        this.dialogue = dialogue;
        this.npc = npc;
    }

    /// <summary>
    /// Pushes the delayed dialogue onto the NPC's stack if it's past time to do so..
    /// </summary>
    /// <param name="currenttime">The current in-game time.</param>
    /// <returns>True if pushed, false otherwise.</returns>
    public bool PushIfPastTime(int currenttime)
    {
        if (currenttime > this.time)
        {
            this.npc.CurrentDialogue.Push(this.dialogue);
            return true;
        }
        return false;
    }
}

/// <summary>
/// Static. Handles logic, patches, and console commands related to the special order dialogues.
/// </summary>
internal class DialogueManager
{
    /// <summary>
    /// A queue of delayed dialogues.
    /// </summary>
    private static readonly PerScreen<Queue<DelayedDialogue>> DelayedDialogues = new(createNewState: () => new Queue<DelayedDialogue>());

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
    public static void Load(long multiplayerID)
    {
        ModEntry.ModMonitor.DebugLog($"Loading dialogue log for {multiplayerID:X8}");
        InternalDialogueLog.Value = DialogueLog.Load(multiplayerID);
    }

    /// <summary>
    /// Loads the PerScreened Dialogue log from a temporary file if available.
    /// Else loads from the usual file.
    /// </summary>
    /// <param name="multiplayerID">The player's unique ID.</param>
    public static void LoadTemp(long multiplayerID)
    {
        ModEntry.ModMonitor.DebugLog($"Loading temp dialogue log for {multiplayerID:X8}");
        InternalDialogueLog.Value = DialogueLog.LoadTempIfAvailable(multiplayerID);
    }

    /// <summary>
    /// Save a dialoguelog for a specific player.
    /// </summary>
    /// <exception cref="SaveNotLoadedError">Save is not loaded.</exception>
    public static void Save()
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
    public static void SaveTemp()
    {
        if (PerscreenedDialogueLog is null)
        {
            throw new SaveNotLoadedError();
        }
        PerscreenedDialogueLog.SaveTemp();
    }

    /// <summary>
    /// Handle the console command to add, remove, or check a seen dialogue.
    /// </summary>
    /// <param name="command">Name of console command.</param>
    /// <param name="args">Arguments for the console coomand.</param>
    public static void ConsoleSpecialOrderDialogue(string command, string[] args)
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
    public static bool HasSeenDialogue(string key, string characterName)
    {
        if (!Context.IsWorldReady)
        {
            throw new SaveNotLoadedError();
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
    public static bool TryAddSeenDialogue(string key, string characterName)
    {
        if (!Context.IsWorldReady)
        {
            throw new SaveNotLoadedError();
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
    public static bool TryRemoveSeenDialogue(string key, string characterName)
    {
        if (!Context.IsWorldReady)
        {
            throw new SaveNotLoadedError();
        }
        return PerscreenedDialogueLog!.TryRemove(key, characterName);
    }

    /// <summary>
    /// Clear any memory of RepeatOrder keys when needed.
    /// </summary>
    /// <param name="removedKeys">List of keys to remove.</param>
    public static void ClearRepeated(List<string> removedKeys)
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
                        ModEntry.ModMonitor.DebugLog($"Removing key {key}");
                        PerscreenedDialogueLog.SeenDialogues.Remove(dialogueKey);
                    }
                }
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
    public static void PostfixCheckDialogue(ref bool __result, ref NPC __instance, int __0, bool __1)
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
                HashSet<string> currentOrders = Game1.player.team.specialOrders.Select((SpecialOrder s) => s.questKey.Value).ToHashSet();
                foreach (SpecialOrder specialOrder in Game1.player.team.availableSpecialOrders)
                {
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
    /// Clears the Delayed Dialogue queue. Call at end of day.
    /// </summary>
    public static void ClearDelayedDialogue() => DelayedDialogues.Value.Clear();

    /// <summary>
    /// Push any available dialogues to the NPC's dialogue stacks.
    /// </summary>
    public static void PushPossibleDelayedDialogues()
    {
        while (DelayedDialogues.Value.TryPeek(out DelayedDialogue result))
        {
            if (result.PushIfPastTime(Game1.timeOfDay))
            {
                // Successfully pushed, remove from queue.
                _ = DelayedDialogues.Value.Dequeue();
            }
            else
            {
                // Everyone else should be behind me in time, so skip to next timeslot.
                return;
            }
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

        // Empty NPC's current dialogue stack and keep it in a queue for now.
        while (npc.CurrentDialogue.TryPop(out Dialogue? result))
        {
            DelayedDialogues.Value.Enqueue(new DelayedDialogue(
                time: Game1.timeOfDay + 100, // delay by one hour.
                npc: npc,
                dialogue: result));
        }

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
