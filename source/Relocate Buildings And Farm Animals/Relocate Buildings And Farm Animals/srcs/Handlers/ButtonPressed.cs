/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/RelocateFarmAnimals
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using RelocateBuildingsAndFarmAnimals.Utilities;

namespace RelocateBuildingsAndFarmAnimals.Handlers
{
	internal static class ButtonPressedHandler
	{
		internal static bool ReloadContent = false;

		/// <inheritdoc cref="IInputEvents.ButtonPressed"/>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		internal static void Apply(object sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsWorldReady)
				return;

			if (Game1.options.menuButton.Any((menuButton) => menuButton.ToSButton().Equals(e.Button)))
			{
				PagedResponsesMenuUtility.ReceiveMenuButtonKeyPress(e.Button);
			}
		}
	}
}
