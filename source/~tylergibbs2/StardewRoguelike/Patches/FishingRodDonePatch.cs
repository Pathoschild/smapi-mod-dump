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
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(FishingRod), "doDoneFishing")]
    internal class FishingRodDonePatch
    {
        public static void Postfix(FishingRod __instance)
        {
            Farmer lastUser = (Farmer)__instance.GetType().BaseType.GetField("lastUser", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(__instance)!;
            if (lastUser is null || !lastUser.IsLocalPlayer)
                return;

            __instance.increment_FishingRodUses();

            int maxUses = Perks.HasPerk(Perks.PerkType.Fisherman) ? 10 : 5;
            if (__instance.get_FishingRodUses() >= maxUses)
            {
                Game1.player.removeItemFromInventory(__instance);
                Game1.playSound("breakingGlass");
            }
        }
    }
}
