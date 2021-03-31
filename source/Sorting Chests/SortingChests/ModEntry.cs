/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aRooooooba/SortingChests
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Menus;
using StardewValley;
using StardewValley.Objects;

namespace SortingChests
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ChestFactory chestFactory;
        private int skipTriggers;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            skipTriggers = 0;
            chestFactory = new ChestFactory(helper.Multiplayer, Monitor);
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.World.ChestInventoryChanged += OnChestInventoryChanged;
        }


        /*********
        ** Private methods
        *********/
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            skipTriggers += chestFactory.SortChestsInAllLocations();
            Game1.exitActiveMenu();
        }

        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnChestInventoryChanged(object sender, ChestInventoryChangedEventArgs e)
        {
            if (skipTriggers > 0)
            {
                skipTriggers--;
                return;
            }
            bool menuOpened = Game1.activeClickableMenu != null;
            skipTriggers += chestFactory.UpdateContent(e.Chest, e.Location, e.Added, e.Removed, e.QuantityChanged);
            if (!menuOpened)
                Game1.exitActiveMenu();
        }
    }
}