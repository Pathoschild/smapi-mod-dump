/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Models.ContentPack;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    // TODO: When updated to SDV v1.6, delete this patch
    internal class BuildingPaintMenuPatch : PatchTemplate
    {
        private readonly Type _object = typeof(BuildingPaintMenu);

        internal BuildingPaintMenuPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(BuildingPaintMenu), nameof(BuildingPaintMenu.LoadRegionData), null), prefix: new HarmonyMethod(GetType(), nameof(LoadRegionDataPrefix)));
        }

        private static bool LoadRegionDataPrefix(BuildingPaintMenu __instance, Dictionary<string, string> ____paintData)
        {
            string data = null;
            if (__instance.regionData != null || __instance.building is not GenericBuilding genericBuilding || genericBuilding is null)
            {
                return true;
            }

            __instance.regionData = new Dictionary<string, Vector2>();
            __instance.regionNames = new List<string>();

            var paintDataKey = __instance.buildingType;
            if (String.IsNullOrEmpty(genericBuilding.skinID.Value) is false)
            {
                paintDataKey = genericBuilding.skinID.Value;
            }

            if (____paintData.ContainsKey(paintDataKey))
            {
                data = ____paintData[paintDataKey].Replace("\n", "").Replace("\t", "");
            }

            if (String.IsNullOrEmpty(data))
            {
                return true;
            }

            string[] data_split = data.Split('/');
            for (int i = 0; i < data_split.Length / 2; i++)
            {
                if (data_split[i].Trim() == "")
                {
                    continue;
                }
                string region_name = data_split[i * 2];
                string[] brightness_split = data_split[i * 2 + 1].Split(' ');
                int min_brightness = -100;
                int max_brightness = 100;
                if (brightness_split.Length >= 2)
                {
                    try
                    {
                        min_brightness = int.Parse(brightness_split[0]);
                        max_brightness = int.Parse(brightness_split[1]);
                    }
                    catch (Exception)
                    {
                    }
                }
                __instance.regionData[region_name] = new Vector2(min_brightness, max_brightness);
                __instance.regionNames.Add(region_name);
            }

            return false;
        }
    }
}
