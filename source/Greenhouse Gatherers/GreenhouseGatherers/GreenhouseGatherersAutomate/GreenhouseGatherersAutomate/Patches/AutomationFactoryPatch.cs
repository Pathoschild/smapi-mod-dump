/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/GreenhouseGatherers
**
*************************************************/

using Harmony;
using StardewValley;
using System.Reflection;
using StardewModdingAPI;
using GreenhouseGatherers.GreenhouseGatherers.Objects;
using Microsoft.Xna.Framework;
using System;
using StardewValley.Objects;

namespace GreenhouseGatherersAutomate.GreenhouseGatherersAutomate.Patches
{
    [HarmonyPatch]
    public class AutomationFactoryPatch
    {
        private static IMonitor monitor = AutomateModResources.GetMonitor();

        internal static MethodInfo TargetMethod()
        {
            return AccessTools.Method("Pathoschild.Stardew.Automate.Framework.AutomationFactory:GetFor", new Type[]{ typeof(StardewValley.Object), typeof(GameLocation), typeof(Vector2).MakeByRefType() });
        }

        internal static bool Prefix(StardewValley.Object obj, GameLocation location, in Vector2 tile)
        {
            try
            {
                if (obj is HarvestStatue)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                monitor.Log($"There was a problem with a Harmony patch; Harvest Statues will not output harvested products. See log for details.", LogLevel.Error);
                monitor.Log($"An exception occured while trying to patch Pathoschild.Stardew.Automate.Framework.AutomationFactory:GetFor(): {e}", LogLevel.Trace);
            }

            return true;
        }
    }
}