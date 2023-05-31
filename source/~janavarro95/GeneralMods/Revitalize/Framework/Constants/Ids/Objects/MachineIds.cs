/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegasis.Revitalize.Framework.Constants.Ids.Objects
{
    public static class MachineIds
    {
        /// <summary>
        /// Prefix for machine ids.
        /// </summary>
        public const string PREFIX = "Omegasis.Revitalize.Objects.Machines."; //Change this to start with Omegasis.

        //Mining Drills.
        public const string CoalMiningDrill = PREFIX + "CoalMiningDrill";
        public const string ElectricMiningDrill = PREFIX + "ElectricMiningDrill";
        public const string NuclearMiningDrill = PREFIX + "NuclearMiningDrill";
        public const string MagicalMiningDrill = PREFIX + "MagicalMiningDrill";
        public const string GalaxyMiningDrill = PREFIX + "GalaxyMiningDrill";


        //Geode Crushers.
        public const string AdvancedBurnerGeodeCrusher = PREFIX + "GeodeCrushers.CoalAdvancedGeodeCrusher";
        public const string ElectricAdvancedGeodeCrusher = PREFIX + "GeodeCrushers.ElectricAdvancedGeodeCrusher";
        public const string NuclearAdvancedGeodeCrusher = PREFIX + "GeodeCrushers.NuclearAdvancedGeodeCrusher";
        public const string MagicalAdvancedGeodeCrusher = PREFIX + "GeodeCrushers.MagicalAdvancedGeodeCrusher";
        public const string GalaxyAdvancedGeodeCrusher = PREFIX + "GeodeCrushers.GalaxyAdvancedGeodeCrusher";
        public const string Anvil = CraftingStations.Anvil_Id;


        //Charcoal Kilns
        public const string AdvancedCharcoalKiln = PREFIX + "CharcoalKilns.AdvancedCharcoalKiln";
        public const string DeluxeCharcoalKiln = PREFIX + "CharcoalKilns.DeluxeCharcoalKiln";
        public const string SuperiorCharcoalKiln = PREFIX + "CharcoalKilns.SuperiorCharcoalKiln";

        //Furnaces
        public const string ElectricFurnace = PREFIX + "ElectricFurnace";
        public const string NuclearFurnace = PREFIX + "NuclearFurnace";
        public const string MagicalFurnace = PREFIX + "MagicalFurnace";

        //Generated code below this point.
        public const string BurnerGenerator = PREFIX + "EnergyGeneration.BurnerGenerator";
        public const string AdvancedGenerator = "Omegasis.Revitalize.Objects.Machines.EnergyGeneration.AdvancedGenerator";
        public const string NuclearGenerator = "Omegasis.Revitalize.Objects.Machines.EnergyGeneration.NuclearGenerator";
		public const string AdvancedSolarPanel = "Omegasis.Revitalize.Objects.Machines.EnergyGeneration.AdvancedSolarPanel";
		public const string SuperiorSolarPanel = "Omegasis.Revitalize.Objects.Machines.EnergyGeneration.SuperiorSolarPanel";
		public const string CrystalRefiner = "Omegasis.Revitalize.Objects.Machines.CrystalRefiner";
		public const string Windmill = "Omegasis.Revitalize.Objects.EnergyGeneration.Windmill";
    }
}
