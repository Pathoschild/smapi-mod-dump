/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FlipBuildings.Handlers
{
	internal static class AssetReadyHandler
	{
		/// <inheritdoc cref="IContentEvents.AssetReady"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, AssetReadyEventArgs e)
		{
			if (!Context.IsGameLaunched)
				return;

			if (e.Name.IsEquivalentTo("Data/Buildings"))
			{
				UpdateTickedHandler.ReloadContent = true;
			}
		}
	}
}
