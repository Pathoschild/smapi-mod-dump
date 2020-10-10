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
    /// <summary>A content source that comes from this mod. </summary>
    internal class BAGIDataSource : IDataSource
    {
        private readonly IModHelper helper;
        public Format.Unloaded.UnloadedData UnloadedData { get; }

        public BAGIDataSource(IModHelper helper)
        {
            this.helper = helper;
            this.UnloadedData = this.helper.Data.ReadJsonFile<Format.Unloaded.UnloadedData>("assets/data.json");
            this.Manifest = this.helper.ModRegistry.Get(this.helper.ModRegistry.ModID).Manifest;
        }


        public IManifest Manifest { get; }

        public T Load<T>(string path)
        {
            return this.helper.Content.Load<T>(path);
        }

    }
}
