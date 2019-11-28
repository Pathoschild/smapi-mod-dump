using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Particles
{
    public class Particle
    {
        public int lifetime;
        public int lifeDuration;
        public Vector2 position;
        public float scale;
        public float rotation;
        public int spriteIndex;

        public Particle(Vector2 position, float scale, float rotation, int lifetime, int lifeDuration)
        {
            this.lifetime = lifetime;
            this.lifeDuration = lifeDuration;
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }

        public void draw(SpriteBatch b, Texture2D spriteSheet, Rectangle sprite, float depth, Vector2 local, Color tint)
        {
            b.Draw(spriteSheet, new Vector2(position.X + local.X, position.Y + local.Y), sprite, tint, rotation, new Vector2(sprite.Width / 2f, sprite.Height / 2f), scale, SpriteEffects.None, depth);
        }
    }
}
