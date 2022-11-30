/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewRoguelike.Extensions;
using StardewValley.Locations;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class GenerateContentsPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "generateContents");

        public static bool Prefix(MineShaft __instance)
        {
            __instance.GetType().GetField("ladderHasSpawned", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, false);
            __instance.GetType().GetField("loadedDarkArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, false);
            __instance.loadLevel(__instance.mineLevel);

            __instance.GetType().GetProperty("isMonsterArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, false);
            __instance.GetType().GetProperty("isSlimeArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, false);
            __instance.GetType().GetProperty("isQuarryArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, false);
            __instance.GetType().GetProperty("isDinoArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, false);
            __instance.GetType().GetProperty("lighting", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, new Color(80, 80, 40));

            MineShaft.permanentMineChanges.Clear();

            __instance.findLadder();
            __instance.GetType().GetMethod("populateLevel", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(__instance, null);

            if (__instance.IsNormalFloor() || BossFloor.IsBossFloor(__instance))
                __instance.GetType().GetProperty("isMonsterArea", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, true);

            return false;
        }
    }
}
