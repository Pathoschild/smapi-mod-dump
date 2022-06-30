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
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Patches.Buildings
{
    internal class ChestPatch : PatchTemplate
    {

        private readonly Type _object = typeof(Chest);

        internal ChestPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Chest.GetActualCapacity), null), postfix: new HarmonyMethod(GetType(), nameof(GetActualCapacityPostfix)));
        }

        internal static void GetActualCapacityPostfix(Chest __instance, ref int __result)
        {
            if (__instance.modData.ContainsKey(ModDataKeys.CUSTOM_CHEST_CAPACITY) && int.TryParse(__instance.modData[ModDataKeys.CUSTOM_CHEST_CAPACITY], out int capacity))
            {
                __result = capacity;
            }
        }
    }
}
