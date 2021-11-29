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

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class ScavengerWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModState.ScavengerHunt ??= new();
			if (ModState.ScavengerHunt.TreasureTile is not null) ModState.ScavengerHunt.End();
			if (Game1.CurrentEvent is null && e.NewLocation.IsOutdoors &&
			    !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName == "Town"))
				ModState.ScavengerHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}