/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.IO;
using xTile.Dimensions;

namespace PublicAccessTV
{
	public class GarbageChannel : Channel
	{
		internal static readonly string EventID = "79400102";
		internal static readonly string EventMap = "Town";
		internal static readonly Dictionary<string, string> Events = new ()
		{
			{ "79400102/n kdau.never", "echos/<<viewport>>/farmer <<farmerstart>> Linus <<linusstart>>/move Linus <<linusmove1>> true/playSound dirtyHit/pause 1000/playSound dirtyHit/pause 1000/textAboveHead Linus \"{{linus01}}\"/pause 250/jump farmer/pause 750/faceDirection farmer <<farmerface>>/pause 250/emote farmer 16/move Linus <<linusmove2>>/pause 500/speak Linus \"{{linus02}}#$b#{{linus03}}$h\"/emote farmer 32/speak Linus \"{{linus04}}#$b#{{linus05}}\"/emote farmer 40/speak Linus \"$q -1 null#{{linus06}}#$r -1 50 kdau.PublicAccessTV.garbage1#{{farmer01}}#$r -1 0 kdau.PublicAccessTV.garbage2#{{farmer02}}#$r -1 -50 kdau.PublicAccessTV.garbage3#{{farmer03}}\"/pause 500/speak Linus \"{{linus08}}\"/move Linus <<linusmove3>> 2 true/viewport move <<linusmove3>> 6000/fade/viewport -1000 -1000/fork 79400102_Reject/mail kdau.PublicAccessTV.garbage%&NL&%/end dialogue Linus \"{{linus09}}\"" },
			{ "79400102_Reject", "end invisible Linus" },
		};
		internal static readonly Dictionary<Garbage.Can, string> EventPositions = new ()
		{
			// "linusstart(X Y F)/linusmove1(X Y F)/linusmove2(X Y F)/farmerface(F)/linusmove3(X Y)"
			{ Garbage.Can.SamHouse, "6 87 1/3 0 1/2 0 1/3/0 6" },
			{ Garbage.Can.HaleyHouse, "24 93 0/0 -3 3/-4 0 3/1/0 6" },
			{ Garbage.Can.ManorHouse, "51 83 1/3 0 2/0 2 1/3/0 6" },
			{ Garbage.Can.ArchaeologyHouse, "102 95 0/0 -3 1/5 0 1/3/-6 0" },
			{ Garbage.Can.Blacksmith, "86 83 1/9 0 0/0 -1 1/3/0 6" },
			{ Garbage.Can.Saloon, "44 65 1/5 0 2/0 6 3/1/0 6" },
			{ Garbage.Can.JoshHouse, "52 58 3/-2 0 2/0 6 1/3/-6 0" },
			{ Garbage.Can.JojaMart, "103 61 0/0 -4 1/6 0 1/3/0 6" },
			// for this class, MovieTheater collapses into JojaMart
			// alternate event positions for SVE
			{ Garbage.Can.SVE_SamHouse, "14 87 3/-5 0 2/0 3 3/1/6 0" },
			{ Garbage.Can.SVE_HaleyHouse, "33 85 3/-3 0 3/-2 0 3/1/0 6" },
			{ Garbage.Can.SVE_ArchaeologyHouse, "97 83 3/-3 0 2/0 9 1/3/-6 0" },
			{ Garbage.Can.SVE_JoshHouse, "44 65 1/6 0 0/0 -1 1/3/0 6" },
			{ Garbage.Can.SVE_Saloon, "44 65 1/5 0 2/0 6 3/1/0 6" },
			{ Garbage.Can.SVE_JenkinsHouse, "65 59 0/0 -4 0/0 -2 1/3/0 6" },
			{ Garbage.Can.SVE_ManorHouse, "52 90 0/0 -4 1/0 0 1/3/0 4" },
			{ Garbage.Can.SVE_Blacksmith, "109 83 3/-3 0 3/-7 0 3/1/6 0" },
		};

		internal static readonly string DialogueCharacter = "Linus";
		internal static readonly Dictionary<string, string> Dialogue = new ()
		{
			{ "kdau.PublicAccessTV.garbage1", "{{linus07a}}$h" },
			{ "kdau.PublicAccessTV.garbage2", "{{linus07b}}" },
			{ "kdau.PublicAccessTV.garbage3", "{{linus07c}}%fork$s" },
		};

		private static readonly PerScreen<bool[]> GarbageChecked = new ();

		public GarbageChannel ()
			: base ("garbage")
		{
			Helper.ModContent.Load<Texture2D>
				(Path.Combine ("assets", "garbage_backgrounds.png"));
		}

