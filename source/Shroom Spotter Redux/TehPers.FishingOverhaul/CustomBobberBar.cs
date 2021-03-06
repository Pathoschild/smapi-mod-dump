/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using TehPers.Core;
using TehPers.FishingOverhaul.Api;
using TehPers.FishingOverhaul.Configs;

namespace TehPers.FishingOverhaul {
    public class CustomBobberBar : BobberBar {
        private readonly ReflectedField<BobberBar, bool> _treasure;
        private readonly ReflectedField<BobberBar, bool> _treasureCaught;
        private readonly ReflectedField<BobberBar, float> _treasurePosition;
        private readonly ReflectedField<BobberBar, float> _treasureAppearTimer;
        private readonly ReflectedField<BobberBar, float> _treasureScale;

        private readonly ReflectedField<BobberBar, float> _distanceFromCatching;
        private readonly ReflectedField<BobberBar, float> _treasureCatchLevel;

        private readonly ReflectedField<BobberBar, float> _bobberBarPos;
        private readonly ReflectedField<BobberBar, bool> _bobberInBar;
        private readonly ReflectedField<BobberBar, float> _difficulty;
        private readonly ReflectedField<BobberBar, int> _fishQuality;
        private readonly ReflectedField<BobberBar, bool> _perfect;
        private readonly ReflectedField<BobberBar, float> _scale;
        private readonly ReflectedField<BobberBar, bool> _flipBubble;
        private readonly ReflectedField<BobberBar, int> _bobberBarHeight;
        private readonly ReflectedField<BobberBar, float> _reelRotation;
        private readonly ReflectedField<BobberBar, float> _bobberPosition;
        private readonly ReflectedField<BobberBar, bool> _bossFish;
        private readonly ReflectedField<BobberBar, int> _motionType;
        private readonly ReflectedField<BobberBar, int> _fishSize;
        private readonly ReflectedField<BobberBar, int> _whichFish;

        private readonly ReflectedField<BobberBar, Vector2> _barShake;
        private readonly ReflectedField<BobberBar, Vector2> _fishShake;
        private readonly ReflectedField<BobberBar, Vector2> _treasureShake;
        private readonly ReflectedField<BobberBar, Vector2> _everythingShake;

        private readonly ReflectedField<BobberBar, SparklingText> _sparkleText;

        private float _lastDistanceFromCatching;
        private float _lastTreasureCatchLevel;
        private bool _perfectChanged;
        private bool _treasureChanged;
        private bool _notifiedFailOrSucceed;
        private readonly int _origStreak;
        private readonly int _origQuality;
        private readonly int _origFish;

        public Farmer User { get; }
        public bool Unaware { get; }

