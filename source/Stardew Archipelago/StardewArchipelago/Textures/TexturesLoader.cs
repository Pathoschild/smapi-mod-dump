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
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Textures
{
    public class TexturesLoader
    {
        public static Texture2D GetTexture(IMonitor monitor, IModHelper modHelper, string texture, LogLevel failureLogLevel = LogLevel.Error)
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

            const string texturesFolder = "Textures";
            var relativePathToTexture = Path.Combine(currentModFolder, texturesFolder, texture);
            if (!File.Exists(relativePathToTexture))
            {
                monitor.Log($"Tried to load texture '{relativePathToTexture}', but it couldn't be found!", failureLogLevel);
                return null;
            }

            monitor.Log($"Loading Texture file '{relativePathToTexture}'", LogLevel.Trace);
            return Texture2D.FromFile(service.GraphicsDevice, relativePathToTexture);
        }
    }
}
