/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Incognito357/PrairieKingUIEnhancements
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Minigames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Minigames.AbigailGame;

namespace PrairieKingUIEnhancements
{
    public abstract class Renderable
    {
        private static Texture2D pixel;
        public virtual void Render(SpriteBatch b, Config config, AbigailGame game) { }
        public virtual void Tick(Config config, AbigailGame game) { }
        public virtual void SaveLoaded(Save save) { }
        public virtual void Save(ref Save save) { }

        protected static void DrawRect(SpriteBatch b, Rectangle rect, Color c)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(b.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
            }
            b.Draw(pixel, rect, c);
        }

        private static readonly string[] sizes = { "", "K", "M", "B", "T", "Q" };
        protected static string ToShortString(int value)
        {
            double shortVal = value;
            int order = 0;
            while (shortVal > 1000 && order < sizes.Length - 1)
            {
                order++;
                shortVal /= 1000.0f;
            }
            return string.Format(shortVal > 100 ? "{0:0#}{1}" : "{0:0.#}{1}", shortVal, sizes[order]);
        }
    }
}