        public CustomBobberBar(Farmer user, int whichFish, float fishSize, bool treasure, int bobber) : base(whichFish, fishSize, treasure, bobber) {
            this.User = user;
            this._origStreak = ModFishing.Instance.Api.GetStreak(user);
            this._origFish = whichFish;

            /* Private field hooks */
            this._treasure = new ReflectedField<BobberBar, bool>(this, "treasure");
            this._treasure = new ReflectedField<BobberBar, bool>(this, "treasure");
            this._treasureCaught = new ReflectedField<BobberBar, bool>(this, "treasureCaught");
            this._treasurePosition = new ReflectedField<BobberBar, float>(this, "treasurePosition");
            this._treasureAppearTimer = new ReflectedField<BobberBar, float>(this, "treasureAppearTimer");
            this._treasureScale = new ReflectedField<BobberBar, float>(this, "treasureScale");

            this._distanceFromCatching = new ReflectedField<BobberBar, float>(this, "distanceFromCatching");
            this._treasureCatchLevel = new ReflectedField<BobberBar, float>(this, "treasureCatchLevel");

            this._bobberBarPos = new ReflectedField<BobberBar, float>(this, "bobberBarPos");
            this._bobberInBar = new ReflectedField<BobberBar, bool>(this, "bobberInBar");
            this._difficulty = new ReflectedField<BobberBar, float>(this, "difficulty");
            this._fishQuality = new ReflectedField<BobberBar, int>(this, "fishQuality");
            this._perfect = new ReflectedField<BobberBar, bool>(this, "perfect");
            this._scale = new ReflectedField<BobberBar, float>(this, "scale");
            this._flipBubble = new ReflectedField<BobberBar, bool>(this, "flipBubble");
            this._bobberBarHeight = new ReflectedField<BobberBar, int>(this, "bobberBarHeight");
            this._reelRotation = new ReflectedField<BobberBar, float>(this, "reelRotation");
            this._bobberPosition = new ReflectedField<BobberBar, float>(this, "bobberPosition");
            this._bossFish = new ReflectedField<BobberBar, bool>(this, "bossFish");
            this._motionType = new ReflectedField<BobberBar, int>(this, "motionType");
            this._fishSize = new ReflectedField<BobberBar, int>(this, "fishSize");
            this._whichFish = new ReflectedField<BobberBar, int>(this, "whichFish");

            this._barShake = new ReflectedField<BobberBar, Vector2>(this, "barShake");
            this._fishShake = new ReflectedField<BobberBar, Vector2>(this, "fishShake");
            this._treasureShake = new ReflectedField<BobberBar, Vector2>(this, "treasureShake");
            this._everythingShake = new ReflectedField<BobberBar, Vector2>(this, "everythingShake");

            this._sparkleText = new ReflectedField<BobberBar, SparklingText>(this, "sparkleText");

            this._lastDistanceFromCatching = this._distanceFromCatching.Value;
            this._lastTreasureCatchLevel = this._treasureCatchLevel.Value;

            /* Actual code */
            ConfigMain config = ModFishing.Instance.MainConfig;
            IFishTraits traits = ModFishing.Instance.Api.GetFishTraits(whichFish);

            // Check if fish is unaware
            this.Unaware = Game1.random.NextDouble() < ModFishing.Instance.Api.GetUnawareChance(user, whichFish);

            // Applies difficulty modifier, including if fish is unaware
            float difficulty = traits?.Difficulty ?? this._difficulty.Value;
            difficulty *= config.DifficultySettings.BaseDifficultyMult;
            difficulty *= 1F + this._origStreak * config.DifficultySettings.DifficultyStreakEffect;
            if (this.Unaware) {
                difficulty *= config.UnawareSettings.UnawareMult;
                Game1.showGlobalMessage(ModFishing.Translate("text.unaware", ModFishing.Translate("text.percent", 1F - config.UnawareSettings.UnawareMult)));
            }
            this._difficulty.Value = difficulty;

            // Adjusts additional traits about the fish
            if (traits != null) {
                this._motionType.Value = (int) traits.MotionType;
                this._fishSize.Value = traits.MinSize + (int) ((traits.MaxSize - traits.MinSize) * fishSize) + 1;
            }

            // Adjusts quality to be increased by streak
            int fishQuality = this._fishQuality.Value;
            this._origQuality = fishQuality;
            int qualityBonus = (int) Math.Floor((double) this._origStreak / config.StreakSettings.StreakForIncreasedQuality);
            fishQuality = Math.Min(fishQuality + qualityBonus, 3);
            if (fishQuality == 3) fishQuality++; // Iridium-quality fish. Only possible through your perfect streak
            this._fishQuality.Value = fishQuality;

            // Increase the user's perfect streak (this will be dropped to 0 if they don't get a perfect catch)
            if (this._origStreak >= config.StreakSettings.StreakForIncreasedQuality)
                this._sparkleText.Value = new SparklingText(Game1.dialogueFont, ModFishing.Translate("text.streak", this._origStreak), Color.Yellow, Color.White);
        }

        public override void update(GameTime time) {
            // Speed warp on catching fish
            float distanceFromCatching = this._distanceFromCatching.Value;
            float delta = distanceFromCatching - this._lastDistanceFromCatching;
            float mult = delta > 0 ? ModFishing.Instance.MainConfig.DifficultySettings.CatchSpeed : ModFishing.Instance.MainConfig.DifficultySettings.DrainSpeed;
            distanceFromCatching = this._lastDistanceFromCatching + delta * mult;
            this._lastDistanceFromCatching = distanceFromCatching;
            this._distanceFromCatching.Value = distanceFromCatching;

            // Speed warp on catching treasure
            float treasureCatchLevel = this._treasureCatchLevel.Value;
            delta = treasureCatchLevel - this._lastTreasureCatchLevel;
            mult = delta > 0 ? ModFishing.Instance.MainConfig.DifficultySettings.TreasureCatchSpeed : ModFishing.Instance.MainConfig.DifficultySettings.TreasureDrainSpeed;
            treasureCatchLevel = this._lastTreasureCatchLevel + delta * mult;
            this._lastTreasureCatchLevel = treasureCatchLevel;
            this._treasureCatchLevel.Value = treasureCatchLevel;

            bool perfect = this._perfect.Value;
            bool treasure = this._treasure.Value;
            bool treasureCaught = this._treasureCaught.Value;

            // Check if still perfect, otherwise apply changes to loot
            if (!this._perfectChanged && !perfect) {
                this._perfectChanged = true;
                this._fishQuality.Value = Math.Min(this._origQuality, ModFishing.Instance.MainConfig.DifficultySettings.PreventGoldOnNormalCatch ? 1 : 2);
                ModFishing.Instance.Api.SetStreak(this.User, 0);
                if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate(treasure ? "text.warnStreak" : "text.lostStreak", this._origStreak));
                }
            }

