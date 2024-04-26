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
using PredictiveCore;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PublicAccessTV
{
	public class MiningChannel : Channel
	{
		public static readonly List<Mining.FloorType> GilTypes = new ()
		{
			Mining.FloorType.Mushroom,
			Mining.FloorType.Treasure,
			Mining.FloorType.PepperRex,
		};

		public MiningChannel ()
			: base ("mining")
		{
			Helper.ModContent.Load<Texture2D>
				(Path.Combine ("assets", "mining_backgrounds.png"));
		}

		internal override bool isAvailable =>
			Mining.IsAvailable &&
			(Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.mining") ||
				Game1.player.mailbox.Contains ("kdau.PublicAccessTV.mining"));

		internal override void update ()
		{
			if (Mining.IsAvailable &&
				!Game1.player.mailReceived.Contains ("kdau.PublicAccessTV.mining") &&
				!Game1.player.mailbox.Contains ("kdau.PublicAccessTV.mining") &&
				Game1.player.mailReceived.Contains ("guildMember") &&
				(Config.BypassFriendships ||
					!Helper.ModRegistry.IsLoaded ("FlashShifter.StardewValleyExpandedCP") ||
					Game1.player.getFriendshipHeartLevelForNPC ("MarlonFay") >= 2))
			{
				Game1.player.mailbox.Add ("kdau.PublicAccessTV.mining");
			}

			base.update ();
		}

		internal override void reset ()
		{
			Game1.player.mailReceived.Remove ("kdau.PublicAccessTV.mining");
			Game1.player.mailbox.Remove ("kdau.PublicAccessTV.mining");
		}

		internal override void show (TV tv)
		{
			SDate today = SDate.Now ();
			List<Mining.Prediction> predictions = Mining.ListFloorsForDate (today);

			TemporaryAnimatedSprite background = loadBackground (tv, 0);
			TemporaryAnimatedSprite marlon = loadPortrait (tv, "Marlon");
			TemporaryAnimatedSprite gil = loadPortrait (tv, "Gil");

			// Opening scene: Marlon greets the viewer.
			queueScene (new Scene (Helper.Translation.Get ((predictions.Count == 0)
				? "mining.opening.none" : "mining.opening"),
				background, marlon)
			{ musicTrack = "MarlonsTheme" });

			// Marlon or Gil reports on each type of special floor.
			string joiner = CultureInfo.CurrentCulture.TextInfo.ListSeparator + " ";
			foreach (var typeGroup in predictions.GroupBy ((p) => p.type))
			{
				string floorsText;
				if (typeGroup.Key == Mining.FloorType.Treasure &&
					typeGroup.First ().item != null)
				{
					floorsText = Helper.Translation.Get ("mining.floorAndItem", new
					{
						num = typeGroup.First ().floor,
						itemName = typeGroup.First ().item.DisplayName,
					});
				}
				else
				{
					List<int> floors = typeGroup
						.Select ((p) => p.floor)
						.ToList ();
					if (floors.Count == 1)
					{
						floorsText = Helper.Translation.Get ("mining.floor",
							new { num = floors[0] });
					}
					else
					{
						int lastNum = floors[floors.Count - 1];
						floors.RemoveAt (floors.Count - 1);
						floorsText = Helper.Translation.Get ("mining.floors",
							new { nums = string.Join (joiner, floors), lastNum });
					}
				}

				queueScene (new Scene (Helper.Translation.Get ($"mining.prediction.{typeGroup.Key}",
						new { floors = floorsText, }),
					loadBackground (tv, (int) typeGroup.Key + 1),
					GilTypes.Contains (typeGroup.Key) ? gil : marlon)
				{ musicTrack = "MarlonsTheme" });
			}

			// Closing scene: Marlon signs off.
			bool progress = Mining.IsProgressDependent;
			queueScene (new Scene
				(Helper.Translation.Get ($"mining.closing.{(progress ? "progress" : "standard")}"),
				background, marlon)
			{ musicTrack = "MarlonsTheme" });

			runNextScene (tv);
		}
	}
}
