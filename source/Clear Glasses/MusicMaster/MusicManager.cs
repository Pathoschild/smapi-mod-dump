/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aurpine/Stardew-SpriteMaster
**
*************************************************/

using LinqFasterer;
using Microsoft.Xna.Framework.Audio;
using MusicMaster.Harmonize;
using MusicMaster.Types;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using static StardewValley.Game1;

namespace MusicMaster;

internal static class MusicManager {
	private static float LastMusicVolume = 1.0f;
	private static float MusicVolume => Game1.options.musicVolumeLevel;

	private static class Tracks {
		internal static readonly Track MainTheme = new("maintheme",
			"P5-Colors Flying High -opening version-",
			"P5-Wake Up, Get Up, Get Out There",
			"P5-Royal Days"
		) { RandomPerDay = false };
		internal static readonly Track Ambush = new("ambush", "P5-Keeper of Lust") { RandomPerDay = false, AlwaysStop = true };
		internal static readonly Track Battle = new("battle", "P5-Last Surprise", "P5-Take Over") { RandomPerDay = false, AlwaysStop = true };
		internal static readonly Track Victory = new("victory", "P5-Victory") { RandomPerDay = false };
		internal static readonly Track WizardsHouse = new("wizards_house", "P5-The Poem of Everyones Souls");
		internal static readonly Track AmbientDay = new("ambient_day", "P5-Beneath the Mask -instrumental version-", "P5-Tokyo Daylight") { FadeOut = true };
		internal static readonly Track AmbientNight = new("ambient_night", "P5-Beneath the Mask");
		internal static readonly Track AmbientDayRain = new("ambient_day_rain", "P5-Beneath the Mask -rain, instrumental version-") { FadeOut = true };
		internal static readonly Track AmbientNightRain = new("ambient_night_rain", "P5-Beneath the Mask -rain-");
		internal static readonly Track AmbientDayCommon = new("ambient_day_common", "P5-Tokyo Daylight");
		internal static readonly Track MarlonsShop = new("marlons_shop", "P5-Layer Cake"); // Iwai's Shop Music
		internal static readonly Track Shop = new("shop", "P5-TRIPLE SEVEN"); // Shop Music
		internal static readonly Track DayEnd = new("dayend", "P5-Everyday Days"); // Activity Music
		internal static readonly Track Mines = new("mines", "P5-Ark"); // Shido's Palace
		internal static readonly Track SkullCavern = new("skull_cavern", "P5-The Whims of Fate"); // Nijima's Palace

		internal static Track Ambient {
			get {
				bool raining = Game1.isRaining;
				bool night = Game1.timeOfDay >= 1700;

				return (raining, night) switch {
					(false, false) => Tracks.AmbientDay,
					(true, false) => Tracks.AmbientDayRain,
					(false, true) => Tracks.AmbientNight,
					(true, true) => Tracks.AmbientNightRain
				};
			}
		}

		internal static void Initialize() {
			foreach (var field in typeof(Tracks).GetFields(BindingFlags.NonPublic | BindingFlags.Static)) {
				_ = field.GetValue(null);
			}
		}
	}

		internal static int CurrentDay = -1;
	private static bool IsTitleMenu => Game1.activeClickableMenu is StardewValley.Menus.TitleMenu;

	internal static void Initialize(IMod mod, IModHelper helper) {
		Tracks.Initialize();

		new Thread(FadeOutLoop) {
			Name = "MusicMaster FadeOut Thread",
			IsBackground = true
		}.Start();

		new Thread(PendingCueLoop) {
			Name = "MusicMaster Pending Cue Thread",
			IsBackground = true
		}.Start();

		helper.Events.GameLoop.GameLaunched += (_, _) => OnTitle();
		helper.Events.GameLoop.ReturnedToTitle += (_, _) => OnTitle();
		helper.Events.GameLoop.TimeChanged += (_, _) => OnTimeChanged();
		helper.Events.GameLoop.DayEnding += (_, _) => OnDayEnded();
		helper.Events.GameLoop.DayStarted += (_, _) => OnDayStarted();
		helper.Events.GameLoop.UpdateTicked += (_, _) => OnTicked();

		helper.Events.World.NpcListChanged += (_, args) => OnNpcListChanged(args);

		helper.Events.Player.Warped += (_, args) => OnLocationChanged(args.NewLocation);
	}

