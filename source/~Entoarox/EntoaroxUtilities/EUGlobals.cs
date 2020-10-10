/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Utilities
{
    public static class EUGlobals
    {
        public static IEntoUtilsApi Api => Internals.Api.EntoUtilsApi.Instance;

        public delegate bool TypeIdResolverDelegate(StardewValley.Item item, ref string typeId);
    }
}
