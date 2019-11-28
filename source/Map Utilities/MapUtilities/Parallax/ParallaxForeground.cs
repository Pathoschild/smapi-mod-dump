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