	private static void OnTitle() {
		ResetCues();
		ChangeMusicTrack(Tracks.MainTheme);
	}

	private static void OnDayStarted() {
		ResetCues();
		ChangeMusicTrack(Tracks.Ambient);
	}

	private static void OnDayEnded() {
		ChangeMusicTrack(Tracks.DayEnd);
	}

	private static void OnTimeChanged() {
		ChangeMusicTrack(LastTrack);
	}

	private static void OnLocationChanged(GameLocation location) {
		bool combat = Combat != CombatType.None;
		Combat = CombatType.None;
		ChangeMusicTrack(combat ? LastLastTrack : LastTrack);
	}

	private static void OnNpcListChanged(NpcListChangedEventArgs args) {
		if (!args.IsCurrentLocation) {
			return;
		}

		if (Combat is CombatType.None) {
			return;
		}

		if (args.Location.getCharacters().AnyF(npc => npc is Monster {IsMonster: true})) {
			return;
		}

		Combat = CombatType.None;
		ChangeMusicTrack(LastLastTrack);
	}

	private static void OnTicked() {
		UpdateMusicVolume();
	}

	private static readonly object ResetCuesLock = new();
	private static void ResetCues() {
		lock (ResetCuesLock) {

			Combat = CombatType.None;

			FadeOut.Clear();

			foreach (var cue in DailyCues) {
				lock (cue.Cue) {
					cue.Cue.Stop(true);
				}
			}

			DailyCues.Clear();


			if (IsTitleMenu) {
				CurrentDay = -1;
			}
			else {
				int currentSeason = Game1.currentSeason.ToLowerInvariant() switch {
					"spring" => 0,
					"summer" => 1,
					"fall" => 2,
					"winter" => 3,
					_ => 3 // this shouldn't happen
				};
				currentSeason += Game1.year * 4; // 4 seasons per year
				CurrentDay = Game1.dayOfMonth + (currentSeason * 28); // 28 days per season
			}

			CurrentSong = null;
			CurrentTrack = null;

			LastTrack = null;
			LastLastTrack = null;

			lock (PendingCueLock) {
				PendingCue = null;
			}
		}

		Debug.Info("ResetCues");
	}

	private static Track? LastLastTrack = null;
	private static Track? LastTrack = null;

	private static volatile Track? CurrentTrack = null;

	private enum CombatType {
		None = 0,
		Regular = 1,
		Ambush = 2
	}

	private static CombatType Combat = CombatType.None;

	private static ConcurrentSet<TrackCue> DailyCues = new();
	private static ConcurrentSet<TrackCue> FadeOut = new();

	private static volatile TrackCue? CurrentSong = null;

	private static void FadeOutLoop() {
		List<TrackCue> localCues = new();

		while (true) {
			FadeOut.CopyTo(localCues);

			foreach (var cue in localCues) {
				bool finished;
				lock (cue.Cue) {
					float volume = cue.Cue.Volume = MusicVolume * Math.Clamp(cue.Cue.Volume - 0.02f, 0.0f, 1.0f);
					finished = volume == 0.0f;
					if (finished) {
						//cue.Cue.Stop();
					}
				}

				if (finished) {
					FadeOut.Remove(cue);
				}
			}

			bool anyCues = localCues.Count != 0;

			localCues.Clear();

			Thread.Sleep(anyCues ? 100 : 1_000);
		}
	}

	[Harmonize(
		typeof(Farmer),
		"takeDamage",
		Harmonize.Harmonize.Fixation.Prefix,
		Harmonize.Harmonize.PriorityLevel.Last,
		instance: true,
		critical: false
	)]
	public static bool TakeDamage(Farmer __instance, int damage, bool overrideParry, Monster damager) {
		if (__instance != Game1.player) {
			return true;
		}

		if (!__instance.currentLocation.getCharacters().AnyF(npc => npc is Monster { IsMonster: true })) {
			return true;
		}

		if (Combat is CombatType.None) {
			Combat = CombatType.Ambush;
			ChangeMusicTrack(LastTrack);
		}

		return true;
	}

