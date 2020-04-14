using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PredictiveCore;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.IO;
using xTile.Dimensions;
using SObject = StardewValley.Object;

namespace PublicAccessTV
{
	public class GarbageChannel : Channel
	{
		internal static readonly string EventMap = "Town";
		internal static readonly Dictionary<string,string> Events =
			new Dictionary<string, string>
		{
			{ "79400102/n kdau.never", "echos/<<viewport>>/farmer <<farmerstart>> Linus <<linusstart>>/move Linus <<linusmove1>> true/playSound dirtyHit/pause 1000/playSound dirtyHit/pause 1000/textAboveHead Linus \"{{linus01}}\"/pause 250/jump farmer/pause 750/faceDirection farmer <<farmerface>>/pause 250/emote farmer 16/move Linus <<linusmove2>>/pause 500/speak Linus \"{{linus02}}#$b#{{linus03}}$h\"/emote farmer 32/speak Linus \"{{linus04}}#$b#{{linus05}}\"/emote farmer 40/speak Linus \"$q -1 null#{{linus06}}#$r -1 50 kdau.PublicAccessTV.garbage1#{{farmer01}}#$r -1 0 kdau.PublicAccessTV.garbage2#{{farmer02}}#$r -1 -50 kdau.PublicAccessTV.garbage3#{{farmer03}}\"/pause 500/speak Linus \"{{linus08}}\"/move Linus <<linusmove3>> 2 true/viewport move <<linusmove3>> 6000/fade/viewport -1000 -1000/fork 79400102_Reject/mail kdau.PublicAccessTV.garbage%&NL&%/end dialogue Linus \"{{linus09}}\"" },
			{ "79400102_Reject", "end invisible Linus" },
		};
		internal static readonly Dictionary<GarbageCan, string> EventPositions =
			new Dictionary<GarbageCan, string>
		{
			// "linusstart(X Y F)/linusmove1(X Y F)/(linusmove2(X Y F)/farmerface(F)/linusmove3(X Y)"
			{ GarbageCan.SamHouse, "6 87 1/3 0 1/2 0 1/3/0 6" },
			{ GarbageCan.HaleyHouse, "24 93 0/0 -3 3/-4 0 3/1/0 6" },
			{ GarbageCan.ManorHouse, "51 83 1/3 0 2/0 2 1/3/0 6" },
			{ GarbageCan.ArchaeologyHouse, "102 95 0/0 -3 1/5 0 1/3/-6 0" },
			{ GarbageCan.Blacksmith, "86 83 1/9 0 0/0 -1 1/3/0 6" },
			{ GarbageCan.Saloon, "44 65 1/5 0 2/0 6 3/1/0 6" },
			{ GarbageCan.JoshHouse, "52 58 3/-2 0 2/0 6 1/3/-6 0" },
			{ GarbageCan.JojaMart, "103 61 0/0 -4 1/6 0 1/3/0 6" },
			// for this class, MovieTheater collapses into JojaMart
			// alternate event positions for SVE
			{ GarbageCan.SVE_SamHouse, "14 87 3/-5 0 2/0 3 3/1/6 0" },
			{ GarbageCan.SVE_HaleyHouse, "33 85 3/-3 0 3/-2 0 3/1/0 6" },
			{ GarbageCan.SVE_AdventureGuild, "26 93 1/2 0 2/0 3 2/0/0 -6" },
			{ GarbageCan.SVE_JoshHouse, "44 65 1/6 0 0/0 -1 1/3/0 6" },
			{ GarbageCan.SVE_Saloon, "44 65 1/5 0 2/0 6 3/1/0 6" },
			{ GarbageCan.SVE_JenkinsHouse, "65 59 0/0 -4 0/0 -2 1/3/0 6" },
			{ GarbageCan.SVE_ManorHouse, "52 90 0/0 -4 1/0 0 1/3/0 4" },
		};

