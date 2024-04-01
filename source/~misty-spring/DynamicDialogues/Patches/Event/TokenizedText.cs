/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using StardewValley;
using StardewValley.TokenizableStrings;

namespace DynamicDialogues.Patches;

internal partial class EventPatches
{
    internal static void Pre_Speak(Event @event, ref string[] args, EventContext context)
    {
        if (Game1.dialogueUp)
            return;
        
        if (!ArgUtility.TryGet(args, 1, out _, out var error) || !ArgUtility.TryGet(args, 2, out var speakText, out error))
        {
            context.LogErrorAndSkip(error);
            return;
        }

        if (Game1.content.IsValidTranslationKey(speakText))
            return;

        if (speakText.Contains("[LocalizedText ", StringComparison.OrdinalIgnoreCase) == false)
            return;

        var fixedStr = TokenParser.ParseText(speakText);

        args[2] = fixedStr;
    }
    
    internal static void Pre_Message(Event @event, ref string[] args, EventContext context)
    {
        if (Game1.dialogueUp)
            return;
        
        if (!ArgUtility.TryGet(args, 1, out var message, out var error))
        {
            context.LogErrorAndSkip(error);
            return;
        }

        //allow string keys
        if (Game1.content.IsValidTranslationKey(message))
        {
            args[1] = Game1.content.LoadString(message);
            return;
        }

        if (message.Contains("[LocalizedText ", StringComparison.OrdinalIgnoreCase) == false)
            return;

        var fixedStr = TokenParser.ParseText(message);

        args[1] = fixedStr;
    }
}