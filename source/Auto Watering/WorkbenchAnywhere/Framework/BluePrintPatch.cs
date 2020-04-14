using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkbenchAnywhere.Framework
{
    public static class BluePrintPatch
    {
        private static IMonitor _monitor;
        private static ModConfig _config;
        private static MaterialStorage _materialStorage;

        public static void Initialize(IMonitor monitor, ModConfig config, MaterialStorage materialStorage)
        {
            _monitor = monitor;
            _config = config;
            _materialStorage = materialStorage;
        }

        public static bool doesFarmerHaveEnoughResourcesToBuild_Prefix(BluePrint __instance)
        {
            try
            {
                // if not enabled or not buildings/upgrade blue print
                return (!_config.CarpenterUsesMaterialChests ||
                    !(__instance.blueprintType == "Buildings" || __instance.blueprintType == "Upgrades"));
            }
            catch (Exception ex)
            {
                _monitor.Log($"Unhandled exception in {nameof(doesFarmerHaveEnoughResourcesToBuild_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        public static void doesFarmerHaveEnoughResourcesToBuild_Postfix(BluePrint __instance, ref bool __result)
        {
            if (!__result)
                __result = _materialStorage.HaveMatsFor(__instance);
        }

        public static bool consumeResources_Prefix(BluePrint __instance)
        {
            try
            {
                _monitor.Log($"Called consumeResources_Prefix with {__instance.blueprintType}", LogLevel.Warn);

                // if not enabled or not buildings/upgrade blue print
                if (!_config.CarpenterUsesMaterialChests ||
                    !(__instance.blueprintType == "Buildings" || __instance.blueprintType == "Upgrades"))
                    return true;

                _materialStorage.ConsumeResources(__instance);
                return false;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Unhandled exception in {nameof(doesFarmerHaveEnoughResourcesToBuild_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}
