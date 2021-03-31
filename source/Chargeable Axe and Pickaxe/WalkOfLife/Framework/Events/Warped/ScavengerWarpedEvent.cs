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

namespace TheLion.AwesomeProfessions
{
	internal class ScavengerWarpedEvent : WarpedEvent
	{
		private ScavengerHunt _Hunt { get; }

		/// <summary>Construct an instance.</summary>
		internal ScavengerWarpedEvent(ScavengerHunt hunt)
		{
			_Hunt = hunt;
		}

		/// <summary>Raised after the current player moves to a new location. Trigger Scavenger hunt events.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event arguments.</param>
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;
			
			if (_Hunt.TreasureTile != null) _Hunt.End();

			if (Game1.CurrentEvent == null && e.NewLocation.IsOutdoors && !(e.NewLocation.IsFarm || e.NewLocation.NameOrUniqueName.Equals("Town")))
				_Hunt.TryStartNewHunt(e.NewLocation);
		}
	}
}
