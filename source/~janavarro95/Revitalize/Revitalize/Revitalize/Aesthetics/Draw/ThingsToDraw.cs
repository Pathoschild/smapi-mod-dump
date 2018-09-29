using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Revitalize.Objects;
using Revitalize.Resources;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Draw
{
    class ThingsToDraw
    {

        public static void drawAllHuds()
        {
            Magic.MagicMonitor.drawMagicMeter();
        }

        public static void drawAllObjects()
        {
            /*
            foreach (var v in Lists.DecorationsToDraw)
            {
                if (Game1.player.currentLocation == v.thisLocation)
                {
                    v.draw(Game1.spriteBatch,(int)v.tileLocation.X,(int)v.tileLocation.Y);
                }
            }
            */
        }

        public static void drawAllFurniture()
        {

            //int i = 0;
            SpriteBatch b = Game1.spriteBatch;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            /*
            foreach (var v in Lists.DecorationsToDraw)
            {
                //Log.Async(i);
                //i++;
                if (v is Decoration && v.thisLocation==Game1.player.currentLocation)
                {
                    if (v.boundingBox.Height == 0 && v.boundingBox.Width == 0) (v as Decoration).drawInfront(b, (int)v.tileLocation.X, (int)v.tileLocation.Y);
                }
            }
            b.End();
            */
        }
    }
}
