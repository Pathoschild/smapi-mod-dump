/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using BirbCore.Attributes;
using HarmonyLib;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace WinterStarSpouse;

[HarmonyPatch(typeof(Event), nameof(Event.setUpPlayerControlSequence))]
class Event_SetUpPlayerControlSequence
{
    static void Postfix(string id, Event __instance)
    {
        try
        {
            if (id != "christmas")
            {
                return;
            }

            NPC signiificantOther = Utilities.GetSoleSignificantOther();
            if (signiificantOther == null)
            {
                // No spouse :(
                return;
            }

            Random random = new((int)(Game1.uniqueIDForThisGame / 2uL) ^ Game1.year ^ (int)Game1.player.UniqueMultiplayerID);
            if (Utilities.SpouseAsRecipient())
            {
                __instance.secretSantaRecipient = signiificantOther;
                Log.Trace("Overrode secret santa recipient with " + signiificantOther.Name);
            }

            if (random.Next(100) < ModEntry.Config.SpouseIsGiverChance)
            {
                __instance.mySecretSanta = signiificantOther;
                Log.Trace("Overrode secret santa giver with " + signiificantOther.Name);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}


[HarmonyPatch(typeof(LetterViewerMenu), MethodType.Constructor, new Type[] { typeof(string), typeof(string), typeof(bool) })]
class LetterViewerMenu_Constructor
{
    internal static void Postfix(string mailTitle, bool fromCollection, LetterViewerMenu __instance)
    {
        try
        {
            if (mailTitle != "winter_24" && mailTitle != "winter_18")
            {
                return;
            }
            if (!Utilities.SpouseAsRecipient())
            {
                return;
            }
            NPC significantOther = Utilities.GetSoleSignificantOther();
            if (significantOther == null)
            {
                return;
            }
            // Reload the mail text, because the parameter has already been changed in-place with the wrong gift recipient.
            Dictionary<string, string> mailDict = Game1.content.Load<Dictionary<string, string>>("Data\\mail");
            string mail = mailDict[mailTitle];

            mail = mail.Split(new string[1] { "[#]" }, StringSplitOptions.None)[0];
            mail = mail.Replace("@", Game1.player.Name);

            bool hide_secret_santa = fromCollection;
            if (Game1.currentSeason == "winter" && Game1.dayOfMonth >= 18 && Game1.dayOfMonth <= 25)
            {
                hide_secret_santa = false;
            }
            if (hide_secret_santa)
            {
                return;
            }

            mail = mail.Replace("%secretsanta", significantOther.displayName);

            int page_height = __instance.height - 128;
            if (__instance.HasInteractable())
            {
                page_height = __instance.height - 128 - 32;
            }
            __instance.mailMessage = SpriteText.getStringBrokenIntoSectionsOfHeight(mail, __instance.width - 64, page_height);
        }
        catch (Exception e)
        {
            Log.Error($"Failed in {MethodBase.GetCurrentMethod().DeclaringType}\n{e}");
        }
    }
}

class Utilities
{
    internal static NPC GetSoleSignificantOther()
    {
        NPC significantOther = null;
        if (Game1.player.getSpouse() != null)
        {
            significantOther = Game1.player.getSpouse();
        }
        else
        {
            foreach (string friend in Game1.player.friendshipData.Keys)
            {
                Friendship friendship = Game1.player.friendshipData[friend];
                if (friendship.IsDating())
                {
                    if (significantOther != null)
                    {
                        // If you are dating multiple people, don't affect Christmas Star
                        return null;
                    }
                    significantOther = Game1.getCharacterFromName(friend);
                }
            }
        }
        return significantOther;
    }

    internal static bool SpouseAsRecipient()
    {
        Random random = new((int)(Game1.uniqueIDForThisGame / 2uL) ^ Game1.year ^ ((int)Game1.player.UniqueMultiplayerID * 123));

        return random.Next(100) < ModEntry.Config.SpouseIsRecipientChance;
    }
}
