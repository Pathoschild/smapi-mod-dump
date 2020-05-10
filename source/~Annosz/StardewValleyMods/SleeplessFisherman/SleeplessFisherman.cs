using Harmony;
using StardewModdingAPI;
using System;

namespace SleeplessFisherman
{
    public class SleeplessFisherman : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.doEmote), new Type[] { typeof(int) }),
                prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.doEmote_Prefix))
            );
        }
    }
}