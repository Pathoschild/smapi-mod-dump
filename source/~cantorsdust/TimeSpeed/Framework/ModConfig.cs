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
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default number of seconds per 10-game-minutes, or <c>null</c> to freeze time globally. The game uses 7 seconds by default.</summary>
        public double? DefaultTickLength { get; set; } = 7.0;

        /// <summary>The number of seconds per 10-game-minutes (or <c>null</c> to freeze time) for each location. The key can be a location name or <see cref="LocationType"/> value.</summary>
        /// <remarks>Most location names can be found at "\Stardew Valley\Content\Maps" directory. They usually match the file name without its extension. 'Mine' is a special case which includes all mine maps.</remarks>
        /// <example>
        /// This will set the Mines to 28 seconds per 10-game-minutes, freeze time indoors and use <see cref="DefaultTickLength"/> for outdoors:
        /// <code>
        /// "TickLengthByLocation": {
        ///     "Mine": 28,
        ///     "Indoors": null
        /// }
        /// </code>
        /// 
        /// This will freeze time on your farm and set it to 14 seconds per 10-game-minutes elsewhere.
        /// <code>
        /// "TickLengthByLocation": {
        ///     "Indoors": 14,
        ///     "Outdoors": 14,
        ///     "Farm":null
        /// }
        /// </code>
        /// 
        /// This will freeze time in the Saloon. All other locations will default to <see cref="DefaultTickLength"/>.
        /// <code>
        /// "TickLengthByLocation": {
        ///     "Saloon": null
        /// }
        /// </code>
        /// </example>
        public Dictionary<string, double?> TickLengthByLocation { get; set; } = new(StringComparer.OrdinalIgnoreCase)
        {
            { LocationType.Indoors.ToString(), 14 },
            { LocationType.Outdoors.ToString(), 7 },
            { LocationType.Mine.ToString(), 7 },
            { LocationType.SkullCavern.ToString(), 7 },
            { LocationType.VolcanoDungeon.ToString(), 7 }
        };

        /// <summary>Whether to change tick length on festival days.</summary>
        public bool EnableOnFestivalDays { get; set; } = true;

        /// <summary>The time at which to freeze time everywhere (or <c>null</c> to disable this). This should be 24-hour military time (e.g. 800 for 8am, 1600 for 8pm, etc).</summary>
        public int? FreezeTimeAt { get; set; } = null;

        /// <summary>Whether to show a message about the time settings when you enter a location.</summary>
        public bool LocationNotify { get; set; } = false;

        /// <summary>The keyboard bindings used to control the flow of time. See available keys at <a href="https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.input.keys.aspx" />.</summary>
        public ModControlsConfig Keys { get; set; } = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether time should be frozen at a given location.</summary>
        /// <param name="location">The game location.</param>
        public bool ShouldFreeze(GameLocation location)
        {
            return this.GetTickLengthOrFreeze(location) == null;
        }

        /// <summary>Get whether the time should be frozen at a given time of day.</summary>
        /// <param name="time">The time of day in 24-hour military format (e.g. 1600 for 8pm).</param>
        public bool ShouldFreeze(int time)
        {
            return this.FreezeTimeAt == time;
        }

        /// <summary>Get whether time settings should be applied on a given day.</summary>
        /// <param name="season">The season to check.</param>
        /// <param name="dayOfMonth">The day of month to check.</param>
        public bool ShouldScale(string season, int dayOfMonth)
        {
            return this.EnableOnFestivalDays || !Utility.isFestivalDay(dayOfMonth, season);
        }

        /// <summary>Get the tick interval to apply for a location.</summary>
        /// <param name="location">The game location.</param>
        public int? GetTickInterval(GameLocation location)
        {
            return (int?)((this.GetTickLengthOrFreeze(location) ?? this.DefaultTickLength) * 1000);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the config file is deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            this.TickLengthByLocation = new(this.TickLengthByLocation, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>Get the tick length to apply for a given location, or <c>null</c> to freeze time.</summary>
        /// <param name="location">The game location.</param>
        private double? GetTickLengthOrFreeze(GameLocation location)
        {
            // check by location name
            if (this.TickLengthByLocation.TryGetValue(location.Name, out double? tickLength))
                return tickLength;

            // check by location type
            foreach (LocationType type in this.GetLocationTypes(location))
            {
                if (this.TickLengthByLocation.TryGetValue(type.ToString(), out tickLength))
                    return tickLength;
            }

            // default
            return this.DefaultTickLength;
        }

        /// <summary>Get the applicable location types in priority order.</summary>
        /// <param name="location">The location to check.</param>
        private IEnumerable<LocationType> GetLocationTypes(GameLocation location)
        {
            // specific type
            if (location is MineShaft shaft)
            {
                yield return shaft.mineLevel <= 120
                    ? LocationType.Mine
                    : LocationType.SkullCavern;
            }
            else if (location is VolcanoDungeon)
                yield return LocationType.VolcanoDungeon;
            else if (location.Name == "DeepWoods" || location.Name.StartsWith("DeepWoods_"))
                yield return LocationType.DeepWoods;

            // indoors or outdoors
            yield return location.IsOutdoors
                ? LocationType.Outdoors
                : LocationType.Indoors;
        }
    }
}
