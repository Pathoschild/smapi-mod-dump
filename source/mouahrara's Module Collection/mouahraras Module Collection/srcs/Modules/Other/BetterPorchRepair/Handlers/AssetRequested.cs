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
using mouahrarasModuleCollection.Other.BetterPorchRepair.Utilities;

namespace mouahrarasModuleCollection.Other.BetterPorchRepair.Handlers
{
	internal static class AssetRequestedHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.AssetRequested"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, AssetRequestedEventArgs e)
		{
			// Load assets
			BetterPorchRepairUtility.RepairPorch(e);
		}
	}
}
