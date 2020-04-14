using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace PublicAccessTV
{
	public class TrainsChannel : Channel
	{
		internal static readonly string EventMap = "Railroad";
		internal static readonly Dictionary<string,string> Events =
			new Dictionary<string, string>
		{
			{ "79400101/f Demetrius 500/l kdau.PublicAccessTV.trains/t 600 1900", "archaeo/35 46/farmer 30 55 0 Demetrius 28 46 1/addBigProp 29 46 106/move farmer 0 -9 3 true/pause 750/animate Demetrius true false 500 24/pause 250/playSound openBox/pause 1000/faceDirection Demetrius 2/jump Demetrius 4/pause 1250/faceDirection Demetrius 1/speak Demetrius \"{{demetrius01}}\"/animate Demetrius true false 1000 24/pause 750/playSound woodyHit/removeObject 29 46/pause 250/move Demetrius 1 0 1/speak Demetrius \"{{demetrius02}}$h#$b#{{demetrius03}}$n\"/viewport move 0 4 1500/move Demetrius 0 3 2 true/move farmer 0 1 3/move Demetrius 0 1 1/speak Demetrius \"{{demetrius04}}$s#$b#{{demetrius05}}\"/emote farmer 16/speak Demetrius \"$q -1 null#{{demetrius06}}#$r -1 30 kdau.PublicAccessTV.trains1#{{farmer01}}#$r -1 0 kdau.PublicAccessTV.trains2#{{farmer02}}#$r -1 -30 kdau.PublicAccessTV.trains3#{{farmer03}}\"/fork 79400101_Reject/pause 500/speak Demetrius \"{{demetrius08}}\"/move farmer 0 10 2 true/move Demetrius 0 10 2 true/viewport move 0 5 1500/fade/viewport -1000 -1000/mail kdau.PublicAccessTV.trains%&NL&%/end dialogue Demetrius \"{{demetrius09}}\"" },
			{ "79400101_Reject", "pause 1000/speak Demetrius \"{{demetrius08}}$s\"/move Demetrius 0 10 2 true/viewport move 0 5 1500/fade/viewport -1000 -1000/end invisible Demetrius" },
		};

		internal static readonly string DialogueCharacter = "Demetrius";
		internal static readonly Dictionary<string, string> Dialogue =
			new Dictionary<string, string>
		{
			{ "kdau.PublicAccessTV.trains1", "{{demetrius07a}}$h" },
			{ "kdau.PublicAccessTV.trains2", "{{demetrius07b}}" },
			{ "kdau.PublicAccessTV.trains3", "{{demetrius07c}}%fork$a" },
		};

		public TrainsChannel ()
			: base ("trains")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "trains_backgrounds.png"));
		}

		internal override bool isAvailable =>
			base.isAvailable && Trains.IsAvailable &&
			(Config.BypassFriendships ||
				Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.trains"));

		internal override void reset ()
		{
			Game1.player.mailReceived.Remove ("kdau.PublicAccessTV.trains");
			Game1.player.mailForTomorrow.Remove ("kdau.PublicAccessTV.trains%&NL&%");
			Game1.player.eventsSeen.Remove (79400101);
		}

		internal override void show (TV tv)
		{
			List<TrainPrediction> predictions =
				Trains.ListNextTrainsForDate (Utilities.Now (), 3);
			if (predictions.Count < 1)
			{
				throw new Exception ("No trains found.");
			}

			TemporaryAnimatedSprite background = loadBackground (tv, 0,
				(Game1.isRaining || Game1.isDarkOut ()) ? 1 : 0);
			TemporaryAnimatedSprite portrait = loadPortrait (tv, "Demetrius");

			bool sve = Helper.ModRegistry.IsLoaded ("FlashShifter.SVEMusic");
			string musicTrack = sve ? "distantBanjo" : "archaeo";

			// Opening scene: Demetrius greets the viewer.
			queueScene (new Scene (Helper.Translation.Get ("trains.opening"),
				background, portrait) { musicTrack = musicTrack });

			// Next scheduled train. Demetrius's reaction depends on whether the
			// train is today, later in the next 7 days, or later than that.
			string nextMessage;
			TemporaryAnimatedSprite nextPortrait;
			string nextSound = null;
			WorldDate now = Utilities.Now ();
			if (predictions[0].date == now)
			{
				nextMessage = "today";
				nextPortrait = loadPortrait (tv, "Demetrius", 0, 3);
				nextSound = "trainWhistle";
			}
			else if (predictions[0].date.TotalDays < now.TotalDays + 7)
			{
				nextMessage = "thisWeek";
				nextPortrait = loadPortrait (tv, "Demetrius", 1, 0);
				nextSound = "distantTrain";
			}
			else
			{
				nextMessage = "later";
				nextPortrait = loadPortrait (tv, "Demetrius", 0, 1);
			}
			queueScene (new Scene (Helper.Translation.Get ($"trains.next.{nextMessage}", new
				{
					date = predictions[0].date.Localize (),
					dayOfWeek = Utilities.GetLocalizedDayOfWeek (predictions[0].date),
					time = Game1.getTimeOfDayString (predictions[0].time),
				}),
				background, nextPortrait)
				{ musicTrack = musicTrack, soundCue = nextSound });

			// Second and third scheduled trains.
			if (predictions.Count >= 3)
			{
				queueScene (new Scene (Helper.Translation.Get ("trains.later", new
				{
					date1 = predictions[1].date.Localize (),
					date2 = predictions[2].date.Localize (),
				}), background, loadPortrait (tv, "Demetrius", 1, 1))
				{ musicTrack = musicTrack });
			}

			// Closing scene: Demetrius signs off.
			queueScene (new Scene (Helper.Translation.Get ("trains.closing"),
				background, portrait) { musicTrack = musicTrack });

			runProgram (tv);
		}
	}
}
