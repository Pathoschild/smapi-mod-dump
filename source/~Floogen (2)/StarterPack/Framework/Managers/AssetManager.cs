/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/Archery
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.IO;

namespace StarterPack.Framework.Managers
{
    internal class AssetManager
    {
        internal string assetFolderPath;

        // Textures
        internal readonly Texture2D noteTextures;

        public AssetManager(IModHelper helper)
        {
            // Get the asset folder path
            assetFolderPath = helper.ModContent.GetInternalAssetName(Path.Combine("Framework", "Assets")).Name;

            // Load in the assets
            //noteTextures = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "Notes.png"));
        }
    }
}
