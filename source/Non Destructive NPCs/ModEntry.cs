using Harmony;
using StardewModdingAPI;
using StardewValley;

namespace NonDestructiveNPCs
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.characterDestroyObjectWithinRectangle)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.neverDestroyObjectWithinRectangle))
            );
        }

        private static bool neverDestroyObjectWithinRectangle(ref bool __result)
        {
            __result = false;
            return false;
        }

    }
}
