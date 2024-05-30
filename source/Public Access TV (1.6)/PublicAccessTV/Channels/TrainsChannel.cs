/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/PredictiveMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PredictiveCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;

namespace PublicAccessTV
{
	public class HeritageTrain : Train
	{
		public HeritageTrain ()
		{
			NetFields.AddField(cars).AddField(type).AddField(position.NetFields);
			Random random = new ();
			type.Value = Train.uniformColorPlainTrain;
			speed = 0.1f;
			smokeTimer = speed * 2000f;
			cars.Add (new TrainCar (random, 3, -1, Color.MediumPurple));
			for (int i = 1; i < 8; ++i)
			{
				cars.Add (new TrainCar (random, TrainCar.plainCar, 20,
					Color.MediumPurple, 0, 0));
			}
		}
	}

	public class TrainsChannel : Channel
	{
		internal static readonly string EventID = "79400101";
		internal static readonly string EventMap = "Railroad";
		internal static readonly Dictionary<string, string> Events = new ()
		{
			{ "79400101/n kdau.never", "archaeo/39 46/farmer 30 55 0 Demetrius 28 46 1/addBigProp 29 46 106/move farmer 0 -9 3 true/pause 750/animate Demetrius true false 500 24/pause 250/playSound openBox/pause 1000/faceDirection Demetrius 2/jump Demetrius 4/pause 1250/faceDirection Demetrius 1/speak Demetrius \"{{demetrius01}}\"/animate Demetrius true false 1000 24/pause 750/playSound woodyHit/removeObject 29 46/pause 250/move Demetrius 1 0 1/speak Demetrius \"{{demetrius02}}$h#$b#{{demetrius03}}\"/viewport move -4 4 1500/advancedMove Demetrius false 0 4/advancedMove farmer false 0 4/pause 1450/stopAdvancedMoves/pause 50/faceDirection Demetrius 1 true/faceDirection farmer 3/pause 50/speak Demetrius \"{{demetrius04}}$s#$b#{{demetrius05}}\"/emote farmer 16/speak Demetrius \"$q -1 null#{{demetrius06}}#$r -1 30 kdau.PublicAccessTV.trains1#{{farmer01}}#$r -1 0 kdau.PublicAccessTV.trains2#{{farmer02}}#$r -1 -30 kdau.PublicAccessTV.trains3#{{farmer03}}\"/fork 79400101_Reject/pause 500/speak Demetrius \"{{demetrius08}}\"/advancedMove farmer false 0 10/advancedMove Demetrius false 0 10/pause 1500/stopAdvancedMoves/mail kdau.PublicAccessTV.trains%&NL&%/end dialogue Demetrius \"{{demetrius09}}\"" },
			{ "79400101_Reject", "stopMusic/pause 1000/speak Demetrius \"{{demetrius08}}$s\"/advancedMove Demetrius false 0 10/pause 1500/stopAdvancedMoves/end invisible Demetrius" },
		};

		internal static readonly string DialogueCharacter = "Demetrius";
		internal static readonly Dictionary<string, string> Dialogue = new ()
		{
			{ "kdau.PublicAccessTV.trains1", "{{demetrius07a}}$h" },
			{ "kdau.PublicAccessTV.trains2", "{{demetrius07b}}" },
			{ "kdau.PublicAccessTV.trains3", "{{demetrius07c}}%fork$a" },
		};

		public TrainsChannel ()
			: base ("trains")
		{
			Helper.ModContent.Load<Texture2D>
				(Path.Combine ("assets", "trains_backgrounds.png"));
		}

