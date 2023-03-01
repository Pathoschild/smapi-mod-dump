/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CommunityToolkit.Diagnostics;

using Microsoft.Xna.Framework;

namespace AtraShared.Utils.Extensions;

/// <summary>
/// Small extensions to Stardew's NPC class.
/// </summary>
public static class NPCExtensions
{
    /// <summary>
    /// Clears the NPC's current dialogue stack and pushes a new dialogue onto that stack.
    /// </summary>
    /// <param name="npc">NPC.</param>
    /// <param name="dialogueKey">Dialogue key.</param>
    public static void ClearAndPushDialogue(
        this NPC npc,
        string dialogueKey)
    {
        Guard.IsNotNull(npc);

        if (!string.IsNullOrWhiteSpace(dialogueKey) && npc.Dialogue.TryGetValue(dialogueKey, out string? dialogue))
        {
            npc.CurrentDialogue.Clear();
            npc.CurrentDialogue.Push(new Dialogue(dialogue, npc) { removeOnNextMove = true });
        }
    }

    /// <summary>
    /// Tries to apply the marriage dialogue if it exists.
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <param name="dialogueKey">Dialogue key to search for.</param>
    /// <param name="add">To add to the stack instead of replacing.</param>
    /// <param name="clearOnMovement">To clear dialogue if the NPC moves.</param>
    /// <returns>True if successfully applied.</returns>
    public static bool TryApplyMarriageDialogueIfExisting(
        this NPC npc,
        string dialogueKey,
        bool add = false,
        bool clearOnMovement = false)
    {
        string dialogue = npc.tryToGetMarriageSpecificDialogueElseReturnDefault(dialogueKey);
        if (string.IsNullOrWhiteSpace(dialogue))
        {
            return false;
        }
        else
        {
            // make endearment token work. This is basically copied from game code.
            dialogue = dialogue.Replace(MarriageDialogueReference.ENDEARMENT_TOKEN_LOWER, npc.getTermOfSpousalEndearment().ToLower(), StringComparison.Ordinal);
            dialogue = dialogue.Replace(MarriageDialogueReference.ENDEARMENT_TOKEN, npc.getTermOfSpousalEndearment(), StringComparison.Ordinal);

            if (!add)
            {
                npc.CurrentDialogue.Clear();
                npc.currentMarriageDialogue.Clear();
            }
            npc.CurrentDialogue.Push(new Dialogue(dialogue, npc) { removeOnNextMove = clearOnMovement });
            return true;
        }
    }

    /// <summary>
    /// Given a base key, gets a random dialogue from a set.
    /// </summary>
    /// <param name="npc">NPC.</param>
    /// <param name="basekey">Base key to use.</param>
    /// <param name="random">Random to use, defaults to Game1.random if null.</param>
    /// <returns>null if no dialogue key found, a random dialogue key otherwise.</returns>
    public static string? GetRandomDialogue(
        this NPC npc,
        string? basekey,
        Random? random)
    {
        if (basekey is null)
        {
            return null;
        }
        if (random is null)
        {
            random = Game1.random;
        }
        if (npc.Dialogue?.ContainsKey(basekey) == true)
        {
            int index = 1;
            while (npc.Dialogue.ContainsKey($"{basekey}_{++index}"))
            {
            }
            int selection = random.Next(1, index);
            return (selection == 1) ? basekey : $"{basekey}_{selection}";
        }
        return null;
    }

    /// <summary>
    /// Helper method to get an NPC's raw schedule string for a specific key.
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <param name="scheduleKey">Schedule key to look for.</param>
    /// <param name="rawData">Raw schedule string.</param>
    /// <returns>True if successful, false otherwise.</returns>
    /// <remarks>Does **not** set _lastLoadedScheduleKey, intentionally.</remarks>
    public static bool TryGetScheduleEntry(
        this NPC npc,
        string scheduleKey,
        [NotNullWhen(returnValue: true)] out string? rawData)
    {
        rawData = null;
        Dictionary<string, string>? scheduleData = npc.getMasterScheduleRawData();
        if (scheduleData is null || scheduleKey is null)
        {
            return false;
        }
        return scheduleData.TryGetValue(scheduleKey, out rawData);
    }

    /// <summary>
    /// Gets the tile an NPC is currently facing.
    /// </summary>
    /// <param name="npc">NPC in question.</param>
    /// <returns>Tile they're facing.</returns>
    public static Vector2 GetFacingTile(this Character npc)
    {
        Vector2 tile = npc.Position / Game1.tileSize;
        return npc.facingDirection.Get() switch
        {
            Game1.up => tile - Vector2.UnitY,
            Game1.down => tile + Vector2.UnitY,
            Game1.left => tile - Vector2.UnitX,
            Game1.right => tile + Vector2.UnitX,
            _ => tile,
        };
    }
}