/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/EscasModdingPlugins
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace EscasModdingPlugins
{
    /// <summary>Adds a version of the "Action" "Kitchen" tile property that can function outside of FarmHouse and IslandFarmHouse locations.</summary>
    /// <remarks>This feature was added directly to Stardew v1.6, but has been preserved to provide backward compatibility for other mods.</remarks>
    public static class ActionKitchen
    {
        /// <summary>The full name of this patch's "Action" tile property value.</summary>
        public static string ActionName { get; set; }

        /// <summary>True if this class's behavior is currently enabled.</summary>
        public static bool Enabled { get; private set; } = false;

        /// <summary>The monitor instance to use for log messages. Null if not provided.</summary>
        private static IMonitor Monitor { get; set; } = null;

        /// <summary>Initializes this class and enables its features.</summary>
        /// <param name="monitor">The monitor instance to use for console/log messages.</param>
        public static void Enable(IMonitor monitor)
        {
            if (Enabled)
                return; //do nothing

            //store args
            Monitor = monitor;

            //initialize assets/properties
            ActionName = ModEntry.PropertyPrefix + "Kitchen";

            GameLocation.RegisterTileAction(ActionName, ActivateKitchen);

            Enabled = true;
        }

        /// <summary>Imitates the base game's "Kitchen" action.</summary>
        public static bool ActivateKitchen(GameLocation location, string[] args, Farmer player, Point point)
        {
            location.ActivateKitchen();
            return true;
        }
    }
}
