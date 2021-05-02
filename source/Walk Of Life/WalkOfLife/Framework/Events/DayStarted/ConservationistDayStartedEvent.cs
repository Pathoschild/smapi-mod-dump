/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods/-/tree/master/WalkOfLife
**
*************************************************/

using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using TheLion.Common;
using SUtility = StardewValley.Utility;

namespace TheLion.AwesomeProfessions
{
	internal class ConservationistDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			foreach (var location in Game1.locations)
			{
				foreach (var obj in location.Objects.Values)
				{
					if (obj is CrabPot crabpot && Game1.getFarmer(obj.owner.Value).IsLocalPlayer && Utility.IsTrash(crabpot.heldObject.Value))
					{
						AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", amount: 1);
						if (AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", uint.Parse) % 10 == 0)
							SUtility.improveFriendshipWithEveryoneInRegion(Game1.player, 1, 2);
					}
				}
			}
		}
	}
}