using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace StardewHypnos
{
    /// <summary>
    /// Represents the minimum relationship types.
    /// </summary>
    internal enum MinimumRelationshipType
    {
        /// <summary>
        /// NPC must be dating, engaged or married with the Farmer
        /// </summary>
        OnlyPartners,

        /// <summary>
        /// NPC must be friends with the Farmer
        /// </summary>
        Friends,

        /// <summary>
        /// NPC must have no bond with the Farmer
        /// </summary>
        Everyone,
    }

    /// <summary>
    /// The mod configuration.
    /// </summary>
    internal class ModConfig
    {
        /// <summary>
        /// Gets or sets the "minimum" relationship type to use new features.
        /// </summary>
        public MinimumRelationshipType MinimumRelationship { get; set; } = MinimumRelationshipType.OnlyPartners;

        /// <summary>
        /// Gets or sets the minimum amount of hearts with a NPC to enter their houses and sleep there.
        /// </summary>
        public int MinimumFriendshipHearts { get; set; } = 6;

        /// <summary>
        /// Gets or sets a value indicating whether doors should be open at all times for locations owned by allowed NPCs.
        /// </summary>
        public bool KeepFriendDoorsOpen { get; set; } = true;

        /// <summary>
        /// Gets or sets a collection of possible allowed NPCs in certain location.
        /// </summary>
        public Dictionary<string, List<string>> NPCsByWarp { get; set; } = new Dictionary<string, List<string>>
        {
            {
                "AnimalShop",
                new List<string>
                {
                    "Marnie",
                    "Shane",
                }
            },
            {
                "SeedShop",
                new List<string>
                {
                    "Abigail",
                    "Pierre",
                    "Caroline",
                }
            },
            {
                "Trailer",
                new List<string>
                {
                    "Penny",
                    "Pam",
                }
            },
            {
                "HaleyHouse",
                new List<string>
                {
                    "Haley",
                    "Emily",
                }
            },
            {
                "LeahHouse",
                new List<string>
                {
                    "Leah",
                }
            },
            {
                "ScienceHouse",
                new List<string>
                {
                    "Maru",
                    "Robin",
                    "Demetrius",
                    "Sebastian",
                }
            },
            {
                "JoshHouse",
                new List<string>
                {
                    "Alex",
                }
            },
            {
                "ElliottHouse",
                new List<string>
                {
                    "Elliott",
                }
            },
            {
                "SamHouse",
                new List<string>
                {
                    "Sam",
                    "Kent",
                    "Jodi",
                }
            },
            {
                "ManorHouse",
                new List<string>
                {
                    "Lewis",
                }
            },
            {
                "SebastianRoom",
                new List<string>
                {
                    "Sebastian",
                }
            },
            {
                "HarveyRoom",
                new List<string>
                {
                    "Harvey",
                }
            },
            {
                "Saloon",
                new List<string>
                {
                    "Gus",
                }
            },
            {
                "Tent",
                new List<string>
                {
                    "Linus",
                }
            },
            {
                "Blacksmith",
                new List<string>
                {
                    "Clint",
                }
            },
        };

        /// <summary>
        /// Gets or sets a collection of bed TileIndexes by map names.
        /// </summary>
        public Dictionary<string, List<int>> BedTileIndexes { get; set; } = new Dictionary<string, List<int>>
        {
            {
                "townInterior",
                new List<int> { 384, 386, 390, 448, 450, 454, 836, 1123, 1294 }
            },
            {
                "ElliottHouseTiles",
                new List<int> { 25 }
            },
        };

        /// <summary>
        /// Gets or sets a collection of blacklisted bed locations by map names.
        /// </summary>
        public Dictionary<string, List<Vector2>> BlacklistedBedTiles { get; set; } = new Dictionary<string, List<Vector2>>
        {
            {
                "AnimalShop",
                new List<Vector2>
                {
                    new Vector2(1, 7),
                }
            },
            {
                "SamHouse",
                new List<Vector2>
                {
                    new Vector2(8, 22),
                }
            },
        };
    }
}
