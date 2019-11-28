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
    public class ParallaxLayer
    {
        public const int Tile = 0;
        public const int Stretch = 1;
        public const int Fill = 2;
        public const int Fit = 3;
        public const int None = 4;

        public Texture2D layerImage;
        public float depth;
        public Vector2 position = Vector2.Zero;

        public int fillMode;
        public float zoomScale;

        public bool onHorizon;

        public float horizonOffset;

        public List<Particles.ParticleSystem> particleSystems;
        public Dictionary<Vector2, StardewValley.TerrainFeatures.TerrainFeature> features;
        public Dictionary<Vector2, StardewValley.Object> objects;

        public ParallaxLayer(Texture2D layerImage, float depth, bool useHorizon, int horizon)
        {
            this.layerImage = layerImage;
            this.depth = depth;
            this.fillMode = Tile;
            this.zoomScale = 2f;
            this.onHorizon = useHorizon;
            this.horizonOffset = horizon - (layerImage.Height * zoomScale);
            this.particleSystems = new List<Particles.ParticleSystem>();
            this.objects = new Dictionary<Vector2, StardewValley.Object>();
            this.features = new Dictionary<Vector2, StardewValley.TerrainFeatures.TerrainFeature>();
            Logger.log("Set horizon offset to " + horizonOffset + "; " + horizon + " - (" + layerImage.Height + " * " + zoomScale + ")");
        }

        public void update(xTile.Dimensions.Rectangle viewport)
        {
            Vector2 mapOrigin = Game1.GlobalToLocal(viewport, Vector2.Zero);
            position.X = mapOrigin.X * depth;
            position.Y = mapOrigin.Y * depth + (onHorizon ? horizonOffset : 0);
            foreach(Particles.ParticleSystem system in particleSystems)
            {
                system.update(Game1.currentGameTime, Game1.currentLocation);
            }
            foreach(Vector2 location in objects.Keys)
            {
                objects[location].updateWhenCurrentLocation(Game1.currentGameTime, Game1.currentLocation);
            }
            foreach(Vector2 location in features.Keys)
            {
                features[location].tickUpdate(Game1.currentGameTime, new Vector2(location.X + position.X, location.Y + position.Y), Game1.currentLocation);
            }
        }

        public void draw(SpriteBatch b)
        {
            float zoom = Game1.options.zoomLevel;

            //Tile causes the layer to repeat in tiles on the x-axis.  Useful for close-up layers which will move by quickly.
            if (fillMode == Tile)
            {
                int width = (int)(layerImage.Width * zoomScale);
                int minCount = (int)Math.Ceiling((float)Game1.viewport.Width / width) + 1;

                int startX = (int)(position.X > 0 ? (width * -1) + position.X : position.X);

                for(int index = 0; index < minCount; index++)
                {
                    b.Draw(
                        layerImage,
                        new Vector2(startX + (index * width), position.Y),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, layerImage.Width, layerImage.Height)),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        zoomScale,
                        SpriteEffects.None,
                        1E-07f + (1E-07f * depth)
                    );
                }
            }
            //None causes the layer to not extend in any way.  The image is scaled to the game's pixel zoom and otherwise left as-is.
            else if (fillMode == None)
            {
                b.Draw(
                    layerImage,
                    position,
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, layerImage.Width, layerImage.Height)),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    zoomScale,
                    SpriteEffects.None,
                    1E-07f + (1E-07f * depth)
                );
            }
            //Stretch causes the layer to be stretched in the x- and y-axis independently, which can affect the aspect ratio.  Useful for a panorama that just needs to fill the whole screen.
            else if (fillMode == Stretch)
            {
                b.Draw(
                    layerImage,
                    new Rectangle((int)position.X, (int)position.Y, (int)(Game1.viewport.Width * zoom), (int)(Game1.viewport.Height * zoom)),
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, layerImage.Width, layerImage.Height)),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1E-07f + (1E-07f * depth)
                );
            }
            //Fill causes the layer to scale uniformly until it covers the viewport's area.  Useful to make a panoramic shot whose aspect ratio is important, and cropping is not an issue.
            else if (fillMode == Fill)
            {
                float scaleMult = Math.Max((Game1.viewport.Width / layerImage.Width), (Game1.viewport.Height / layerImage.Height));
                b.Draw(
                    layerImage,
                    new Rectangle((int)position.X, (int)position.Y, (int)(layerImage.Width * scaleMult), (int)(layerImage.Height * scaleMult)),
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, layerImage.Width, layerImage.Height)),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1E-07f + (1E-07f * depth)
                );
            }
            //Fit causes the layer to scale uniformly until it covers the viewport's area.  Useful to make a panoramic shot whose aspect ratio is important, and cropping is not an issue.
            else if (fillMode == Fit)
            {
                float scaleMult = Math.Min((Game1.viewport.Width / (layerImage.Width)), (Game1.viewport.Height / (layerImage.Height)));
                b.Draw(
                    layerImage,
                    new Rectangle((int)position.X, (int)position.Y, (int)(layerImage.Width * scaleMult), (int)(layerImage.Height * scaleMult)),
                    new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, layerImage.Width, layerImage.Height)),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1E-07f + (1E-07f * depth)
                );
            }

            foreach(Particles.ParticleSystem system in particleSystems)
            {
                system.drawAsChild(b, new Vector2(position.X, position.Y), (1E-06f + (1E-07f * depth) + 1E-08f));
            }
        }
    }
}
