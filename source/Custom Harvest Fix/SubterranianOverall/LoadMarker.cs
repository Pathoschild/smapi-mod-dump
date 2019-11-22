using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace SubterranianOverhaul
{
    public class LoadMarker : StardewValley.Object
    {
        static bool hasProcessed = true;

        public LoadMarker()
        {
            
        }

        public override bool isPassable()
        {
            return true;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            return;
        }
    }
}
