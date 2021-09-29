/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TwinBuilderOne/StardewValleyMods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System.IO;

namespace MoreBundles
{
    class BundleDataLoader : IAssetLoader
    {
        public bool CanLoad<T>(IAssetInfo asset) => asset.AssetNameEquals("Data\\ExpertBundles");
        public T Load<T>(IAssetInfo asset) => JsonConvert.DeserializeObject<T>(File.ReadAllText(Path.Combine(ModEntry.ModHelper.DirectoryPath, "assets", "Bundles.json")));
    }
}
