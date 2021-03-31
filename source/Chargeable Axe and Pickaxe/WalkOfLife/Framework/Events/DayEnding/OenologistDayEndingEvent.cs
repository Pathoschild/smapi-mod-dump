/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.IO;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class OenologistDayEndingEvent : DayEndingEvent
	{
		/// <summary>Construct an instance.</summary>
		internal OenologistDayEndingEvent() { }

		/// <summary>Raised before the game ends the current day. Receive Oenologist mail from the SWA about Decanter's award.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			foreach (var item in Game1.getFarm().getShippingBin(Game1.player))
			{
				if (Utility.IsWine(item))
				{
					switch ((item as SObject).Quality)
					{
						case SObject.bestQuality:
							Data.OenologyFameAccrued += 3;
							break;
						case SObject.highQuality:
							Data.OenologyFameAccrued += 1;
							break;
						case SObject.medQuality:
							Data.OenologyFameAccrued += 0;
							break;
						case SObject.lowQuality:
							Data.OenologyFameAccrued = Math.Max(Data.OenologyFameAccrued - 1, 0);
							break;
					}
				}
			}

			if (Game1.dayOfMonth == 28 && Data.OenologyFameAccrued > 0)
			{
				uint awardLevel = Utility.GetLocalPlayerOenologyAwardLevel();
				if (awardLevel > Data.HighestOenologyAwardEarned)
				{
					Data.HighestOenologyAwardEarned = awardLevel;
					AwesomeProfessions.ModHelper.Content.InvalidateCache(Path.Combine("Data", "mail"));
					Game1.addMailForTomorrow("OenologistAwardNotice");
				}
			}
		}
	}
}
