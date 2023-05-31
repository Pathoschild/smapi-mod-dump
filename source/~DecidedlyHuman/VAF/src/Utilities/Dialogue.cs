/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using SDialogue = StardewValley.Dialogue;

namespace VAF.Utilities;

public class Dialogue
{
    public static string? GetDialogueKey(SDialogue dialogue)
    {
        string?[] key = null;

        key = dialogue.TranslationKey.Split(":");

        if (key.Length != 2)
            return null;

        return key[1];
    }

    public static string? GetDialoguePath(SDialogue dialogue)
    {
        string?[] path = null;

        path = dialogue.TranslationKey.Split(":");

        if (path.Length != 2)
            return null;

        return path[0];
    }
}
