/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MadaraUchiha/NonDestructiveNPCs
**
*************************************************/

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
