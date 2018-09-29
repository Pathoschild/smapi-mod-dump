using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using TehPers.Stardew.FishingOverhaul.Configs;
using StardewValley.Tools;
using SFarmer = StardewValley.Farmer;

namespace TehPers.Stardew.FishingOverhaul {
    public class CustomBobberBar : BobberBar {

        private readonly IPrivateField<bool> _treasureField;
        private readonly IPrivateField<bool> _treasureCaughtField;
        private IPrivateField<float> _treasurePositionField;
        private IPrivateField<float> _treasureAppearTimerField;
        private IPrivateField<float> _treasureScaleField;

        private readonly IPrivateField<float> _distanceFromCatchingField;
        private readonly IPrivateField<float> _treasureCatchLevelField;

        private IPrivateField<float> _bobberBarPosField;
        private readonly IPrivateField<float> _difficultyField;
        private readonly IPrivateField<int> _fishQualityField;
        private readonly IPrivateField<bool> _perfectField;

        private readonly IPrivateField<SparklingText> _sparkleTextField;

        private float _lastDistanceFromCatching;
        private float _lastTreasureCatchLevel;
        private bool _perfectChanged;
        private bool _treasureChanged;
        private bool _notifiedFailOrSucceed;
        private readonly int _origStreak;
        private readonly int _origQuality;

        public SFarmer User;

        public CustomBobberBar(SFarmer user, int whichFish, float fishSize, bool treasure, int bobber, int waterDepth) : base(whichFish, fishSize, treasure, bobber) {
            this.User = user;
            this._origStreak = FishHelper.GetStreak(user);

            /* Private field hooks */
            this._treasureField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<bool>(this, "treasure");
            this._treasureCaughtField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<bool>(this, "treasureCaught");
            this._treasurePositionField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasurePosition");
            this._treasureAppearTimerField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasureAppearTimer");
            this._treasureScaleField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasureScale");

            this._distanceFromCatchingField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "distanceFromCatching");
            this._treasureCatchLevelField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "treasureCatchLevel");

