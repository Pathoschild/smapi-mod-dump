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
using StardewValley.Locations;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ProspectorWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer) return;

			ModEntry.ProspectorHunt ??= new();
			if (ModEntry.ProspectorHunt.TreasureTile is not null) ModEntry.ProspectorHunt.End();
			if (Game1.CurrentEvent is null && e.NewLocation is MineShaft)
				ModEntry.ProspectorHunt.TryStartNewHunt(e.NewLocation);
		}
	}
}