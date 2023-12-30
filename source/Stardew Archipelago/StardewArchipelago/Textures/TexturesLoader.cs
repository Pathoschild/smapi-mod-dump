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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace StardewArchipelago.Textures
{
    public class TexturesLoader
    {
        public static Texture2D GetTexture(IModHelper modHelper, string texture)
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
                return null;
            }

            return Texture2D.FromFile(service.GraphicsDevice, relativePathToTexture);
        }
    }
}
