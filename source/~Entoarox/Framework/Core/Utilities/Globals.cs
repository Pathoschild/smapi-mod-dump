/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace Entoarox.Framework.Core.Utilities
{
    internal static class Globals
    {
        /*********
        ** Public methods
        *********/
        public static string GetModName(IModLinked mod)
        {
            return EntoaroxFrameworkMod.SHelper.ModRegistry.Get(mod.ModID).Manifest.Name;
        }
    }
}
