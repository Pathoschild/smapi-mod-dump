using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities
{
    public static class Loader
    {
        public static IContentHelper loader;

        public static object load<Type>(string path, string modPath = "")
        {
            try
            {
                return loader.Load<Type>(path, ContentSource.GameContent);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException)
            {
                Logger.log("Could not find " + path + " in game content, using default (if provided)");
            }
            try
            {
                if (modPath.Length > 0)
                    path = modPath;
                return loader.Load<Type>(path, ContentSource.ModFolder);
            }
            catch (Microsoft.Xna.Framework.Content.ContentLoadException e)
            {
                Logger.log("Tried to load file " + path + ", but none could be found in either the game content or in MapUtilites!", LogLevel.Error);
                throw e;
            }
        }
    }
}
