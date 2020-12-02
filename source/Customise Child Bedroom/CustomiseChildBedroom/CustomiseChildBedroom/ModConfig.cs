/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/CustomiseChildBedroom
**
*************************************************/

using StardewValley;
using System.Collections.Generic;
using System.Linq;

namespace CustomiseChildBedroom
{
    class ModConfig
    {
        /*****************************/
        /**      Properties         **/
        /*****************************/
        //public List<Farmer> Farmers { get; set; }

        public List<Farm> Farms { get; set; } = new List<Farm>() { };

        public Farm GetFarm(string FarmName) => Farms.Find(x => x.FarmName == FarmName);
        public Farm GetCurrentFarm() => GetFarm(Game1.player.farmName.Value);

        public void Save()
        {
            ModEntry.Helper.WriteConfig(this);
        }
        
    }

    /// <summary>
    /// Class for the farm -- we have a list of them in the config
    /// </summary>
    class Farm
    {
        public string FarmName { get; set; } = "";
        public Dictionary<string, Farmer> FarmerInfo { get; set; } = new Dictionary<string, Farmer>() { };

        public Farmer GetFarmer(string FarmerName)
        {
            KeyValuePair<string, Farmer> Farmer = FarmerInfo.FirstOrDefault(x => x.Key == FarmerName);
            return Farmer.Value;
        }
    }

    /// <summary>
    /// Class for the farmer -- we have a list of them in the Farm object in the config
    /// </summary>
    class Farmer
    {
        /*****************************/
        /**      Properties         **/
        /*****************************/

        ///<summary>Determines whether or not to show the crib</summary>
        public bool ShowCrib { get; set; } = true;

        ///<summary>Determines whether or not to show the bed closest to the crib</summary>
        public bool ShowLeftBed { get; set; } = true;

        ///<summary>Determines whether or not to show the bed furthest from the crib</summary>
        public bool ShowRightBed { get; set; } = true;

        public void ToggleCrib() => ShowCrib = !ShowCrib;
        public void ToggleLeftBed() => ShowLeftBed = !ShowLeftBed;
        public void ToggleRightBed() => ShowRightBed = !ShowRightBed;
    }
}
