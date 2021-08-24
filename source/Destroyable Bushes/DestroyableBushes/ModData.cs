/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/DestroyableBushes
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;

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

            /// <summary>Returns all tiles that would be obstructed by this bush's collision box.</summary>
            /// <returns>Each tile that would be obstructed by this bush's collision box.</returns>
            public IEnumerable<Vector2> GetCollisionTiles()
            {
                switch (Size)
                {
                    case Bush.mediumBush: //2 tiles wide
                    case Bush.walnutBush:
                        yield return Tile;
                        yield return new Vector2(Tile.X + 1, Tile.Y);
                        break;
                    case Bush.largeBush: //3 tiles wide
                        yield return Tile;
                        yield return new Vector2(Tile.X + 1, Tile.Y);
                        yield return new Vector2(Tile.X + 2, Tile.Y);
                        break;
                    default: //1 tile wide
                        yield return Tile;
                        break;
                }
            }
        }
    }
}
