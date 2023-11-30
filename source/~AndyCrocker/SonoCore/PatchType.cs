/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace SonoCore
{
    /// <summary>The types of a Harmony patch.</summary>
    public enum PatchType
    {
        /// <summary>A patch that gets applied to the beginning of a method.</summary>
        Prefix,

        /// <summary>A patch that changes the IL directly.</summary>
        Transpiler,

        /// <summary>A patch that gets applied to the end of a method.</summary>
        Postfix
    }
}
