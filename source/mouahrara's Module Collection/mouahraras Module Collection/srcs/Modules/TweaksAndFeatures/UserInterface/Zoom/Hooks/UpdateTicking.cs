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
using mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Utilities;

namespace mouahrarasModuleCollection.TweaksAndFeatures.UserInterface.Zoom.Hooks
{
	internal static class UpdateTickingHook
	{
		/// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, UpdateTickingEventArgs e)
		{
			if (!ModEntry.Config.UserInterfaceZoom)
				return;
			if (ModEntry.Helper.Input.IsDown(ModEntry.Config.UserInterfaceZoomInKey))
			{
				ZoomUtility.AddZoomLevel(120);
				return;
			}
			if (ModEntry.Helper.Input.IsDown(ModEntry.Config.UserInterfaceZoomOutKey))
			{
				ZoomUtility.AddZoomLevel(-120);
				return;
			}
		}
	}
}