		internal override bool isAvailable =>
			Trains.IsAvailable &&
			(Config.BypassFriendships ||
				Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.trains"));

		internal override void reset ()
		{
			Game1.player.mailReceived.Remove ("kdau.PublicAccessTV.trains");
			Game1.player.mailForTomorrow.Remove ("kdau.PublicAccessTV.trains%&NL&%");
			Game1.player.eventsSeen.Remove (EventID);
		}

		public static void CheckEvent ()
		{
			// Must be during a game day with the underlying module available.
			if (!Context.IsWorldReady || !Trains.IsAvailable ||
					// If bypassing friendships, no need for the event.
					Config.BypassFriendships ||
					// Must not have seen this event yet.
					Game1.player.eventsSeen.Contains (EventID) ||
					// Must have two or more hearts with Demetrius.
					Game1.player.getFriendshipHeartLevelForNPC ("Demetrius") < 2 ||
					// Must be on the Railroad map.
					Game1.currentLocation?.Name != "Railroad" ||
					Game1.currentLocation is not Railroad rr ||
					// Must be before sunset.
					Game1.timeOfDay >= 1900 ||
					// Must not have another event starting.
					Game1.eventUp ||
					// Must not already have a train active or imminent.
					rr.train.Value != null || rr.trainTimer.Value > 0)
				return;

			// Summon a train that the player will just miss.
			Train train = new HeritageTrain ();
			train.position.X = (train.cars.Count - 3.5f) * 128 * 4 + 4480;
			rr.train.Value = train;
			Helper.Events.Display.RenderedWorld += ForceDrawTrain;

			// Run the event script.
			var eventString = Game1.content.LoadString("Data\\Events\\Railroad:79400101/n kdau.never");
            Game1.currentLocation.startEvent(new Event(eventString: eventString, fromAssetName: null, eventID: EventID));
        }

		private static void ForceDrawTrain (object _sender,
			RenderedWorldEventArgs e)
		{
			if (Game1.currentLocation is Railroad rr && rr.train.Value != null)
				rr.train.Value.draw (e.SpriteBatch, Game1.currentLocation);
			else
				Helper.Events.Display.RenderedWorld -= ForceDrawTrain;
		}

		internal override void show (TV tv)
		{
			List<Trains.Prediction> predictions =
				Trains.ListNextTrainsFromDate (SDate.Now (), 3);
			if (predictions.Count < 1)
			{
				throw new Exception ("No trains found.");
			}

			GameLocation location = Game1.getLocationFromName ("Mountain");
			TemporaryAnimatedSprite background = loadBackground (tv, 0,
				(Game1.IsRainingHere (location) || Game1.isDarkOut (location)) ? 1 : 0);
			TemporaryAnimatedSprite portrait = loadPortrait (tv, "Demetrius");

			// Opening scene: Demetrius greets the viewer.
			queueScene (new Scene (Helper.Translation.Get ("trains.opening"),
				background, portrait)
			{ musicTrack = "archaeo" });

			// Next scheduled train. Demetrius's reaction depends on whether the
			// train is today, later in the next 7 days, or later than that.
			string nextMessage;
			TemporaryAnimatedSprite nextPortrait;
			string nextSound = null;
			SDate now = SDate.Now ();
			if (predictions[0].date == now)
			{
				nextMessage = "today";
				nextPortrait = loadPortrait (tv, "Demetrius", 0, 3);
				nextSound = "trainWhistle";
			}
			else if (predictions[0].date.DaysSinceStart < now.DaysSinceStart + 7)
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
				date = predictions[0].date.ToLocaleString (),
				dayOfWeek = Utilities.GetLocalizedDayOfWeek (predictions[0].date),
				time = Game1.getTimeOfDayString (predictions[0].time),
			}),
				background, nextPortrait)
			{ musicTrack = "archaeo", soundCue = nextSound });

			// Second and third scheduled trains.
			if (predictions.Count >= 3)
			{
				queueScene (new Scene (Helper.Translation.Get ("trains.later", new
				{
					date1 = predictions[1].date.ToLocaleString (),
					date2 = predictions[2].date.ToLocaleString (),
				}), background, loadPortrait (tv, "Demetrius", 1, 1))
				{ musicTrack = "archaeo" });
			}

			// Closing scene: Demetrius signs off.
			queueScene (new Scene (Helper.Translation.Get ("trains.closing"),
				background, portrait)
			{ musicTrack = "archaeo" });

			runNextScene (tv);
		}
	}
}
