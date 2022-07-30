/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Microsoft.Toolkit.Diagnostics;
using StardewModdingAPI.Utilities;

namespace AtraCore.Framework.DialogueManagement;

/// <summary>
/// Handles queuing dialogue for later.
/// </summary>
public class QueuedDialogueManager
{
    /// <summary>
    /// A queue of delayed dialogues.
    /// </summary>
    private static readonly PerScreen<Queue<DelayedDialogue>> DelayedDialogues = new(createNewState: () => new Queue<DelayedDialogue>());

    /// <summary>
    /// Empty NPC's current dialogue stack and keep it in a queue for now.
    /// </summary>
    /// <param name="npc">NPC to push dialogue of.</param>
    public static void PushCurrentDialogueToQueue(NPC npc)
    {
        Guard.IsNotNull(npc, nameof(npc));

        while (npc.CurrentDialogue.TryPop(out Dialogue? result))
        {
            DelayedDialogues.Value.Enqueue(new DelayedDialogue(
                time: Game1.timeOfDay + 100, // delay by one hour.
                npc: npc,
                dialogue: result));
        }
    }

    /// <summary>
    /// Clears the Delayed Dialogue queue. Call at end of day.
    /// </summary>
    internal static void ClearDelayedDialogue() => DelayedDialogues.Value.Clear();

    /// <summary>
    /// Push any available dialogues to the NPC's dialogue stacks.
    /// </summary>
    internal static void PushPossibleDelayedDialogues()
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
}