		internal static readonly string DialogueCharacter = "Linus";
		internal static readonly Dictionary<string,string> Dialogue =
			new Dictionary<string, string>
		{
			{ "kdau.PublicAccessTV.garbage1", "{{linus07a}}$h" },
			{ "kdau.PublicAccessTV.garbage2", "{{linus07b}}" },
			{ "kdau.PublicAccessTV.garbage3", "{{linus07c}}%fork$s" },
		};

		private static bool[] garbageChecked;

		public GarbageChannel ()
			: base ("garbage")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "garbage_backgrounds.png"));
		}

		internal override bool isAvailable =>
			base.isAvailable && Garbage.IsAvailable &&
			(Config.BypassFriendships ||
				Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.garbage"));

		internal override void update ()
		{
			garbageChecked = GetCurrentCansChecked ();
			base.update ();
		}

		internal override void reset ()
		{
			Game1.player.mailReceived.Remove ("kdau.PublicAccessTV.garbage");
			Game1.player.mailForTomorrow.Remove ("kdau.PublicAccessTV.garbage%&NL&%");
			Game1.player.eventsSeen.Remove (79400102);
			garbageChecked = GetCurrentCansChecked ();
		}

		private static bool[] GetCurrentCansChecked ()
		{
			GameLocation town = Game1.getLocationFromName ("Town");
			var value = Helper.Reflection.GetField<NetArray<bool, NetBool>>
				(town, "garbageChecked").GetValue ();
			bool[] result = new bool[8];
			for (int i = 0; i < 8; ++i)
				result[i] = value[i];
			return result;
		}

		public static void CheckEvent ()
		{
			// Must be during a game day.
			if (!Context.IsWorldReady || garbageChecked == null ||
					// If bypassing friendships, no need for the event.
					Config.BypassFriendships ||
					// Must be on the Town map.
					Game1.currentLocation?.Name != "Town" ||
					// Must not have seen this event yet.
					Game1.player.eventsSeen.Contains (79400102))
				return;

			// Find whether any can has been checked since the last run.
			bool[] current = GetCurrentCansChecked ();
			GarbageCan? can = null;
			for (int i = 0; i < 8; ++i)
			{
				if (!garbageChecked[i] && current[i])
				{
					garbageChecked[i] = true;
					bool sve = Helper.ModRegistry.IsLoaded
						("FlashShifter.StardewValleyExpandedALL");
					can = (GarbageCan) i + (sve ? 100 : 0);
					// Don't break, in the unlikely event that multiple cans
					// were checked since last run and need to be updated.
				}
			}

			// Must have just checked a can.
			if (!can.HasValue ||
					// Underlying module must be available.
					!Garbage.IsAvailable ||
					// Must have four or more hearts with Linus.
					Game1.player.getFriendshipHeartLevelForNPC ("Linus") < 4 ||
					// Must have seen the vanilla event with Linus in town.
					!Game1.player.eventsSeen.Contains (502969))
				return;

			// Stop further runs of this method immediately.
			garbageChecked = null;

			// Build event script based on the can that was checked.
			int viewportX = Game1.viewportCenter.X / Game1.tileSize;
			int viewportY = Game1.viewportCenter.Y / Game1.tileSize;
			Location canLoc = Garbage.CanLocations[can.Value];
			string[] canPos = EventPositions[can.Value].Split ('/');
			bool canObstructed = can == GarbageCan.ManorHouse ||
				can == GarbageCan.SVE_ManorHouse ||
				can == GarbageCan.SVE_AdventureGuild;
			string eventScript = Events["79400102/n kdau.never"]
				.Replace ("<<viewport>>", $"{viewportX} {viewportY}")
				.Replace ("<<farmerstart>>", canObstructed
					? $"{canLoc.X - 1} {canLoc.Y} 1" : $"{canLoc.X} {canLoc.Y + 1} 0")
				.Replace ("<<linusstart>>", canPos[0])
				.Replace ("<<linusmove1>>", canPos[1])
				.Replace ("<<linusmove2>>", canPos[2])
				.Replace ("<<farmerface>>", canPos[3])
				.Replace ("<<linusmove3>>", canPos[4])
			;

			// Run the event, after a delay to allow the can action to finish.
			DelayedAction.functionAfterDelay (() =>
				Game1.currentLocation.startEvent (new Event (eventScript, 79400102)),
				500);
		}

		private static readonly Dictionary<GarbageCan, int> CanScenes =
			new Dictionary<GarbageCan, int>
		{
			{ GarbageCan.SamHouse, 1 },
			{ GarbageCan.HaleyHouse, 2 },
			{ GarbageCan.ManorHouse, 3 },
			{ GarbageCan.ArchaeologyHouse, 4 },
			{ GarbageCan.Blacksmith, 5 },
			{ GarbageCan.Saloon, 6 },
			{ GarbageCan.JoshHouse, 7 },
			{ GarbageCan.JojaMart, 8 },
			{ GarbageCan.MovieTheater, 9 },
			// alternate can scenes for SVE (reuse vanilla or just show tent)
			{ GarbageCan.SVE_SamHouse, 1 },
			{ GarbageCan.SVE_HaleyHouse, 2 },
			{ GarbageCan.SVE_AdventureGuild, 0 },
			{ GarbageCan.SVE_JoshHouse, 7 },
			{ GarbageCan.SVE_Saloon, 6 },
			{ GarbageCan.SVE_JenkinsHouse, 0 },
			{ GarbageCan.SVE_ManorHouse, 3 },
		};

		internal override void show (TV tv)
		{
			WorldDate today = Utilities.Now ();
			List<GarbagePrediction> predictions = Garbage.ListLootForDate (today);

			int seasonIndex = today.SeasonIndex;
			TemporaryAnimatedSprite background = loadBackground (tv, 0, seasonIndex);
			TemporaryAnimatedSprite portrait = loadPortrait (tv, "Linus");

			// Opening scene: Linus greets the viewer.
			queueScene (new Scene (Helper.Translation.Get ("garbage.opening", new
				{
					playerName = Game1.player.Name,
				}), background, portrait) { musicTrack = "echos" });

			// Linus sadly notes that the cans are empty today.
			if (predictions.Count < 1)
			{
				queueScene (new Scene (Helper.Translation.Get ("garbage.none"),
					background, loadPortrait (tv, "Linus", 0, 1))
					{ musicTrack = "echos" });
			}

			// Linus reports on the content of each non-empty can.
			foreach (GarbagePrediction prediction in predictions)
			{
				string can = prediction.can.ToString ().Replace ("SVE_", "");

				string type;
				TemporaryAnimatedSprite reactionPortrait;
				if (prediction.loot is Hat)
				{
					type = "garbageHat";
					reactionPortrait = loadPortrait (tv, "Linus", 1, 1);
				}
				else if (prediction.loot.ParentSheetIndex == 217) // placeholder
				{
					type = "dishOfTheDay";
					reactionPortrait = loadPortrait (tv, "Linus", 1, 0);
				}
				else if (prediction.special)
				{
					type = "special";
					reactionPortrait = loadPortrait (tv, "Linus", 1, 0);
				}
				else
				{
					type = "generic";
					reactionPortrait = portrait;
				}

				queueScene (new Scene
					(Helper.Translation.Get ($"garbage.can.{can}") + "^...^" +
						Helper.Translation.Get ($"garbage.item.{type}", new
						{
							itemName = prediction.loot.DisplayName,
						}),
					loadBackground (tv, CanScenes[prediction.can], seasonIndex),
					reactionPortrait)
					{ musicTrack = "echos" });
			}

			// Closing scene: Linus signs off.
			bool progress = Garbage.IsProgressDependent;
			queueScene (new Scene
				(Helper.Translation.Get ($"garbage.closing.{(progress? "progress" : "standard")}"),
				background, portrait) { musicTrack = "echos" });

			runProgram (tv);
		}
	}
}
