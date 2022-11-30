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
using StardewValley.Locations;
using StardewValley;
using StardewValley.Monsters;
using System;
using Microsoft.Xna.Framework;
using Netcode;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(Grub), "behaviorAtGameTick")]
    internal class GrubBehaviorAtGameTickPatch
    {
        public static bool Prefix(Grub __instance, GameTime time)
        {
            NetBool pupating = (NetBool)__instance.GetType().GetField("pupating", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
            int metamorphCounter = (int)__instance.GetType().GetField("metamorphCounter", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;

            if (pupating.Value)
            {
                __instance.Scale = 1f + (float)Math.Sin(time.TotalGameTime.Milliseconds * ((float)Math.PI / 8f)) / 12f;
                __instance.GetType().GetField("metamorphCounter", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, metamorphCounter - time.ElapsedGameTime.Milliseconds);
                if (metamorphCounter <= 0)
                {
                    __instance.Health = -500;
                    Game1.createRadialDebris(__instance.currentLocation, __instance.Sprite.textureName.Value, new Rectangle(208, 424, 32, 40), 4, __instance.getStandingX(), __instance.getStandingY(), 25, (int)__instance.getTileLocation().Y);
                    Game1.createRadialDebris(__instance.currentLocation, __instance.Sprite.textureName.Value, new Rectangle(208, 424, 32, 40), 8, __instance.getStandingX(), __instance.getStandingY(), 15, (int)__instance.getTileLocation().Y);
                    if (__instance.currentLocation is MineShaft)
                    {
                        Monster toSpawn = ((MineShaft)__instance.currentLocation).BuffMonsterIfNecessary(new Fly(__instance.Position, __instance.hard.Value)
                        {
                            currentLocation = __instance.currentLocation
                        });
                        Roguelike.AdjustMonster((MineShaft)__instance.currentLocation, ref toSpawn);
                        __instance.currentLocation.characters.Add(toSpawn);
                    }
                    else
                    {
                        Monster toSpawn = new Fly(__instance.Position, __instance.hard.Value)
                        {
                            currentLocation = __instance.currentLocation
                        };
                        Roguelike.AdjustMonster((MineShaft)__instance.currentLocation, ref toSpawn);
                        __instance.currentLocation.characters.Add(toSpawn);
                    }
                }

                return false;
            }

            return true;
        }
    }
}
