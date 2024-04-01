/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;

namespace StardewLocations
{
    /// <summary>The mod entry class loaded by SMAPI.</summary>
    internal class StardewLocations : Mod
    {
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += onWarped;
        }

        /// <summary>Raised after a player warps to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void onWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer && Game1.player?.currentLocation != null)
                Game1.showGlobalMessage(getLocationName(e.NewLocation.Name, e.NewLocation));
        }

        /// <summary>Get the display name for an in-game location.</summary>
        /// <param name="name">The in-game location name.</param>
        private string getLocationName(string name, GameLocation loc)
        {
            var i18n = Helper.Translation;
            var location = loc;//Game1.player.currentLocation;
            var locationData = "";
            
            
            
            if (location is MineShaft shaft)
            {
                locationData =  shaft.mineLevel > 120 ? i18n.Get("location_text", null)+ $" \n\r" + i18n.Get("SkullCave", new { mine_level = shaft.mineLevel }) : i18n.Get("location_text", null) + $" \n\r" + i18n.Get("UndergroundMine", new { mine_level = shaft.mineLevel });
            }


            if (location is Cabin)
            {
                locationData = i18n.Get("location_text", null) + "\n\r"+ i18n.Get("Cabin", new
                {
                    cabin_owner = GetMapOwnersName(loc)
                });
            }
            
            locationData = i18n.Get("location_text", null) + "\n\r" + i18n.Get(name, new
            {
                farm_name = Game1.player.farmName,
                player_name = Game1.player.Name,
                cabin_owner = GetMapOwnersName(loc)
            });


            //Nothing Found use Default
            if (string.IsNullOrEmpty(locationData) || locationData.Contains("translation"))
            {
                locationData = i18n.Get("location_text", null) + "\n\r" + Game1.currentLocation.Name;
            };
            
            return locationData;
        }

        /// <summary>
        /// Get the map owners name, then returns it
        /// </summary>
        /// <param name="loc">The Map</param>
        /// <returns></returns>
        private string GetMapOwnersName(GameLocation loc)
        {
            if (loc is Cabin cabin)
            {
                return string.IsNullOrEmpty(cabin.owner.Name) ? "UnOwned" : cabin.owner.Name;
            }

            return "";
        }
    }
}