            // Check if lost perfect, but got treasure
            if (!this._treasureChanged && !perfect && treasure && treasureCaught) {
                this._treasureChanged = true;
                int qualityBonus = (int) Math.Floor((double) this._origStreak / ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality);
                int quality = this._origQuality;
                quality = Math.Min(quality + qualityBonus, 3);
                if (quality == 3) quality++;
                this._fishQuality.Value = quality;
            }

            // Base call
            base.update(time);

            // Check if done fishing
            distanceFromCatching = this._distanceFromCatching.Value;
            if (this._notifiedFailOrSucceed)
                return;

            if (distanceFromCatching <= 0.0) {
                // Failed to catch fish
                this._notifiedFailOrSucceed = true;

                if (treasure) {
                    this._notifiedFailOrSucceed = true;
                    if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                        Game1.showGlobalMessage(ModFishing.Translate("text.lostStreak", this._origStreak));
                    }
                }
            } else if (distanceFromCatching >= 1.0) {
                // Succeeded in catching the fish
                this._notifiedFailOrSucceed = true;

                if (perfect) {
                    ModFishing.Instance.Api.SetStreak(this.User, this._origStreak + 1);
                } else if (treasure && treasureCaught) {
                    if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality)
                        Game1.showGlobalMessage(ModFishing.Translate("text.keptStreak", this._origStreak));
                    ModFishing.Instance.Api.SetStreak(this.User, this._origStreak);
                }

                // Invoke fish caught event
                int curFish = this._whichFish.Value;
                FishingEventArgs eventArgs = new FishingEventArgs(curFish, this.User, this.User.CurrentTool as FishingRod);
                ModFishing.Instance.Api.OnFishCaught(eventArgs);
                if (eventArgs.ParentSheetIndex != curFish) {
                    this._whichFish.Value = eventArgs.ParentSheetIndex;
                }
            }
        }

        public override void emergencyShutDown() {
            // Failed to catch fish
            if (!this._notifiedFailOrSucceed) {
                this._notifiedFailOrSucceed = true;
                ModFishing.Instance.Api.SetStreak(this.User, 0);
                if (this._origStreak >= ModFishing.Instance.MainConfig.StreakSettings.StreakForIncreasedQuality) {
                    Game1.showGlobalMessage(ModFishing.Translate("text.lostStreak", this._origStreak));
                }
            }

            base.emergencyShutDown();
        }

        public override void draw(SpriteBatch b) {
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen - (this._flipBubble.Value ? 44 : 20) + 104, this.yPositionOnScreen - 16 + 314) + this._everythingShake.Value, new Rectangle(652, 1685, 52, 157), Color.White * 0.6f * this._scale.Value, 0.0f, new Vector2(26f, 78.5f) * this._scale.Value, 4f * this._scale.Value, this._flipBubble.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1f / 1000f);
            b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 70, this.yPositionOnScreen + 296) + this._everythingShake.Value, new Rectangle(644, 1999, 37, 150), Color.White * this._scale.Value, 0.0f, new Vector2(18.5f, 74f) * this._scale.Value, 4f * this._scale.Value, SpriteEffects.None, 0.01f);
            if (this._scale.Value >= 1.0) {
                SpriteBatch spriteBatch1 = b;
                Texture2D mouseCursors1 = Game1.mouseCursors;
                Vector2 position1 = new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._bobberBarPos.Value) + this._barShake.Value + this._everythingShake.Value;
                Rectangle? sourceRectangle1 = new Rectangle(682, 2078, 9, 2);
                TimeSpan timeOfDay;
                Color color1;
                if (!this._bobberInBar.Value) {
                    Color color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    double num = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color1 = color2 * (float) num;
                } else
                    color1 = Color.White;
                const double num1 = 0.0;
                Vector2 zero1 = Vector2.Zero;
                const double num2 = 4.0;
                const int num3 = 0;
                const double num4 = 0.889999985694885;
                spriteBatch1.Draw(mouseCursors1, position1, sourceRectangle1, color1, (float) num1, zero1, (float) num2, num3, (float) num4);
                SpriteBatch spriteBatch2 = b;
                Texture2D mouseCursors2 = Game1.mouseCursors;
                Vector2 position2 = new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._bobberBarPos.Value + 8) + this._barShake.Value + this._everythingShake.Value;
                Rectangle? sourceRectangle2 = new Rectangle(682, 2081, 9, 1);
                Color color3;
                if (!this._bobberInBar.Value) {
                    Color color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    double num5 = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color3 = color2 * (float) num5;
                } else
                    color3 = Color.White;
                const double num6 = 0.0;
                Vector2 zero2 = Vector2.Zero;
                Vector2 scale = new Vector2(4f, this._bobberBarHeight.Value - 16);
                const int num7 = 0;
                const double num8 = 0.889999985694885;
                spriteBatch2.Draw(mouseCursors2, position2, sourceRectangle2, color3, (float) num6, zero2, scale, num7, (float) num8);
                SpriteBatch spriteBatch3 = b;
                Texture2D mouseCursors3 = Game1.mouseCursors;
                Vector2 position3 = new Vector2(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._bobberBarPos.Value + this._bobberBarHeight.Value - 8) + this._barShake.Value + this._everythingShake.Value;
                Rectangle? sourceRectangle3 = new Rectangle(682, 2085, 9, 2);
                Color color4;
                if (!this._bobberInBar.Value) {
                    Color color2 = Color.White * 0.25f;
                    timeOfDay = DateTime.Now.TimeOfDay;
                    double num5 = Math.Round(Math.Sin(timeOfDay.TotalMilliseconds / 100.0), 2) + 2.0;
                    color4 = color2 * (float) num5;
                } else
                    color4 = Color.White;
                const double num9 = 0.0;
                Vector2 zero3 = Vector2.Zero;
                const double num10 = 4.0;
                const int num11 = 0;
                const double num12 = 0.889999985694885;
                spriteBatch3.Draw(mouseCursors3, position3, sourceRectangle3, color4, (float) num9, zero3, (float) num10, num11, (float) num12);
                b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 124, this.yPositionOnScreen + 4 + (int) (580.0 * (1.0 - this._distanceFromCatching.Value)), 16, (int) (580.0 * this._distanceFromCatching.Value)), Utility.getRedToGreenLerpColor(this._distanceFromCatching.Value));
                b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 18, this.yPositionOnScreen + 514) + this._everythingShake.Value, new Rectangle(257, 1990, 5, 10), Color.White, this._reelRotation.Value, new Vector2(2f, 10f), 4f, SpriteEffects.None, 0.9f);
                b.Draw(Game1.mouseCursors, new Vector2(this.xPositionOnScreen + 64 + 18, this.yPositionOnScreen + 12 + 24 + this._treasurePosition.Value) + this._treasureShake.Value + this._everythingShake.Value, new Rectangle(638, 1865, 20, 24), Color.White, 0.0f, new Vector2(10f, 10f), 2f * this._treasureScale.Value, SpriteEffects.None, 0.85f);
                if (this._treasureCatchLevel.Value > 0.0 && !this._treasureCaught.Value) {
                    b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._treasurePosition.Value, 40, 8), Color.DimGray * 0.5f);
                    b.Draw(Game1.staminaRect, new Rectangle(this.xPositionOnScreen + 64, this.yPositionOnScreen + 12 + (int) this._treasurePosition.Value, (int) (this._treasureCatchLevel.Value * 40.0), 8), Color.Orange);
                }

                // Draw the fish
                Vector2 fishPos = new Vector2(this.xPositionOnScreen + 64 + 18, this.yPositionOnScreen + 12 + 24 + this._bobberPosition.Value) + this._fishShake.Value + this._everythingShake.Value;
                if (ModFishing.Instance.MainConfig.ShowFish && !ModFishing.Instance.Api.IsHidden(this._origFish)) {
                    Rectangle fishSrc = GameLocation.getSourceRectForObject(this._origFish);
                    b.Draw(Game1.objectSpriteSheet, fishPos, fishSrc, Color.White, 0.0f, new Vector2(10f, 10f), 2.25f, SpriteEffects.None, 0.88f);
                } else {
                    Rectangle fishSrc = new Rectangle(614 + (this._bossFish.Value ? 20 : 0), 1840, 20, 20);
                    b.Draw(Game1.mouseCursors, fishPos, fishSrc, Color.White, 0.0f, new Vector2(10f, 10f), 2f, SpriteEffects.None, 0.88f);
                }

                // Draw the sparkle text
                this._sparkleText.Value?.draw(b, new Vector2(this.xPositionOnScreen - 16, this.yPositionOnScreen - 64));
            }
            if (Game1.player.fishCaught == null || Game1.player.fishCaught.Count != 0)
                return;
            Vector2 position = new Vector2(this.xPositionOnScreen + (this._flipBubble.Value ? this.width + 64 + 8 : -200), this.yPositionOnScreen + 192);
            if (!Game1.options.gamepadControls)
                b.Draw(Game1.mouseCursors, position, new Rectangle(644, 1330, 48, 69), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 0.88f);
            else
                b.Draw(Game1.controllerMaps, position, Utility.controllerMapSourceRect(new Rectangle(681, 0, 96, 138)), Color.White, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
        }
    }
}
