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
using StardewValley;
using StardewValley.Menus;
using mouahrarasModuleCollection.UserInterface.Zoom.Utilities;

namespace mouahrarasModuleCollection.UserInterface.Zoom.Handlers
{
	internal static class UpdateTickingHandler
	{
		/// <inheritdoc cref="IGameLoopEvents.UpdateTicking"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, UpdateTickingEventArgs e)
		{
			if (!MenusPatchUtility.ShouldProcess(Game1.activeClickableMenu))
				return;

			bool isZoomInKeyDown = ModEntry.Helper.Input.IsDown(ModEntry.Config.UserInterfaceZoomInKey);
			bool isZoomOutKeyDown = ModEntry.Helper.Input.IsDown(ModEntry.Config.UserInterfaceZoomOutKey);

			if (!isZoomInKeyDown || !isZoomOutKeyDown)
			{
				if (isZoomInKeyDown)
				{
					ZoomUtility.AddZoomLevel(120);
				}
				else if (isZoomOutKeyDown)
				{
					ZoomUtility.AddZoomLevel(-120);
				}
			}
		}
	}
}
