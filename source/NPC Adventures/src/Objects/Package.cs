using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Objects
{
    class Package : Chest
    {
        public Package(List<Item> items, Vector2 location, int giftBoxIndex = 0, string message = null) : base(0, items, location, true, giftBoxIndex)
        {
            this.Message = message;
        }

        public string Message { get; internal set; }
        public string GivenFrom { get; internal set; }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
                return true;

            // Read message first if some message was set
            if (this.Message != null)
            {
                who.Halt();
                Game1.drawLetterMessage(this.Message);
                this.Message = null; // Drop message after read
                return true;
            }
                
            who.Halt();
            who.freezePause = 1000;
            this.CreateUnpackAnimation(who.currentLocation);

            // Drop items from gift box
            foreach (Item item in this.items.Where(itm => itm != null))
            {
                Game1.createItemDebris(item, who.getStandingPosition(), who.FacingDirection, who.currentLocation);
            }

            this.items.Clear();
            who.currentLocation.playSound("openBox");
            who.currentLocation.removeObject(this.TileLocation, false); // Destroy box

            return true;
        }

        private void CreateUnpackAnimation(GameLocation location)
        {
            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(
                "LooseSprites\\Giftbox", new Rectangle(0, this.giftboxIndex.Value * 32, 16, 32), 80f, 11, 1,
                (this.TileLocation * 64f) - new Vector2(0.0f, 52f), false, false, this.TileLocation.Y / 10000f, 0.0f,
                Color.White, 4f, 0.0f, 0.0f, 0.0f, false)
            {
                destroyable = false,
                holdLastFrame = true
            };

            location.temporarySprites.Add(temporaryAnimatedSprite);
        }
    }
}
