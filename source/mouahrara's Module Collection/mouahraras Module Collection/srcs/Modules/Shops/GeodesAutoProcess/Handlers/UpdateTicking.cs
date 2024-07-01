/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI.Events;
using mouahrarasModuleCollection.Shops.GeodesAutoProcess.Utilities;

namespace mouahrarasModuleCollection.Shops.GeodesAutoProcess.Handlers
{
	internal static class UpdateTickingHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, UpdateTickingEventArgs e)
		{
			if (!ModEntry.Config.ShopsGeodesAutoProcess)
				return;

			if (GeodesAutoProcessUtility.IsProcessing() && GeodesAutoProcessUtility.GeodeMenu.geodeAnimationTimer <= 0)
				GeodesAutoProcessUtility.CrackGeodeSecure();
		}
	}
}
