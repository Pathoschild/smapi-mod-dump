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
	public class NightEventsChannel : Channel
	{
		public NightEventsChannel ()
			: base ("nightEvents")
		{
			Helper.Content.Load<Texture2D>
				(Path.Combine ("assets", "nightEvents_backgrounds.png"));
		}

		internal override bool isAvailable =>
			base.isAvailable && NightEvents.IsAvailable &&
			getCurrentEvent () != NightEventType.None;

		internal override void show (TV tv)
		{
			NightEventType currentEvent = getCurrentEvent ();
			if (currentEvent == NightEventType.None)
			{
				throw new Exception ("No night event found.");
			}

			TemporaryAnimatedSprite background = loadBackground (tv, 0);
			TemporaryAnimatedSprite portrait = loadPortrait (tv, "Governor", 1, 0);
			bool newYear = currentEvent == NightEventType.NewYear;

			// Opening scene: the governor greets the viewer.
			queueScene (new Scene
				(Helper.Translation.Get ($"nightEvents.{currentEvent}.opening"),
				background, portrait)
				{ soundCue = "Cowboy_Secret", soundAsset = newYear
					? "nightEvents_newYear" : "nightEvents_opening" });

			// The governor reacts to the event.
			TemporaryAnimatedSprite reactionBackground = background;
			string reactionSound = null;
			Point reactionIndex = new Point (0, newYear ? 0 : 1);
			if (currentEvent == NightEventType.StrangeCapsule)
			{
				reactionBackground = loadBackground (tv, 0, 1);
				reactionSound = "UFO";
				reactionIndex = new Point (1, 1);
			}
			queueScene (new Scene (Helper.Translation.Get ($"nightEvents.{currentEvent}.reaction"),
				reactionBackground, loadPortrait (tv, "Governor", reactionIndex))
				{ soundCue = reactionSound });

			// Closing scene: the governor signs off.
			queueScene (new Scene (Helper.Translation.Get ($"nightEvents.{currentEvent}.closing"),
				background, portrait));

			runProgram (tv);
		}

		private NightEventType getCurrentEvent ()
		{
			WorldDate tonight = Utilities.Now ();

			List<NightEventPrediction> predictions =
				NightEvents.ListNextEventsForDate (tonight, 1);
			if (predictions.Count >= 1 && predictions[0].date == tonight)
			{
				switch (predictions[0].type)
				{
				case NightEventType.Meteorite:
				case NightEventType.StrangeCapsule:
					return predictions[0].type;
				}
			}

			if (tonight.Season == "winter" && tonight.DayOfMonth == 28)
			{
				return NightEventType.NewYear;
			}

			return NightEventType.None;
		}
	}
}
