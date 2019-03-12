using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Moongates
{
    public class MoongateNPC : NPC
    {
        public MoongateNPC() : base()
        {
            ResetSprite();
        }
        public MoongateNPC(string defaultMap, string name) : base(null, Vector2.Zero, defaultMap, 2, name, false, null, Game1.content.Load<Texture2D>("Portraits\\Haley"))
        {
            ResetSprite();
        }

        public override void update(GameTime time, GameLocation location, long id, bool move)
        {
            base.update(time, location, id, move);
            Sprite.Animate(time, 0, 4, 200f);
        }

        public override bool collideWith(StardewValley.Object o)
        {
            return false;
        }

        public void DoGateWarp(MoongateNPC mnpc)
        {
            var ymod = bwdyworks.Modworks.Locations.IsTilePathableAndClear(mnpc.currentLocation, new Point((int)mnpc.getTileLocation().X, (int)mnpc.getTileLocation().Y + 1)) ? 1 : -1;
            Game1.player.warpFarmer(new Warp(mnpc.getTileLocationPoint().X, mnpc.getTileLocationPoint().Y + 1, mnpc.currentLocation.Name, mnpc.getTileLocationPoint().X, mnpc.getTileLocationPoint().Y + ymod, false));
        }

        public override void collisionWithFarmerBehavior()
        {
            if (Name == "MoongateFlow")
            {
                Game1.showGlobalMessage("Moongate: Ebb");
                DoGateWarp(Mod.MoongateEbb);
            }
            else if (Name == "MoongateEbb")
            {
                Game1.showGlobalMessage("Moongate: Flow");
                DoGateWarp(Mod.MoongateFlow);
            }
            else if (Name == "MoongateWane")
            {
                Game1.showGlobalMessage("Moongate: Wax");
                DoGateWarp(Mod.MoongateWax);
            }
            else if (Name == "MoongateWax")
            {
                Game1.showGlobalMessage("Moongate: Wane");
                DoGateWarp(Mod.MoongateWane);
            }
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            Sprite.Animate(time, 0, 4, 200f);

            farmerPassesThrough = true;
            if(Game1.player.currentLocation == currentLocation)
            {
                if(Game1.player.getTileLocationPoint() == getTileLocationPoint())
                {
                    collisionWithFarmerBehavior();
                }
            }
        }

        public void ResetForMapEntry()
        {
            ResetSprite();
        }

        public void ResetSprite()
        {
            if (Name == "MoongateFlow" || Name == "MoongateEbb")
                Sprite = new AnimatedSprite(Mod.TextureMoongateTidal, 0, 32, 64) { framesPerAnimation = 4 };
            else Sprite = new AnimatedSprite(Mod.TextureMoongateLunar, 0, 32, 64) { framesPerAnimation = 4 };
            drawOffset.X = -34;
            drawOffset.Y = -34;
            HideShadow = true;
            farmerPassesThrough = true;
        }
    }
}
