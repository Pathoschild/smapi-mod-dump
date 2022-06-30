/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewHitboxes
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace StardewHitboxes.Patches
{
    [HarmonyPatch(typeof(GameLocation), "draw")]
    internal class GameLocationDraw
    {
        public static void Postfix(GameLocation __instance, SpriteBatch b)
        {
            foreach (Farmer farmer in __instance.farmers)
                ModEntry.DrawHitbox(b, farmer);

            foreach (Character character in __instance.characters)
                ModEntry.DrawHitbox(b, character);
        }
    }
}
