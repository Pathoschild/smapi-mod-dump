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

namespace RememberFacedDirection
{
    public class RememberFacedDirection : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            RememberFacedDirectionPatches.Initialize(Monitor);

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.pressActionButton)),
               prefix: new HarmonyMethod(typeof(RememberFacedDirectionPatches), nameof(RememberFacedDirectionPatches.Game1_PressActionButton_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.holdUpItemThenMessage)),
               prefix: new HarmonyMethod(typeof(RememberFacedDirectionPatches), nameof(RememberFacedDirectionPatches.Farmer_HoldUpItemThenMessage_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.doneEating)),
               postfix: new HarmonyMethod(typeof(RememberFacedDirectionPatches), nameof(RememberFacedDirectionPatches.Farmer_DoneEating_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.showReceiveNewItemMessage)),
               postfix: new HarmonyMethod(typeof(RememberFacedDirectionPatches), nameof(RememberFacedDirectionPatches.Farmer_ShowReceiveNewItemMessage_Postfix))
            );
        }
    }
}