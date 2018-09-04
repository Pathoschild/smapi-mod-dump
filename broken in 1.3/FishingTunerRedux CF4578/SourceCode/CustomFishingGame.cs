using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;

namespace FishingTunerRedux {
	public class CustomFishingGame : BobberBar {
		public static Farmer Player => Game1.player;
		public static int FishingLevel => Player.FishingLevel;
		public static int LuckLevel => Player.LuckLevel;
		public static float DailyLuck => (float) Game1.dailyLuck;
		public static FishingRod Rod => Player.CurrentTool as FishingRod;

		public static BindingFlags ReadFlags = BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance;
		public static BindingFlags WriteFlags = BindingFlags.NonPublic | BindingFlags.Instance;

		private FishingTunerReduxMod Mod;

		public Vector2 BarShake {
			get {
				return (Vector2) typeof(BobberBar).GetField("barShake", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("barShake", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberAcceleration {
			get {
				return (float) typeof(BobberBar).GetField("bobberAcceleration", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberAcceleration", WriteFlags).SetValue(this, value);
			}
		}
		public int BobberBarHeight {
			get {
				return (int) typeof(BobberBar).GetField("bobberBarHeight", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberBarHeight", WriteFlags).SetValue(this, value);
			}
		}
		public bool BobberInBar {
			get {
				return (bool) typeof(BobberBar).GetField("bobberInBar", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberInBar", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberBarAcceleration {
			get {
				return (float) typeof(BobberBar).GetField("bobberBarAcceleration", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberBarAcceleration", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberBarPosition {
			get {
				return (float) typeof(BobberBar).GetField("bobberBarPos", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberBarPos", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberBarSpeed {
			get {
				return (float) typeof(BobberBar).GetField("bobberBarSpeed", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberBarSpeed", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberPosition {
			get {
				return (float) typeof(BobberBar).GetField("bobberPosition", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberPosition", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberSpeed {
			get {
				return (float) typeof(BobberBar).GetField("bobberSpeed", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberSpeed", WriteFlags).SetValue(this, value);
			}
		}
		public float BobberTargetPosition {
			get {
				return (float) typeof(BobberBar).GetField("bobberTargetPosition", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bobberTargetPosition", WriteFlags).SetValue(this, value);
			}
		}
		public int BobberType {
			get {
				return (int) typeof(BobberBar).GetField("whichBobber", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("whichBobber", WriteFlags).SetValue(this, value);
			}
		}
		public bool BossFish {
			get {
				return (bool) typeof(BobberBar).GetField("bossFish", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("bossFish", WriteFlags).SetValue(this, value);
			}
		}
		public bool ButtonPressed {
			get {
				return (bool) typeof(BobberBar).GetField("buttonPressed", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("buttonPressed", WriteFlags).SetValue(this, value);
			}
		}
		public float Difficulty {
			get {
				return (float) typeof(BobberBar).GetField("difficulty", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("difficulty", WriteFlags).SetValue(this, value);
			}
		}
		public float DistanceFromCatching {
			get {
				return (float) typeof(BobberBar).GetField("distanceFromCatching", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("distanceFromCatching", WriteFlags).SetValue(this, value);
			}
		}
		public Vector2 EverythingShake {
			get {
				return (Vector2) typeof(BobberBar).GetField("everythingShake", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("everythingShake", WriteFlags).SetValue(this, value);
			}
		}
		public float EverythingShakeTimer {
			get {
				return (float) typeof(BobberBar).GetField("everythingShakeTimer", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("everythingShakeTimer", WriteFlags).SetValue(this, value);
			}
		}
		public bool FadeIn {
			get {
				return (bool) typeof(BobberBar).GetField("fadeIn", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("fadeIn", WriteFlags).SetValue(this, value);
			}
		}
		public bool FadeOut {
			get {
				return (bool) typeof(BobberBar).GetField("fadeOut", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("fadeOut", WriteFlags).SetValue(this, value);
			}
		}
		public int FishQuality {
			get {
				return (int) typeof(BobberBar).GetField("fishQuality", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("fishQuality", WriteFlags).SetValue(this, value);
			}
		}
		public int FishSize {
			get {
				return (int) typeof(BobberBar).GetField("fishSize", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("fishSize", WriteFlags).SetValue(this, value);
			}
		}
		public int FishSizeReductionTimer {
			get {
				return (int) typeof(BobberBar).GetField("fishSizeReductionTimer", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("fishSizeReductionTimer", WriteFlags).SetValue(this, value);
			}
		}
		public int FishSizeMin {
			get {
				return (int) typeof(BobberBar).GetField("minFishSize", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("minFishSize", WriteFlags).SetValue(this, value);
			}
		}
		public int FishSizeMax {
			get {
				return (int) typeof(BobberBar).GetField("maxFishSize", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("maxFishSize", WriteFlags).SetValue(this, value);
			}
		}
		public Vector2 FishShake {
			get {
				return (Vector2) typeof(BobberBar).GetField("fishShake", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("fishShake", WriteFlags).SetValue(this, value);
			}
		}
		public int FishType {
			get {
				return (int) typeof(BobberBar).GetField("whichFish", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("whichFish", WriteFlags).SetValue(this, value);
			}
		}
		public float FloaterSinkerAcceleration {
			get {
				return (float) typeof(BobberBar).GetField("floaterSinkerAcceleration", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("floaterSinkerAcceleration", WriteFlags).SetValue(this, value);
			}
		}
		public int MotionType {
			get {
				return (int) typeof(BobberBar).GetField("motionType", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("motionType", WriteFlags).SetValue(this, value);
			}
		}
		public bool Perfect {
			get {
				return (bool) typeof(BobberBar).GetField("perfect", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("perfect", WriteFlags).SetValue(this, value);
			}
		}
		public float ReelRotation {
			get {
				return (float) typeof(BobberBar).GetField("reelRotation", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("reelRotation", WriteFlags).SetValue(this, value);
			}
		}
		public float Scale {
			get {
				return (float) typeof(BobberBar).GetField("scale", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("scale", WriteFlags).SetValue(this, value);
			}
		}
		public SparklingText SparkleText {
			get {
				return (SparklingText) typeof(BobberBar).GetField("sparkleText", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("sparkleText", WriteFlags).SetValue(this, value);
			}
		}
		public bool Treasure {
			get {
				return (bool) typeof(BobberBar).GetField("treasure", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasure", WriteFlags).SetValue(this, value);
			}
		}
		public float TreasureAppearTimer {
			get {
				return (float) typeof(BobberBar).GetField("treasureAppearTimer", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasureAppearTimer", WriteFlags).SetValue(this, value);
			}
		}
		public float TreasureCatchLevel {
			get {
				return (float) typeof(BobberBar).GetField("treasureCatchLevel", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasureCatchLevel", WriteFlags).SetValue(this, value);
			}
		}
		public bool TreasureCaught {
			get {
				return (bool) typeof(BobberBar).GetField("treasureCaught", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasureCaught", WriteFlags).SetValue(this, value);
			}
		}
		public float TreasurePosition {
			get {
				return (float) typeof(BobberBar).GetField("treasurePosition", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasurePosition", WriteFlags).SetValue(this, value);
			}
		}
		public float TreasureScale {
			get {
				return (float) typeof(BobberBar).GetField("treasureScale", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasureScale", WriteFlags).SetValue(this, value);
			}
		}
		public Vector2 TreasureShake {
			get {
				return (Vector2) typeof(BobberBar).GetField("treasureShake", ReadFlags).GetValue(this);
			}
			set {
				typeof(BobberBar).GetField("treasureShake", WriteFlags).SetValue(this, value);
			}
		}

		public bool TreasureInBar { get; set; }
		public int CastDistance { get; set; }

		public CustomFishingGame(BobberBar baseGame, FishingTunerReduxMod mod)
		: base(
			(int) typeof(BobberBar).GetField("whichFish", ReadFlags).GetValue(baseGame),
			0.0f,
			false,
			(!(Player.CurrentTool is FishingRod) || Rod.attachments.Length < 2 || Rod.attachments[1] == null) ? 0 : Rod.attachments[1].parentSheetIndex) {

			Mod = mod;
			CastDistance = (int) typeof(FishingRod).GetField("clearWaterDistance", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).GetValue(Rod);

			Difficulty = (float) typeof(BobberBar).GetField("difficulty", ReadFlags).GetValue(baseGame);
			MotionType = (int) typeof(BobberBar).GetField("motionType", ReadFlags).GetValue(baseGame);
			FishSize = (int) typeof(BobberBar).GetField("fishSize", ReadFlags).GetValue(baseGame);
			FishQuality = (int) typeof(BobberBar).GetField("fishQuality", ReadFlags).GetValue(baseGame);

			Mod.LoadMotionConfig(MotionType);

			AdjustDifficulty();
			SetBobberBarHeight();
			TreasureChance();
			SetInitialDistance();
			TreasureAppearTimer = FishingTunerReduxMod.rand.Next(Configs.GeneralSettings.MinTreasureAppearanceTime, Configs.GeneralSettings.MaxTreasureAppearanceTime + 1);
		}

		public override void update(GameTime time) {
			if (SparkleText != null && SparkleText.update(time)) {
				SparkleText = null;
			}

			if (EverythingShakeTimer > 0.0) {
				EverythingShakeTimer -= time.ElapsedGameTime.Milliseconds;
				EverythingShake = new Vector2(Game1.random.Next(-10, 11) / 10f, Game1.random.Next(-10, 11) / 10f);

				if (EverythingShakeTimer <= 0.0) {
					EverythingShake = Vector2.Zero;
				}
			}

			if (FadeIn) {
				Scale += 0.05f;

				if (Scale >= 1.0f) {
					Scale = 1f;
					FadeIn = false;
				}
			}
			else if (FadeOut) {
				if (EverythingShakeTimer > 0.0f || SparkleText != null) {
					return;
				}

				Scale -= 0.05f;

				if (Scale <= 0.0f) {
					Scale = 0.0f;
					FadeOut = false;

					if (DistanceFromCatching >= 1 && Player.CurrentTool != null && Player.CurrentTool is FishingRod) {
						Rod.pullFishFromWater(FishType, FishSize, FishQuality, (int) Difficulty, TreasureCaught, Perfect);
					}
					else {
						if (Player.CurrentTool != null && Player.CurrentTool is FishingRod) {
							Rod.doneFishing(Player, true);
						}
						Player.completelyStopAnimatingOrDoingAction();
					}
					Game1.exitActiveMenu();
				}
			}
			else {
				MoveFish(time);

				BobberInBar = BobberPosition + 16 <= BobberBarPosition - 32 + BobberBarHeight && BobberPosition - 16 >= BobberBarPosition - 32;
				if (BobberPosition >= (548 - BobberBarHeight) && BobberBarPosition >= (568 - BobberBarHeight - 4)) {
					BobberInBar = true;
				}

				MoveBobberBar(time);

				if (Treasure) {
					UpdateTreasure(time);
				}
				if (BobberInBar) {
					ReelIn(time);
				}
				else {
					ReelOut(time);
				}

				DistanceFromCatching = DistanceFromCatching.Clamp(0, 1);

				if (Player.CurrentTool != null) {
					Player.CurrentTool.tickUpdate(time, Player);
				}

				if (DistanceFromCatching <= 0.0) {
					FishEscaped(time);
				}
				else if (DistanceFromCatching >= 1.0) {
					FishCaught(time);
				}
			}

			BobberPosition = BobberPosition.Clamp(0, 548);
		}

		public virtual void AdjustDifficulty() {
			Difficulty += Configs.GeneralSettings.BaseDifficultyAdditive;
			Difficulty += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.DifficultyAdditive : 0;
			Difficulty += CastDistance * Configs.CastDistance.DifficultyAdditive;
			Difficulty += DailyLuck * Configs.DailyLuck.DifficultyAdditive;
			Difficulty += LuckLevel * Configs.PlayerLuck.DifficultyAdditive;
			Difficulty += FishingLevel * Configs.FishingLevel.DifficultyAdditive;
			Difficulty += Configs.Bait == null ? 0 : Configs.Bait.DifficultyAdditive;
			Difficulty += Configs.Tackle == null ? 0 : Configs.Tackle.DifficultyAdditive;
			Difficulty += Configs.FishMotion.DifficultyAdditive;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				Difficulty += LSD.DifficultyAdditive;
			}

			Difficulty *= Configs.GeneralSettings.BaseDifficultyMultiplier;
			Difficulty *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.DifficultyMultiplier : 1;
			Difficulty *= (1 + (CastDistance * Configs.CastDistance.DifficultyMultiplier));
			Difficulty *= (1 + (DailyLuck * Configs.DailyLuck.DifficultyMultiplier));
			Difficulty *= (1 + (LuckLevel * Configs.PlayerLuck.DifficultyMultiplier));
			Difficulty *= (1 + (FishingLevel * Configs.FishingLevel.DifficultyMultiplier));
			Difficulty *= Configs.Bait == null ? 1 : Configs.Bait.DifficultyMultiplier;
			Difficulty *= Configs.Tackle == null ? 1 : Configs.Tackle.DifficultyMultiplier;
			Difficulty *= Configs.FishMotion.DifficultyMultiplier;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				Difficulty *= LSD.DifficultyMultiplier;
			}
		}

		public virtual void TreasureChance() {
			if (Configs.GeneralSettings.AlwaysFindTreasure) {
				Treasure = true;
			}
			else {
				float TreasureChance = Configs.GeneralSettings.BaseTreasureChance;
				TreasureChance += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.TreasureChanceAdditive : 0;
				TreasureChance += CastDistance * Configs.CastDistance.TreasureChanceAdditive;
				TreasureChance += DailyLuck * Configs.DailyLuck.TreasureChanceAdditive;
				TreasureChance += LuckLevel * Configs.PlayerLuck.TreasureChanceAdditive;
				TreasureChance += Difficulty * Configs.FishDifficulty.TreasureChanceAdditive;
				TreasureChance += FishingLevel * Configs.FishingLevel.TreasureChanceAdditive;
				TreasureChance += Configs.Bait == null ? 0 : Configs.Bait.TreasureChanceAdditive;
				TreasureChance += Configs.Tackle == null ? 0 : Configs.Tackle.TreasureChanceAdditive;
				TreasureChance += Configs.FishMotion.TreasureChanceAdditive;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					TreasureChance += LSD.TreasureChanceAdditive;
				}

				TreasureChance *= Player.professions.Contains(9) ? 2 : 1; // Pirate profession, doubles treasure chance.
				TreasureChance *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.TreasureChanceMultiplier : 1;
				TreasureChance *= (1 + (CastDistance * Configs.CastDistance.TreasureChanceMultiplier));
				TreasureChance *= (1 + (DailyLuck * Configs.DailyLuck.TreasureChanceMultiplier));
				TreasureChance *= (1 + (LuckLevel * Configs.PlayerLuck.TreasureChanceMultiplier));
				TreasureChance *= (1 + (Difficulty * Configs.FishDifficulty.TreasureChanceMultiplier));
				TreasureChance *= (1 + (FishingLevel * Configs.FishingLevel.TreasureChanceMultiplier));
				TreasureChance *= Configs.Bait == null ? 1 : Configs.Bait.TreasureChanceMultiplier;
				TreasureChance *= Configs.Tackle == null ? 1 : Configs.Tackle.TreasureChanceMultiplier;
				TreasureChance *= Configs.FishMotion.TreasureChanceMultiplier;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					TreasureChance *= LSD.TreasureChanceMultiplier;
				}

				Treasure = FishingTunerReduxMod.rand.NextDouble() < TreasureChance;
			}
		}

		public virtual void SetBobberBarHeight() {
			BobberBarHeight = Configs.GeneralSettings.BaseBobberSize;
			BobberBarHeight += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.BobberBarSizeAdditive : 0;
			BobberBarHeight += CastDistance * Configs.CastDistance.BobberBarSizeAdditive;
			BobberBarHeight += (int) (DailyLuck * Configs.DailyLuck.BobberBarSizeAdditive);
			BobberBarHeight += LuckLevel * Configs.PlayerLuck.BobberBarSizeAdditive;
			BobberBarHeight += (int) (Difficulty * Configs.FishDifficulty.BobberBarSizeAdditive);
			BobberBarHeight += FishingLevel * Configs.FishingLevel.BobberBarSizeAdditive;
			BobberBarHeight += Configs.Bait == null ? 0 : Configs.Bait.BobberBarSizeAdditive;
			BobberBarHeight += Configs.Tackle == null ? 0 : Configs.Tackle.BobberBarSizeAdditive;
			BobberBarHeight += Configs.FishMotion.BobberBarSizeAdditive;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				BobberBarHeight += LSD.BobberBarSizeAdditive;
			}

			float HeightMultiplier = (1 + (CastDistance * Configs.CastDistance.BobberBarSizeMultiplier));
			HeightMultiplier *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.BobberBarSizeMultiplier : 1;
			HeightMultiplier *= (1 + (DailyLuck * Configs.DailyLuck.BobberBarSizeMultiplier));
			HeightMultiplier *= (1 + (LuckLevel * Configs.PlayerLuck.BobberBarSizeMultiplier));
			HeightMultiplier *= (1 + (Difficulty * Configs.FishDifficulty.BobberBarSizeMultiplier));
			HeightMultiplier *= (1 + (FishingLevel * Configs.FishingLevel.BobberBarSizeMultiplier));
			HeightMultiplier *= Configs.Bait == null ? 1 : Configs.Bait.BobberBarSizeMultiplier;
			HeightMultiplier *= Configs.Tackle == null ? 1 : Configs.Tackle.BobberBarSizeMultiplier;
			HeightMultiplier *= Configs.FishMotion.BobberBarSizeMultiplier;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				HeightMultiplier *= LSD.BobberBarSizeMultiplier;
			}

			HeightMultiplier *= Game1.tileSize / 64f; // Zoom modifier

			BobberBarHeight = (int) (BobberBarHeight * HeightMultiplier);
			BobberBarHeight = BobberBarHeight.Clamp(1, 568);
		}

		public virtual void SetInitialDistance() {
			bool NewFish = true;
			Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>(Path.Combine("Data", "Fish"));
			if (Player.fishCaught != null && Player.fishCaught.ContainsKey(FishType) && Player.fishCaught[FishType].Length > 0) {
				NewFish = false;
			}

			if (NewFish && !BossFish) {
				DistanceFromCatching = Configs.GeneralSettings.BaseNewFishTypeCatchPercentage;
				DistanceFromCatching += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.InitialNewFishTypeCatchPercentageAdditive : 0;
				DistanceFromCatching += CastDistance * Configs.CastDistance.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += DailyLuck * Configs.DailyLuck.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += LuckLevel * Configs.PlayerLuck.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += Difficulty * Configs.FishDifficulty.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += FishingLevel * Configs.FishingLevel.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += Configs.Bait == null ? 0 : Configs.Bait.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += Configs.Tackle == null ? 0 : Configs.Tackle.InitialNewFishTypeCatchPercentageAdditive;
				DistanceFromCatching += Configs.FishMotion.InitialNewFishTypeCatchPercentageAdditive;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					DistanceFromCatching += LSD.InitialNewFishTypeCatchPercentageAdditive;
				}
			}
			else {
				DistanceFromCatching = Configs.GeneralSettings.BaseFishCatchPercentage;
				DistanceFromCatching += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.InitialFishCatchPercentageAdditive : 0;
				DistanceFromCatching += CastDistance * Configs.CastDistance.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += DailyLuck * Configs.DailyLuck.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += LuckLevel * Configs.PlayerLuck.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += Difficulty * Configs.FishDifficulty.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += FishingLevel * Configs.FishingLevel.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += Configs.Bait == null ? 0 : Configs.Bait.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += Configs.Tackle == null ? 0 : Configs.Tackle.InitialFishCatchPercentageAdditive;
				DistanceFromCatching += Configs.FishMotion.InitialFishCatchPercentageAdditive;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					DistanceFromCatching += LSD.InitialFishCatchPercentageAdditive;
				}
			}

			DistanceFromCatching += BossFish ? Configs.GeneralSettings.BossFishInitialCatchPercentageAdditive : 0;

			if (NewFish) {
				DistanceFromCatching *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.InitialNewFishTypeCatchPercentageMultiplier : 1;
				DistanceFromCatching *= (1 + (CastDistance * Configs.CastDistance.InitialNewFishTypeCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (DailyLuck * Configs.DailyLuck.InitialNewFishTypeCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (LuckLevel * Configs.PlayerLuck.InitialNewFishTypeCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (Difficulty * Configs.FishDifficulty.InitialNewFishTypeCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (FishingLevel * Configs.FishingLevel.InitialNewFishTypeCatchPercentageMultiplier));
				DistanceFromCatching *= Configs.Bait == null ? 1 : Configs.Bait.InitialNewFishTypeCatchPercentageMultiplier;
				DistanceFromCatching *= Configs.Tackle == null ? 1 : Configs.Tackle.InitialNewFishTypeCatchPercentageMultiplier;
				DistanceFromCatching *= Configs.FishMotion.InitialNewFishTypeCatchPercentageMultiplier;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					DistanceFromCatching *= LSD.InitialNewFishTypeCatchPercentageMultiplier;
				}
			}
			else {
				DistanceFromCatching *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.InitialFishCatchPercentageMultiplier : 1;
				DistanceFromCatching *= (1 + (CastDistance * Configs.CastDistance.InitialFishCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (DailyLuck * Configs.DailyLuck.InitialFishCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (LuckLevel * Configs.PlayerLuck.InitialFishCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (Difficulty * Configs.FishDifficulty.InitialFishCatchPercentageMultiplier));
				DistanceFromCatching *= (1 + (FishingLevel * Configs.FishingLevel.InitialFishCatchPercentageMultiplier));
				DistanceFromCatching *= Configs.Bait == null ? 1 : Configs.Bait.InitialFishCatchPercentageMultiplier;
				DistanceFromCatching *= Configs.Tackle == null ? 1 : Configs.Tackle.InitialFishCatchPercentageMultiplier;
				DistanceFromCatching *= Configs.FishMotion.InitialFishCatchPercentageMultiplier;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					DistanceFromCatching *= LSD.InitialFishCatchPercentageMultiplier;
				}
			}

			DistanceFromCatching *= BossFish ? Configs.GeneralSettings.BossFishInitialCatchPercentageMultiplier : 1;

			if (Treasure) {
				TreasureCatchLevel += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.InitialTreasureCatchPercentageAdditive : 0;
				TreasureCatchLevel += DailyLuck * Configs.DailyLuck.InitialTreasureCatchPercentageAdditive;
				TreasureCatchLevel += LuckLevel * Configs.PlayerLuck.InitialTreasureCatchPercentageAdditive;
				TreasureCatchLevel += Difficulty * Configs.FishDifficulty.InitialTreasureCatchPercentageAdditive;
				TreasureCatchLevel += FishingLevel * Configs.FishingLevel.InitialTreasureCatchPercentageAdditive;
				TreasureCatchLevel += Configs.Bait == null ? 0 : Configs.Bait.InitialTreasureCatchPercentageAdditive;
				TreasureCatchLevel += Configs.Tackle == null ? 0 : Configs.Tackle.InitialTreasureCatchPercentageAdditive;
				TreasureCatchLevel += Configs.FishMotion.InitialTreasureCatchPercentageAdditive;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					TreasureCatchLevel += LSD.InitialTreasureCatchPercentageAdditive;
				}

				TreasureCatchLevel *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.InitialTreasureCatchPercentageMultiplier : 1;
				TreasureCatchLevel *= (1 + (DailyLuck * Configs.DailyLuck.InitialTreasureCatchPercentageMultiplier));
				TreasureCatchLevel *= (1 + (LuckLevel * Configs.PlayerLuck.InitialTreasureCatchPercentageMultiplier));
				TreasureCatchLevel *= (1 + (Difficulty * Configs.FishDifficulty.InitialTreasureCatchPercentageMultiplier));
				TreasureCatchLevel *= (1 + (FishingLevel * Configs.FishingLevel.InitialTreasureCatchPercentageMultiplier));
				TreasureCatchLevel *= Configs.Bait == null ? 1 : Configs.Bait.InitialTreasureCatchPercentageMultiplier;
				TreasureCatchLevel *= Configs.Tackle == null ? 1 : Configs.Tackle.InitialTreasureCatchPercentageMultiplier;
				TreasureCatchLevel *= Configs.FishMotion.InitialTreasureCatchPercentageMultiplier;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					TreasureCatchLevel *= LSD.InitialTreasureCatchPercentageMultiplier;
				}
			}

			DistanceFromCatching = DistanceFromCatching.Clamp(0, 1);
			TreasureCatchLevel = TreasureCatchLevel.Clamp(0, 1);
		}

		public virtual void UpdateTreasure(GameTime time) {
			float OldTimer = TreasureAppearTimer;
			TreasureAppearTimer -= time.ElapsedGameTime.Milliseconds;
			if (TreasureAppearTimer <= 0.0) {
				if (TreasureScale < 1.0 && !TreasureCaught) {
					if (OldTimer > 0.0) {
						TreasurePosition = BobberBarPosition > 274.0 ? Game1.random.Next(8, Math.Max(9, (int) BobberBarPosition - 20)) : Game1.random.Next(Math.Min(499, (int) BobberBarPosition + BobberBarHeight), 500);
						Game1.playSound("dwop");
					}
					TreasureScale = Math.Min(1f, TreasureScale + 0.1f);
				}
				TreasureInBar = TreasurePosition + 16 <= BobberBarPosition - 32 + BobberBarHeight && TreasurePosition - 16 >= BobberBarPosition - 32;
				if (TreasureInBar && !TreasureCaught) {
					float TreasureProgressChange = Configs.GeneralSettings.BaseTreasureCatchProgressGainRate;
					TreasureProgressChange += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.TreasureCatchProgressGainRateAdditive : 0;
					TreasureProgressChange += CastDistance * Configs.CastDistance.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += DailyLuck * Configs.DailyLuck.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += LuckLevel * Configs.PlayerLuck.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += Difficulty * Configs.FishDifficulty.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += FishingLevel * Configs.FishingLevel.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += Configs.Bait == null ? 0 : Configs.Bait.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += Configs.Tackle == null ? 0 : Configs.Tackle.TreasureCatchProgressGainRateAdditive;
					TreasureProgressChange += Configs.FishMotion.TreasureCatchProgressGainRateAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						TreasureProgressChange += LSD.TreasureCatchProgressGainRateAdditive;
					}

					TreasureProgressChange *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.TreasureCatchProgressGainRateMultiplier : 1;
					TreasureProgressChange *= (1 + (CastDistance * Configs.CastDistance.TreasureCatchProgressGainRateMultiplier));
					TreasureProgressChange *= (1 + (DailyLuck * Configs.DailyLuck.TreasureCatchProgressGainRateMultiplier));
					TreasureProgressChange *= (1 + (LuckLevel * Configs.PlayerLuck.TreasureCatchProgressGainRateMultiplier));
					TreasureProgressChange *= (1 + (Difficulty * Configs.FishDifficulty.TreasureCatchProgressGainRateMultiplier));
					TreasureProgressChange *= (1 + (FishingLevel * Configs.FishingLevel.TreasureCatchProgressGainRateMultiplier));
					TreasureProgressChange *= Configs.Bait == null ? 1.0f : Configs.Bait.TreasureCatchProgressGainRateMultiplier;
					TreasureProgressChange *= Configs.Tackle == null ? 1.0f : Configs.Tackle.TreasureCatchProgressGainRateMultiplier;
					TreasureProgressChange *= Configs.FishMotion.TreasureCatchProgressGainRateMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						TreasureProgressChange *= LSD.TreasureCatchProgressGainRateMultiplier;
					}

					TreasureCatchLevel += TreasureProgressChange;
					TreasureShake = new Vector2(Game1.random.Next(-2, 3), Game1.random.Next(-2, 3));
					if (TreasureCatchLevel >= 1.0) {
						Game1.playSound("newArtifact");
						TreasureCaught = true;
					}
				}
				else if (TreasureCaught) {
					TreasureScale = Math.Max(0.0f, TreasureScale - 0.1f);
				}
				else {
					TreasureShake = Vector2.Zero;

					float TreasureProgressChange = Configs.GeneralSettings.BaseTreasureCatchProgressLossRate;
					TreasureProgressChange += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.TreasureCatchProgressLossRateAdditive : 0;
					TreasureProgressChange += CastDistance * Configs.CastDistance.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += DailyLuck * Configs.DailyLuck.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += LuckLevel * Configs.PlayerLuck.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += Difficulty * Configs.FishDifficulty.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += FishingLevel * Configs.FishingLevel.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += Configs.Bait == null ? 0 : Configs.Bait.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += Configs.Tackle == null ? 0 : Configs.Tackle.TreasureCatchProgressLossRateAdditive;
					TreasureProgressChange += Configs.FishMotion.TreasureCatchProgressLossRateAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						TreasureProgressChange += LSD.TreasureCatchProgressLossRateAdditive;
					}

					TreasureProgressChange *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.TreasureCatchProgressLossRateMultiplier : 1;
					TreasureProgressChange *= (1 + (CastDistance * Configs.CastDistance.TreasureCatchProgressLossRateMultiplier));
					TreasureProgressChange *= (1 + (DailyLuck * Configs.DailyLuck.TreasureCatchProgressLossRateMultiplier));
					TreasureProgressChange *= (1 + (LuckLevel * Configs.PlayerLuck.TreasureCatchProgressLossRateMultiplier));
					TreasureProgressChange *= (1 + (Difficulty * Configs.FishDifficulty.TreasureCatchProgressLossRateMultiplier));
					TreasureProgressChange *= (1 + (FishingLevel * Configs.FishingLevel.TreasureCatchProgressLossRateMultiplier));
					TreasureProgressChange *= Configs.Bait == null ? 1 : Configs.Bait.TreasureCatchProgressLossRateMultiplier;
					TreasureProgressChange *= Configs.Tackle == null ? 1 : Configs.Tackle.TreasureCatchProgressLossRateMultiplier;
					TreasureProgressChange *= Configs.FishMotion.TreasureCatchProgressLossRateMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						TreasureProgressChange *= LSD.TreasureCatchProgressLossRateMultiplier;
					}

					TreasureCatchLevel -= TreasureProgressChange;
				}

				TreasureCatchLevel = TreasureCatchLevel.Clamp(0, 1);
			}
		}

		public virtual void FishEscaped(GameTime time) {
			FadeOut = true;
			EverythingShakeTimer = 500f;
			Game1.playSound("fishEscape");
			if (BobberBar.unReelSound != null) {
				BobberBar.unReelSound.Stop(AudioStopOptions.Immediate);
			}
			if (BobberBar.reelSound != null) {
				BobberBar.reelSound.Stop(AudioStopOptions.Immediate);
			}
		}

		public virtual void FishCaught(GameTime time) {
			EverythingShakeTimer = 500f;
			Game1.playSound("jingle1");
			FadeOut = true;
			if (BobberBar.unReelSound != null) {
				BobberBar.unReelSound.Stop(AudioStopOptions.Immediate);
			}
			if (BobberBar.reelSound != null) {
				BobberBar.reelSound.Stop(AudioStopOptions.Immediate);
			}
			if (Perfect) {
				SparkleText = new SparklingText(Game1.dialogueFont, "Perfect!", Color.Yellow, Color.White, false, 0.1, 1500, -1, 500);
				if (Game1.isFestival()) {
					Game1.CurrentEvent.perfectFishing();
				}
			}
			else if (FishSize == FishSizeMax) {
				--FishSize;
			}
		}

		public virtual void ReelIn(GameTime time) {
			float CatchProgressChange = Configs.GeneralSettings.BaseFishCatchProgressGainRate;
			CatchProgressChange += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishCatchProgressGainRateAdditive : 0;
			CatchProgressChange += CastDistance * Configs.CastDistance.FishCatchProgressGainRateAdditive;
			CatchProgressChange += DailyLuck * Configs.DailyLuck.FishCatchProgressGainRateAdditive;
			CatchProgressChange += LuckLevel * Configs.PlayerLuck.FishCatchProgressGainRateAdditive;
			CatchProgressChange += FishingLevel * Configs.FishingLevel.FishCatchProgressGainRateAdditive;
			CatchProgressChange += Configs.Bait == null ? 0 : Configs.Bait.FishCatchProgressGainRateAdditive;
			CatchProgressChange += Configs.Tackle == null ? 0 : Configs.Tackle.FishCatchProgressGainRateAdditive;
			CatchProgressChange += Configs.FishMotion.FishCatchProgressGainRateAdditive;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				CatchProgressChange += LSD.FishCatchProgressGainRateAdditive;
			}

			CatchProgressChange *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishCatchProgressGainRateMultiplier : 1;
			CatchProgressChange *= (1 + (CastDistance * Configs.CastDistance.FishCatchProgressGainRateMultiplier));
			CatchProgressChange *= (1 + (DailyLuck * Configs.DailyLuck.FishCatchProgressGainRateMultiplier));
			CatchProgressChange *= (1 + (LuckLevel * Configs.PlayerLuck.FishCatchProgressGainRateMultiplier));
			CatchProgressChange *= (1 + (Difficulty * Configs.FishDifficulty.FishCatchProgressGainRateMultiplier));
			CatchProgressChange *= (1 + (FishingLevel * Configs.FishingLevel.FishCatchProgressGainRateMultiplier));
			CatchProgressChange *= Configs.Bait == null ? 1 : Configs.Bait.FishCatchProgressGainRateMultiplier;
			CatchProgressChange *= Configs.Tackle == null ? 1 : Configs.Tackle.FishCatchProgressGainRateMultiplier;
			CatchProgressChange *= Configs.FishMotion.FishCatchProgressGainRateMultiplier;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				CatchProgressChange *= LSD.FishCatchProgressGainRateMultiplier;
			}

			DistanceFromCatching += CatchProgressChange;
			DistanceFromCatching = DistanceFromCatching.Clamp(0, 1);

			ReelRotation += 0.3926991f;
			FishShake = new Vector2(Game1.random.Next(-10, 11) / 10f, Game1.random.Next(-10, 11) / 10f);
			BarShake = Vector2.Zero;
			Rumble.rumble(0.1f, 1000f);

			if (BobberBar.unReelSound != null) {
				BobberBar.unReelSound.Stop(AudioStopOptions.Immediate);
			}
			if (Game1.soundBank != null && (BobberBar.reelSound == null || BobberBar.reelSound.IsStopped || BobberBar.reelSound.IsStopping)) {
				BobberBar.reelSound = Game1.soundBank.GetCue("fastReel");
			}
			if (BobberBar.reelSound != null && !BobberBar.reelSound.IsPlaying && !BobberBar.reelSound.IsStopping) {
				BobberBar.reelSound.Play();
			}
		}

		public virtual void ReelOut(GameTime time) {
			if (Player.fishCaught != null && Player.fishCaught.Count != 0 || Game1.currentMinigame != null) {
				float CatchProgressChange = Configs.GeneralSettings.BaseFishCatchProgressLossRate;
				CatchProgressChange += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishCatchProgressLossRateAdditive : 0;
				CatchProgressChange += CastDistance * Configs.CastDistance.FishCatchProgressLossRateAdditive;
				CatchProgressChange += DailyLuck * Configs.DailyLuck.FishCatchProgressLossRateAdditive;
				CatchProgressChange += LuckLevel * Configs.PlayerLuck.FishCatchProgressLossRateAdditive;
				CatchProgressChange += Difficulty * Configs.FishDifficulty.FishCatchProgressLossRateAdditive;
				CatchProgressChange += FishingLevel * Configs.FishingLevel.FishCatchProgressLossRateAdditive;
				CatchProgressChange += Configs.Bait == null ? 0 : Configs.Bait.FishCatchProgressLossRateAdditive;
				CatchProgressChange += Configs.Tackle == null ? 0 : Configs.Tackle.FishCatchProgressLossRateAdditive;
				CatchProgressChange += Configs.FishMotion.FishCatchProgressLossRateAdditive;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					CatchProgressChange += LSD.FishCatchProgressLossRateAdditive;
				}

				if (TreasureInBar) {
					CatchProgressChange += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishEscapeDuringTreasureAdditive : 0;
					CatchProgressChange += CastDistance * Configs.CastDistance.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += DailyLuck * Configs.DailyLuck.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += LuckLevel * Configs.PlayerLuck.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += Difficulty * Configs.FishDifficulty.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += FishingLevel * Configs.FishingLevel.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += Configs.Bait == null ? 0 : Configs.Bait.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += Configs.Tackle == null ? 0 : Configs.Tackle.FishEscapeDuringTreasureAdditive;
					CatchProgressChange += Configs.FishMotion.FishEscapeDuringTreasureAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						CatchProgressChange += LSD.FishEscapeDuringTreasureAdditive;
					}
				}

				CatchProgressChange *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishCatchProgressLossRateMultiplier : 1;
				CatchProgressChange *= (1 + (CastDistance * Configs.CastDistance.FishCatchProgressLossRateMultiplier));
				CatchProgressChange *= (1 + (DailyLuck * Configs.DailyLuck.FishCatchProgressLossRateMultiplier));
				CatchProgressChange *= (1 + (LuckLevel * Configs.PlayerLuck.FishCatchProgressLossRateMultiplier));
				CatchProgressChange *= (1 + (Difficulty * Configs.FishDifficulty.FishCatchProgressLossRateMultiplier));
				CatchProgressChange *= (1 + (FishingLevel * Configs.FishingLevel.FishCatchProgressLossRateMultiplier));
				CatchProgressChange *= Configs.Bait == null ? 1 : Configs.Bait.FishCatchProgressLossRateMultiplier;
				CatchProgressChange *= Configs.Tackle == null ? 1 : Configs.Tackle.FishCatchProgressLossRateMultiplier;
				CatchProgressChange *= Configs.FishMotion.FishCatchProgressLossRateMultiplier;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					CatchProgressChange *= LSD.FishCatchProgressLossRateMultiplier;
				}

				if (TreasureInBar) {
					CatchProgressChange *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishEscapeDuringTreasureMultiplier : 1;
					CatchProgressChange *= (1 + (CastDistance * Configs.CastDistance.FishEscapeDuringTreasureMultiplier));
					CatchProgressChange *= (1 + (DailyLuck * Configs.DailyLuck.FishEscapeDuringTreasureMultiplier));
					CatchProgressChange *= (1 + (LuckLevel * Configs.PlayerLuck.FishEscapeDuringTreasureMultiplier));
					CatchProgressChange *= (1 + (Difficulty * Configs.FishDifficulty.FishEscapeDuringTreasureMultiplier));
					CatchProgressChange *= (1 + (FishingLevel * Configs.FishingLevel.FishEscapeDuringTreasureMultiplier));
					CatchProgressChange *= Configs.Bait == null ? 1 : Configs.Bait.FishEscapeDuringTreasureMultiplier;
					CatchProgressChange *= Configs.Tackle == null ? 1 : Configs.Tackle.FishEscapeDuringTreasureMultiplier;
					CatchProgressChange *= Configs.FishMotion.FishEscapeDuringTreasureMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						CatchProgressChange *= LSD.FishEscapeDuringTreasureMultiplier;
					}
				}

				DistanceFromCatching -= CatchProgressChange;
				DistanceFromCatching = DistanceFromCatching.Clamp(0, 1);
			}

			if (!TreasureInBar || TreasureCaught) {
				if (!FishShake.Equals(Vector2.Zero)) {
					Game1.playSound("tinyWhip");
					if (!Configs.GeneralSettings.AlwaysPerfect) {
						Perfect = false;
					}
					Rumble.stopRumbling();
				}

				FishSizeReductionTimer -= time.ElapsedGameTime.Milliseconds;
				if (FishSizeReductionTimer <= 0) {
					FishSize = Math.Max(FishSizeMin, FishSize - 1);

					float Timer = Configs.GeneralSettings.BaseFishSizeReductionTimer;
					Timer += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishSizeReductionTimerAdditive : 0;
					Timer += CastDistance * Configs.CastDistance.FishSizeReductionTimerAdditive;
					Timer += DailyLuck * Configs.DailyLuck.FishSizeReductionTimerAdditive;
					Timer += LuckLevel * Configs.PlayerLuck.FishSizeReductionTimerAdditive;
					Timer += Difficulty * Configs.FishDifficulty.FishSizeReductionTimerAdditive;
					Timer += FishingLevel * Configs.FishingLevel.FishSizeReductionTimerAdditive;
					Timer += Configs.Bait == null ? 0 : Configs.Bait.FishSizeReductionTimerAdditive;
					Timer += Configs.Tackle == null ? 0 : Configs.Tackle.FishSizeReductionTimerAdditive;
					Timer += Configs.FishMotion.FishSizeReductionTimerAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						Timer += LSD.FishSizeReductionTimerAdditive;
					}

					Timer *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishSizeReductionTimerMultiplier : 1;
					Timer *= (1 + (DailyLuck * Configs.DailyLuck.FishSizeReductionTimerMultiplier));
					Timer *= (1 + (CastDistance * Configs.CastDistance.FishSizeReductionTimerMultiplier));
					Timer *= (1 + (LuckLevel * Configs.PlayerLuck.FishSizeReductionTimerMultiplier));
					Timer *= (1 + (Difficulty * Configs.FishDifficulty.FishSizeReductionTimerMultiplier));
					Timer *= (1 + (FishingLevel * Configs.FishingLevel.FishSizeReductionTimerMultiplier));
					Timer *= Configs.Bait == null ? 1 : Configs.Bait.FishSizeReductionTimerMultiplier;
					Timer *= Configs.Tackle == null ? 1 : Configs.Tackle.FishSizeReductionTimerMultiplier;
					Timer *= Configs.FishMotion.FishSizeReductionTimerMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						Timer *= LSD.FishSizeReductionTimerMultiplier;
					}

					FishSizeReductionTimer = (int) Timer;
				}

				ReelRotation -= 3.141593f / Math.Max(10f, 200f - Math.Abs(BobberPosition - (BobberBarPosition + (BobberBarHeight / 2))));
				BarShake = new Vector2(Game1.random.Next(-10, 11) / 10f, Game1.random.Next(-10, 11) / 10f);
				FishShake = Vector2.Zero;
				if (BobberBar.reelSound != null) {
					BobberBar.reelSound.Stop(AudioStopOptions.Immediate);
				}
				if (Game1.soundBank != null && (BobberBar.unReelSound == null || BobberBar.unReelSound.IsStopped)) {
					BobberBar.unReelSound = Game1.soundBank.GetCue("slowReel");
					BobberBar.unReelSound.SetVariable("Pitch", 600f);
				}
				if (BobberBar.unReelSound != null && !BobberBar.unReelSound.IsPlaying && !BobberBar.unReelSound.IsStopping) {
					BobberBar.unReelSound.Play();
				}
			}
		}

		public virtual void MoveFish(GameTime time) {
			if (Game1.random.NextDouble() < Difficulty * (MotionType == Motions.Smooth ? 20.0 : 1.0) / 4000.0 && (MotionType != Motions.Smooth || BobberTargetPosition == -1f)) {
				float HeightAboveBobber = 548f - BobberPosition;
				float DifficultyAdjustment = Math.Min(99f, Difficulty + Game1.random.Next(10, 45)) / 100f;
				BobberTargetPosition = BobberPosition + Game1.random.Next(-(int) BobberPosition, (int) HeightAboveBobber) * DifficultyAdjustment;
			}

			CalculateFishAcceleration();

			BobberPosition += BobberSpeed + FloaterSinkerAcceleration;

			ApplyGravity();

			BobberPosition = BobberPosition.Clamp(0, 532);
		}

		public virtual void CalculateFishAcceleration() {
			float AccelerationMultiplier = Configs.GeneralSettings.BaseFishAccelerationMultiplier;
			AccelerationMultiplier *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.FishAccelerationMultiplier : 1;
			AccelerationMultiplier *= (1 + (CastDistance * Configs.CastDistance.FishAccelerationMultiplier));
			AccelerationMultiplier *= (1 + (DailyLuck * Configs.DailyLuck.FishAccelerationMultiplier));
			AccelerationMultiplier *= (1 + (LuckLevel * Configs.PlayerLuck.FishAccelerationMultiplier));
			AccelerationMultiplier *= (1 + (Difficulty * Configs.FishDifficulty.FishAccelerationMultiplier));
			AccelerationMultiplier *= (1 + (FishingLevel * Configs.FishingLevel.FishAccelerationMultiplier));
			AccelerationMultiplier *= Configs.Bait == null ? 1 : Configs.Bait.FishAccelerationMultiplier;
			AccelerationMultiplier *= Configs.Tackle == null ? 1 : Configs.Tackle.FishAccelerationMultiplier;
			AccelerationMultiplier *= Configs.FishMotion.FishAccelerationMultiplier;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				AccelerationMultiplier *= LSD.FishAccelerationMultiplier;
			}

			if (BobberTargetPosition < BobberPosition && BobberTargetPosition != -1) { // Target location is above fish's current location? Need to confirm through testing.
				FloaterSinkerAcceleration = Math.Max(FloaterSinkerAcceleration - Configs.FishMotion.FloatAccelerationBonus, -Configs.FishMotion.FloatAccelerationBonusLimit);
				FloaterSinkerAcceleration *= AccelerationMultiplier;
			}
			else if (BobberTargetPosition > BobberPosition) {
				FloaterSinkerAcceleration = Math.Min(FloaterSinkerAcceleration + Configs.FishMotion.SinkAccelerationBonus, Configs.FishMotion.SinkAccelerationBonusLimit);
				FloaterSinkerAcceleration *= AccelerationMultiplier;
			}
			else {
				FloaterSinkerAcceleration = 0;
			}

			if (Math.Abs(BobberPosition - BobberTargetPosition) > 3.0 && BobberTargetPosition != -1.0) {
				BobberAcceleration = AccelerationMultiplier * ((BobberTargetPosition - BobberPosition) / (Game1.random.Next(10, 30) + (100f - Math.Min(100f, Difficulty))));
				BobberSpeed += ((BobberAcceleration - BobberSpeed) / 5.0f);
			}
			else {
				BobberTargetPosition = Game1.random.NextDouble() >= (Difficulty * Configs.FishMotion.SmoothMovementChance) ? -1f : BobberPosition + (Game1.random.NextDouble() < 0.5 ? Game1.random.Next(-100, -51) : Game1.random.Next(50, 101));
			}
			if (Game1.random.NextDouble() < Difficulty * Configs.FishMotion.DartChance) {
				BobberTargetPosition = BobberPosition + (Game1.random.NextDouble() < 0.5 ? Game1.random.Next(-100 - (int) (Difficulty * 2), -51) : Game1.random.Next(50, (int) (101 + Difficulty * 2)));
			}

			BobberTargetPosition = BobberTargetPosition.Clamp(-1, 548);
		}

		public virtual void ApplyGravity() {
			float Gravity = Configs.GeneralSettings.BaseGravityFactor;
			Gravity += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.GravityFactorAdditive : 0;
			Gravity += CastDistance * Configs.CastDistance.GravityFactorAdditive;
			Gravity += DailyLuck * Configs.DailyLuck.GravityFactorAdditive;
			Gravity += LuckLevel * Configs.PlayerLuck.GravityFactorAdditive;
			Gravity += Difficulty * Configs.FishDifficulty.GravityFactorAdditive;
			Gravity += FishingLevel * Configs.FishingLevel.GravityFactorAdditive;
			Gravity += Configs.Bait == null ? 0 : Configs.Bait.GravityFactorAdditive;
			Gravity += Configs.Tackle == null ? 0 : Configs.Tackle.GravityFactorAdditive;
			Gravity += Configs.FishMotion.GravityFactorAdditive;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				Gravity += LSD.GravityFactorAdditive;
			}

			Gravity *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.GravityFactorMultiplier : 1;
			Gravity *= (1 + (CastDistance * Configs.CastDistance.GravityFactorMultiplier));
			Gravity *= (1 + (DailyLuck * Configs.DailyLuck.GravityFactorMultiplier));
			Gravity *= (1 + (LuckLevel * Configs.PlayerLuck.GravityFactorMultiplier));
			Gravity *= (1 + (Difficulty * Configs.FishDifficulty.GravityFactorMultiplier));
			Gravity *= (1 + (FishingLevel * Configs.FishingLevel.GravityFactorMultiplier));
			Gravity *= Configs.Bait == null ? 1 : Configs.Bait.GravityFactorMultiplier;
			Gravity *= Configs.Tackle == null ? 1 : Configs.Tackle.GravityFactorMultiplier;
			Gravity *= Configs.FishMotion.GravityFactorMultiplier;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				Gravity *= LSD.GravityFactorMultiplier;
			}

			Gravity = Math.Max(0, Gravity);
			BobberPosition += Gravity;
		}

		public virtual void MoveBobberBar(GameTime time) {
			bool ButtonWasPressed = ButtonPressed;
			ButtonPressed = Game1.oldMouseState.LeftButton == ButtonState.Pressed || Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton) || Game1.options.gamepadControls && (Game1.oldPadState.IsButtonDown(Buttons.X) || Game1.oldPadState.IsButtonDown(Buttons.A));
			if (!ButtonWasPressed && ButtonPressed) {
				Game1.playSound("fishingRodBend");
			}

			CalculateBobberBarAcceleration();

			if (BobberInBar) {
				ApplyHookStickiness();
			}

			CheckForBounce();
		}

		public virtual void CalculateBobberBarAcceleration() {
			float BobberBarAcceleration = ButtonPressed ? -0.25f : 0.25f;
			if (ButtonPressed && BobberBarAcceleration < 0.0 && (BobberBarPosition == 0.0 || BobberBarPosition == (568 - BobberBarHeight))) {
				BobberBarSpeed = 0.0f;
			}
			if (BobberInBar) {
				BobberBarAcceleration *= Configs.Bait == null ? 1 : Configs.Bait.HookAccelerationMultiplier;
				BobberBarAcceleration *= Configs.Tackle == null ? 1 : Configs.Tackle.HookAccelerationMultiplier;
				BobberBarAcceleration *= Configs.FishMotion.HookAccelerationMultiplier;
			}

			BobberBarSpeed += BobberBarAcceleration;
		}

		public virtual void ApplyHookStickiness() {
			float SpeedAdjustment = Configs.GeneralSettings.BaseHookStickiness;
			SpeedAdjustment += FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.HookStickinessAdditive : 0;
			SpeedAdjustment += CastDistance * Configs.CastDistance.HookStickinessAdditive;
			SpeedAdjustment += DailyLuck * Configs.DailyLuck.HookStickinessAdditive;
			SpeedAdjustment += LuckLevel * Configs.PlayerLuck.HookStickinessAdditive;
			SpeedAdjustment += Difficulty * Configs.FishDifficulty.HookStickinessAdditive;
			SpeedAdjustment += FishingLevel * Configs.FishingLevel.HookStickinessAdditive;
			SpeedAdjustment += Configs.Bait == null ? 0 : Configs.Bait.HookStickinessAdditive;
			SpeedAdjustment += Configs.Tackle == null ? 0 : Configs.Tackle.HookStickinessAdditive;
			SpeedAdjustment += Configs.FishMotion.HookStickinessAdditive;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				SpeedAdjustment += LSD.HookStickinessAdditive;
			}

			SpeedAdjustment *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.HookStickinessMultiplier : 1;
			SpeedAdjustment *= (1 + (CastDistance * Configs.CastDistance.HookStickinessMultiplier));
			SpeedAdjustment *= (1 + (DailyLuck * Configs.DailyLuck.HookStickinessMultiplier));
			SpeedAdjustment *= (1 + (LuckLevel * Configs.PlayerLuck.HookStickinessMultiplier));
			SpeedAdjustment *= (1 + (Difficulty * Configs.FishDifficulty.HookStickinessMultiplier));
			SpeedAdjustment *= (1 + (FishingLevel * Configs.FishingLevel.HookStickinessMultiplier));
			SpeedAdjustment *= Configs.Bait == null ? 1 : Configs.Bait.HookStickinessMultiplier;
			SpeedAdjustment *= Configs.Tackle == null ? 1 : Configs.Tackle.HookStickinessMultiplier;
			SpeedAdjustment *= Configs.FishMotion.HookStickinessMultiplier;
			foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
				SpeedAdjustment *= LSD.HookStickinessMultiplier;
			}

			if (BobberPosition + 16 < BobberBarPosition + (BobberBarHeight / 2f)) {
				BobberBarSpeed -= SpeedAdjustment;
			}
			else {
				BobberBarSpeed += SpeedAdjustment;
			}
		}

		public virtual void CheckForBounce() {
			float BobberBarOldPosition = BobberBarPosition;
			BobberBarPosition += BobberBarSpeed;

			if (BobberBarPosition + BobberBarHeight > 568.0) {
				BobberBarPosition = (568f - BobberBarHeight);

				float Bounciness = Configs.GeneralSettings.BaseBounciness;
				Bounciness *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.BobberBarBottomBounciness : 1;
				Bounciness *= (1 + (CastDistance * Configs.CastDistance.BobberBarBottomBounciness));
				Bounciness *= (1 + (DailyLuck * Configs.DailyLuck.BobberBarBottomBounciness));
				Bounciness *= (1 + (LuckLevel * Configs.PlayerLuck.BobberBarBottomBounciness));
				Bounciness *= (1 + (Difficulty * Configs.FishDifficulty.BobberBarBottomBounciness));
				Bounciness *= (1 + (FishingLevel * Configs.FishingLevel.BobberBarBottomBounciness));
				Bounciness *= Configs.Bait == null ? 1 : Configs.Bait.BobberBarBottomBounciness;
				Bounciness *= Configs.Tackle == null ? 1 : Configs.Tackle.BobberBarBottomBounciness;
				Bounciness *= Configs.FishMotion.BobberBarBottomBounciness;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					Bounciness *= LSD.BobberBarBottomBounciness;
				}

				BobberBarSpeed = (-BobberBarSpeed * Bounciness);
				if (BobberBarOldPosition + BobberBarHeight < 568.0) {
					Game1.playSound("shiny4");
				}
			}
			else if (BobberBarPosition < 0.0) {
				BobberBarPosition = 0.0f;

				float Bounciness = Configs.GeneralSettings.BaseBounciness;
				Bounciness *= FishingTunerReduxMod.BobberIsOnFishingPoint ? Configs.FishingPoint.BobberBarTopBounciness : 1;
				Bounciness *= (1 + (CastDistance * Configs.CastDistance.BobberBarTopBounciness));
				Bounciness *= (1 + (DailyLuck * Configs.DailyLuck.BobberBarTopBounciness));
				Bounciness *= (1 + (LuckLevel * Configs.PlayerLuck.BobberBarTopBounciness));
				Bounciness *= (1 + (Difficulty * Configs.FishDifficulty.BobberBarTopBounciness));
				Bounciness *= (1 + (FishingLevel * Configs.FishingLevel.BobberBarTopBounciness));
				Bounciness *= Configs.Bait == null ? 1 : Configs.Bait.BobberBarTopBounciness;
				Bounciness *= Configs.Tackle == null ? 1 : Configs.Tackle.BobberBarTopBounciness;
				Bounciness *= Configs.FishMotion.BobberBarTopBounciness;
				foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
					Bounciness *= LSD.BobberBarTopBounciness;
				}

				BobberBarSpeed = (-BobberBarSpeed * Bounciness);
				if (BobberBarOldPosition > 0.0) {
					Game1.playSound("shiny4");
				}
			}
		}
	}
}