using StardewValley;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MoreMultiplayerInfo.Helpers
{
    public class LocationHelper
    {

        static Dictionary<string, string> Locations => new Dictionary<string, string>
        {
            { "AdventureGuild", "Adventurer's Guild" },
            { "AnimalShop", "Marnie's Ranch" },
            { "ArchaeologyHouse", "Archaeology Office" },
            { "Barn", "Farm Barn" },
            { "BathHouse_Entry", "Bath House Entrance" },
            { "BathHouse_MensLocker", "Bath House" },
            { "BathHouse_Pool", "Bath House Pool" },
            { "BathHouse_WomensLocker", "Bath House" },
            { "BusStop", "Bus Stop" },
            { "Cabin", "Farm Cabin" },
            { "CommunityCenter", "Community Center" },
            { "Coop", "Farm Coop" },
            { "Farm", Game1.player.farmName?.Value + " Farm" },
            { "FarmCave", "Farm Cave" },
            { "FarmHouse", "Farm House" },
            { "FishShop", "Fishing Shop" },
            { "Forest", "Cindersap Forest" },
            { "Greenhouse", "Farm Greenhouse" },
            { "HaleyHouse", "2 Willow Lane" },
            { "Hospital", "Harvey's Clinic" },
            { "JojaMart", "Joja Mart" },
            { "JoshHouse", "1 River Road" },
            { "LeahHouse", "Leah's Cottage" },
            { "ManorHouse", "Mayor's Manor" },
            { "Saloon", "Stardrop Saloon" },
            { "SamHouse", "1 Willow Lane" },
            { "SandyHouse", "Oasis" },
            { "ScienceHouse", "Carpenter's Shop" },
            { "SebastianRoom", "Sebastian's Room" },
            { "SeedShop", "General Store" },
            { "SkullCave", "Skull Cavern Entrance" },
            { "Tent", "Mountain Tent" },
            { "Town", "Pelican Town" },
            { "UndergroundMine", "Mountain Mines Entrance" },
            { "WitchHut", "Witch Hut" },
            { "WitchSwamp", "Witch Swamp" },
            { "WitchWarpCave", "Witch Warp Cave" },
            { "WizardHouse", "Wizard's Tower" },
            { "WizardHouseBasement", "Wizard Tower Basement" },
            { "Woods", "Secret Woods" },
        };

        public static string GetFriendlyLocationName(string locationName)
        {
            Regex regex = new Regex(@"\d+$");
            if (regex.IsMatch(locationName))
            {
                var floor = regex.Match(locationName);
                var mine = locationName.Contains("UndergroundMine") ? "Mountain Mine" : "Skull Cavern";

                return $"Floor {floor} of {mine}";
            }

            if (Locations.ContainsKey(locationName))
            {
                return Locations[locationName];
            }

            return locationName;
        }
    }
}
