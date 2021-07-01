/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Linq;

namespace TransparentObjects
{
    public class ObjectPatches
    {
        private static IMonitor Monitor;
        private static ModConfig Config;
        private static IModHelper Helper;

        public static void Initialize(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            Monitor = monitor;
            Config = config;
            Helper = helper;
        }
        public static void Object_draw_Prefix(StardewValley.Object __instance, ref float alpha)
        {

            if (!ModEntry.IsAllowed(__instance.name) || ModEntry.IsOff())
                return;

            float maxDistance = Config.TransparencyMaxDistance;
            float minAlpha = Math.Min(1f, Math.Max(0, Config.MinTransparency));
            Vector2 playerCenter = new Vector2(Game1.player.position.X + 32, Game1.player.position.Y + 32);
            Vector2 objectCenter = new Vector2(__instance.TileLocation.X * 64 + 32, __instance.TileLocation.Y * 64);
            float distance = Vector2.Distance(playerCenter, objectCenter);
            if (__instance.bigCraftable && distance < maxDistance)
            {
                float fraction = (Math.Max(0,distance)) / maxDistance;
                alpha = minAlpha + (1 - minAlpha) * fraction;
            }
        }
    }
}
