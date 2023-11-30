/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Bundles;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Textures
{
    public static class BundleIcons
    {
        public const string BUNDLE_SUFFIX = "bundle";

        public static Texture2D GetBundleIcon(IModHelper modHelper, string bundleName)
        {
            if (Game1.content.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) is not IGraphicsDeviceService service)
            {
                throw new InvalidOperationException("No Graphics Device Service");
            }

            var currentModFolder = modHelper.DirectoryPath;
            if (!Directory.Exists(currentModFolder))
            {
                throw new InvalidOperationException("Could not find StardewArchipelago folder");
            }

            var texturesFolder = "Textures";
            var bundlesFolder = "Bundles";
            var cleanName = bundleName.Replace("'", "").Replace(" ", "_");
            var fileNameBundleName = $"{cleanName}_{BUNDLE_SUFFIX}.png";
            var pathToTexture = Path.Combine(currentModFolder, texturesFolder, bundlesFolder, fileNameBundleName);
            if (File.Exists(pathToTexture))
            {
                return Texture2D.FromFile(service.GraphicsDevice, pathToTexture);
            }

            return null;
        }
    }
}
