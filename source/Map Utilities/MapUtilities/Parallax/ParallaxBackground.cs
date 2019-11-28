using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Parallax
{
    public class ParallaxBackground : Background
    {
        public List<ParallaxLayer> layers;

        public ParallaxBackground()
        {
            this.layers = new List<ParallaxLayer>();
        }

        public ParallaxBackground(List<ParallaxLayer> layers)
        {
            this.layers = layers;
        }

        public new void update(xTile.Dimensions.Rectangle viewport)
        {
            foreach(ParallaxLayer layer in layers)
            {
                layer.update(viewport);
            }
        }

        public new void draw(SpriteBatch b)
        {
            foreach(ParallaxLayer layer in layers)
            {
                layer.draw(b);
            }
        }
    }
}
