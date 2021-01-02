/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Underscore76/SDVPracticeMod
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace SpeedrunPractice.Framework
{
    class DrawHelper
    {
        private static Texture2D SolidColor;
        private const int OutlineWidth = 1;
        private static Color OutlineColor = Color.Black;
        static DrawHelper()
        {
            SolidColor = new Texture2D(Game1.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] data = new Color[1] { new Color(255, 255, 255, 255) };
            SolidColor.SetData(data);
        }


        public static void DrawProgressBar(SpriteBatch spriteBatch, Rectangle region, List<int> zoneCounts, List<Color> zoneColors)
        {
            // determine number of blocks
            int numZones = zoneCounts.Sum();
            if (numZones == 0)
                return;
            // normalize for weird float issues
            int zoneWidth = region.Width / numZones;
            region.Width = zoneWidth * numZones;
            // draw the base rectangle
            DrawOutlinedRect(spriteBatch, region, OutlineWidth, Color.White, Color.Black);

            // draw the different zones
            Rectangle zoneRegion = new Rectangle(region.X, region.Y, zoneWidth, region.Height);
            for (int i = 0; i < zoneCounts.Count; ++i)
            {
                for (int j = 0; j < zoneCounts[i]; j++)
                {
                    DrawOutlinedRect(spriteBatch, zoneRegion, OutlineWidth, zoneColors[i], Color.Black);
                    zoneRegion.X += zoneWidth;
                }
            }
        }

        public static void DrawProgressBar(SpriteBatch spriteBatch, Rectangle region, List<int> zoneCounts, List<Color> zoneColors, int highlightZone, Color highlightColor)
        {
            // determine number of blocks
            int numZones = zoneCounts.Sum();
            if (numZones == 0)
                return;
            // normalize for weird float issues
            int zoneWidth = region.Width / numZones;
            region.Width = zoneWidth * numZones;
            // draw the base rectangle
            DrawOutlinedRect(spriteBatch, region, OutlineWidth, Color.White, OutlineColor);

            // draw the different zones
            Rectangle zoneRegion = new Rectangle(region.X, region.Y, zoneWidth, region.Height);
            for (int i = 0; i < zoneCounts.Count; ++i)
            {
                for (int j = 0; j < zoneCounts[i]; j++)
                {
                    DrawOutlinedRect(spriteBatch, zoneRegion, OutlineWidth, zoneColors[i], OutlineColor);
                    zoneRegion.X += zoneWidth;
                }
            }

            // draw the highlight
            zoneRegion.X = region.X + zoneWidth * highlightZone;
            DrawOutlinedRect(spriteBatch, zoneRegion, OutlineWidth, highlightColor, OutlineColor);
        }

        public static void DrawOutlinedRect(SpriteBatch spriteBatch, Rectangle region, int lineWidth, Color interiorColor, Color outlineColor)
        {
            spriteBatch.Draw(SolidColor, region, outlineColor);
            spriteBatch.Draw(SolidColor, new Rectangle(region.X + lineWidth, region.Y + lineWidth, region.Width - 2 * lineWidth, region.Height - 2 * lineWidth), interiorColor);
        }
    }
}
