using Harmony;
using LuckyLeprechaunBoots.Utils;
using LuckyLeprechaunBoots;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LuckyLeprechaunBoots
{
    /// <summary>
    /// Base class for Lucky Leprechaun Boots mod
    /// </summary>
    public class LuckyLeprechaunBootsMod : Mod
    {
        /// <summary>
        /// Configuration for the mod, read from config.json
        /// </summary>
        public static ModConfig Config { get; private set; }

        /// <summary>
        /// Entry method for Lucky Leprechaun Boots mod
        /// </summary>
        /// <param name="helper">SMAPI API for helping with writing mods</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            if (!Config.Enabled)
                return;

            PerformHarmonyPatches();
        }

        /// <summary>
        /// Patches the Farmer's DailyLuck property to use our custom value, based on the mod's configuration
        /// </summary>
        private void PerformHarmonyPatches()
        {
            try
            {
                Monitor.Log("Doing harmony patches...");

                HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

                // The original property is from the core Stardew property Farmer.DailyLuck
                MethodInfo originalPropertyGetter = AccessTools.Property(typeof(Farmer), "DailyLuck").GetGetMethod(false);
                // The new property is calculated in DailyLuckPatch in the PatchDailyLuckPostfix method
                MethodInfo newPropertyGetter = AccessTools.Method(typeof(DailyLuckPatch), nameof(DailyLuckPatch.PatchDailyLuckPostfix));

                // Have Harmony take the original property get method, and replace it with the new one we defined in DailyLuckPatch
                harmony.Patch(
                    original: originalPropertyGetter,
                    postfix: new HarmonyMethod(newPropertyGetter));
            }
            catch(Exception e)
            {
                Monitor.Log("Failed to patch DailyLuck with Harmony" + e.ToString(), LogLevel.Error);
                throw;
            }
        }
    }
}
