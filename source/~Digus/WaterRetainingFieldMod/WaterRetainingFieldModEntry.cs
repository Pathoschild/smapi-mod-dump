using Harmony;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;

namespace WaterRetainingFieldMod
{
    public class WaterRetainingFieldModEntry : Mod
    {
        internal static DataLoader DataLoader;

        public override void Entry(IModHelper helper)
        {
            DataLoader = new DataLoader(helper);

            helper.Events.GameLoop.DayStarted += (sender, e) => HoeDirtOverrides.TileLocationState.Clear();

            var harmony = HarmonyInstance.Create("Digus.WaterRetainingFieldMod");

            var hoeDirtDayUpdate = typeof(HoeDirt).GetMethod("dayUpdate");
            var hoeDirtOverridesDayUpdatePrefix = typeof(HoeDirtOverrides).GetMethod("DayUpdatePrefix");
            var hoeDirtOverridesDayUpdatePostfix = typeof(HoeDirtOverrides).GetMethod("DayUpdatePostfix");
            harmony.Patch(hoeDirtDayUpdate, new HarmonyMethod(hoeDirtOverridesDayUpdatePrefix), new HarmonyMethod(hoeDirtOverridesDayUpdatePostfix));
        }
    }
}
