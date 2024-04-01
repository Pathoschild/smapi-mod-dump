/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using FashionSense.Framework.Interfaces.API;
using FSApi = FashionSense.Framework.Interfaces.API.IApi;
using JsonAssets;
using StardewValley;
using System.Collections.Generic;

namespace ichortower_HatMouseLacey
{
    internal class LCHatString
    {
        /*
         * Returns a string identifying the hat currently worn by the given
         * Farmer.
         * Returns null if no hat could be detected.
         *
         * Supports vanilla hats (who.hat, 0 <= id <= 93),
         *   "SV|Hat Name"
         * modded hats (who.hat, any other id),
         *   "MOD|Hat Name"
         * and fashion sense hats (read from FS API)
         *   "FS|Hat Name"
         */
        public static string GetCurrentHatString(Farmer who)
        {
            /* check FS first, since it overrides physical hats */
            var fsapi = HML.ModHelper.ModRegistry.GetApi<FSApi>(
                    "PeacefulEnd.FashionSense");
            if (fsapi != null) {
                var pair = fsapi.GetCurrentAppearanceId(FSApi.Type.Hat, who);
                if (pair.Key) {
                    return $"FS|{pair.Value}";
                }
            }
            if (who.hat.Value != null) {
                /* vanilla hats should have plain integer IDs. also safety
                 * check for id within expected vanilla range */
                if (int.TryParse(who.hat.Value.ItemId, out var vanillaId)) {
                    if (vanillaId <= 93) {
                        return $"SV|{who.hat.Value.Name}";
                    }
                }
                return $"MOD|{who.hat.Value.ItemId}";
            }
            return null;
        }
    }
}
