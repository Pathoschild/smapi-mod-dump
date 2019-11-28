using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongates
{
    public class MGNPC : NPC
    {
        public override bool IsMonster
        {
            get
            {
                return false;// true; //keeps it from giving quests, etc. does throw an error on hitting it with a tool, but the error is caught.
            }
        }

        public MGNPC() : base()
        {
            ResetSprite();

            //Moongates.Mod.instance.Monitor.Log("Created mgnpc using constructor 2: " + Name);
        }

        public bool Colliding = false;
        public bool LastColliding = false;
        public MGNPC(string defaultMap, string name) : base(null, Vector2.Zero, defaultMap, 2, name, false, null, Game1.content.Load<Texture2D>("Portraits\\Haley"))
        {
            ResetSprite();
            //Moongates.Mod.instance.Monitor.Log("Created mgnpc using constructor 1: " + Name);
        }

        public override void update(GameTime time, GameLocation location, long id, bool move)
        {
            LastColliding = Colliding;
            Colliding = false;
            if (!isGlowing)
            {
                if (Name == "MoongateWax")
                {
                    CurrentDialogue.Clear();
                    startGlowing(Color.FromNonPremultiplied(0, 236, 222, 255), false, 0.01f);
                }
                else if (Name == "MoongateWane")
                {
                    CurrentDialogue.Clear();
                    startGlowing(Color.FromNonPremultiplied(236, 0, 183, 255), false, 0.01f);
                }
                else if (Name == "MoongateEbb")
                {
                    CurrentDialogue.Clear();
                    startGlowing(Color.FromNonPremultiplied(187, 255, 57, 255), false, 0.01f);
                }
                else if (Name == "MoongateFlow")
                {
                    CurrentDialogue.Clear();
                    startGlowing(Color.FromNonPremultiplied(179, 129, 255, 255), false, 0.01f);
                }
            }
            //base.update(time, location, id, move);
            updateGlow();
            Sprite.Animate(time, 0, 4, 200f);
        }

        public override bool collideWith(StardewValley.Object o)
        {
            return false;
        }

        public void DoGateWarp(GameSpot destination)
        {
            var ymod = IsTilePathableAndClear(destination.GetGameLocation(), destination.TileX, destination.TileY + 1) ? 1 : -1;
            Game1.warpFarmer(new LocationRequest(destination.GetGameLocation().NameOrUniqueName, destination.GetGameLocation().uniqueName.Value != null, destination.GetGameLocation()), destination.TileX, destination.TileY + ymod, 2);
        }

        public bool IsTilePathableAndClear(GameLocation l, int x, int y)
        {
            xTile.Dimensions.Location tileLoc = new xTile.Dimensions.Location(x, y);
            //is solid?
            if (!l.isTilePassable(tileLoc, Game1.viewport)) return false;
            //is clear and placeable?
            if (!l.isTileLocationTotallyClearAndPlaceable(x, y)) return false;
            //is shadow tile? (out of bounds on interior maps, etc)
            xTile.ObjectModel.PropertyValue shadowProp;
            if (l.map.GetLayer("Back").Tiles[tileLoc].TileIndexProperties.TryGetValue("Shadow", out shadowProp)) return false;
            //is it a water tile?
            if (!string.IsNullOrWhiteSpace(l.doesTileHaveProperty(x, y, "Water", "Back"))) return false;
            //is it a wall tile?
            if (!string.IsNullOrWhiteSpace(l.doesTileHaveProperty(x, y, "NoFurniture", "Back"))) return false;
            //is it an NPC barrier tile?
            if (!string.IsNullOrWhiteSpace(l.doesTileHaveProperty(x, y, "NPCBarrier", "Back"))) return false;
            return true;
        }

        public override void collisionWithFarmerBehavior()
        {
            Colliding = true;
            if (Colliding && !LastColliding)
            {
                DoGateWarp(Mod.Destinations[Name]);
            }
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);
            Sprite.Animate(time, 0, 4, 200f);

            farmerPassesThrough = true;
            if (Game1.player.currentLocation == currentLocation)
            {
                if (Game1.player.getTileLocationPoint() == getTileLocationPoint())
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
