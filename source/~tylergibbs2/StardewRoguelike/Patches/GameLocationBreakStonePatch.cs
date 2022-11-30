/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewRoguelike.HatQuests;
using StardewRoguelike.VirtualProperties;
using StardewValley;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(GameLocation), "breakStone")]
    internal class GameLocationBreakStonePatch
    {
        public static void Postfix(GameLocation __instance, int indexOfStone, int x, int y, Farmer who, Random r)
        {
            // gold ore
            if (indexOfStone != 764 || who != Game1.player)
                return;

            if (Game1.player.get_FarmerActiveHatQuest() is not null)
                Game1.player.get_FarmerActiveHatQuest()!.GoldMined++;

            // Spawn extra gold for hard hat
            if (HatQuest.HasBuffFor(HatQuestType.HARD_HAT))
            {
                Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

                Game1.createMultipleObjectDebris(384, x, y, 2, who.UniqueMultiplayerID, __instance);
                multiplayer.broadcastSprites(__instance, Utility.sparkleWithinArea(new Rectangle(x * 64, (y - 1) * 64, 32, 96), 3, Color.Yellow * 0.5f, 175, 100));
            }
        }
    }
}
