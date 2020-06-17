using System.Collections.Generic;
using xTile.Tiles;

namespace AnimalsNeedWater
{
    /// <summary> Profiles for mods that modify interiors of Barns/Coops. </summary>
    public static class Profiles
    {
        /// <summary> A trough placement profile for default buildings. </summary>
        public static Profile Default { get; set; } = new Profile()
        {
            // tiles for the barn
            barnTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 2,
                    TileY = 6,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big barn
            barn2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 19,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 7,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe barn
            barn3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 7,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 23,
                    TileY = 13,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe barn
            barn3WateringSystem = new WateringSystemTile()
            {
                TileX = 23,
                TileY = 3,
                Layer = "Buildings",
                SystemTilesheetIndex = 0,
                TilesToRemove = new List<SimplifiedTile>()
                {
                    new SimplifiedTile()
                    {
                        TileX = 23,
                        TileY = 3,
                        Layer = "Buildings"
                    },
                    new SimplifiedTile()
                    {
                        TileX = 23,
                        TileY = 2,
                        Layer = "Front"
                    }
                }
            },

            // tiles for the coop
            coopTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 10,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the big coop
            coop2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the deluxe coop
            coop3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 6,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // watering system location for deluxe coop
            coop3WateringSystem = new WateringSystemTile()
            {
                TileX = 20,
                TileY = 2,
                Layer = "Front",
                SystemTilesheetIndex = 1,
                TilesToRemove = new List<SimplifiedTile>()
            }
        };

        /// <summary> A trough placement profile for the More Barn And Coop Animals (#4869 on Nexus) mod by Aair. </summary>
        public static Profile MoreBarnAndCoopAnimalsByAair { get; set; } = new Profile()
        {
            // tiles for the barn
            barnTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 2,
                    TileY = 6,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 4,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 4,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big barn
            barn2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 7,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 5,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 4,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 4,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe barn
            barn3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 7,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 23,
                    TileY = 13,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 2,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 2,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 3,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe barn
            barn3WateringSystem = new WateringSystemTile()
            {
                TileX = 4,
                TileY = 3,
                Layer = "Buildings",
                SystemTilesheetIndex = 0,
                TilesToRemove = new List<SimplifiedTile>()
                {
                    new SimplifiedTile()
                    {
                        TileX = 4,
                        TileY = 3,
                        Layer = "Buildings"
                    },
                    new SimplifiedTile()
                    {
                        TileX = 4,
                        TileY = 2,
                        Layer = "Front"
                    }
                }
            },

            // tiles for the coop
            coopTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the big coop
            coop2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the deluxe coop
            coop3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 6,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // watering system location for deluxe coop
            coop3WateringSystem = new WateringSystemTile()
            {
                TileX = 1,
                TileY = 4,
                Layer = "Front",
                SystemTilesheetIndex = 2,
                TilesToRemove = new List<SimplifiedTile>()
            }
        };

        /// <summary> A trough placement profile for the Clean and Block for Barns and Coops (#3909 on Nexus) mod by pepoluan. </summary>
        public static Profile CleanAndBlockForBarnsAndCoopsByPepoluan { get; set; } = new Profile()
        {
            // tiles for the barn
            barnTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big barn
            barn2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 19,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe barn
            barn3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 23,
                    TileY = 13,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe barn
            barn3WateringSystem = new WateringSystemTile()
            {
                TileX = 23,
                TileY = 3,
                Layer = "Buildings",
                SystemTilesheetIndex = 0,
                TilesToRemove = new List<SimplifiedTile>()
                {
                    new SimplifiedTile()
                    {
                        TileX = 23,
                        TileY = 3,
                        Layer = "Buildings"
                    },
                    new SimplifiedTile()
                    {
                        TileX = 23,
                        TileY = 2,
                        Layer = "Front"
                    }
                }
            },

            // tiles for the coop
            coopTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 10,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the big coop
            coop2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the deluxe coop
            coop3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // watering system location for deluxe coop
            coop3WateringSystem = new WateringSystemTile()
            {
                TileX = 20,
                TileY = 2,
                Layer = "Front",
                SystemTilesheetIndex = 1,
                TilesToRemove = new List<SimplifiedTile>()
            }
        };

        /// <summary> A trough placement profile for the Coop and Barn Facelift (#3874 on Nexus) mod by nykachu. </summary>
        public static Profile CoopAndBarnFaceliftByNykachu { get; set; } = new Profile()
        {
            // tiles for the barn
            barnTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 15,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big barn
            barn2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 19,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe barn
            barn3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 23,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe barn
            barn3WateringSystem = new WateringSystemTile()
            {
                TileX = 23,
                TileY = 2,
                Layer = "Front",
                SystemTilesheetIndex = 1,
                TilesToRemove = new List<SimplifiedTile>()
            },

            // tiles for the coop
            coopTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 11,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the big coop
            coop2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 15,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the deluxe coop
            coop3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 19,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe coop
            coop3WateringSystem = new WateringSystemTile()
            {
                TileX = 19,
                TileY = 2,
                Layer = "Front",
                SystemTilesheetIndex = 1,
                TilesToRemove = new List<SimplifiedTile>()
            }
        };

        /// <summary> A trough placement profile for the Cuter Coops and Better Barns (#4226 on Nexus) mod by DaisyNiko. </summary>
        public static Profile CuterCoopsAndBetterBarnsByDaisyNiko { get; set; } = new Profile()
        {
            // tiles for the barn
            barnTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },

                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 7,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 7,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big barn
            barn2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },

