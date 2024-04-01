/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cantorsdust/StardewMods
**
*************************************************/

using System.Runtime.Serialization;
using StardewValley;

namespace TimeSpeed.Framework
{
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Whether to change tick length on festival days.</summary>
        public bool EnableOnFestivalDays { get; set; } = true;

        /// <summary>Whether to show a message about the time settings when you enter a location.</summary>
        public bool LocationNotify { get; set; } = false;

        /// <summary>The time speed for in-game locations, measured in seconds per in-game minute.</summary>
        public ModSecondsPerMinuteConfig SecondsPerMinute { get; set; } = new();

        /// <summary>The mod configuration for where time should be frozen.</summary>
        public ModFreezeTimeConfig FreezeTime { get; set; } = new();

        /// <summary>The keyboard bindings used to control the flow of time. See available keys at <a href="https://msdn.microsoft.com/en-us/library/microsoft.xna.framework.input.keys.aspx" />.</summary>
        public ModControlsConfig Keys { get; set; } = new();


        /*********
        ** Public methods
        *********/
        /// <summary>Get whether time should be frozen at a given location.</summary>
        /// <param name="location">The game location.</param>
        public bool ShouldFreeze(GameLocation location)
        {
            return this.FreezeTime.ShouldFreeze(location);
        }

        /// <summary>Get whether the time should be frozen at a given time of day.</summary>
        /// <param name="time">The time of day in 24-hour military format (e.g. 1600 for 8pm).</param>
        public bool ShouldFreeze(int time)
        {
            return time >= this.FreezeTime.AnywhereAtTime;
        }

        /// <summary>Get whether time settings should be applied on a given day.</summary>
        /// <param name="season">The season to check.</param>
        /// <param name="dayOfMonth">The day of month to check.</param>
        public bool ShouldScale(Season season, int dayOfMonth)
        {
            return this.EnableOnFestivalDays || !Utility.isFestivalDay(dayOfMonth, season);
        }

        /// <summary>Get the number of milliseconds per minute to apply for a location.</summary>
        /// <param name="location">The game location.</param>
        public int GetMillisecondsPerMinute(GameLocation location)
        {
            return (int)(this.SecondsPerMinute.GetSecondsPerMinute(location) * 1000);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the config file is deserialized.</summary>
        /// <param name="context">The deserialization context.</param>
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            this.SecondsPerMinute ??= new();
            this.FreezeTime ??= new();
            this.Keys ??= new();
        }
    }
}
