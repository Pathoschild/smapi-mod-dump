/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using StardewModdingAPI.Events;
using BuildableGingerIslandFarm.Utilities;

namespace BuildableGingerIslandFarm.Handlers
{
	internal static class AssetRequestedHandler
	{
		/// <inheritdoc cref="IContentEvents.AssetRequested"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, AssetRequestedEventArgs e)
		{
			// Make Ginger Island Farm always active
			GingerIslandFarmUtility.MakeAlwaysActive(e);
		}
	}
}
