/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;

namespace BetterArtisanGoodIcons.Framework.Data.Format.Loaded
{
    internal class LoadedDefinition
    {
        public ItemIndicator[] SourceItems { get; set; } = null;

        public ItemIndicator ArtisanGood { get; set; }

        public Texture2D Texture { get; set; } = null;
    }
}
