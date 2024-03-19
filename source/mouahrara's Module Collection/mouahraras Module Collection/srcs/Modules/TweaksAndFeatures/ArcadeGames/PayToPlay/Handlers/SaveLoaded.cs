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
using mouahrarasModuleCollection.TweaksAndFeatures.ArcadeGames.PayToPlay.Managers;

namespace mouahrarasModuleCollection.TweaksAndFeatures.ArcadeGames.PayToPlay.Handlers
{
	internal static class SaveLoadedHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.SaveLoaded"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, SaveLoadedEventArgs e)
		{
			// Load assets
			AssetManager.Apply();
		}
	}
}
