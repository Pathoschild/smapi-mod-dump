using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;

namespace UltimateTool.Framework
{
    class UBush : Bush
    {
        public static IModHelper helper;
        private IReflectedField<bool> shakeLeft;
        private IReflectedField<float> shakeRotation;
        private IReflectedField<float> maxShake;
        private IReflectedField<long> lastPlayerToHit;
        private IReflectedField<float> shakeTimer;
        private IReflectedField<Rectangle> sourceRect;

        public UBush(Vector2 tileLocation, int size, GameLocation location) : base(tileLocation, size, location)
        {
            shakeLeft = helper.Reflection.GetField<bool>(this, "shakeLeft");
            shakeRotation = helper.Reflection.GetField<float>(this, "shakeRotation");
            maxShake = helper.Reflection.GetField<float>(this, "maxShake");
            lastPlayerToHit = helper.Reflection.GetField<long>(this, "lastPlayerToHit");
            shakeTimer = helper.Reflection.GetField<float>(this, "shakeTimer");
            sourceRect = helper.Reflection.GetField<Rectangle>(this, "sourceRect");
        }

        public void shake(Vector2 tileLocation, bool doEvenIfStillShaking)
        {
            if (!((double)this.maxShake.GetValue() == 0.0 | doEvenIfStillShaking))
                return;
            this.shakeLeft.SetValue((double)Game1.player.getTileLocation().X > (double)tileLocation.X || (double)Game1.player.getTileLocation().X == (double)tileLocation.X && Game1.random.NextDouble() < 0.5);
            this.maxShake.SetValue((float)Math.PI / 128f);
            if (!this.townBush && this.tileSheetOffset == 1 && this.inBloom(Game1.currentSeason, Game1.dayOfMonth))
            {
                int parentSheetIndex = -1;
                string currentSeason = Game1.currentSeason;
                if (!(currentSeason == "spring"))
                {
                    if (currentSeason == "fall")
                        parentSheetIndex = 410;
                }
                else
                    parentSheetIndex = 296;
                if (parentSheetIndex == -1)
                    return;
                this.tileSheetOffset = 0;
                this.setUpSourceRect();
                int num = new Random((int)tileLocation.X + (int)tileLocation.Y * 5000 + (int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed).Next(1, 2) + Game1.player.ForagingLevel / 4;
                for (int index = 0; index < num; ++index)
                    Game1.createItemDebris((Item)new StardewValley.Object(parentSheetIndex, 1, false, -1, Game1.player.professions.Contains(16) ? 4 : 0), Utility.PointToVector2(this.getBoundingBox().Center), Game1.random.Next(1, 4), (GameLocation)null);
                DelayedAction.playSoundAfterDelay("leafrustle", 100);
            }
            else
            {
                if ((double)tileLocation.X != 20.0 || (double)tileLocation.Y != 8.0 || (Game1.dayOfMonth != 28 || Game1.timeOfDay != 1200) || Game1.player.mailReceived.Contains("junimoPlush"))
                    return;
                Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item)new Furniture(1733, Vector2.Zero), new ItemGrabMenu.behaviorOnItemSelect(this.junimoPlushCallback));
            }
        }
    }
}