            this._bobberBarPosField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "bobberBarPos");
            this._difficultyField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<float>(this, "difficulty");
            this._fishQualityField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<int>(this, "fishQuality");
            this._perfectField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<bool>(this, "perfect");

            this._sparkleTextField = ModFishing.INSTANCE.Helper.Reflection.GetPrivateField<SparklingText>(this, "sparkleText");

            this._lastDistanceFromCatching = this._distanceFromCatchingField.GetValue();
            this._lastTreasureCatchLevel = this._treasureCatchLevelField.GetValue();

            /* Actual code */
            ConfigMain config = ModFishing.INSTANCE.Config;
            ConfigStrings strings = ModFishing.INSTANCE.Strings;

            // Choose a random fish, this time using the custom fish selector
            FishingRod rod = Game1.player.CurrentTool as FishingRod;
            //int waterDepth = rod != null ? ModEntry.INSTANCE.Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance") : 0;

            // Applies difficulty modifier, including if fish isn't paying attention
            float difficulty = this._difficultyField.GetValue() * config.BaseDifficultyMult;
            difficulty *= 1f + config.DifficultyStreakEffect * this._origStreak;
            double difficultyChance = config.UnawareChance + user.LuckLevel * config.UnawareLuckLevelEffect + Game1.dailyLuck * config.UnawareDailyLuckEffect;
            if (Game1.random.NextDouble() < difficultyChance) {
                Game1.showGlobalMessage(string.Format(strings.UnawareFish, 1f - config.UnawareMult));
                difficulty *= config.UnawareMult;
            }
            this._difficultyField.SetValue(difficulty);

            // Adjusts quality to be increased by streak
            int fishQuality = this._fishQualityField.GetValue();
            this._origQuality = fishQuality;
            int qualityBonus = (int) Math.Floor((double) this._origStreak / config.StreakForIncreasedQuality);
            fishQuality = Math.Min(fishQuality + qualityBonus, 3);
            if (fishQuality == 3) fishQuality++; // Iridium-quality fish. Only possible through your perfect streak
            this._fishQualityField.SetValue(fishQuality);

            // Increase the user's perfect streak (this will be dropped to 0 if they don't get a perfect catch)
            if (this._origStreak >= config.StreakForIncreasedQuality)
                this._sparkleTextField.SetValue(new SparklingText(Game1.dialogueFont, string.Format(strings.StreakDisplay, this._origStreak), Color.Yellow, Color.White));
            FishHelper.SetStreak(user, this._origStreak + 1);
        }

        public override void update(GameTime time) {
            // Speed warp on normal catching
            float distanceFromCatching = this._distanceFromCatchingField.GetValue();
            float delta = distanceFromCatching - this._lastDistanceFromCatching;
            distanceFromCatching += (ModFishing.INSTANCE.Config.CatchSpeed - 1f) * delta;
            this._lastDistanceFromCatching = distanceFromCatching;
            this._distanceFromCatchingField.SetValue(distanceFromCatching);

            // Speed warp on treasure catching
            float treasureCatchLevel = this._treasureCatchLevelField.GetValue();
            delta = treasureCatchLevel - this._lastTreasureCatchLevel;
            treasureCatchLevel += (ModFishing.INSTANCE.Config.TreasureCatchSpeed - 1f) * delta;
            this._lastTreasureCatchLevel = treasureCatchLevel;
            this._treasureCatchLevelField.SetValue(treasureCatchLevel);

            bool perfect = this._perfectField.GetValue();
            bool treasure = this._treasureField.GetValue();
            bool treasureCaught = this._treasureCaughtField.GetValue();

            ConfigStrings strings = ModFishing.INSTANCE.Strings;

            // Check if still perfect, otherwise apply changes to loot
            if (!this._perfectChanged && !perfect) {
                this._perfectChanged = true;
                this._fishQualityField.SetValue(Math.Min(this._origQuality, 1));
                FishHelper.SetStreak(this.User, 0);
                if (this._origStreak >= ModFishing.INSTANCE.Config.StreakForIncreasedQuality)
                    Game1.showGlobalMessage(treasure ? string.Format(strings.WarnStreak, this._origStreak) : string.Format(strings.LostStreak, this._origStreak));
            }

            if (!this._treasureChanged && !perfect && treasure && treasureCaught) {
                this._treasureChanged = true;
                int qualityBonus = (int) Math.Floor((double) this._origStreak / ModFishing.INSTANCE.Config.StreakForIncreasedQuality);
                int quality = this._origQuality;
                quality = Math.Min(quality + qualityBonus, 3);
                if (quality == 3) quality++;
                this._fishQualityField.SetValue(quality);
            }

            base.update(time);

            distanceFromCatching = this._distanceFromCatchingField.GetValue();

            if (distanceFromCatching <= 0.0) {
                // Failed to catch fish
                //FishHelper.setStreak(this.user, 0);
                if (!this._notifiedFailOrSucceed && treasure) {
                    this._notifiedFailOrSucceed = true;
                    if (this._origStreak >= ModFishing.INSTANCE.Config.StreakForIncreasedQuality)
                        Game1.showGlobalMessage(string.Format(strings.LostStreak, this._origStreak));
                }
            } else if (distanceFromCatching >= 1.0) {
                // Succeeded in catching the fish
                if (!this._notifiedFailOrSucceed && !perfect && treasure && treasureCaught) {
                    this._notifiedFailOrSucceed = true;
                    if (this._origStreak >= ModFishing.INSTANCE.Config.StreakForIncreasedQuality)
                        Game1.showGlobalMessage(string.Format(strings.KeptStreak, this._origStreak));
                    FishHelper.SetStreak(this.User, this._origStreak);
                }
            }
        }
    }
}
