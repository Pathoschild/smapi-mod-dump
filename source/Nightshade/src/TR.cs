/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using StardewModdingAPI;

namespace ichortower
{
    public class TR
    {
        public static IModHelper Helper = Nightshade.instance.Helper;

        public static string Get(string key) {
            return Helper.Translation.Get(key);
        }
    }
}
