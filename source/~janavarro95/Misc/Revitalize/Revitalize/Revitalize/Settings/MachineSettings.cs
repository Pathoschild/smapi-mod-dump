using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Revitalize;
using StardewModdingAPI;

namespace Revitalize.Settings
{
    class MachineSettings : SettingsInterface
    {
        public bool doMachinesConsumePower;

        public  MachineSettings()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes defaults for this setting.
        /// </summary>
        public void Initialize()
        {
            doMachinesConsumePower = true;
        }

        /// <summary>
        /// Loads the settings for this module.
        /// </summary>
        public void LoadSettings()
        {
            try
            {
               SettingsManager.machineSettings= Serialize.ReadFromJsonFile<MachineSettings>(Path.Combine(Serialize.SettingsPath, "MachineSettings" + ".json"));
            }
            catch (Exception e)
            {
                ////Log.AsyncR("Failed to load Machine Settings"); 
            }
        }

        /// <summary>
        /// Saves the settings for this module. 
        /// </summary>
        public void SaveSettings()
        {
            if (File.Exists(Path.Combine(Serialize.SettingsPath, "MachineSettings" + ".json"))) File.Delete(Path.Combine(Serialize.SettingsPath, "MachineSettings" + ".json"));
            Serialize.WriteToJsonFile(Path.Combine(Serialize.SettingsPath, "MachineSettings" + ".json"), (MachineSettings)SettingsManager.machineSettings);
        }
    }
}
