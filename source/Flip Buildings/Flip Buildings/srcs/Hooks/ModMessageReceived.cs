/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/FlipBuildings
**
*************************************************/

using StardewModdingAPI.Events;
using FlipBuildings.Utilities;

namespace FlipBuildings.Hooks
{
	internal static class ModMessageReceived
	{
		/// <inheritdoc cref="IMultiplayerEvents.ModMessageReceived"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, ModMessageReceivedEventArgs e)
		{
			if (e.FromModID == ModEntry.ModManifest.UniqueID)
			{
				if (e.Type == "InvokeMethod")
				{
					if (e.ReadAs<string>() == "FarmHouseHelper.Flip()")
					{
						FarmHouseHelper.Flip();
					}
				}
			}
		}
	}
}
