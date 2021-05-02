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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Framework.Configs
{
    public class GlobalMachineConfig
    {
        public bool doMachinesConsumeEnergy;

        public double solarPanelNonSunnyDayEnergyMultiplier;
        public double solarPanelNightEnergyGenerationMultiplier;
        public bool showMachineNotificationBubble_InventoryFull;
        public float machineNotificationBubbleAlpha;

        public int grinderEnergyConsumption;
        public int grinderTimeToGrind;

        public int miningDrillEnergyConsumption;
        public int miningDrillTimeToMine;

        public int steamBoilerV1_requiredWaterPerOperation;
        public int steamBoilerV1_producedSteamPerOperation;

        public int steamEngineV1_requiredSteamPerOperation;
        public int steamEngineV1_powerGeneratedPerOperation;

        public double windmill_windyDayPowerMultiplier;
        public int windmillV1_basePowerProduction;
        public int windmillV2_basePowerProduction;
        public GlobalMachineConfig()
        {
            this.doMachinesConsumeEnergy = true;
            this.solarPanelNonSunnyDayEnergyMultiplier = 0.0d;
            this.solarPanelNightEnergyGenerationMultiplier = .125d;
            this.showMachineNotificationBubble_InventoryFull = true;
            this.machineNotificationBubbleAlpha = 0.75f;
            this.grinderEnergyConsumption = 20;
            this.grinderTimeToGrind = 30;
            this.miningDrillEnergyConsumption = 50;
            this.miningDrillTimeToMine = 60;
            this.steamBoilerV1_requiredWaterPerOperation = 200;
            this.steamBoilerV1_producedSteamPerOperation = 100;

            this.steamEngineV1_requiredSteamPerOperation = 200;
            this.steamEngineV1_powerGeneratedPerOperation = 10;
            this.windmill_windyDayPowerMultiplier = 1.5d;
            this.windmillV1_basePowerProduction = 4;
            this.windmillV2_basePowerProduction = 12;
        }

        public static GlobalMachineConfig InitializeConfig()
        {
            if (File.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Configs", "MachinesConfig.json")))
                return ModCore.ModHelper.Data.ReadJsonFile<GlobalMachineConfig>(Path.Combine("Configs", "MachinesConfig.json"));
            else
            {
                GlobalMachineConfig Config = new GlobalMachineConfig();
                ModCore.ModHelper.Data.WriteJsonFile<GlobalMachineConfig>(Path.Combine("Configs", "MachinesConfig.json"), Config);
                return Config;
            }
        }
    }
}
