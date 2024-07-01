/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using System;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace mouahrarasModuleCollection.UserInterface.Zoom.Utilities
{
	internal class ZoomUtility
	{
		private static readonly PerScreen<int>		additionalZoom = new(() => 0);
		private static readonly PerScreen<float>	zoomLevel = new(() => -1f);
		private static readonly PerScreen<bool>		zoomLevelMinReached = new(() => false);

		internal static void Reset()
		{
			AdditionalZoom = 0;
			ZoomLevel = -1f;
			ZoomLevelMinReached = false;
			Game1.updateViewportForScreenSizeChange(false, Game1.graphics.PreferredBackBufferWidth, Game1.graphics.PreferredBackBufferHeight);
		}

		internal static int AdditionalZoom
		{
			get => additionalZoom.Value;
			set => additionalZoom.Value = value;
		}

		internal static float ZoomLevel
		{
			get => zoomLevel.Value < 0 ? Game1.options.desiredBaseZoomLevel : zoomLevel.Value;
			set => zoomLevel.Value = value;
		}

		internal static bool ZoomLevelMinReached
		{
			get => zoomLevelMinReached.Value;
			set => zoomLevelMinReached.Value = value;
		}

		internal static void AddZoomLevel(int direction)
		{
			float min = ModEntry.Config.UserInterfaceZoomMinimumZoomLevel;
			float max = Game1.options.desiredBaseZoomLevel;
			int newAdditionalZoom = AdditionalZoom + direction;
			float newZoomLevel = Game1.options.desiredBaseZoomLevel - (-newAdditionalZoom * ModEntry.Config.UserInterfaceZoomMultiplier / 8000f);

			if (direction < 0 && (ZoomLevelMinReached || newZoomLevel < min))
			{
				newZoomLevel = min;
			}
			if (direction > 0 && newZoomLevel > max)
			{
				newZoomLevel = max;
			}
			if (newZoomLevel == min || newZoomLevel == max)
			{
				newAdditionalZoom = (int)Math.Round((newZoomLevel - Game1.options.desiredBaseZoomLevel) * 8000f / ModEntry.Config.UserInterfaceZoomMultiplier);
			}
			ZoomLevel = newZoomLevel;
			AdditionalZoom = newAdditionalZoom;
		}
	}
}
