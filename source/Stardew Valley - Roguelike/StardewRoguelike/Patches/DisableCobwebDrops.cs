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
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Grass), "performToolAction")]
    internal class DisableCobwebDrops
    {
        public static bool Prefix(Grass __instance, Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            location ??= Game1.currentLocation;

            if ((t != null && t is MeleeWeapon weapon && weapon.type.Value != 2) || explosion > 0)
            {
                if (t != null && (t as MeleeWeapon).type.Value != 1)
                    DelayedAction.playSoundAfterDelay("daggerswipe", 50);
                else
                    location.playSound("swordswipe");

                MethodInfo shake = __instance.GetType().GetMethod("shake", BindingFlags.NonPublic | BindingFlags.Instance);
                shake.Invoke(__instance, new object[] { (float)Math.PI * 3f / 32f, (float)Math.PI / 40f, Game1.random.NextDouble() < 0.5 });

                int numberOfWeedsToDestroy = ((explosion <= 0) ? 1 : Math.Max(1, explosion + 2 - Game1.recentMultiplayerRandom.Next(2)));
                if (t is MeleeWeapon && t.InitialParentTileIndex == 53)
                    numberOfWeedsToDestroy = 2;

                if (__instance.grassType.Value == 6 && Game1.random.NextDouble() < 0.5)
                    numberOfWeedsToDestroy = 0;

                __instance.numberOfWeeds.Value -= numberOfWeedsToDestroy;

                Color c = Color.Green;
                switch (__instance.grassType.Value)
                {
                    case 1:
                        switch (location.GetSeasonForLocation())
                        {
                            case "spring":
                                c = new Color(60, 180, 58);
                                break;
                            case "summer":
                                c = new Color(110, 190, 24);
                                break;
                            case "fall":
                                c = new Color(219, 102, 58);
                                break;
                        }
                        break;
                    case 2:
                        c = new Color(148, 146, 71);
                        break;
                    case 3:
                        c = new Color(216, 240, 255);
                        break;
                    case 4:
                        c = new Color(165, 93, 58);
                        break;
                    case 6:
                        c = Color.White * 0.6f;
                        break;
                }

                Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(28, tileLocation * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-16, 16)), c, 8, Game1.random.NextDouble() < 0.5, Game1.random.Next(60, 100)));
            }

            return false;
        }
    }
}
