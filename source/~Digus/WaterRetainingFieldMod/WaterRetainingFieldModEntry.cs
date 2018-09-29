using System.Collections.Generic;
using System.Reflection;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace WaterRetainingFieldMod
{
    public class WaterRetainingFieldModEntry : Mod
    {
        internal static DataLoader DataLoader;

        public override void Entry(IModHelper helper)
        {
            DataLoader = new DataLoader(helper);

            TimeEvents.AfterDayStarted += (x, y) => HoeDirtOverrides.TileLocationState.Clear();

            var harmony = HarmonyInstance.Create("Digus.WaterRetainingFieldMod");

            var hoeDirtDayUpdate = typeof(HoeDirt).GetMethod("dayUpdate");
            var hoeDirtOverridesDayUpdatePrefix = typeof(HoeDirtOverrides).GetMethod("DayUpdatePrefix");
            var hoeDirtOverridesDayUpdatePostfix = typeof(HoeDirtOverrides).GetMethod("DayUpdatePostfix");
            harmony.Patch(hoeDirtDayUpdate, new HarmonyMethod(hoeDirtOverridesDayUpdatePrefix), new HarmonyMethod(hoeDirtOverridesDayUpdatePostfix));
        }
    }
}
