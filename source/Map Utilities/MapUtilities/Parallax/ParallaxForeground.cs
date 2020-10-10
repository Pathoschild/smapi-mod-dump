/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mjSurber/Map-Utilities
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Parallax
{
    public class ParallaxForeground
    {
        public List<ParallaxLayer> layers;

        public ParallaxForeground()
        {
            this.layers = new List<ParallaxLayer>();
        }

        public ParallaxForeground(List<ParallaxLayer> layers)
        {
            this.layers = layers;
        }

        public void update(xTile.Dimensions.Rectangle viewport)
        {
            foreach (ParallaxLayer layer in layers)
            {
                layer.update(viewport);
            }
        }

        public void draw(SpriteBatch b)
        {
            foreach (ParallaxLayer layer in layers)
            {
                layer.draw(b);
            }
        }
    }
}
