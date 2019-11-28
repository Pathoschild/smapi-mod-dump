using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Critters.PackagedCritters
{
    public class Firefly
    {
        private bool glowing;
        private int glowTimer;
        private int id;
        private Vector2 motion;
        private LightSource light;
        private Vector2 relativePosition;

        public Firefly(Vector2 position)
        {
            relativePosition = Vector2.Zero;
            this.motion = new Vector2((float)Game1.random.Next(-10, 11) * 0.1f, (float)Game1.random.Next(-10, 11) * 0.1f);
            this.id = (int)((double)position.X * 10099.0 + (double)position.Y * 77.0 + (double)Game1.random.Next(99999));
            this.light = new LightSource(4, position, (float)Game1.random.Next(4, 6) * 0.1f, Color.Purple * 0.8f, this.id, LightSource.LightContext.None, 0L);
            this.glowing = true;
            Game1.currentLightSources.Add(this.light);
        }

        public void update(GameTime time, GameLocation environment, Vector2 parentLocation)
        {
            relativePosition = relativePosition + motion;
            motion.X += Game1.random.Next(-1, 2) * 0.1f;
            motion.Y += Game1.random.Next(-1, 2) * 0.1f;
            if (motion.X < -1.0)
                motion.X = -1f;
            if (motion.X > 1.0)
                motion.X = 1f;
            if (motion.Y < -1.0)
                motion.Y = -1f;
            if (motion.Y > 1.0)
                motion.Y = 1f;
            if (Game1.random.NextDouble() < 0.01)
                glowing = !glowing;
            if (!Game1.isDarkOut())
                glowing = false;
            if (glowing)
                light.position.Value = relativePosition + parentLocation;
            else
                light.position.Value = new Netcode.NetVector2(new Vector2(-1000, -1000));
        }

        public void drawAboveFrontLayer(SpriteBatch b, Vector2 parentLocation)
        {
            if (!glowing && Game1.isDarkOut())
                return;
            b.Draw(
                Game1.staminaRect,
                Game1.GlobalToLocal(relativePosition + parentLocation),
                new Rectangle?(Game1.staminaRect.Bounds),
                this.glowing ? Color.White : Color.Brown,
                0.0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f
            );
        }
    }
}
