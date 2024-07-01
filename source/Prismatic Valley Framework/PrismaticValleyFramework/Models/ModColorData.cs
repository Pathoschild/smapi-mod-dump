/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jolly-Alpaca/PrismaticValleyFramework
**
*************************************************/

namespace PrismaticValleyFramework.Models {
    /// <summary>
    /// Data structure for override color settings for entities that do not have a Custom Fields field (e.g. boots)
    /// </summary>
    public class ModColorData
    {
        public string Color;
        public string? Palette;
        public string? TextureTarget;
    }
}