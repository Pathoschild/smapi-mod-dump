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