	[Harmonize(
		typeof(GameLocation),
		"damageMonster",
		Harmonize.Harmonize.Fixation.Postfix,
		Harmonize.Harmonize.PriorityLevel.Last,
		instance: true,
		critical: false
	)]
	public static void DamageMonster(
		bool __result,
		Microsoft.Xna.Framework.Rectangle areaOfEffect,
		int minDamage,
		int maxDamage,
		bool isBomb,
		float knockBackModifier,
		int addedPrecision,
		float critChance,
		float critMultiplier,
		bool triggerMonsterInvincibleTimer,
		Farmer who
	) {
		if (!__result || who != Game1.player) {
			return;
		}

		if (!who.currentLocation.getCharacters().AnyF(npc => npc is Monster {IsMonster: true})) {
			return;
		}

		if (Combat is CombatType.None) {
			Combat = CombatType.Regular;
			ChangeMusicTrack(LastTrack);
		}
	}

	private static readonly object PendingCueLock = new();
	private static TrackCue? PendingCue = null;
	private static readonly AutoResetEvent PendingCueEvent = new(false);

	private static void PendingCueLoop() {
		while (true) {
			PendingCueEvent.WaitOne();

			lock (ResetCuesLock) {
				var currentSong = CurrentSong;
				TrackCue? newSong;
				lock (PendingCueLock) {
					newSong = PendingCue;
					CurrentSong = newSong;
				}

				if (newSong is not null) {
					_ = FadeOut.Remove(newSong);

					lock (newSong.Cue) {
						if (newSong.Cue.State == SoundState.Paused) {
							newSong.Cue.Resume();
						}
						else if (newSong.Cue.State == SoundState.Stopped) {
							newSong.Cue.Play();
						}
						else {
							// umm...
						}

						newSong.Cue.Volume = MusicVolume;
					}

					DailyCues.Add(newSong);
				}

				if (currentSong is not null) {
					lock (currentSong.Cue) {
						if (currentSong.Cue.State == SoundState.Playing) {
							if (
								newSong is not null &&
								newSong.Track.Name.Contains("ambient", StringComparison.InvariantCultureIgnoreCase) &&
								currentSong.Track.FadeOut
							) {
								FadeOut.Add(currentSong);
							}
							else {
								if (currentSong.Track.AlwaysStop) {
									currentSong.Cue.Stop();
								}
								else {
									currentSong.Cue.Pause();
								}
							}

							DailyCues.Add(currentSong);
						}
					}
				}

				if (Game1.currentSong is { } currentGameSong) {
					currentGameSong.Stop(AudioStopOptions.Immediate);
					Game1.currentSong = null;
				}
			}
		}
	}

