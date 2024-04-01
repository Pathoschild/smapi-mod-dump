/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SolidFoundations.Framework.Utilities
{
    internal static class Toolkit
    {
        private static List<string> _knownVanillaIndoorTypes = new List<string>()
        {
            "AbandonedJojaMart",
            "AdventureGuild",
            "AnimalHouse",
            "BathHousePool",
            "Beach",
            "BeachNightMarket",
            "BoatTunnel",
            "BugLand",
            "Bus",
            "BusStop",
            "Cabin",
            "Caldera",
            "Cellar",
            "Club",
            "CommunityCenter",
            "DecoratableLocation",
            "Desert",
            "Farm",
            "FarmCave",
            "FarmHouse",
            "FishShop",
            "Forest",
            "IslandEast",
            "IslandFarmCave",
            "IslandFarmHouse",
            "IslandFieldOffice",
            "IslandForestLocation",
            "IslandHut",
            "IslandLocation",
            "IslandNorth",
            "IslandSecret",
            "IslandShrine",
            "IslandSouth",
            "IslandSouthEast",
            "IslandSouthEastCave",
            "IslandWest",
            "IslandWestCave1",
            "JojaMart",
            "LibraryMuseum",
            "ManorHouse",
            "MermaidHouse",
            "Mine",
            "MineShaft",
            "Mountain",
            "MovieTheater",
            "Railroad",
            "SeedShop",
            "Sewer",
            "Shed",
            "ShopLocation",
            "SlimeHutch",
            "Submarine",
            "Summit",
            "Town",
            "WizardHouse",
            "Woods"
        };

        internal static int GetLightSourceIdentifierForBuilding(Point tile, int count)
        {
            var baseId = (tile.X * 5000) + (tile.Y * 6000);
            return baseId + count + 1;
        }

        internal static Rectangle GetRectangleFromString(string rawRectangle)
        {
            Rectangle rectangle;
            try
            {
                string[] array = rawRectangle.Split(' ');
                rectangle = new Rectangle(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
            }
            catch (Exception)
            {
                rectangle = Rectangle.Empty;
            }

            return rectangle;
        }

        internal static bool IsKnownVanillaIndoorType(string type)
        {
            return _knownVanillaIndoorTypes.Contains(type);
        }
    }
}
