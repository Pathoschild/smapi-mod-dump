/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewRoguelike.Extensions;
using StardewRoguelike.VirtualProperties;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class LoadLevelPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "loadLevel");

        public static bool Prefix(MineShaft __instance, int level)
        {
            bool loadedDarkArea = false;

            bool isMonsterArea = false;
            bool isSlimeArea = false;
            bool isQuarryArea = false;
            bool isDinoArea = false;

            __instance.mapPath.Value = "Maps\\Mines\\" + Roguelike.GetMapPath(__instance, avoidWater: __instance.get_MineShaftIsDarkArea());
            __instance.updateMap();

            if (__instance.getMineArea(level) < 40 && !BossFloor.IsBossFloor(__instance))
                __instance.mapImageSource.Value = "Maps\\Mines\\mine";
            else if (__instance.getMineArea(level) == 40)
                __instance.mapImageSource.Value = "Maps\\Mines\\mine_frost";
            else if (__instance.getMineArea(level) == 80)
                __instance.mapImageSource.Value = "Maps\\Mines\\mine_lava";
            else if (__instance.getMineArea(level) == 121)
                __instance.mapImageSource.Value = "Maps\\Mines\\mine_desert";

            if (__instance.get_MineShaftIsDarkArea())
            {
                __instance.mapImageSource.Value += "_dark";
                loadedDarkArea = true;
            }

            if (__instance.GetAdditionalDifficulty() > 0 && !BossFloor.IsBossFloor(__instance))
            {
                string map_image_source = "Maps\\Mines\\mine";
                if (__instance.mapImageSource.Value is not null)
                    map_image_source = __instance.mapImageSource.Value;
                if (map_image_source.EndsWith("_dark"))
                {
                    map_image_source = map_image_source.Remove(map_image_source.Length - "_dark".Length);
                    loadedDarkArea = true;
                }

                string base_map_image_source = map_image_source;
                if (loadedDarkArea)
                    map_image_source += "_dark";
                map_image_source += "_dangerous";

                try
                {
                    __instance.mapImageSource.Value = map_image_source;
                    Game1.temporaryContent.Load<Texture2D>(__instance.mapImageSource.Value);
                }
                catch (ContentLoadException)
                {
                    map_image_source = base_map_image_source + "_dangerous";
                    try
                    {
                        __instance.mapImageSource.Value = map_image_source;
                        Game1.temporaryContent.Load<Texture2D>(__instance.mapImageSource.Value);
                    }
                    catch (ContentLoadException)
                    {
                        map_image_source = base_map_image_source;
                        if (loadedDarkArea)
                            map_image_source += "_dark";

                        try
                        {
                            __instance.mapImageSource.Value = map_image_source;
                            Game1.temporaryContent.Load<Texture2D>(__instance.mapImageSource.Value);
                        }
                        catch (ContentLoadException)
                        {
                            __instance.mapImageSource.Value = base_map_image_source;
                        }
                    }
                }
            }

            __instance.ApplyDiggableTileFixes();
            if (!__instance.isSideBranch())
            {
                MineShaft.lowestLevelReached = Math.Max(MineShaft.lowestLevelReached, level);
                __instance.setMapTileIndex(0, 0, 0, "Buildings");
            }

            if (loadedDarkArea)
                __instance.set_MineShaftIsDarkArea(loadedDarkArea);

            __instance.GetType().GetProperty("isMonsterArea", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, isMonsterArea);
            __instance.GetType().GetProperty("isSlimeArea", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, isSlimeArea);
            __instance.GetType().GetProperty("isQuarryArea", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, isQuarryArea);
            __instance.GetType().GetProperty("isDinoArea", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, isDinoArea);

            __instance.GetType().GetField("loadedDarkArea", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, loadedDarkArea);

            return false;
        }
    }
}