	[Harmonize(
		typeof(StardewValley.Utility),
		"enableMusic",
		Harmonize.Harmonize.Fixation.Finalizer,
		Harmonize.Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static void EnableMusic() {
		UpdateMusicVolume();
	}

	[Harmonize(
		typeof(StardewValley.Utility),
		"enableMusic",
		Harmonize.Harmonize.Fixation.Finalizer,
		Harmonize.Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static void DisableMusic() {
		UpdateMusicVolume();
	}

	private static void UpdateMusicVolume() {
		if (MusicVolume == LastMusicVolume) {
			return;
		}

		foreach (var cue in DailyCues) {
			if (FadeOut.Contains(cue)) {
				continue;
			}

			cue.Cue.Volume = MusicVolume;
		}
	}

	[Harmonize(
		typeof(Game1),
		"updateMusic",
		Harmonize.Harmonize.Fixation.Prefix,
		Harmonize.Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool UpdateMusicImpl() {
		UpdateMusic();
		return false;
	}

	private static void UpdateMusic() {
		var currentSong = CurrentSong;
		if (CurrentTrack is not { } newTrack) {
			return;
		}
		
		if (
			currentSong is not null &&
			currentSong.Track == newTrack
		) {
			return;
		}

		var newSong = newTrack.CurrentCue;

		if (currentSong == newSong || currentSong?.Track.Name == newSong?.Track.Name) {
			return;
		}

		lock (PendingCueLock) {
			if (
				PendingCue is { } pendingCue &&
				pendingCue.Track == newTrack
			) {
				PendingCueEvent.Set();
				return;
			}

			PendingCue = newSong;
		}

		PendingCueEvent.Set();
	}

	private static void ChangeMusicTrack(Track? track) {
		if (track is null) {
			return;
		}

		Debug.Info($"Playing Track '{track.Name}'");

		if (track == CurrentTrack) {
			return;
		}

		LastLastTrack = LastTrack;
		LastTrack = track;

		CurrentTrack = track;

		UpdateMusic();
	}

	[Harmonize(
		typeof(Game1),
		"changeMusicTrack",
		Harmonize.Harmonize.Fixation.Prefix,
		Harmonize.Harmonize.PriorityLevel.Last,
		instance: false,
		critical: false
	)]
	public static bool ChangeMusicTrack(
		ref string? newTrackName,
		bool track_interruptable = false,
		MusicContext music_context = MusicContext.Default
	) {
		if (CurrentTrack == Tracks.DayEnd) {
			return false;
		}

		newTrackName ??= "none";
		if (newTrackName == "none") {
			newTrackName = "ambient";
		}
		else if (
			newTrackName.StartsWith("spring", StringComparison.InvariantCultureIgnoreCase) ||
			newTrackName.StartsWith("summer", StringComparison.InvariantCultureIgnoreCase) ||
			newTrackName.StartsWith("fall", StringComparison.InvariantCultureIgnoreCase) ||
			newTrackName.StartsWith("winter", StringComparison.InvariantCultureIgnoreCase)
		) {
			newTrackName = "ambient";
		}

		if (newTrackName.StartsWith("title")) {
			newTrackName = "title";
		}

		if (Game1.activeClickableMenu is StardewValley.Menus.TitleMenu) {
			newTrackName = "title";
		}

		bool skullCave = (Game1.currentLocation?.Name ?? "").Contains("SkullCave", StringComparison.InvariantCultureIgnoreCase) || (Game1.currentLocation is StardewValley.Locations.MineShaft mineShaft && mineShaft.mineLevel > 120);

		Track? newTrack = null;

		if (Combat is CombatType.Regular) {
			newTrack = Tracks.Battle;
		}
		else if (Combat is CombatType.Ambush) {
			newTrack = Tracks.Ambush;
		}
		else {
			switch (newTrackName.ToLowerInvariant()) {
				case "maintheme":
				case "title":
					newTrack = Tracks.MainTheme;
					break;
				case "earthmine":
					newTrack = Tracks.Mines;
					break;
				case "mm_activity":
					newTrack = Tracks.DayEnd;
					break;
				case "endcredits":
					break;
				default:
				case "ambient": {
					if (skullCave) {
						newTrack = Tracks.SkullCavern;
					}
					else if (Game1.currentLocation is (StardewValley.Locations.Mine or StardewValley.Locations.MineShaft)) {
						newTrack = Tracks.Mines;
					}
					else if (Game1.currentLocation is (StardewValley.Locations.AdventureGuild)) {
						newTrack = Tracks.MarlonsShop;
					}
					else if (Game1.currentLocation is (StardewValley.Locations.ShopLocation
									or StardewValley.Locations.JojaMart)) {
						newTrack = Tracks.Shop;
					}
					else if (Game1.currentLocation is (StardewValley.Locations.WizardHouse) ||
									(Game1.currentLocation?.Name ?? "").Contains("wizard", StringComparison.InvariantCultureIgnoreCase)) {
						newTrack = Tracks.WizardsHouse;
					}
					else {
						newTrack = Tracks.Ambient;
					}
				}

				break;
			}
		}

		ChangeMusicTrack(newTrack);

		return false;
	}
}
