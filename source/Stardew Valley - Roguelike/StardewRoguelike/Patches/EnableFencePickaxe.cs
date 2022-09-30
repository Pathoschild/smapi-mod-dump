/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Fence), "performToolAction")]
    internal class EnableFencePickaxe
    {
        public static bool Prefix(Fence __instance, Tool t, GameLocation location)
        {
            if ((__instance.whichType.Value == 1 || __instance.whichType.Value == 5) && (t == null || t is Pickaxe))
            {
                location.playSound("axchop");
                location.objects.Remove(__instance.tileLocation);
                for (int i = 0; i < 4; i++)
                    location.temporarySprites.Add(new CosmeticDebris(__instance.fenceTexture.Value, new Vector2(__instance.tileLocation.X * 64f + 32f, __instance.tileLocation.Y * 64f + 32f), (float)Game1.random.Next(-5, 5) / 100f, (float)Game1.random.Next(-64, 64) / 30f, (float)Game1.random.Next(-800, -100) / 100f, (int)((__instance.tileLocation.Y + 1f) * 64f), new Rectangle(32 + Game1.random.Next(2) * 16 / 2, 96 + Game1.random.Next(2) * 16 / 2, 8, 8), Color.White, (Game1.soundBank != null) ? Game1.soundBank.GetCue("shiny4") : null, null, 0, 200));

                Game1.createRadialDebris(location, 12, (int)__instance.tileLocation.X, (int)__instance.tileLocation.Y, 6, resource: false);
                Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(12, new Vector2(__instance.tileLocation.X * 64f, __instance.tileLocation.Y * 64f), Color.White, 8, Game1.random.NextDouble() < 0.5, 50f));

                if ((float)__instance.maxHealth.Value - (float)__instance.health.Value < 0.5f)
                {
                    switch (__instance.whichType.Value)
                    {
                        case 1:
                            location.debris.Add(new Debris(new Object(322, 1), __instance.tileLocation.Value * 64f + new Vector2(32f, 32f)));
                            break;
                        case 5:
                            location.debris.Add(new Debris(new Object(298, 1), __instance.tileLocation.Value * 64f + new Vector2(32f, 32f)));
                            break;
                    }
                }
            }

            return true;
        }
    }
}
