/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using BetterArtisanGoodIcons.Framework.Data.Format.Unloaded;
using StardewModdingAPI;

namespace BetterArtisanGoodIcons.Framework.Data.Loading
{
    /// <summary>A content source that comes from Content Packs. </summary>
    internal class ContentPackDataSource : IDataSource
    {
        private readonly IContentPack pack;

        public ContentPackDataSource(IContentPack pack)
        {
            this.pack = pack;
            this.UnloadedData = this.pack.ReadJsonFile<Format.Unloaded.UnloadedData>("data.json");
            this.Manifest = this.pack.Manifest;
        }

        public Format.Unloaded.UnloadedData UnloadedData { get; }
        public IManifest Manifest { get; }

        public T Load<T>(string path)
        {
            return this.pack.LoadAsset<T>(path);
        }
    }
}
