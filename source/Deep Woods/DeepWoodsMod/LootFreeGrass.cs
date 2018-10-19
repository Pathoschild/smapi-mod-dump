using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepWoodsMod
{
    class LootFreeGrass : Grass
    {
        private bool canDropHay = true;

        public LootFreeGrass()
          : base()
        {
        }

        public LootFreeGrass(int which, int numberOfWeeds)
          : base(which, numberOfWeeds)
        {
        }

        /*
         * TODO
        public override bool performToolAction(Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            // We increase number of weeds before calling base.performToolAction, so it will never realize it "died" and thus never create loot.
            int oldNumberOfWeeds = this.numberOfWeeds.Value;
            int newNumberOfWeeds = oldNumberOfWeeds + 100 + explosion;
            this.numberOfWeeds.Value = newNumberOfWeeds;

            // Perform tool action:
            base.performToolAction(t, explosion, tileLocation, location);

            // Reset number of weeds:
            int numberOfWeedsLost = oldNumberOfWeeds - newNumberOfWeeds;
            this.numberOfWeeds.Value = this.numberOfWeeds.Value - numberOfWeedsLost;

            // Return true if we died:
            bool died = this.numberOfWeeds.Value <= 0;

            // If we are spring grass and dropHay is true, drop hay:
            if (died && canDropHay && t is MeleeWeapon && (t.Name.Contains("Scythe") || t.parentSheetIndex == 47) && ((Game1.IsMultiplayer ? Game1.recentMultiplayerRandom : new Random((int)(Game1.uniqueIDForThisGame + tileLocation.X * 1000.0 + tileLocation.Y * 11.0))).NextDouble() < 0.5 && (Game1.getLocationFromName("Farm") as Farm).tryToAddHay(1) == 0))
            {
                Game1MultiplayerAccessProvider.GetMultiplayer().broadcastSprites(t.getLastFarmerToUse().currentLocation, new TemporaryAnimatedSprite[1]
                {
                    new TemporaryAnimatedSprite("Maps\\springobjects", Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 178, 16, 16), 750f, 1, 0, t.getLastFarmerToUse().Position - new Vector2(0.0f, 128f), false, false, t.getLastFarmerToUse().Position.Y / 10000f, 0.005f, Color.White, 4f, -0.005f, 0.0f, 0.0f, false)
                    {
                        motion = { Y = -1f },
                        layerDepth = (float) (1.0 - Game1.random.Next(100) / 10000.0),
                        delayBeforeAnimationStart = Game1.random.Next(350)
                    }
                });
                Game1.addHUDMessage(new HUDMessage("Hay", 1, true, Color.LightGoldenrodYellow, new StardewValley.Object(178, 1, false, -1, 0)));
            }

            return died;
        }
        */
    }
}
