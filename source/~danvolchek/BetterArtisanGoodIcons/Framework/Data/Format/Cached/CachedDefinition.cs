/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetterArtisanGoodIcons.Framework.Data.Loading;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BetterArtisanGoodIcons.Framework.Data.Format.Cached
{
    internal class CachedDefinition
    {
        private Texture2D texture;

        private IDictionary<int, Rectangle> positions = new Dictionary<int, Rectangle>();


        public CachedDefinition(IMonitor monitor, Texture2D texture, ItemIndicator[] items)
        {
            this.texture = texture;

            int x = 0;
            int y = 0;

            foreach (ItemIndicator item in items)
            {
                if (item.TryLoad(out int itemId))
                {
                    this.positions[itemId] = new Rectangle(x, y, VanillaArtisanGood.SpriteSize, VanillaArtisanGood.SpriteSize);
                }
                else
                {
                    item.LoadErrorMessage(monitor, "source item");
                }

                x += VanillaArtisanGood.SpriteSize;
                if (x >= texture.Width)
                {
                    x = 0;
                    y += VanillaArtisanGood.SpriteSize;
                }
            }
        }

        public bool TryGetValue(int id, out Rectangle mainPosition, out Texture2D texture)
        {
            texture = this.texture;
            return this.positions.TryGetValue(id, out mainPosition);
        }
    }
}
