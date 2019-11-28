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
    public class Fireflies : Critter
    {
        List<Firefly> fireflies;

        public Fireflies(Vector2 position, int count=8)
        {
            this.baseFrame = -1;
            this.position = position * 64f;
            this.startingPosition = position * 64f;
            fireflies = new List<Firefly>();
            for(int i = 0; i < count; i++)
            {
                fireflies.Add(new Firefly(position));
            }
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            foreach(Firefly firefly in fireflies)
            {
                firefly.update(time, environment, position);
            }
            return (double)this.position.X < (double)sbyte.MinValue || (double)this.position.Y < (double)sbyte.MinValue || ((double)this.position.X > (double)environment.map.DisplayWidth || (double)this.position.Y > (double)environment.map.DisplayHeight);
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            foreach(Firefly firefly in fireflies)
            {
                firefly.drawAboveFrontLayer(b, position);
            }
        }
    }
}
