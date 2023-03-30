/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace SpousesIsland
{
    internal class BedCode
    {
        //stuff to make characters path to sleep
        internal readonly GameLocation ifh = Game1.getLocationFromName("IslandFarmHouse");
        internal Point GetBedSpot(BedFurniture.BedType bed_type = BedFurniture.BedType.Any)
        {
            return GetBed(bed_type)?.GetBedSpot() ?? new Point(-1000, -1000);
        }
        internal BedFurniture GetBed(BedFurniture.BedType bed_type = BedFurniture.BedType.Any, int index = 0)
        {
            //Furniture f in IslandFarmHouse.Object
            foreach (Furniture f in ifh.furniture)
            {
                if (f is BedFurniture)
                {
                    BedFurniture bed = f as BedFurniture;
                    if (bed_type == BedFurniture.BedType.Any || bed.bedType == bed_type)
                    {
                        if (index == 0)
                        {
                            return bed;
                        }
                        index--;
                    }
                }
            }
            return null;
        }
        internal static void SleepEndFunction(Character c, GameLocation location)
        {
            if (c == null || c is not NPC)
            {
                return;
            }
            if (Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions").ContainsKey(c.Name.ToLower() + "_sleep"))
            {
                (c as NPC).playSleepingAnimation();
            }
            foreach (Furniture furniture in location.furniture)
            {
                if (furniture is BedFurniture && furniture.getBoundingBox(furniture.TileLocation).Intersects(c.GetBoundingBox()))
                {
                    (furniture as BedFurniture).ReserveForNPC();
                    break;
                }
            }
        }

        // path spouse sleep (in islandfarmhouse)
        internal Point GetSpouseBedSpot()
        {
            Point bed_spot = GetSpouseBed().GetBedSpot();
            bed_spot.X++;
            return bed_spot;
        }
        internal virtual BedFurniture GetSpouseBed()
        {
            return GetBed(BedFurniture.BedType.Double);
        }
        internal void MakeSpouseGoToBed(NPC c, GameLocation location)
        {
            if (c.isMarried())
            {
                if (Game1.IsMasterGame && Game1.timeOfDay >= 2200 && c.getTileLocationPoint() != GetSpouseBedSpot() && (Game1.timeOfDay == 2200 || (c.controller == null && Game1.timeOfDay % 100 % 30 == 0)))
                {
                    c.controller =
                        new PathFindController(
                            c,
                            location: ifh,
                            GetSpouseBedSpot(),
                            0,
                            (c, location) =>
                            {
                                c.doEmote(Character.sleepEmote);
                                SleepEndFunction(c, location);
                            }
                    );
                }
            }
        }

        //kid sleep stuff
        internal void MakeKidGoToBed(NPC c, GameLocation ifh)
        {
            if (Game1.IsMasterGame && Game1.timeOfDay >= 2200 && c.getTileLocationPoint() != getKidBedSpot(c.Name) && (Game1.timeOfDay == 2200 || (c.controller == null && Game1.timeOfDay % 100 % 30 == 0)))
            {
                c.controller =
                    new PathFindController(
                        c,
                        location: ifh,
                        getKidBedSpot(c.Name),
                        0,
                        (c, location) =>
                        {
                            c.doEmote(Character.sleepEmote);
                            SleepEndFunction(c, location);
                        }
                );
            }
        }

        internal Point getKidBedSpot(string name)
        {
            Point bed_spot = GetKidBed().GetBedSpot();
            bed_spot.X++;
            return bed_spot;
        }

        internal virtual BedFurniture GetKidBed()
        {
            return GetBed(BedFurniture.BedType.Child);
        }

        internal static bool HasAnyKidBeds()
        {
            var sgv = new BedCode();
            var bed = sgv.GetBed(BedFurniture.BedType.Child);
            if (bed is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    internal class Values
    {
        internal static bool IsIntegrated(string name)
        {
            bool result = name switch
            {
                "Abigail" => true,
                "Alex" => true,
                "Elliott" => true,
                "Emily" => true,
                "Haley" => true,
                "Harvey" => true,
                "Krobus" => true,
                "Leah" => true,
                "Maru" => true,
                "Penny" => true,
                "Sam" => true,
                "Sebastian" => true,
                "Shane" => true,
                "Claire" => true,
                "Lance" => true,
                "Olivia" => true,
                "Sophia" => true,
                "Victor" => true,
                "Wizard" => true,
                _ => false
            };

            return result;
        }
        /// <summary> 
        /// Returns integrated spouse's "Allowed" config. If not integrated, returns false.
        /// </summary>
        internal static bool IntegratedAndEnabled(string name, ModConfig C)
        {
            bool result = name switch
            {
                "Abigail" => C.Allow_Abigail,
                "Alex" => C.Allow_Alex,
                "Elliott" => C.Allow_Elliott,
                "Emily" => C.Allow_Emily,
                "Haley" => C.Allow_Haley,
                "Harvey" => C.Allow_Harvey,
                "Krobus" => C.Allow_Krobus,
                "Leah" => C.Allow_Leah,
                "Maru" => C.Allow_Maru,
                "Penny" => C.Allow_Penny,
                "Sam" => C.Allow_Sam,
                "Sebastian" => C.Allow_Sebastian,
                "Shane" => C.Allow_Shane,
                "Claire" => C.Allow_Claire,
                "Lance" => C.Allow_Lance,
                "Olivia" => C.Allow_Olivia,
                "Sophia" => C.Allow_Sophia,
                "Victor" => C.Allow_Victor,
                "Wizard" => C.Allow_Magnus,
                _ => false
            };

            return result;
        }

        /// <summary> 
        /// Obtains all married NPCs, for non-host in multiplayer.
        /// </summary>
        internal static List<string> GetAllSpouses(Farmer player)
        {
            List<string> Spouses = new();

            foreach (var chara in player.friendshipData.Keys)
            {
                if (player.friendshipData[chara].IsMarried())
                {
                    Spouses.Add(chara);
                }
            }

            return Spouses;
        }

        /// <summary>
        /// Returns a random map and position. Requires Ginger Island Extra Locations
        /// </summary>
        /// <param name="spousename">The name of the character. Used as reference, depending on random result.</param>
        /// <returns></returns>
        internal static string RandomOrDefault(string spousename)
        {
            var enabled = ModEntry.CanRandomize;

            if(enabled)
            {
                return RandomFree();
            }
            else
            {
                string result = spousename switch
                {
                    "Abigail" => "IslandWest 62 84 2",
                    "Alex" => "IslandWest 69 77 2 alex_football/1500 IslandWest 64 83 2",
                    "Elliott" => "IslandNorth 19 15 0 \"Strings\\schedules\\Elliott:marriage_loc3\"",
                    "Emily" => "IslandWestCave1 3 6 1 \"Strings\\schedules\\Emily:marriage_loc3\"",
                    "Haley" => "IslandWest 76 12 2 haley_photo \"Strings\\schedules\\Haley:marriage_loc3\"",
                    "Harvey" => "IslandWest 88 14 2 harvey_read \"Strings\\schedules\\Harvey:marriage_loc3\"",
                    "Krobus" => "IslandFarmCave 2 6 2",
                    "Leah" => "IslandWest 89 72 2 leah_draw \"Strings\\schedules\\Leah:marriage_loc3\"",
                    "Maru" => "IslandFieldOffice 7 8 0 \"Strings\\schedules\\Maru:marriage_loc3\"",
                    "Penny" => "IslandFieldOffice 2 7 2 \"Strings\\schedules\\Penny:marriage_loc3\"",
                    "Sam" => "IslandSouthEast 23 14 2 \"Strings\\schedules\\Sam:marriage_loc3\"",
                    "Sebastian" => "IslandNorth 40 23 2 \"Strings\\schedules\\Sebastian:marriage_loc3\"",
                    "Shane" => "IslandSouthEastCave 29 6 2 \"Strings\\schedules\\Shane:marriage_loc3\"",
                    "Claire" => "IslandWest 87 78 2",
                    "Lance" => "IslandSouthEast 21 28 2 \"Characters\\Dialogue\\Lance:marriage_loc3\"",
                    "Magnus" => "Caldera 22 23 0 \"Characters\\Dialogue\\Wizard:marriage_loc3\"",
                    "Wizard" => "Caldera 22 23 0 \"Characters\\Dialogue\\Wizard:marriage_loc3\"",
                    "Olivia" => "IslandNorth 36 73 0 \"Characters\\Dialogue\\Olivia:marriage_loc3\"",
                    "Sophia" => "IslandFarmHouse 18 12 Sophia_Read \"Characters\\Dialogue\\Sophia:marriage_loc3\"",
                    "Victor" => "IslandFarmHouse 19 5 2 Victor_Book2 \"Characters\\Dialogue\\Victor:marriage_loc3\"",
                    _ => RandomFree()
                };
                return result;
            }
            
        }
        internal static string GetMapName(bool hasMod)
        {
            string result;

            if (!hasMod)
            {
                int Random = Game1.random.Next(1, 6);
                result = Random switch
                {
                    1 => "IslandWest",
                    2 => "IslandSouth",
                    3 => "IslandSouthEast",
                    4 => "IslandEast",
                    5 => "IslandNorth",
                    _ => null
                };

            }
            else
            {
                int Random = Game1.random.Next(1, 14);
                result = Random switch
                {
                    1 => "Custom_GiCave",
                    2 => "Custom_GiForest",
                    3 => "Custom_GiRiver",
                    4 => "Custom_GiClearance",
                    5 => "Custom_IslandSW",
                    6 => "Custom_GiHut",
                    7 => "Custom_GiForestEnd",
                    8 => "Custom_GiRBeach",
                    9 => "IslandWest",
                    10 => "IslandSouth",
                    11 => "IslandSouthEast",
                    12 => "IslandEast",
                    13 => "IslandNorth",
                    _ => null
                };
            }

            return result;
        }
        internal static Point GetRandomMapZone(string where)
        {
            Random Ran = Game1.random;

            Point result = where switch
            {
                "Custom_GiCave" => new Point(Ran.Next(9, 22), Ran.Next(10, 16)),
                "Custom_GiForest" => new Point(Ran.Next(11, 29), Ran.Next(21, 31)),
                "Custom_GiRiver" => new Point(Ran.Next(15, 34), Ran.Next(6, 11)),
                "Custom_GiClearance" => new Point(Ran.Next(11, 22), Ran.Next(13, 26)),
                "Custom_IslandSW" => new Point(Ran.Next(10, 37), Ran.Next(17, 24)),
                "Custom_GiHut" => new Point(Ran.Next(1, 7), Ran.Next(6, 8)),
                "Custom_GiForestEnd" => new Point(Ran.Next(9, 25), Ran.Next(25, 31)),
                "Custom_GiRBeach" => new Point(Ran.Next(27, 35), Ran.Next(6, 23)),
                "IslandWest" => new Point(Ran.Next(74, 95), Ran.Next(44, 67)),
                "IslandSouth" => new Point(Ran.Next(9, 30), Ran.Next(30, 33)),
                "IslandSouthEast" => new Point(Ran.Next(10, 15), Ran.Next(18, 28)),
                "IslandEast" => new Point(Ran.Next(14, 29), Ran.Next(31, 41)),
                "IslandNorth" => new Point(Ran.Next(33, 56), Ran.Next(26, 30)),
                _ => new Point(0, 0)
            };

            return result;
        }

        /// <summary>
        /// Returns a random location from the entire island, regardless of character/name.
        /// </summary>
        /// <returns></returns>
        public static string RandomFree()
        {
            string MapName;
            Point Position = Point.Zero;

            Random Ran = Game1.random;
            var hasMod = ModEntry.HasExGIM;

            if (!hasMod || Ran.Next(0, 11) > 4)
            {
                //only choose vanilla
                MapName = Ran.Next(0, 11) switch
                {
                    0 => "IslandFarmHouse",
                    1 => "IslandWest",
                    2 => "IslandSouth",
                    3 => "IslandSouthEast",
                    4 => "IslandEast",
                    5 => "IslandNorth",
                    6 => "IslandFieldOffice",
                    7 => "IslandFarmCave",
                    8 => "CaptainRoom",
                    9 => "IslandWestCave1",
                    10 => "IslandSouthEastCave",
                    _ => null //if VolcanoDungeon0 is added: x 28 - 35, y 40 - 48
                };

                Position = MapName switch
                {
                    "IslandFarmHouse" => new Point(Ran.Next(5, 14), Ran.Next(2, 28)),
                    "IslandWest" => Ran.Next(0, 4) switch
                    {
                        0 => new Point(Ran.Next(73, 90), Ran.Next(12, 18)),
                        1 => new Point(Ran.Next(56, 104), Ran.Next(79, 87)),
                        2 => new Point(Ran.Next(34, 52), Ran.Next(69, 74)),
                        3 => new Point(Ran.Next(25, 34), Ran.Next(60, 70)),
                        _ => new Point(0, 0)
                    },
                    "IslandSouth" => new Point(Ran.Next(9, 31), Ran.Next(12, 35)),
                    "IslandSouthEast" => new Point(Ran.Next(12, 28), Ran.Next(16, 28)),
                    "IslandEast" => new Point(Ran.Next(12, 30), Ran.Next(30, 45)),
                    "IslandNorth" => new Point(Ran.Next(25, 61), Ran.Next(74, 83)),
                    "IslandFieldOffice" => new Point(Ran.Next(2, 8), Ran.Next(4, 9)),
                    "IslandFarmCave" => new Point(Ran.Next(4, 6), Ran.Next(5, 11)),
                    "CaptainRoom" => new Point(Ran.Next(1, 5), Ran.Next(5, 7)),
                    "IslandWestCave1" => new Point(Ran.Next(2, 11), Ran.Next(3, 9)),
                    "IslandSouthEastCave" => new Point(Ran.Next(14, 29), Ran.Next(8, 11)),
                    _ => new Point(0, 0)
                };
            }
            else
            {
                int RandomM = Ran.Next(1, 9);

                MapName = RandomM switch
                {
                    1 => "Custom_GiCave",
                    2 => "Custom_GiForest",
                    3 => "Custom_GiRiver",
                    4 => "Custom_GiClearance",
                    5 => "Custom_IslandSW",
                    6 => "Custom_GiHut",
                    7 => "Custom_GiForestEnd",
                    8 => "Custom_GiRBeach",
                    _ => null
                };

                Position = MapName switch
                {
                    "Custom_GiCave" => new Point(Ran.Next(9, 22), Ran.Next(10, 16)),
                    "Custom_GiForest" => new Point(Ran.Next(11, 29), Ran.Next(21, 31)),
                    "Custom_GiRiver" => new Point(Ran.Next(15, 34), Ran.Next(6, 11)),
                    "Custom_GiClearance" => new Point(Ran.Next(11, 22), Ran.Next(13, 26)),
                    "Custom_IslandSW" => new Point(Ran.Next(10, 37), Ran.Next(17, 24)),
                    "Custom_GiHut" => new Point(Ran.Next(1, 7), Ran.Next(6, 8)),
                    "Custom_GiForestEnd" => new Point(Ran.Next(9, 25), Ran.Next(25, 31)),
                    "Custom_GiRBeach" => new Point(Ran.Next(27, 35), Ran.Next(6, 23)),
                    _ => new Point(0, 0)
                };
            }


            //make a check that tile is placeable
            var location = Game1.getLocationFromName(MapName);

            if (!location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(Position.ToVector2()))
            {
                //loop until finding a better point
                for(int tries = 0; tries < 30; tries++)
                {
                    Position = GetRandomMapZone(MapName);
                    if (!location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(Position.ToVector2()))
                    {
                        ModEntry.Mon.Log($"New position: {Position.X} {Position.Y}");
                        break;
                    }
                }
            }

            return $"{MapName} {Position.X} {Position.Y} {Ran.Next(0, 4)}";
        }
    }
}