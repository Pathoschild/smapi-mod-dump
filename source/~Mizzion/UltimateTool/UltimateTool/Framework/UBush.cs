using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace UltimateTool.Framework
{
    class UBush : Bush
    {
        public static IModHelper helper;
        private IReflectedField<bool> _shakeLeft;
        private IReflectedField<float> _shakeRotation;
        private IReflectedField<float> _maxShake;
        private IReflectedField<long> _lastPlayerToHit;
        private IReflectedField<float> _shakeTimer;
        private IReflectedField<Rectangle> _sourceRect;

        public UBush(Vector2 tileLocation, int size, GameLocation location) : base(tileLocation, size, location)
        {
            _shakeLeft = helper.Reflection.GetField<bool>(this, "shakeLeft");
            _shakeRotation = helper.Reflection.GetField<float>(this, "shakeRotation");
            _maxShake = helper.Reflection.GetField<float>(this, "maxShake");
            _lastPlayerToHit = helper.Reflection.GetField<long>(this, "lastPlayerToHit");
            _shakeTimer = helper.Reflection.GetField<float>(this, "shakeTimer");
            _sourceRect = helper.Reflection.GetField<Rectangle>(this, "sourceRect");
        }

        public void Shake(Vector2 tileLocation, bool doEvenIfStillShaking)
        {
            if (!((double)_maxShake.GetValue() == 0.0 | doEvenIfStillShaking))
                return;
            _shakeLeft.SetValue((double)Game1.player.getTileLocation().X > (double)tileLocation.X || (double)Game1.player.getTileLocation().X == (double)tileLocation.X && Game1.random.NextDouble() < 0.5);
            _maxShake.SetValue((float)Math.PI / 128f);
            if (!townBush && tileSheetOffset == 1 && inBloom(Game1.currentSeason, Game1.dayOfMonth))
            {
                int parentSheetIndex = -1;
                string currentSeason = Game1.currentSeason;
                if (currentSeason != "spring")
                {
                    if (currentSeason == "fall")
                        parentSheetIndex = 410;
                }
                else
                    parentSheetIndex = 296;
                if (parentSheetIndex == -1)
                    return;
                tileSheetOffset.Value = 0;
                setUpSourceRect();
                int num = new Random((int)tileLocation.X + (int)tileLocation.Y * 5000 + (int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed).Next(1, 2) + Game1.player.ForagingLevel / 4;
                for (int index = 0; index < num; ++index)
                    Game1.createItemDebris((Item)new StardewValley.Object(parentSheetIndex, 1, false, -1, Game1.player.professions.Contains(16) ? 4 : 0), Utility.PointToVector2(getBoundingBox().Center), Game1.random.Next(1, 4), (GameLocation)null);
                DelayedAction.playSoundAfterDelay("leafrustle", 100);
            }
            else
            {
                if ((double)tileLocation.X != 20.0f || (double)tileLocation.Y != 8.0f || (Game1.dayOfMonth != 28 || Game1.timeOfDay != 1200) || Game1.player.mailReceived.Contains("junimoPlush"))
                    return;
                Game1.player.addItemByMenuIfNecessaryElseHoldUp((Item)new Furniture(1733, Vector2.Zero), new ItemGrabMenu.behaviorOnItemSelect(junimoPlushCallback));
            }
        }
    }
}
