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

namespace Archery.Framework.Managers
{
    internal class AssetManager
    {
        internal string assetFolderPath;

        // Base textures
        internal readonly Texture2D baseArmsTexture;
        internal readonly Texture2D crossbowArmsTexture;
        internal readonly Texture2D iconBowTexture;

        // Maps
        internal readonly string arenaMapPath;

        // Recolored textures
        internal Texture2D recoloredArmsTexture;
        internal Texture2D recoloredCrossbowArmsTexture;

        public AssetManager(IModHelper helper)
        {
            // Get the asset folder path
            assetFolderPath = helper.ModContent.GetInternalAssetName(Path.Combine("Framework", "Assets")).Name;

            // Load in the assets
            baseArmsTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "BowArms.png"));
            recoloredArmsTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "BowArms.png"));
            crossbowArmsTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "CrossbowArms.png"));
            recoloredCrossbowArmsTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "CrossbowArms.png"));
            iconBowTexture = helper.ModContent.Load<Texture2D>(Path.Combine(assetFolderPath, "BowIcon.png"));

            arenaMapPath = Path.Combine(assetFolderPath, "Maps", "Arena.tmx");
        }
    }
}
