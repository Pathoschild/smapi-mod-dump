/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;

namespace Circuit.Patches
{
    [HarmonyPatch(typeof(Event), nameof(Event.answerDialogueQuestion))]
    internal class EventAnswerDialogueQuestion
    {
        public static void Postfix()
        {
            if (!ModEntry.ShouldPatch() || Game1.player.dancePartner.Value is null)
                return;

            ModEntry.Instance.TaskManager?.OnFlowerDancePartnerAcquired();
        }
    }
}
