using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace DeepWoodsMod
{
    /// <summary>
    /// Heh, this is the most literal easter egg I've ever written :D
    /// </summary>
    class EasterEgg : TerrainFeature
    {
        private int eggTileIndex;
        private bool wasPickedUp;

        public EasterEgg()
#if SDVBETA
           : base(false)
#endif
        {
            this.eggTileIndex = Game1.random.Next(67, 71);
            this.wasPickedUp = false;
        }

        public override Microsoft.Xna.Framework.Rectangle getBoundingBox(Vector2 tileLocation)
        {
            return new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X * 64, (int)tileLocation.Y * 64, 64, 64);
        }

        public override bool isPassable(Character c = null)
        {
            return true;// this.wasPickedUp;
        }

        public override bool performUseAction(Vector2 tileLocation, GameLocation location)
        {
            if (this.wasPickedUp)
                return false;

            if (Game1.player.addItemToInventoryBool(new EasterEggItem(), false))
            {
                if (!(Game1.player.FarmerSprite is DeepWoodsMod.FarmerSprite))
                {
                    Game1.player.FarmerSprite = new DeepWoodsMod.FarmerSprite(Game1.player.FarmerSprite);
                }
                Game1.player.animateOnce(StardewValley.FarmerSprite.harvestItemUp + Game1.player.FacingDirection);
                Game1.player.canMove = false;
                Game1.player.currentLocation.playSound("coin");
                this.wasPickedUp = true;
                return true;
            }
            else
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                return false;
            }
        }

        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            if (t == null && explosion > 0)
                return true;
            return false;
        }

        public override void draw(SpriteBatch b, Vector2 tileLocation)
        {
            if (this.wasPickedUp)
                return;

            Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64, tileLocation.Y * 64));

            Rectangle destinationRectangle = new Rectangle((int)local.X, (int)local.Y, 64, 64);
            Rectangle sourceRectangle = Game1.getSourceRectForStandardTileSheet(Textures.festivals, this.eggTileIndex, 16, 16);

            b.Draw(Textures.festivals, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
