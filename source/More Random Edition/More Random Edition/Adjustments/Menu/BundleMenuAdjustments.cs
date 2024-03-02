/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using SVItem = StardewValley.Item;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    public class BundleMenuAdjustments
	{
        /// <summary>
        /// Assumes a JunimoNoteMenu was just opened, and overrides it out own definition
        /// that actually supports depositing rings
        /// </summary>
        /// <param name="junimoNoteMenu">The menu that was opened - used to check if it was from a pause</param>
        /// <returns>Null if we didn't override it, and the menu itself if we did</returns>
		public static JunimoNoteMenu OverrideJunimoNoteMenu(JunimoNoteMenu junimoNoteMenu)
		{
			if (junimoNoteMenu.fromGameMenu || // The "game menu" is the pause menu
                (Game1.currentLocation is not CommunityCenter && Game1.currentLocation is not AbandonedJojaMart))
			{
				// This is okay - it's probably from the pause menu
				return null;
			}
            
            int area = Game1.currentLocation is CommunityCenter
                ? GetAreaNumberFromLocation(Game1.player.getTileLocation())
                : 6; // AbandonedJojaMart.cs simply hard-codes this to 6

            CommunityCenter comCenter = Game1.getLocationFromName("CommunityCenter") as CommunityCenter;
            JunimoNoteMenu overriddenJunimoNoteMenu = new OverriddenJunimoNoteMenu(area, comCenter.bundlesDict());

            // Release the lock that was just put on it before requesting the new one
            comCenter.bundleMutexes[area].ReleaseLock();
            comCenter.bundleMutexes[area].RequestLock(() => Game1.activeClickableMenu = overriddenJunimoNoteMenu);
            Game1.activeClickableMenu = overriddenJunimoNoteMenu;

            return overriddenJunimoNoteMenu;
        }

        /// <summary>
        /// Taken from the original CommunityCenter.cs code - gets the area of the bundle
        /// the player is trying to access, based on where they currently are
        /// </summary>
        /// <param name="tileLocation">The tile location</param>
        /// <returns>The area number, or -1 if not found</returns>
        private static int GetAreaNumberFromLocation(Vector2 tileLocation)
        {
            CommunityCenter currentLoc = (CommunityCenter)Game1.currentLocation;
            for (int area = 0; area < currentLoc.areasComplete.Count; ++area)
            {
                if (GetAreaBounds(area).Contains((int)tileLocation.X, (int)tileLocation.Y))
                    return area;
            }
            return -1;
        }

        /// <summary>
        /// Logic from the original CommunityCenter.cs which takes the area ID and grabs the
        /// bounds it exists in in the CommunityCenter map
        /// </summary>
        /// <param name="area">The area</param>
        /// <returns>The area of the region on the map</returns>
        private static Rectangle GetAreaBounds(int area)
        {
            return area switch
            {
                0 => new Rectangle(0, 0, 22, 11),
                1 => new Rectangle(0, 12, 21, 17),
                2 => new Rectangle(35, 4, 9, 9),
                3 => new Rectangle(52, 9, 16, 12),
                4 => new Rectangle(45, 0, 15, 9),
                5 => new Rectangle(22, 13, 28, 9),
                7 => new Rectangle(44, 10, 6, 3),
                8 => new Rectangle(22, 4, 13, 9),
                _ => Rectangle.Empty,
            };
        }

        /// <summary>
        /// Fixes the ability to highlight rings in the bundle menu
        /// </summary>
        public static void FixRingSelection(JunimoNoteMenu e)
		{
			if (!Globals.Config.Bundles.Randomize)
			{
				return;
			}

            e.inventory.highlightMethod = HighlightBundleCompatibleItems;
		}

		/// <summary>
		/// A copy of the Utlity.cs code for highlightSmallObjects, but with rings included
		/// </summary>
		/// <param name="item">The Stardew Valley item</param>
		/// <returns>True if the item should be draggable, false otherwise</returns>
		private static bool HighlightBundleCompatibleItems(SVItem item)
		{
			if (item is Ring)
			{
				return true;
			}
			else if (item is SVObject)
			{
				return !(bool)((NetFieldBase<bool, NetBool>)(item as SVObject).bigCraftable);
			}
			return false;
		}

		/// <summary>
		/// Replaces the bundle names with our custom ones
		/// Only really necessary for non-English locales
		/// </summary>
		/// <param name="menu">The menu to adjust</param>
		public static void InsertCustomBundleNames(JunimoNoteMenu menu)
		{
            if (!Globals.Config.Bundles.Randomize)
            {
                return;
            }

            foreach (StardewValley.Menus.Bundle bundle in menu.bundles)
            {
                bundle.label = BundleRandomizer.BundleToName[bundle.bundleIndex];
            }
        }

        /// <summary>
        /// Adds tooltips for the bundle items so that you can see where to get fish/foragables, etc
        /// </summary>
        /// 
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

