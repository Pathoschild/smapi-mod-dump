/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StardewValley;
using StardewValley.Locations;

namespace TimeSpeed.Framework
{
    /// <summary>The mod configuration for where or when time should be frozen.</summary>
    internal class ModFreezeTimeConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The time at which to freeze time everywhere (or <c>null</c> to disable this). This should be 24-hour military time (e.g. 800 for 8am, 1600 for 8pm, etc).</summary>
        public int? AnywhereAtTime { get; set; }

        /// <summary>Whether to freeze time indoors.</summary>
        public bool Indoors { get; set; } = false;

        /// <summary>Whether to freeze time outdoors.</summary>
        public bool Outdoors { get; set; } = false;

        /// <summary>Whether to freeze time in the mines.</summary>
        public bool Mines { get; set; } = false;

        /// <summary>Whether to freeze time in the Skull Cavern.</summary>
        public bool SkullCavern { get; set; } = false;

        /// <summary>Whether to freeze time in the Volcano Dungeon.</summary>
        public bool VolcanoDungeon { get; set; } = false;

        /// <summary>The names of custom locations in which to freeze time.</summary>
        /// <remarks>Location names can be seen in-game using the <a href="https://www.nexusmods.com/stardewvalley/mods/679">Debug Mode</a> mod.</remarks>
        public HashSet<string> ByLocationName { get; set; } = new(StringComparer.OrdinalIgnoreCase);


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether time should be frozen in the given location.</summary>
        /// <param name="location">The location to check.</param>
        public bool ShouldFreeze(GameLocation location)
        {
            if (location == null)
                return false;

            // by location name
            if (this.ByLocationName.Contains(location.Name))
                return true;

            // by location name (Deep Woods mod)
            if ((location.Name == "DeepWoods" || location.Name.StartsWith("DeepWoods_")) && this.ByLocationName.Contains("DeepWoods"))
                return true;

            // mines / Skull Cavern
            if (location is MineShaft shaft)
            {
                return shaft.mineLevel <= 120
                    ? this.Mines
                    : this.SkullCavern;
            }

            // volcano dungeon
            if (location is VolcanoDungeon)
                return this.VolcanoDungeon;

            // indoors or outdoors
            return location.IsOutdoors
                ? this.Outdoors
                : this.Indoors;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the config file is deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            this.ByLocationName = new(this.ByLocationName ?? new(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
