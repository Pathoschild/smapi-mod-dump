using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace RemoveEatSound
{
    public class RemoveEatSoundMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            PerformHarmonyPatches();
        }

        /// <summary>
        /// Patches the Game1.playSound method to not play the eat sound
        /// </summary>
        private void PerformHarmonyPatches()
        {
            try
            {
                Monitor.Log("Doing harmony patches...");

                HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

                MethodInfo originalMethod = AccessTools.Method(typeof(Game1), "playSound");
                MethodInfo prefixMethod = AccessTools.Method(typeof(EatSoundPatch), nameof(EatSoundPatch.Prefix));

                // Have Harmony take the original property get method, and replace it with the new one we defined in DailyLuckPatch
                harmony.Patch(
                    original: originalMethod,
                    prefix: new HarmonyMethod(prefixMethod));
            }
            catch (Exception e)
            {
                Monitor.Log("Failed to patch RemoveEatSound with Harmony" + e.ToString(), LogLevel.Error);
                throw;
            }
        }
    }
}