		internal override bool isAvailable =>
			Garbage.IsAvailable &&
			(Config.BypassFriendships ||
				Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.garbage"));

		internal override void update ()
		{
			GarbageChecked.Value = GetCurrentCansChecked ();
			base.update ();
		}

		internal override void reset ()
		{
			Game1.player.mailReceived.Remove ("kdau.PublicAccessTV.garbage");
			Game1.player.mailForTomorrow.Remove ("kdau.PublicAccessTV.garbage%&NL&%");
			Game1.player.eventsSeen.Remove (EventID);
			GarbageChecked.Value = GetCurrentCansChecked ();
		}

		private static bool[] GetCurrentCansChecked ()
		{
			string[] cans = {
				"JodiAndKent",
				"EmilyAndHaley",
				"Mayor",
				"Museum",
				"Blacksmith",
				"Saloon",
				"Evelyn",
				"JojaMart"
			};
            bool[] result = new bool[8];
            for (int i = 0; i < 8; ++i)
                result[i] = Game1.netWorldState.Value.CheckedGarbage.Contains(cans[i]);
			return result;
		}

		public static void CheckEvent ()
		{
			// Must be during a game day.
			if (!Context.IsWorldReady || GarbageChecked.Value == null ||
					// If bypassing friendships, no need for the event.
					Config.BypassFriendships ||
					// Must be on the Town map.
					Game1.currentLocation?.Name != "Town" ||
					// Must not have seen this event yet.
					Game1.player.eventsSeen.Contains (EventID))
				return;

			// Find whether any can has been checked since the last run.
			bool[] current = GetCurrentCansChecked ();
			Garbage.Can? can = null;
			for (int i = 0; i < 8; ++i)
			{
				if (!GarbageChecked.Value[i] && current[i])
				{
					GarbageChecked.Value[i] = true;
					bool sve = Helper.ModRegistry.IsLoaded
						("FlashShifter.StardewValleyExpandedCP");
					can = (Garbage.Can) i + (sve ? 100 : 0);
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
					!Game1.player.eventsSeen.Contains ("502969"))
				return;

			// Stop further runs of this method immediately.
			GarbageChecked.Value = null;

			// Build event script based on the can that was checked.
			int viewportX = Game1.viewportCenter.X / Game1.tileSize;
			int viewportY = Game1.viewportCenter.Y / Game1.tileSize;
			Location canLoc = Garbage.CanLocations[can.Value];
			string[] canPos = EventPositions[can.Value].Split ('/');
			bool canObstructed = can == Garbage.Can.ManorHouse ||
				can == Garbage.Can.SVE_ManorHouse;
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
				Game1.currentLocation.startEvent (new Event (eventString: eventScript, fromAssetName: null, eventID: EventID)),
				500);
		}

		private static readonly Dictionary<Garbage.Can, int> CanScenes = new ()
		{
			{ Garbage.Can.SamHouse, 1 },
			{ Garbage.Can.HaleyHouse, 2 },
			{ Garbage.Can.ManorHouse, 3 },
			{ Garbage.Can.ArchaeologyHouse, 4 },
			{ Garbage.Can.Blacksmith, 5 },
			{ Garbage.Can.Saloon, 6 },
			{ Garbage.Can.JoshHouse, 7 },
			{ Garbage.Can.JojaMart, 8 },
			{ Garbage.Can.MovieTheater, 9 },
			// alternate can scenes for SVE (reuse vanilla or just show tent)
			{ Garbage.Can.SVE_SamHouse, 1 },
			{ Garbage.Can.SVE_HaleyHouse, 2 },
			{ Garbage.Can.SVE_ArchaeologyHouse, 4 },
			{ Garbage.Can.SVE_JoshHouse, 7 },
			{ Garbage.Can.SVE_Saloon, 6 },
			{ Garbage.Can.SVE_JenkinsHouse, 0 },
			{ Garbage.Can.SVE_ManorHouse, 3 },
			{ Garbage.Can.SVE_Blacksmith, 5 },
		};

		internal override void show (TV tv)
		{
			SDate today = SDate.Now ();
			List<Garbage.Prediction> predictions = Garbage.ListLootForDate (today);

			int seasonIndex = today.SeasonIndex;
			TemporaryAnimatedSprite background = loadBackground (tv, 0, seasonIndex);
			TemporaryAnimatedSprite portrait = loadPortrait (tv, "Linus");

			// Opening scene: Linus greets the viewer.
			queueScene (new Scene (Helper.Translation.Get ("garbage.opening", new { playerName = Game1.player.Name }), background, portrait)
			{ musicTrack = "echos" });

			// Linus sadly notes that the cans are empty today.
			if (predictions.Count < 1)
			{
				queueScene (new Scene (Helper.Translation.Get ("garbage.none"),
					background, loadPortrait (tv, "Linus", 0, 1))
				{ musicTrack = "echos" });
			}

			// Linus reports on the content of each non-empty can.
			foreach (Garbage.Prediction prediction in predictions)
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
				(Helper.Translation.Get ($"garbage.closing.{(progress ? "progress" : "standard")}"),
				background, portrait)
			{ musicTrack = "echos" });

			runNextScene (tv);
		}
	}
}
