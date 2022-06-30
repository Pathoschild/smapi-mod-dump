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
using StardewValley.Projectiles;

namespace StardewHitboxes.Patches
{
    [HarmonyPatch(typeof(Projectile), "draw")]
    internal class ProjectileDraw
    {
        public static void Postfix(Projectile __instance, SpriteBatch b)
        {
            ModEntry.DrawHitbox(b, __instance);
        }
    }
}
