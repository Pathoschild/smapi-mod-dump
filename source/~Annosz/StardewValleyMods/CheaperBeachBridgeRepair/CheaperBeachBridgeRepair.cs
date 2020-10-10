/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/StardewValleyModding
**
*************************************************/

using Harmony;
using StardewModdingAPI;

namespace CheaperBeachBridgeRepair
{
    public class CheaperBeachBridgeRepair : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            BeachPatches.Initialize(Helper.ReadConfig<ModConfig>(), Monitor);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Locations.Beach), nameof(StardewValley.Locations.Beach.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(BeachPatches), nameof(BeachPatches.AnswerDialogueAction_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Locations.Beach), nameof(StardewValley.Locations.Beach.checkAction)),
               prefix: new HarmonyMethod(typeof(BeachPatches), nameof(BeachPatches.checkAction_Prefix))
            );
        }
    }
}