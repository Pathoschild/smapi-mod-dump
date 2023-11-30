/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/mouahrarasModuleCollection
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace mouahrarasModuleCollection.FarmView.Zoom.Utilities
{
	internal class ZoomUtility
	{
		private static readonly PerScreen<int>	zoomLevel = new(() => 0);
		private static readonly PerScreen<bool>	zoomLevelMinReached = new(() => false);

		internal static void Reset()
		{
			zoomLevel.Value = 0;
			zoomLevelMinReached.Value = false;
		}

		internal static void AddZoomLevel(int direction)
		{
			if (direction < 0 && zoomLevelMinReached.Value)
				return;
			if (direction > 0 && zoomLevel.Value + direction > 0)
				return;
			zoomLevel.Value += direction;
		}

		internal static int GetZoomLevel()
		{
			return zoomLevel.Value;
		}


		internal static void SetZoomLevelMinReached(bool value)
		{
			zoomLevelMinReached.Value = value;
		}

		internal static bool GetZoomLevelMinReached()
		{
			return zoomLevelMinReached.Value;
		}
	}
}
