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
    public enum BreakableType
    {
        /// <summary>
        ///     A pickaxe is required to break this resource.
        /// </summary>
        Pickaxe,

        /// <summary>
        ///     An axe is required to break this resource.
        /// </summary>
        Axe,

        /// <summary>
        ///     A hoe is required to dig up artifact spots.
        /// </summary>
        Hoe,

        /// <summary>
        ///     We don't want to deal with this object.
        /// </summary>
        NotAllowed
    }
}
