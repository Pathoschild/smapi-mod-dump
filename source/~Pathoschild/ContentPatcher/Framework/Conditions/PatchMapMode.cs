/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace ContentPatcher.Framework.Conditions
{
    /// <summary>Indicates how a map should be patched.</summary>
    public enum PatchMapMode
    {
        /// <summary>Replace matching tiles. Target tiles missing in the source area are kept as-is.</summary>
        Overlay,

        /// <summary>Replace all tiles on layers that exist in the source map.</summary>
        ReplaceByLayer,

        /// <summary>Replace all tiles with the source map.</summary>
        Replace
    }
}
