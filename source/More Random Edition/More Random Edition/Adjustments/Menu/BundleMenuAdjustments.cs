/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.Menus;

namespace Randomizer
{
    public class BundleMenuAdjustments
	{
        /// <summary>
        /// Adds tooltips for the bundle items so that you can see where to get fish/foragables, etc
        /// </summary>
        public static void AddDescriptionsToBundleTooltips()
		{
			if (Game1.activeClickableMenu is not JunimoNoteMenu menu || 
				!Globals.Config.Bundles.Randomize || 
				!Globals.Config.Bundles.ShowDescriptionsInBundleTooltips)
			{
				return;
			}

			foreach (ClickableTextureComponent ingredient in menu.ingredientList)
			{
				ingredient.hoverText = $"{ingredient.item.DisplayName}:\n{ingredient.item.getDescription()}";
			}
		}
	}
}

