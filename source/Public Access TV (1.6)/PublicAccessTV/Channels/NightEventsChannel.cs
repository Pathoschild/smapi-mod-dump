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
using StardewModdingAPI.Utilities;
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
			Helper.ModContent.Load<Texture2D>
				(Path.Combine ("assets", "nightEvents_backgrounds.png"));
		}

		internal override bool isAvailable =>
			NightEvents.IsAvailable &&
			getCurrentEvent () != NightEvents.Event.None;

		internal override void show (TV tv)
		{
			NightEvents.Event currentEvent = getCurrentEvent ();
			if (currentEvent == NightEvents.Event.None)
			{
				throw new Exception ("No night event found.");
			}

			TemporaryAnimatedSprite background = loadBackground (tv, 0);
			TemporaryAnimatedSprite portrait = loadPortrait (tv, "Governor", 1, 0);
			bool newYear = currentEvent == NightEvents.Event.NewYear;

			// Opening scene: the governor greets the viewer.
			queueScene (new Scene
				(Helper.Translation.Get ($"nightEvents.{currentEvent}.opening"),
				background, portrait)
			{
                // workaround, see comments from Scene.cs playSoundWithSoundPlayer()
                soundCue = "Cowboy_Secret" // ,
				// soundAsset = newYear
				// 	? "nightEvents_newYear" : "nightEvents_opening"
			});

			// The governor reacts to the event.
			TemporaryAnimatedSprite reactionBackground = background;
			string reactionSound = null;
			Point reactionIndex = new (0, newYear ? 0 : 1);
			if (currentEvent == NightEvents.Event.StrangeCapsule)
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

			runNextScene (tv);
		}

		private NightEvents.Event getCurrentEvent ()
		{
			SDate tonight = SDate.Now ();

			List<NightEvents.Prediction> predictions =
				NightEvents.ListNextEventsFromDate (tonight, 1);
			if (predictions.Count >= 1 && predictions[0].date == tonight)
			{
				switch (predictions[0].@event)
				{
				case NightEvents.Event.Meteorite:
				case NightEvents.Event.StrangeCapsule:
					return predictions[0].@event;
				}
			}

			if (tonight.Season == Season.Winter && tonight.Day == 28)
				return NightEvents.Event.NewYear;

			return NightEvents.Event.None;
		}
	}
}
