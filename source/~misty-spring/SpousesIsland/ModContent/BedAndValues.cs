/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace SpousesIsland.ModContent
{
    internal static class BedCode
    {
        //stuff to make characters path to sleep
        internal static Point GetBedSpot(BedFurniture.BedType bedType = BedFurniture.BedType.Any)
        {
            return GetBed(bedType)?.GetBedSpot() ?? new Point(-1000, -1000);
        }
        private static BedFurniture GetBed(BedFurniture.BedType bedType = BedFurniture.BedType.Any, int index = 0)
        {
            //Furniture f in IslandFarmHouse.Object
            foreach (var f in Game1.getLocationFromName("IslandFarmHouse").furniture)
            {
                if (f is not BedFurniture bed) continue;
                if (bedType != BedFurniture.BedType.Any && bed.bedType != bedType) continue;
                if (index == 0)
                {
                    return bed;
                }
                index--;
            }
            return null;
        }
        private static void SleepEndFunction(Character c, GameLocation location)
        {
            if (c is not NPC npc)
            {
                return;
            }
            if (Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions").ContainsKey(npc.Name.ToLower() + "_sleep"))
            {
                npc.playSleepingAnimation();
            }
            foreach (Furniture furniture in location.furniture)
            {
                if (furniture is not BedFurniture bedFurniture || !bedFurniture.getBoundingBox(bedFurniture.TileLocation).Intersects(npc.GetBoundingBox())) continue;
                bedFurniture.ReserveForNPC();
                break;
            }
        }

        // path spouse sleep (in islandfarmhouse)
        private static Point GetSpouseBedSpot()
        {
            var bedSpot = GetSpouseBed().GetBedSpot();
            bedSpot.X++;
            return bedSpot;
        }

        private static BedFurniture GetSpouseBed()
        {
            return GetBed(BedFurniture.BedType.Double);
        }
        internal static void MakeSpouseGoToBed(NPC c, GameLocation location)
        {
            if (!c.isMarried()) return;
            if (!Game1.IsMasterGame || Game1.timeOfDay < 2200 || c.getTileLocationPoint() == GetSpouseBedSpot() ||
                (Game1.timeOfDay != 2200 && (c.controller != null || Game1.timeOfDay % 100 % 30 != 0))) return;
            //stop npc first
            c.Halt();
            c.followSchedule = false;
            c.ignoreScheduleToday = true;

            //path to bed
            c.controller =
                new PathFindController(
                    c,
                    location: Game1.getLocationFromName("IslandFarmHouse"),
                    GetSpouseBedSpot(),
                    0,
                    (who, gameLocation) =>
                    {
                        who.doEmote(Character.sleepEmote);
                        SleepEndFunction(who, gameLocation);
                    }
                );
        }

        //kid sleep stuff
        internal static void MakeKidGoToBed(NPC c, GameLocation ifh)
        {
            if (Game1.IsMasterGame && Game1.timeOfDay >= 2200 && c.getTileLocationPoint() != GetKidBedSpot() && (Game1.timeOfDay == 2200 || (c.controller == null && Game1.timeOfDay % 100 % 30 == 0)))
            {
                c.controller =
                    new PathFindController(
                        c,
                        location: ifh,
                        GetKidBedSpot(),
                        0,
                        (character, location) =>
                        {
                            character.doEmote(Character.sleepEmote);
                            SleepEndFunction(character, location);
                        }
                );
            }
        }

        private static Point GetKidBedSpot()
        {
            Point bedSpot = GetKidBed().GetBedSpot();
            bedSpot.X++;
            return bedSpot;
        }

        private static BedFurniture GetKidBed()
        {
            return GetBed(BedFurniture.BedType.Child);
        }

        internal static bool HasAnyKidBeds()
        {
            var bed = GetBed(BedFurniture.BedType.Child);
            return bed is not null;
        }

        internal static bool HasCrib(Farmer player)
        {
            var where = Utility.getHomeOfFarmer(player);

            //0 means no crib. so, we return whether crib isn't 0 (doing it this way in case any mod changes crib int to something other than 1)
            return where.cribStyle.Value != 0;
        }
    }
    internal static class Values
    {
        internal static bool IsIntegrated(string name)
        {
            var result = name switch
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
        internal static bool IntegratedAndEnabled(string name)
        {
            var result = name switch
            {
                "Abigail" => ModEntry.Config.Allow_Abigail,
                "Alex" => ModEntry.Config.Allow_Alex,
                "Elliott" => ModEntry.Config.Allow_Elliott,
                "Emily" => ModEntry.Config.Allow_Emily,
                "Haley" => ModEntry.Config.Allow_Haley,
                "Harvey" => ModEntry.Config.Allow_Harvey,
                "Krobus" => ModEntry.Config.Allow_Krobus,
                "Leah" => ModEntry.Config.Allow_Leah,
                "Maru" => ModEntry.Config.Allow_Maru,
                "Penny" => ModEntry.Config.Allow_Penny,
                "Sam" => ModEntry.Config.Allow_Sam,
                "Sebastian" => ModEntry.Config.Allow_Sebastian,
                "Shane" => ModEntry.Config.Allow_Shane,
                "Claire" => ModEntry.Config.Allow_Claire,
                "Lance" => ModEntry.Config.Allow_Lance,
                "Olivia" => ModEntry.Config.Allow_Olivia,
                "Sophia" => ModEntry.Config.Allow_Sophia,
                "Victor" => ModEntry.Config.Allow_Victor,
                "Wizard" => ModEntry.Config.Allow_Magnus,
                _ => false
            };

            return result;
        }

        /// <summary> 
        /// Obtains all married NPCs, for non-host in multiplayer.
        /// </summary>
        internal static List<string> GetAllSpouses(Farmer player)
        {
            List<string> spouses = new();

            foreach (var chara in player.friendshipData.Keys)
            {
                if (player.friendshipData[chara].IsMarried())
                {
                    spouses.Add(chara);
                }
            }

            return spouses;
        }

        /// <summary>
        /// Returns a random map and position. Requires Ginger Island Extra Locations
        /// </summary>
        /// <param name="spousename">The name of the character. Used as reference, depending on random result.</param>
        /// <returns></returns>
        internal static string RandomOrDefault(string spousename)
        {
            var enabled = ModEntry.Config.ScheduleRandom;

            if(enabled)
            {
                return RandomFree();
            }
            else
            {
                var result = spousename switch
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
                var random = Game1.random.Next(1, 6);
                result = random switch
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
                var random = Game1.random.Next(1, 14);
                result = random switch
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

        private static Point GetRandomMapZone(string where)
        {
            var ran = Game1.random;

            var result = where switch
            {
                "Custom_GiCave" => new Point(ran.Next(9, 22), ran.Next(10, 16)),
                "Custom_GiForest" => new Point(ran.Next(11, 29), ran.Next(21, 31)),
                "Custom_GiRiver" => new Point(ran.Next(15, 34), ran.Next(6, 11)),
                "Custom_GiClearance" => new Point(ran.Next(11, 22), ran.Next(13, 26)),
                "Custom_IslandSW" => new Point(ran.Next(10, 37), ran.Next(17, 24)),
                "Custom_GiHut" => new Point(ran.Next(1, 7), ran.Next(6, 8)),
                "Custom_GiForestEnd" => new Point(ran.Next(9, 25), ran.Next(25, 31)),
                "Custom_GiRBeach" => new Point(ran.Next(27, 35), ran.Next(6, 23)),
                "IslandWest" => new Point(ran.Next(74, 95), ran.Next(44, 67)),
                "IslandSouth" => new Point(ran.Next(9, 30), ran.Next(30, 33)),
                "IslandSouthEast" => new Point(ran.Next(10, 15), ran.Next(18, 28)),
                "IslandEast" => new Point(ran.Next(14, 29), ran.Next(31, 41)),
                "IslandNorth" => new Point(ran.Next(33, 56), ran.Next(26, 30)),
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
            string mapName;
            var position = Point.Zero;

            var ran = Game1.random;
            var hasMod = ModEntry.InstalledMods["ExGIM"];

            if (!hasMod || ran.Next(0, 11) > 4)
            {
                //only choose vanilla
                mapName = ran.Next(0, 11) switch
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

                position = mapName switch
                {
                    "IslandFarmHouse" => new Point(ran.Next(5, 14), ran.Next(2, 28)),
                    "IslandWest" => ran.Next(0, 4) switch
                    {
                        0 => new Point(ran.Next(73, 90), ran.Next(12, 18)),
                        1 => new Point(ran.Next(56, 104), ran.Next(79, 87)),
                        2 => new Point(ran.Next(34, 52), ran.Next(69, 74)),
                        3 => new Point(ran.Next(25, 34), ran.Next(60, 70)),
                        _ => new Point(0, 0)
                    },
                    "IslandSouth" => new Point(ran.Next(9, 31), ran.Next(12, 35)),
                    "IslandSouthEast" => new Point(ran.Next(12, 28), ran.Next(16, 28)),
                    "IslandEast" => new Point(ran.Next(12, 30), ran.Next(30, 45)),
                    "IslandNorth" => new Point(ran.Next(25, 61), ran.Next(74, 83)),
                    "IslandFieldOffice" => new Point(ran.Next(2, 8), ran.Next(4, 9)),
                    "IslandFarmCave" => new Point(ran.Next(4, 6), ran.Next(5, 11)),
                    "CaptainRoom" => new Point(ran.Next(1, 5), ran.Next(5, 7)),
                    "IslandWestCave1" => new Point(ran.Next(2, 11), ran.Next(3, 9)),
                    "IslandSouthEastCave" => new Point(ran.Next(14, 29), ran.Next(8, 11)),
                    _ => new Point(0, 0)
                };
            }
            else
            {
                var randomM = ran.Next(1, 9);

                mapName = randomM switch
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

                position = mapName switch
                {
                    "Custom_GiCave" => new Point(ran.Next(9, 22), ran.Next(10, 16)),
                    "Custom_GiForest" => new Point(ran.Next(11, 29), ran.Next(21, 31)),
                    "Custom_GiRiver" => new Point(ran.Next(15, 34), ran.Next(6, 11)),
                    "Custom_GiClearance" => new Point(ran.Next(11, 22), ran.Next(13, 26)),
                    "Custom_IslandSW" => new Point(ran.Next(10, 37), ran.Next(17, 24)),
                    "Custom_GiHut" => new Point(ran.Next(1, 7), ran.Next(6, 8)),
                    "Custom_GiForestEnd" => new Point(ran.Next(9, 25), ran.Next(25, 31)),
                    "Custom_GiRBeach" => new Point(ran.Next(27, 35), ran.Next(6, 23)),
                    _ => new Point(0, 0)
                };
            }


            //make a check that tile is placeable
            var location = Game1.getLocationFromName(mapName);

            if (!location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(position.ToVector2()))
            {
                //loop until finding a better point
                for(var tries = 0; tries < 30; tries++)
                {
                    position = GetRandomMapZone(mapName);
                    if (location.isTileLocationTotallyClearAndPlaceableIgnoreFloors(position.ToVector2())) continue;
                    ModEntry.Mon.Log($"New position: {position.X} {position.Y}");
                    break;
                }
            }

            return $"{mapName} {position.X} {position.Y} {ran.Next(0, 4)}";
        }
    }
}