                /*******
                ** Large Troughs
                *******/
                // #1
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 7,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 7,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                },
                // #2
                new TroughTile()
                {
                    TileX = 19,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 19,
                    TileY = 4,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 4,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe barn
            barn3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 4,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },

                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 7,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 7,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe barn
            barn3WateringSystem = new WateringSystemTile()
            {
                TileX = 8,
                TileY = 10,
                Layer = "Buildings",
                SystemTilesheetIndex = 0,
                TilesToRemove = new List<SimplifiedTile>()
                {
                    new SimplifiedTile()
                    {
                        TileX = 8,
                        TileY = 10,
                        Layer = "Buildings"
                    },
                    new SimplifiedTile()
                    {
                        TileX = 8,
                        TileY = 9,
                        Layer = "Front"
                    }
                }
            },

            // tiles for the coop
            coopTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 12,
                    TileY = 4,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },

                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 5,
                    TileY = 9,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 9,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 5,
                    TileY = 8,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 6,
                    TileY = 8,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big coop
            coop2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 15,
                    TileY = 4,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },

                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 15,
                    TileY = 10,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 15,
                    TileY = 9,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe coop
            coop3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 23,
                    TileY = 4,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                },

                /*******
                ** Large Troughs
                *******/
                // #1
                new TroughTile()
                {
                    TileX = 10,
                    TileY = 11,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 11,
                    TileY = 11,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 10,
                    TileY = 10,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 11,
                    TileY = 10,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                },
                // #2
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 20,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe coop
            coop3WateringSystem = new WateringSystemTile()
            {
                TileX = 22,
                TileY = 3,
                Layer = "Buildings",
                SystemTilesheetIndex = 0,
                TilesToRemove = new List<SimplifiedTile>()
                {
                    new SimplifiedTile()
                    {
                        TileX = 22,
                        TileY = 3,
                        Layer = "Buildings"
                    },
                    new SimplifiedTile()
                    {
                        TileX = 22,
                        TileY = 2,
                        Layer = "Front"
                    }
                }
            }
        };

        /// <summary> A trough placement profile for the Cleaner Barns and Coops (#3472 on Nexus) mod by Froststar11. </summary>
        public static Profile CleanerBarnsAndCoopsByFroststar11 { get; set; } = new Profile()
        {
            // tiles for the barn
            barnTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 13,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the big barn
            barn2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 17,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 18,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // tiles for the deluxe barn
            barn3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Large Troughs
                *******/
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 6,
                    FullTroughTilesheetIndex = 8
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 3,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 7,
                    FullTroughTilesheetIndex = 9
                },
                new TroughTile()
                {
                    TileX = 21,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 1,
                    FullTroughTilesheetIndex = 4
                },
                new TroughTile()
                {
                    TileX = 22,
                    TileY = 2,
                    Layer = "Front",
                    EmptyTroughTilesheetIndex = 2,
                    FullTroughTilesheetIndex = 5
                }
            },

            // watering system location for deluxe barn
            barn3WateringSystem = new WateringSystemTile()
            {
                TileX = 23,
                TileY = 3,
                Layer = "Buildings",
                SystemTilesheetIndex = 0,
                TilesToRemove = new List<SimplifiedTile>()
            },

            /******
            ** Original coop maps don't contain any water troughs at all, so we'll have to add some.
            ******/

            // tiles for the coop
            coopTroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 10,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the big coop
            coop2TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 14,
                    TileY = 5,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // tiles for the deluxe coop
            coop3TroughTiles = new List<TroughTile>()
            {
                /*******
                ** Small Troughs
                *******/
                new TroughTile()
                {
                    TileX = 1,
                    TileY = 6,
                    Layer = "Buildings",
                    EmptyTroughTilesheetIndex = 0,
                    FullTroughTilesheetIndex = 3
                }
            },

            // watering system location for deluxe coop
            coop3WateringSystem = new WateringSystemTile()
            {
                TileX = 1,
                TileY = 5,
                Layer = "Front",
                SystemTilesheetIndex = 2,
                TilesToRemove = new List<SimplifiedTile>()
            }
        };
    }

    public class Profile
    {
        public List<TroughTile> barnTroughTiles;

        public List<TroughTile> barn2TroughTiles;

        public List<TroughTile> barn3TroughTiles;

        public WateringSystemTile barn3WateringSystem;

        public List<TroughTile> coopTroughTiles;

        public List<TroughTile> coop2TroughTiles;

        public List<TroughTile> coop3TroughTiles;

        public WateringSystemTile coop3WateringSystem;
    }

    public class TroughTile
    {
        public int TileX { get; set; }
        public int TileY { get; set; }
        public string Layer { get; set; }
        public int EmptyTroughTilesheetIndex { get; set; }
        public int FullTroughTilesheetIndex { get; set; }
    }

    public class WateringSystemTile
    {
        public int TileX { get; set; }
        public int TileY { get; set; }
        public string Layer { get; set; }
        public int SystemTilesheetIndex { get; set; }
        public List<SimplifiedTile> TilesToRemove { get; set; }
    }

    public class SimplifiedTile
    {
        public int TileX { get; set; }
        public int TileY { get; set; }
        public string Layer { get; set; }
    }
}
