/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/UiModSuite
**
*************************************************/

using UiModSuite;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using UiModSuite.UiMods;

namespace UiModSuite.Options {
    class ModOptionsPage : OptionsWindow {

        public enum Setting : int {
            ALLOW_EXPERIENCE_BAR_TO_FADE_OUT = 1,
            SHOW_EXPERIENCE_BAR = 2,
            SHOW_EXP_GAIN = 3,
            SHOW_LEVEL_UP_ANIMATION = 4,
            SHOW_HEART_FILLS = 5,
            SHOW_EXTRA_ITEM_INFORMATION = 6,
            SHOW_LOCATION_Of_TOWNSPEOPLE = 7,
            SHOW_LUCK_ICON = 8,
            SHOW_TRAVELING_MERCHANT = 9,
            SHOW_LOCATION_OF_TOWNSPEOPLE_SHOW_QUEST_ICON = 10,
            SHOW_CROP_AND_BARREL_TOOLTIP_ON_HOVER = 11,
            SHOW_BIRTHDAY_ICON = 12,
            SHOW_ANIMALS_NEED_PETS = 13,
            SHOW_SPRINKLER_SCARECROW_RANGE = 14,
            SHOW_ITEMS_REQUIRED_FOR_BUNDLES = 15,
            SHOW_HARVEST_PRICES_IN_SHOP = 16,
            DISPLAY_CALENDAR_AND_BILLBOARD = 17,
        }

        /// <summary>
        /// This class provides a page to handle attaching and removing mods
        /// </summary>
        internal ModOptionsPage( List<ModOptionsElement> options ) {
            this.options = options;
        }

        [Obsolete( "Never fires" )]
        internal static int getSliderValue( Setting setting ) {
            return ModEntry.modData.intSettings[ (int) setting ];
        }

        internal static bool getCheckboxValue( Setting setting ) {
            return ModEntry.modData.boolSettings[ (int) setting ];
        }

        [Obsolete( "Never fires" )]
        internal static string getSelectValue( Setting setting ) {
            return ModEntry.modData.stringSettings[ (int) setting ];
        }

    }
}
