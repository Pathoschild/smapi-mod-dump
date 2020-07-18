using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Newtonsoft.Json;

namespace DestroyableBushes
{
    /// <summary>A set of serializable save data for this mod.</summary>
    public class ModData
    {
        /// <summary>A list of destroyed bushes that have not yet been respawned.</summary>
        public List<DestroyedBush> DestroyedBushes { get; set; } = new List<DestroyedBush>();

        /// <summary>The information needed to respawn a destroyed bush.</summary>
        public class DestroyedBush
        {
            public string LocationName { get; set; }
            public Vector2 Tile { get; set; }
            public int Size { get; set; }

            [JsonProperty]
            private int day;
            [JsonProperty]
            private string season;
            [JsonProperty]
            private int year;

            [JsonIgnore]
            public SDate DateDestroyed
            {
                get
                {
                    return new SDate(day, season, year);
                }
                set
                {
                    day = value.Day;
                    season = value.Season;
                    year = value.Year;
                }
            }

            public DestroyedBush()
            {

            }

            public DestroyedBush(string locationName, Vector2 tile, int size)
            {
                LocationName = locationName;
                Tile = tile;
                Size = size;
                DateDestroyed = SDate.Now();
            }
        }
    }
}
