using System.Reflection;
using CustomCrystalariumMod;
using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomCrystalariumMod
{
    public class CustomCrystalariumModEntry : Mod
    {
        public static IMonitor ModMonitor;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            new DataLoader(helper);

            var harmony = HarmonyInstance.Create("Digus.CustomCrystalariumMod");

            var objectGetMinutesForCrystalarium = typeof(Object).GetMethod("getMinutesForCrystalarium", BindingFlags.NonPublic | BindingFlags.Instance);
            var objectOverridesGetMinutesForCrystalarium = typeof(ObjectOverrides).GetMethod("GetMinutesForCrystalarium");
            harmony.Patch(objectGetMinutesForCrystalarium, new HarmonyMethod(objectOverridesGetMinutesForCrystalarium), null);

            var objectPerformObjectDropInAction = typeof(Object).GetMethod("performObjectDropInAction");
            var objectOverridesPerformObjectDropInAction = typeof(ObjectOverrides).GetMethod("PerformObjectDropInAction");
            harmony.Patch(objectPerformObjectDropInAction, new HarmonyMethod(objectOverridesPerformObjectDropInAction), null);

            if (DataLoader.ModConfig.GetObjectBackOnChange && !DataLoader.ModConfig.GetObjectBackImmediately)
            {
                var objectPerformRemoveAction = typeof(Object).GetMethod("performRemoveAction");
                var objectOverridesPerformRemoveAction = typeof(ObjectOverrides).GetMethod("PerformRemoveAction");
                harmony.Patch(objectPerformRemoveAction, new HarmonyMethod(objectOverridesPerformRemoveAction), null);
            }
        }
    }
}
