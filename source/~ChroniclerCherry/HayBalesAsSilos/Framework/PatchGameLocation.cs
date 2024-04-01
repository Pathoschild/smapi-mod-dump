/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using System.Linq;
using StardewValley;

namespace HayBalesAsSilos.Framework
{
    public class PatchGameLocation
    {
        internal static void After_GetHayCapacity(ref GameLocation __instance, ref int __result)
        {
            if (!ModEntry.GetAllAffectedMaps().Contains(Game1.currentLocation))
                return;

            if (__result > 0 || !ModEntry.Config.RequiresConstructedSilo)
            {
                int hayBales = __instance.Objects.Values.Count(p => p.QualifiedItemId == ModEntry.HayBaleQualifiedId);
                if (hayBales == 0)
                    return;

                __result += hayBales * ModEntry.Config.HayPerBale;
            }
        }
    }
}
