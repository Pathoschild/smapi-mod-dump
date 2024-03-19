/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using StardewValley;

namespace SpousesIsland.Additions;

internal static class Dialogues
{

    /// <summary>
    /// Create dialogue and push to front of stack.
    /// </summary>
    /// <param name="who"></param>
    /// <param name="text"></param>
    internal static void Push(NPC who, string text)
    {
        var dialogue = new Dialogue(who, null, text);
        who.CurrentDialogue.Push(dialogue);
    }

    /// <summary>
    /// Set dialogue 
    /// </summary>
    /// <param name="who"></param>
    /// <param name="text"></param>
    internal static void Set(NPC who, string text, bool add = true)
    {
        var dialogue = new Dialogue(who, null, text);
        who.setNewDialogue(dialogue, add, false);
    }

    /// <summary>
    /// Draws dialogue to screen.
    /// </summary>
    /// <param name="who"></param>
    /// <param name="text"></param>
    // Because this method doesn't exist anymore in Game1, we do its equiv.
    public static void Draw(NPC who, string text)
    {
        var db = new Dialogue(who, null, text);
        who.CurrentDialogue.Push(db);
        Game1.drawDialogue(who);
    }
}
