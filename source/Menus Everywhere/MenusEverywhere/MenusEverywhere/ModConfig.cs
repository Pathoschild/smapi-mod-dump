/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nman130/Stardew-Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace MenusEverywhere
{
    /// <summary>The configuration file creation for the mod.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default values.</summary>
        public static ModConfig Defaults { get; } = new ModConfig();

        /*********
        ** Keyboard buttons
        *********/
        /// <summary>The button which opens the calendar.</summary>
        public SButton CalendarKey { get; set; } = SButton.F1;

        /// <summary>The button which opens the request board.</summary>
        public SButton RequestKey { get; set; } = SButton.F2;

        /// <summary>The button which opens the monster eradication goal menu.</summary>
        public SButton MonsterEradicationKey { get; set; } = SButton.F3;

        /// <summary>The button which opens the shipping bin.</summary>
        public SButton BinKey { get; set; } = SButton.F5;

        /*********
        ** Toggleables
        **********/

        /// <summary>Whether the bundles can be accessed from outside the community center.</summary>
        public bool CanAccessBundles { get; set; } = true;

        /// <summary>Whether the shipping bin can be accessed from outside the farm.</summary>
        public bool CanAccessBin { get; set; } = false;
    }
}
