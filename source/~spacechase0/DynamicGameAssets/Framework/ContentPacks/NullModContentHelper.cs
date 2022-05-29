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
    internal class NullModContentHelper : IModContentHelper
    {
        public string ModID => "null";

        public IAssetName GetInternalAssetName(string relativePath)
        {
            return null; // Probably should implement this kinda...
        }

        public IAssetData GetPatchHelper<T>(T data, string relativePath = null) where T : notnull
        {
            return null; // Probably should implement this kinda...
        }

        public T Load<T>(string relativePath) where T : notnull
        {
            return default(T);
        }
    }
}
