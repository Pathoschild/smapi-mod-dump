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
         * json assets hats (who.hat, id >= 94),
         *   "JA|Hat Name"
         * and fashion sense hats (read from FS API)
         *   "FS|Hat Name"
         *
         * If the hat is "real" and isn't vanilla or json assets, it counts
         * as "other" and returns:
         *   "OT|Hat Name"
         */
        public static string GetCurrentHatString(Farmer who)
        {
            /* check FS first, since it overrides physical hats */
            var fsapi = ModEntry.HELPER.ModRegistry.GetApi<FSApi>(
                    "PeacefulEnd.FashionSense");
            if (fsapi != null) {
                var pair = fsapi.GetCurrentAppearanceId(FSApi.Type.Hat, who);
                if (pair.Key) {
                    return $"FS|{pair.Value}";
                }
            }
            if (who.hat.Value != null) {
                int hatId = who.hat.Value.which.Value;
                if (hatId <= 93) {
                    return $"SV|{who.hat.Value.Name}";
                }
                else {
                    var jaapi = ModEntry.HELPER.ModRegistry.GetApi<JsonAssets.IApi>(
                            "spacechase0.JsonAssets");
                    if (jaapi != null) {
                        var allHats = jaapi.GetAllHatIds();
                        foreach (var pair in allHats) {
                            if (pair.Value == hatId) {
                                return $"JA|{who.hat.Value.Name}";
                            }
                        }
                    }
                    return $"OT|{who.hat.Value.Name}";
                }
            }
            return null;
        }
    }
}
