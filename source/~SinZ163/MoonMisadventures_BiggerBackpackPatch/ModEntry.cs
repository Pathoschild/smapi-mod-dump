/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoonMisadventures_BiggerBackpackPatch
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(this.ModManifest.UniqueID);
            ObjectPatches.Initialize(Monitor);
            if (helper.ModRegistry.IsLoaded("bcmpinc.WearMoreRings"))
            {
                harmony.Patch(
                    original: AccessTools.Constructor(typeof(InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                    postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.InventoryPage_Constructor__Postfix))
                );
            }

            harmony.Patch(
                original: AccessTools.Method(Type.GetType("MoonMisadventures.Patches.InventoryPageNecklaceDrawPatch,MoonMisadventures"), "Postfix"),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.DrawHoverPatch_Prefix))
            );
            harmony.Patch(
                original: AccessTools.Method(Type.GetType("MoonMisadventures.Patches.InventoryPageNecklaceHoverPatch,MoonMisadventures"), "Postfix"),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.DrawHoverPatch_Prefix))
            );
            this.Monitor.Log("This mod overrides a patch by Moon Misadventures. If you notice issues with Moon Misadventures, make sure it happens without this mod before reporting it to the MoonMisadventures page.", LogLevel.Trace);
        }
    }
}
