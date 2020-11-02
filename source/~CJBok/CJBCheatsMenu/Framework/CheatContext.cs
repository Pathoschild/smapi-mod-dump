/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using CJBCheatsMenu.Framework.Models;
using StardewModdingAPI;
using StardewValley;

namespace CJBCheatsMenu.Framework
{
    /// <summary>Context metadata available to cheat implementations.</summary>
    internal class CheatContext
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get a cached list of all in-game locations.</summary>
        private readonly Func<IEnumerable<GameLocation>> GetAllLocationsImpl;


        /*********
        ** Accessors
        *********/
        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; }

        /// <summary>Simplifies access to private code.</summary>
        public IReflectionHelper Reflection { get; }

        /// <summary>The display width of an option slot during the last cheats menu render.</summary>
        public int SlotWidth { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="reflection">Simplifies access to private code.</param>
        /// <param name="getAllLocations">Get a cached list of all in-game locations.</param>
        public CheatContext(ModConfig config, IReflectionHelper reflection, Func<IEnumerable<GameLocation>> getAllLocations)
        {
            this.Config = config;
            this.Reflection = reflection;
            this.GetAllLocationsImpl = getAllLocations;
        }

        /// <summary>Get all in-game locations.</summary>
        public IEnumerable<GameLocation> GetAllLocations()
        {
            return this.GetAllLocationsImpl();
        }
    }
}
