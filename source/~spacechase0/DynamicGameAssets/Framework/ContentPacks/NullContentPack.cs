/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace DynamicGameAssets.Framework.ContentPacks
{
    internal class NullContentPack : IContentPack
    {
        public string DirectoryPath => null;

        public IManifest Manifest => new NullManifest();

        public ITranslationHelper Translation => new NullTranslationHelper();

        public string GetActualAssetKey(string key)
        {
            return key;
        }

        public bool HasFile(string path)
        {
            return false;
        }

        public T LoadAsset<T>(string key)
        {
            return default;
        }

        public TModel ReadJsonFile<TModel>(string path) where TModel : class
        {
            return default;
        }

        public void WriteJsonFile<TModel>(string path, TModel data) where TModel : class
        {
        }
    }
}
