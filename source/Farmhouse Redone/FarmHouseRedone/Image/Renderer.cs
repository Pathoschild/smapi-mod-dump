using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace FarmHouseRedone.Image
{
    public static class Renderer
    {
        public static Texture2D testRender;

        public static ImageInfo render(int width, int height, int pointRadius)
        {
            //Test code
            ImageInfo canvas = new ImageInfo(Color.Transparent, width, height);
            Vector2 point = new Vector2(Game1.random.Next(width), Game1.random.Next(height));

            Vector2 pointB = new Vector2(Game1.random.Next(width), Game1.random.Next(height));

            List<Model> models = new List<Model>();

            models.Add(new Model(pointRadius, point));
            models.Add(new Model(pointRadius, pointB));

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    double highest = 0;
                    int angle = 0;
                    foreach(Model model in models)
                    {
                        if(model.rayHits(new Vector2(x, y)))
                        {
                            double hitHeight = model.getHeight(new Vector2(x, y));
                            if(hitHeight >= highest)
                            {
                                highest = hitHeight;
                                angle = model.getLuminenceDiffuse(new Vector2(x, y), Vector3.Zero);
                            }
                        }
                    }
                    //float greyScale = (float)(highest / pointRadius);
                    canvas.setPixel(x, y, new Color(angle, angle, angle));

                    //double proximityA = Math.Sqrt(Math.Pow(x - point.X, 2) + Math.Pow(y - point.Y, 2));
                    //double proximityB = Math.Sqrt(Math.Pow(x - pointB.X, 2) + Math.Pow(y - pointB.Y, 2));

                    //int proximity = (int)Math.Min(proximityA, proximityB);
                    //float distance = (float)(Math.Min(proximityA, proximityB) / pointRadius);
                    //if (proximity < pointRadius)
                    //{
                    //    canvas.setPixel(x, y, new Color(1f - distance, 1f - distance, 1f - distance));
                    //}
                    //if(proximity == pointRadius)
                    //{
                    //    canvas.setPixel(x, y, Color.Black);
                    //}
                }
            }

            return canvas;
        }
    }
}
