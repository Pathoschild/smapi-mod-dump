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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    [HarmonyPatch(typeof(MineShaft), "findLadder")]
    internal class MineShaftFindLadderAbuse
    {
        public static bool Prefix(MineShaft __instance)
        {
            int level = Roguelike.GetLevelFromMineshaft(__instance);
            int x = -1;
            int y = -1;
            if (BossFloor.IsBossFloor(level) && BossFloor.GetBossIndexForFloor(level) == 7)
            {
                x = 24;
                y = 10;
            }
            else if (ForgeFloor.IsForgeFloor(__instance))
            {
                x = 8;
                y = 7;
            }
            else if (ChallengeFloor.IsChallengeFloor(__instance))
            {
                Vector2 spawnTile = ChallengeFloor.GetSpawnLocation(__instance);
                if (spawnTile == Vector2.Zero)
                    return true;

                x = (int)spawnTile.X;
                y = (int)spawnTile.Y;
            }

            if (x != -1 && y != -1)
            {
                __instance.GetType().GetProperty("tileBeneathLadder", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, new Vector2(x, y));
                __instance.sharedLights[x + y * 999] = new LightSource(4, new Vector2(x, y - 2) * 64f + new Vector2(32f, 0f), 0.25f, new Color(0, 20, 50), x + y * 999, LightSource.LightContext.None, 0L);
                __instance.sharedLights[x + y * 998] = new LightSource(4, new Vector2(x, y - 1) * 64f + new Vector2(32f, 0f), 0.5f, new Color(0, 20, 50), x + y * 998, LightSource.LightContext.None, 0L);
                __instance.sharedLights[x + y * 997] = new LightSource(4, new Vector2(x, y) * 64f + new Vector2(32f, 0f), 0.75f, new Color(0, 20, 50), x + y * 997, LightSource.LightContext.None, 0L);
                __instance.sharedLights[x + y * 1000] = new LightSource(4, new Vector2(x, y + 1) * 64f + new Vector2(32f, 0f), 1f, new Color(0, 20, 50), x + y * 1000, LightSource.LightContext.None, 0L);

                return false;
            }

            return true;
        }
    }
}
