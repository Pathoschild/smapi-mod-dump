/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using AnythingAnywhere.Framework.UI;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Linq;
using System.Threading;
using Object = StardewValley.Object;


namespace AnythingAnywhere.Framework.Patches.Menus
{
    internal class CarpenterMenuPatch : PatchTemplate
    {
        private readonly Type _object = typeof(CarpenterMenu);

        internal CarpenterMenuPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(CarpenterMenu.tryToBuild)), prefix: new HarmonyMethod(GetType(), nameof(TryToBuildPrefix)));
        }

        // Old test build method
        private static bool TryToBuildPrefix(CarpenterMenu __instance, ref bool __result)
        {
            if (Game1.activeClickableMenu is BuildAnywhereMenu && ModEntry.modConfig.EnableFreeBuild)
            {
                NetString skinId = __instance.currentBuilding.skinId;
                Vector2 tileLocation = new Vector2((Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
                if (__instance.TargetLocation.buildStructure(__instance.currentBuilding.buildingType.Value, tileLocation, Game1.player, out var building, /*__instance.Blueprint.MagicalConstruction*/ true, true))
                {
                    building.skinId.Value = skinId.Value;
                    if (building.isUnderConstruction())
                    {
                        Game1.netWorldState.Value.MarkUnderConstruction(__instance.Builder, building);
                    }
                    __result = true;
                    return false;
                }
            }
            __result = false;
            return true;
        }
    }
}
