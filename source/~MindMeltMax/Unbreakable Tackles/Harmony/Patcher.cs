/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Harmony;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unbreakable_Tackles.Harmony
{
    public class Patcher
    {
        public static void Init()
        {
            HarmonyInstance harmony = HarmonyInstance.Create(ModEntry.IHelper.ModRegistry.ModID);

            if (ModEntry.IConfig.consumeBait)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(FishingRod), "doDoneFishing", new[] { typeof(bool) }),
                    postfix: new HarmonyMethod(typeof(FishingRodPatches), nameof(FishingRodPatches.doDoneFishing_postfix))
                );
            }
            else
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.doneFishing), new[] { typeof(Farmer), typeof(bool) }),
                    prefix: new HarmonyMethod(typeof(FishingRodPatches), nameof(FishingRodPatches.doneFishing_prefix))
                );
            }
        }
    }
}
