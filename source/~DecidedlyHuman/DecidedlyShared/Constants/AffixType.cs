/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

namespace DecidedlyShared.Constants
{
    /// <summary>
    /// Indicates the type of affix for a Harmony patch. Can be either a prefix, or a postfix.
    /// </summary>
    public enum AffixType
    {
        /// <summary>
        /// A prefix. This has the potential to override a method entirely.
        /// </summary>
        Prefix,

        /// <summary>
        /// A postfix. This runs at the end of a patched method.
        /// </summary>
        Postfix
    }
}
