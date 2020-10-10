/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace BetterArtisanGoodIcons.Framework.Data.Format.Loaded
{
    internal class LoadedData
    {
        public IManifest Manifest { get; set; }

        public LoadedDefinition[] ArtisanGoods { get; set; } = null;

        public bool CanBeOverwritten { get; set; } = false;
    }
}
