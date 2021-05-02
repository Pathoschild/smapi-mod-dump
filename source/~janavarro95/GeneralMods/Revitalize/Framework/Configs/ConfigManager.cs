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

namespace Revitalize.Framework.Configs
{
    /// <summary>
    /// Handles holding all of the config information.
    /// </summary>
    public class ConfigManager
    {
        /// <summary>
        /// The config file for vanilla machine recipes.
        /// </summary>
        public VanillaMachineRecipeConfig vanillaMachineConfig;

        public Shops_BlacksmithConfig shops_blacksmithConfig;
        public FurnitureConfig furnitureConfig;
        public ObjectsConfig objectsConfig;

        /// <summary>
        /// The config file to be used for machines.
        /// </summary>
        public GlobalMachineConfig machinesConfig;

        public MiningDrillConfig miningDrillConfig;

        public ConfigManager()
        {
            this.vanillaMachineConfig = VanillaMachineRecipeConfig.InitializeConfig();
            this.shops_blacksmithConfig = Shops_BlacksmithConfig.InitializeConfig();
            this.furnitureConfig = FurnitureConfig.InitializeConfig();
            this.machinesConfig = GlobalMachineConfig.InitializeConfig();
            this.objectsConfig = ObjectsConfig.InitializeConfig();
            this.miningDrillConfig = MiningDrillConfig.InitializeConfig();
        }
    }
}
