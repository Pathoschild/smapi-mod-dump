using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Parallax
{
    public static class SkyHandler
    {
        public static DynamicSky sky;

        public static void init()
        {
            sky = new DynamicSky((Texture2D)Loader.loader.Load<Texture2D>("Content/Sky/SkyGradient.png", StardewModdingAPI.ContentSource.ModFolder), (Texture2D)Loader.loader.Load<Texture2D>("Content/Sky/Stars.png", StardewModdingAPI.ContentSource.ModFolder));
        }
    }
}
