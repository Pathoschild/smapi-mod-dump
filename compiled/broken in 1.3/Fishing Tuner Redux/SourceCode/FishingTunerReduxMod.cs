using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace FishingTunerRedux {
	public class FishingTunerReduxMod : Mod
    {
		public static Farmer Player => Game1.player;
		public static GameLocation Location => Player.currentLocation;
		public static IClickableMenu ActiveMenu => Game1.activeClickableMenu;
		public static BobberBar BaseBobber => ActiveMenu as BobberBar;
		public static FishingRod Rod => Player.CurrentTool as FishingRod;
		public static int LuckLevel => Player.luckLevel;
		public static int FishingLevel => Player.fishingLevel;
		public static float DailyLuck => (float) Game1.dailyLuck;

		public static Random rand;
		public static Keys RefreshConfigKey;
		public static bool Fishing;
		public static bool Nibbling;
		public static bool BobberIsOnFishingPoint;

		public static Dictionary<int, List<int>> LegendariesCaught = new Dictionary<int, List<int>>();

		public static IMonitor StaticMonitor;

		public override void Entry(IModHelper helper) {
			rand = new Random();
			MenuEvents.MenuChanged += MenuEvents_MenuChanged;
			GameEvents.OneSecondTick += GameEvents_OneSecondTick;
			GameEvents.UpdateTick += GameEvents_OnUpdateTick;
			SaveEvents.BeforeSave += SaveEvents_BeforeSave;
			SaveEvents.AfterSave += SaveEvents_AfterSave;
			SaveEvents.AfterLoad += SaveEvents_AfterLoad;
			
			ControlEvents.KeyPressed += ControlEvents_KeyPressed;

			InitializeConfigs();

			StaticMonitor = Monitor;
		}

		public virtual void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e) {
			if (e.NewMenu is BobberBar && !(e.NewMenu is CustomFishingGame)) {
				Game1.activeClickableMenu = new CustomFishingGame(e.NewMenu as BobberBar, this);

				if (Configs.GeneralSettings.VerboseLogging) { Monitor.Log("Fishing Tuner Redux: Intercepted BobberBar menu and replaced with CustomFishingGame menu.", LogLevel.Info); }
			}
		}

		public virtual void GameEvents_OneSecondTick(object sender, EventArgs e) {
			if (Configs.GeneralSettings.InfiniteBait && Player?.CurrentTool is FishingRod && Player?.CurrentTool?.attachments?.Length > 1 && Player.CurrentTool.attachments[1] != null) {
				Player.CurrentTool.attachments[0].Stack = Player.CurrentTool.attachments[0].maximumStackSize();
			}

			if (Configs.GeneralSettings.InfiniteTackle && Player?.CurrentTool is FishingRod && Player?.CurrentTool?.attachments?.Length > 1 && Player.CurrentTool.attachments[1] != null) {
				Player.CurrentTool.attachments[1].Stack = Player.CurrentTool.attachments[1].maximumStackSize();
				Player.CurrentTool.attachments[1].scale = new Vector2(Player.CurrentTool.attachments[1].scale.X, 1.1f);
			}

			if (Configs.GeneralSettings.RecatchLegendaries && !Configs.GeneralSettings.RecatchLegendariesOnceDaily) {
				ClearLegendaryFish();
			}

			if (!Player.usingTool || !(Player.CurrentTool is FishingRod && Rod.isFishing)) {
				if (!(Game1.activeClickableMenu is BobberBar)) {
					Fishing = false;
					Nibbling = false;
				}
			}
		}

		public virtual void GameEvents_OnUpdateTick(object sender, EventArgs e) {
			if (Player.usingTool && Player.CurrentTool is FishingRod && Rod.isFishing) {
				if (!Fishing) {
					Fishing = true;

					Configs.Bait = null;
					if (Rod.attachments?.Length > 0 && Rod.attachments[0] != null) {
						LoadBaitConfig(Rod.attachments[0].parentSheetIndex);
					}

					Configs.Tackle = null;
					if (Rod.attachments?.Length > 1 && Rod.attachments[1] != null) {
						LoadTackleConfig(Rod.attachments[1].parentSheetIndex);
					}

					Vector2 BobberLocation = (Vector2) typeof(FishingRod).GetField("bobber", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance).GetValue(Rod);
					Point SplashPoint = Location.fishSplashPoint;
					Rectangle rectangle = new Rectangle(Location.fishSplashPoint.X * Game1.tileSize, Location.fishSplashPoint.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
					if (new Rectangle((int) BobberLocation.X - Game1.tileSize * 5 / 4, (int) BobberLocation.Y - Game1.tileSize * 5 / 4, Game1.tileSize, Game1.tileSize).Intersects(rectangle)) {
						BobberIsOnFishingPoint = true;
					}
					else { BobberIsOnFishingPoint = false; }

					Configs.AppliedLocationEffects.Clear();
					foreach (LocationSettingsBoundary LSB in Configs.LocationSettings.LocationSettings) {
						if (LSB.WithinBoundary(Game1.currentLocation, BobberLocation)) {
							Configs.AppliedLocationEffects.Add(LSB.Settings);
						}
					}

					float minTime = Configs.GeneralSettings.MinTimeUntilBite;
					minTime += BobberIsOnFishingPoint ? Configs.FishingPoint.MinTimeUntilBiteAdditive : 0;
					minTime += DailyLuck * Configs.DailyLuck.MinTimeUntilBiteAdditive;
					minTime += LuckLevel * Configs.PlayerLuck.MinTimeUntilBiteAdditive;
					minTime += FishingLevel == 0 ? 0 : (FishingLevel * Configs.FishingLevel.MinTimeUntilBiteAdditive);
					minTime += Configs.Bait == null ? 0 : Configs.Bait.MinTimeUntilBiteAdditive;
					minTime += Configs.Tackle == null ? 0 : Configs.Tackle.MinTimeUntilBiteAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						minTime += LSD.MinTimeUntilBiteAdditive;
					}

					minTime *= BobberIsOnFishingPoint ? Configs.FishingPoint.MinTimeUntilBiteMultiplier : 1;
					minTime *= (1 + (DailyLuck * Configs.DailyLuck.MinTimeUntilBiteMultiplier));
					minTime *= (1 + (LuckLevel * Configs.PlayerLuck.MinTimeUntilBiteMultiplier));
					minTime *= (1 + (FishingLevel * Configs.FishingLevel.MinTimeUntilBiteMultiplier));
					minTime *= Configs.Bait == null ? 1 : Configs.Bait.MinTimeUntilBiteMultiplier;
					minTime *= Configs.Tackle == null ? 1 : Configs.Tackle.MinTimeUntilBiteMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						minTime *= LSD.MinTimeUntilBiteMultiplier;
					}

					float maxTime = Configs.GeneralSettings.MaxTimeUntilBite;
					maxTime += BobberIsOnFishingPoint ? Configs.FishingPoint.MaxTimeUntilBiteAdditive : 0;
					maxTime += DailyLuck * Configs.DailyLuck.MaxTimeUntilBiteAdditive;
					maxTime += LuckLevel * Configs.PlayerLuck.MaxTimeUntilBiteAdditive;
					maxTime += FishingLevel * Configs.FishingLevel.MaxTimeUntilBiteAdditive;
					maxTime += Configs.Bait == null ? 0 : Configs.Bait.MaxTimeUntilBiteAdditive;
					maxTime += Configs.Tackle == null ? 0 : Configs.Tackle.MaxTimeUntilBiteAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						maxTime += LSD.MaxTimeUntilBiteAdditive;
					}

					maxTime *= BobberIsOnFishingPoint ? Configs.FishingPoint.MaxTimeUntilBiteMultiplier : 1;
					maxTime *= (1 + (DailyLuck * Configs.DailyLuck.MaxTimeUntilBiteMultiplier));
					maxTime *= (1 + (LuckLevel * Configs.PlayerLuck.MaxTimeUntilBiteMultiplier));
					maxTime *= (1 + (FishingLevel * Configs.FishingLevel.MaxTimeUntilBiteMultiplier));
					maxTime *= Configs.Bait == null ? 1 : Configs.Bait.MaxTimeUntilBiteMultiplier;
					maxTime *= Configs.Tackle == null ? 1 : Configs.Tackle.MaxTimeUntilBiteMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						maxTime *= LSD.MaxTimeUntilBiteMultiplier;
					}

					maxTime = Math.Max(minTime + 1, maxTime);
					Rod.timeUntilFishingBite = rand.Next((int) minTime, (int) maxTime);
					Rod.timeUntilFishingBite = Math.Max(1.0f, Rod.timeUntilFishingBite);

					if (Configs.GeneralSettings.VerboseLogging) { Monitor.Log(String.Format("Fishing Tuner Redux: Rod.timeUntilFishingBite set to {0}", Rod.timeUntilFishingBite), LogLevel.Info); }
				}

				if (Rod.timeUntilFishingNibbleDone != -1 && !Nibbling) {
					Nibbling = true;

					float minTime = Configs.GeneralSettings.MinNibbleTime;
					minTime += BobberIsOnFishingPoint ? Configs.FishingPoint.MinNibbleTimeAdditive : 0;
					minTime += DailyLuck * Configs.DailyLuck.MinNibbleTimeAdditive;
					minTime += LuckLevel * Configs.PlayerLuck.MinNibbleTimeAdditive;
					minTime += FishingLevel == 0 ? 0 : (FishingLevel * Configs.FishingLevel.MinNibbleTimeAdditive);
					minTime += Configs.Bait == null ? 0 : Configs.Bait.MinNibbleTimeAdditive;
					minTime += Configs.Tackle == null ? 0 : Configs.Tackle.MinNibbleTimeAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						minTime += LSD.MinNibbleTimeAdditive;
					}

					minTime *= BobberIsOnFishingPoint ? Configs.FishingPoint.MinNibbleTimeMultiplier : 1;
					minTime *= (1 + (DailyLuck * Configs.DailyLuck.MinNibbleTimeMultiplier));
					minTime *= (1 + (LuckLevel * Configs.PlayerLuck.MinNibbleTimeMultiplier));
					minTime *= (1 + (FishingLevel * Configs.FishingLevel.MinNibbleTimeMultiplier));
					minTime *= Configs.Bait == null ? 1 : Configs.Bait.MinNibbleTimeMultiplier;
					minTime *= Configs.Tackle == null ? 1 : Configs.Tackle.MinNibbleTimeMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						minTime *= LSD.MinNibbleTimeMultiplier;
					}

					float maxTime = Configs.GeneralSettings.MaxNibbleTime;
					maxTime += BobberIsOnFishingPoint ? Configs.FishingPoint.MaxTimeUntilBiteAdditive : 0;
					maxTime += DailyLuck * Configs.DailyLuck.MaxNibbleTimeAdditive;
					maxTime += LuckLevel * Configs.PlayerLuck.MaxNibbleTimeAdditive;
					maxTime += FishingLevel * Configs.FishingLevel.MaxNibbleTimeAdditive;
					maxTime += Configs.Bait == null ? 0 : Configs.Bait.MaxNibbleTimeAdditive;
					maxTime += Configs.Tackle == null ? 0 : Configs.Tackle.MaxNibbleTimeAdditive;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						maxTime += LSD.MaxNibbleTimeAdditive;
					}

					maxTime *= BobberIsOnFishingPoint ? Configs.FishingPoint.MaxTimeUntilBiteMultiplier : 1;
					maxTime *= (1 + (DailyLuck * Configs.DailyLuck.MaxNibbleTimeMultiplier));
					maxTime *= (1 + (LuckLevel * Configs.PlayerLuck.MaxNibbleTimeMultiplier));
					maxTime *= (1 + (FishingLevel * Configs.FishingLevel.MaxNibbleTimeMultiplier));
					maxTime *= Configs.Bait == null ? 1 : Configs.Bait.MaxNibbleTimeMultiplier;
					maxTime *= Configs.Tackle == null ? 1 : Configs.Tackle.MaxNibbleTimeMultiplier;
					foreach (LocationSettingsData LSD in Configs.AppliedLocationEffects) {
						maxTime *= LSD.MaxNibbleTimeMultiplier;
					}

					maxTime = Math.Max(minTime + 1, maxTime);
					Rod.timeUntilFishingNibbleDone = rand.Next((int) minTime, (int) maxTime);
					Rod.timeUntilFishingNibbleDone = Math.Max(1.0f, Rod.timeUntilFishingNibbleDone);

					if (Configs.GeneralSettings.VerboseLogging) { Monitor.Log(String.Format("Fishing Tuner Redux: Rod.timeUntilFishingNibbleDone set to {0}", Rod.timeUntilFishingNibbleDone), LogLevel.Info); }
				}
			}
		}

		public virtual void SaveEvents_BeforeSave(object sender, EventArgs e) {
			foreach (int ID in LegendariesCaught.Keys) {
				if (Configs.GeneralSettings.VerboseLogging) { Monitor.Log("Fishing Tuner Redux: Re-adding legendary fish to Farmer.fishCaught", LogLevel.Info); }
				Player.fishCaught.Add(ID, LegendariesCaught[ID].ToArray());
			}
			LegendariesCaught.Clear();
		}

		public virtual void SaveEvents_AfterSave(object sender, EventArgs e) {
			if (Configs.GeneralSettings.RecatchLegendaries && Configs.GeneralSettings.RecatchLegendariesOnceDaily) {
				ClearLegendaryFish();
			}
		}

		public virtual void SaveEvents_AfterLoad(object sender, EventArgs e) {
			if (Configs.GeneralSettings.RecatchLegendaries && Configs.GeneralSettings.RecatchLegendariesOnceDaily) {
				ClearLegendaryFish();
			}
		}

		public virtual void ClearLegendaryFish() {
			foreach (int ID in LegendaryFish.ToArray()) {
				if (Player.fishCaught.ContainsKey(ID)) {
					if (!LegendariesCaught.ContainsKey(ID)) {
						LegendariesCaught.Add(ID, new List<int>());
					}
					LegendariesCaught[ID].AddRange(Player.fishCaught[ID]);
					Player.fishCaught.Remove(ID);

					if (Configs.GeneralSettings.VerboseLogging) { Monitor.Log("Fishing Tuner Redux: Removing legendary fish from Farmer.fishCaught", LogLevel.Info); }
				}
			}
		}

		public virtual void InitializeConfigs() {
			LoadConfigs();
			Helper.WriteJsonFile(Path.Combine("Configs", "GeneralSettings.json"), Configs.GeneralSettings);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishingPoint.json"), Configs.FishingPoint);
			Helper.WriteJsonFile(Path.Combine("Configs", "CastDistance.json"), Configs.CastDistance);
			Helper.WriteJsonFile(Path.Combine("Configs", "DailyLuck.json"), Configs.DailyLuck);
			Helper.WriteJsonFile(Path.Combine("Configs", "PlayerLuck.json"), Configs.PlayerLuck);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishDifficulty.json"), Configs.FishDifficulty);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishingLevel.json"), Configs.FishingLevel);
			Helper.WriteJsonFile(Path.Combine("Configs", "LocationSettings.json"), Configs.LocationSettings);

			LoadBaitConfig(Baits.Bait);
			Helper.WriteJsonFile(Path.Combine("Configs", "Bait", "Bait.json"), Configs.Bait);
			LoadBaitConfig(Baits.Magnet);
			Helper.WriteJsonFile(Path.Combine("Configs", "Bait", "Magnet.json"), Configs.Bait);
			LoadBaitConfig(Baits.WildBait);
			Helper.WriteJsonFile(Path.Combine("Configs", "Bait", "WildBait.json"), Configs.Bait);

			LoadTackleConfig(Tackles.BarbedHook);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "BarbedHook.json"), Configs.Tackle);
			LoadTackleConfig(Tackles.CorkBobber);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "CorkBobber.json"), Configs.Tackle);
			LoadTackleConfig(Tackles.DressedSpinner);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "DressedSpinner.json"), Configs.Tackle);
			LoadTackleConfig(Tackles.LeadBobber);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "LeadBobber.json"), Configs.Tackle);
			LoadTackleConfig(Tackles.Spinner);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "Spinner.json"), Configs.Tackle);
			LoadTackleConfig(Tackles.TrapBobber);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "TrapBobber.json"), Configs.Tackle);
			LoadTackleConfig(Tackles.TreasureHunter);
			Helper.WriteJsonFile(Path.Combine("Configs", "Tackle", "TreasureHunter.json"), Configs.Tackle);

			LoadMotionConfig(Motions.Mixed);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishMotion", "Mixed.json"), Configs.FishMotion);
			LoadMotionConfig(Motions.Dart);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishMotion", "Dart.json"), Configs.FishMotion);
			LoadMotionConfig(Motions.Smooth);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishMotion", "Smooth.json"), Configs.FishMotion);
			LoadMotionConfig(Motions.Sinker);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishMotion", "Sinker.json"), Configs.FishMotion);
			LoadMotionConfig(Motions.Floater);
			Helper.WriteJsonFile(Path.Combine("Configs", "FishMotion", "Floater.json"), Configs.FishMotion);

			Configs.Bait = null;
			Configs.Tackle = null;
			Configs.FishMotion = null;
		}

		public virtual void LoadBaitConfig(int BaitItem) {
			switch (BaitItem) {
				case (Baits.Bait):
					Configs.Bait = Helper.ReadJsonFile<BaitConfig>(Path.Combine("Configs", "Bait", "Bait.json")) ?? new BaitConfig();
					break;
				case (Baits.Magnet):
					Configs.Bait = Helper.ReadJsonFile<MagnetConfig>(Path.Combine("Configs", "Bait", "Magnet.json")) ?? new MagnetConfig();
					break;
				case (Baits.WildBait):
					Configs.Bait = Helper.ReadJsonFile<WildBaitConfig>(Path.Combine("Configs", "Bait", "WildBait.json")) ?? new WildBaitConfig();
					break;
				default:
					Configs.Bait = null;
					Monitor.Log(String.Format("Fishing Tuner Redux: Unexpected bait value ({0})!", BaitItem), LogLevel.Error);
					break;
			}
		}

		public virtual void LoadTackleConfig(int TackleItem) {
			switch (TackleItem) {
				case (Tackles.Spinner):
					Configs.Tackle = Helper.ReadJsonFile<SpinnerConfig>(Path.Combine("Configs", "Tackle", "Spinner.json")) ?? new SpinnerConfig();
					break;
				case (Tackles.DressedSpinner):
					Configs.Tackle = Helper.ReadJsonFile<DressedSpinnerConfig>(Path.Combine("Configs", "Tackle", "DressedSpinner.json")) ?? new DressedSpinnerConfig();
					break;
				case (Tackles.BarbedHook):
					Configs.Tackle = Helper.ReadJsonFile<BarbedHookConfig>(Path.Combine("Configs", "Tackle", "BarbedHook.json")) ?? new BarbedHookConfig();
					break;
				case (Tackles.LeadBobber):
					Configs.Tackle = Helper.ReadJsonFile<LeadBobberConfig>(Path.Combine("Configs", "Tackle", "LeadBobber.json")) ?? new LeadBobberConfig();
					break;
				case (Tackles.TreasureHunter):
					Configs.Tackle = Helper.ReadJsonFile<TreasureHunterConfig>(Path.Combine("Configs", "Tackle", "TreasureHunter.json")) ?? new TreasureHunterConfig();
					break;
				case (Tackles.TrapBobber):
					Configs.Tackle = Helper.ReadJsonFile<TrapBobberConfig>(Path.Combine("Configs", "Tackle", "TrapBobber.json")) ?? new TrapBobberConfig();
					break;
				case (Tackles.CorkBobber):
					Configs.Tackle = Helper.ReadJsonFile<CorkBobberConfig>(Path.Combine("Configs", "Tackle", "CorkBobber.json")) ?? new CorkBobberConfig();
					break;
				default:
					Configs.Tackle = null;
					Monitor.Log(String.Format("Fishing Tuner Redux: Unexpected tackle value ({0})!", TackleItem), LogLevel.Error);
					break;
			}
		}

		public virtual void LoadMotionConfig(int MotionType) {
			switch (MotionType) {
				case (Motions.Mixed):
					Configs.FishMotion = Helper.ReadJsonFile<MixedMotionConfig>(Path.Combine("Configs", "FishMotion", "Mixed.json")) ?? new MixedMotionConfig();
					break;
				case (Motions.Dart):
					Configs.FishMotion = Helper.ReadJsonFile<DartMotionConfig>(Path.Combine("Configs", "FishMotion", "Dart.json")) ?? new DartMotionConfig();
					break;
				case (Motions.Smooth):
					Configs.FishMotion = Helper.ReadJsonFile<SmoothMotionConfig>(Path.Combine("Configs", "FishMotion", "Smooth.json")) ?? new SmoothMotionConfig();
					break;
				case (Motions.Sinker):
					Configs.FishMotion = Helper.ReadJsonFile<SinkerMotionConfig>(Path.Combine("Configs", "FishMotion", "Sinker.json")) ?? new SinkerMotionConfig();
					break;
				case (Motions.Floater):
					Configs.FishMotion = Helper.ReadJsonFile<FloaterMotionConfig>(Path.Combine("Configs", "FishMotion", "Floater.json")) ?? new FloaterMotionConfig();
					break;
				default:
					Configs.FishMotion = null;
					Monitor.Log(String.Format("Fishing Tuner Redux: Unexpected motion value ({0})!", MotionType), LogLevel.Error);
					break;
			}
		}

		public virtual void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e) {
			if (e.KeyPressed == RefreshConfigKey) {
				LoadConfigs();
				if (Configs.GeneralSettings == null) { Monitor.Log("Fishing Tuner Redux: General Settings config null after reloading!", LogLevel.Error); }
				else if (Configs.GeneralSettings.VerboseLogging) { Monitor.Log("Fishing Tuner Redux: Configs reloaded", LogLevel.Info); }
			}
		}

		public virtual void LoadConfigs() {
			Configs.GeneralSettings = Helper.ReadJsonFile<GeneralSettingsConfig>(Path.Combine("Configs", "GeneralSettings.json")) ?? new GeneralSettingsConfig();
			Configs.FishingPoint = Helper.ReadJsonFile<FishingPointConfig>(Path.Combine("Configs", "FishingPoint.json")) ?? new FishingPointConfig();
			Configs.CastDistance = Helper.ReadJsonFile<CastDistanceConfig>(Path.Combine("Configs", "CastDistance.json")) ?? new CastDistanceConfig();
			Configs.DailyLuck = Helper.ReadJsonFile<DailyLuckConfig>(Path.Combine("Configs", "DailyLuck.json")) ?? new DailyLuckConfig();
			Configs.PlayerLuck = Helper.ReadJsonFile<PlayerLuckConfig>(Path.Combine("Configs", "PlayerLuck.json")) ?? new PlayerLuckConfig();
			Configs.FishDifficulty = Helper.ReadJsonFile<FishDifficultyConfig>(Path.Combine("Configs", "FishDifficulty.json")) ?? new FishDifficultyConfig();
			Configs.FishingLevel = Helper.ReadJsonFile<FishingLevelConfig>(Path.Combine("Configs", "FishingLevel.json")) ?? new FishingLevelConfig();
			Configs.LocationSettings = Helper.ReadJsonFile<LocationSettingsConfig>(Path.Combine("Configs", "LocationSettings.json")) ?? new LocationSettingsConfig();
			RefreshConfigKey = (Keys) Enum.Parse(typeof(Keys), Configs.GeneralSettings.RefreshConfigKey.ToUpper());
		}
    }
}