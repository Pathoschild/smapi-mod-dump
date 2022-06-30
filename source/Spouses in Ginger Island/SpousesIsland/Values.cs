/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace SpousesIsland
{
    internal class SGIValues
    {
        private static Random locRandom;
        internal static Random Ran
        {
            get
            {
                locRandom ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 30));
                return locRandom;
            }
        }
        /// <summary>
        /// Checks spouse name. If it coincides with an integrated one, return true (for warning)
        /// </summary>
        /// <param name="spouse"> The name of the spouse to compare.</param>
        /// <returns></returns>
        internal static bool CheckSpouseName(string spouse)
        {
            switch (spouse)
            {
                case "Abigail":
                case "Alex":
                case "Elliott":
                case "Emily":
                case "Haley":
                case "Harvey":
                case "Krobus":
                case "Leah":
                case "Maru":
                case "Penny":
                case "Sam":
                case "Sebastian":
                case "Shane":
                case "Claire":
                case "Lance":
                case "Olivia":
                case "Sophia":
                case "Victor":
                case "Wizard":
                    return true;
                default:
                    return false;
            }
        }

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

        //returns a list with the spouses that the mod edits
        internal static List<string> SpousesAddedByMod()
        {
            List<string> SpousesAddedByMod = new ();
            SpousesAddedByMod.Add("Abigail");
            SpousesAddedByMod.Add("Alex");
            SpousesAddedByMod.Add("Emily");
            SpousesAddedByMod.Add("Elliott");
            SpousesAddedByMod.Add("Haley");
            SpousesAddedByMod.Add("Harvey");
            SpousesAddedByMod.Add("Krobus");
            SpousesAddedByMod.Add("Leah");
            SpousesAddedByMod.Add("Maru");
            SpousesAddedByMod.Add("Penny");
            SpousesAddedByMod.Add("Sam");
            SpousesAddedByMod.Add("Sebastian");
            SpousesAddedByMod.Add("Shane");
            SpousesAddedByMod.Add("Claire");
            SpousesAddedByMod.Add("Lance");
            SpousesAddedByMod.Add("Olivia");
            SpousesAddedByMod.Add("Sophia");
            SpousesAddedByMod.Add("Victor");
            SpousesAddedByMod.Add("Wizard");
            return SpousesAddedByMod;
        }
        //returns a random map and position from a list
        internal static string RandomMap_nPos(string spousename, bool ModInstalled, bool ActivatedConfig)
        {
            int choice = Ran.Next(1, 11);
            if (choice is 1 || ModInstalled is false || ActivatedConfig is false)
            {
                string result = spousename switch
                {
                    "Abigail" => "a1800 IslandWest 62 84 2",
                    "Alex" => "1300 IslandWest 69 77 2 alex_football/1500 IslandWest 64 83 2",
                    "Elliott" => "a1900 IslandNorth 19 15 0 \"Strings\\schedules\\Elliott:marriage_loc3\"",
                    "Emily" => "1700 IslandWestCave1 3 6 1 \"Strings\\schedules\\Emily:marriage_loc3\"",
                    "Haley" => "1400 IslandWest 76 12 2 haley_photo \"Strings\\schedules\\Haley:marriage_loc3\"",
                    "Harvey" => "1600 IslandWest 88 14 2 harvey_read \"Strings\\schedules\\Harvey:marriage_loc3\"",
                    "Krobus" => "1900 IslandFarmCave 2 6 2",
                    "Leah" => "1600 IslandWest 89 72 2 leah_draw \"Strings\\schedules\\Leah:marriage_loc3\"",
                    "Maru" => "1700 IslandFieldOffice 7 8 0 \"Strings\\schedules\\Maru:marriage_loc3\"",
                    "Penny" => "1700 IslandFieldOffice 2 7 2 \"Strings\\schedules\\Penny:marriage_loc3\"",
                    "Sam" => "1700 IslandSouthEast 23 14 2 \"Strings\\schedules\\Sam:marriage_loc3\"",
                    "Sebastian" => "1600 IslandNorth 40 23 2 \"Strings\\schedules\\Sebastian:marriage_loc3\"",
                    "Shane" => "a1900 IslandSouthEastCave 29 6 2 \"Strings\\schedules\\Shane:marriage_loc3\"",
                    "Claire" => "1600 IslandWest 87 78 2",
                    "Lance" => "1600 IslandSouthEast 21 28 2 \"Characters\\Dialogue\\Lance:marriage_loc3\"",
                    "Magnus" => "1800 Caldera 22 23 0 \"Characters\\Dialogue\\Wizard:marriage_loc3\"",
                    "Olivia" => "1600 IslandNorth 36 73 0 \"Characters\\Dialogue\\Olivia:marriage_loc3\"",
                    "Sophia" => "1600 IslandFarmHouse 18 12 Sophia_Read \"Characters\\Dialogue\\Sophia:marriage_loc3\"",
                    "Victor" => "1600 IslandFarmHouse 19 5 2 Victor_Book2 \"Characters\\Dialogue\\Victor:marriage_loc3\"",
                    _ => "1630 IslandFarmHouse 5 5"
                };
                return result;
            }
            else
            {
                int hour;
                string MapName;
                int RandomM = Ran.Next(1, 8);

                switch (spousename)
                {
                    case "Abigail":
                        hour = 1400;
                        break;
                    case "Alex":
                        hour = 1300;
                        break;
                    case "Elliott":
                        hour = 1600;
                        break;
                    case "Emily":
                        hour = 1600;
                        break;
                    case "Haley":
                        hour = 1300;
                        break;
                    case "Harvey":
                        hour = 1530;
                        break;
                    case "Krobus":
                        hour = 1700;
                        break;
                    case "Leah":
                        hour = 1600;
                        break;
                    case "Maru":
                        hour = 1700;
                        break;
                    case "Penny":
                        hour = 1600;
                        break;
                    case "Sam":
                        hour = 1640;
                        break;
                    case "Sebastian":
                        hour = 1600;
                        break;
                    case "Shane":
                        hour = 1530;
                        break;
                    case "Claire":
                        hour = 1600;
                        break;
                    case "Lance":
                        hour = 1600;
                        break;
                    case "Magnus":
                        hour = 1800;
                        break;
                    case "Olivia":
                        hour = 1600;
                        break;
                    case "Sophia":
                        hour = 1600;
                        break;
                    case "Victor":
                        hour = 1600;
                        break;
                    default:
                        hour = 0;
                        break;
                }
                switch (RandomM)
                {
                    case 1:
                        MapName = "Custom_GiCave";
                        break;
                    case 2:
                        MapName = "Custom_GiForest";
                        break;
                    case 3:
                        MapName = "Custom_GiRiver";
                        break;
                    case 4:
                        MapName = "Custom_GiClearance";
                        break;
                    case 5:
                        MapName = "Custom_IslandSW";
                        break;
                    case 6:
                        MapName = "Custom_GiHut";
                        break;
                    case 7:
                        MapName = "Custom_GiForestEnd";
                        break;
                    case 8:
                        MapName = "Custom_GiRBeach";
                        break;
                    default:
                        MapName = null;
                        break;
                }
                Point Position = MapName switch
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
                string result = $"{hour} {MapName} {Position.X} {Position.Y} {Ran.Next(0,4)}";
                return result;
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
            var sgv = new SGIValues();
            var bed = sgv.GetBed(BedFurniture.BedType.Child);
            if(bed is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}