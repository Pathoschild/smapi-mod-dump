using BetterFarmAnimalVariety.Models;
using Paritee.StardewValleyAPI.Buildings.AnimalShop.FarmAnimals;
using Paritee.StardewValleyAPI.FarmAnimals;
using Paritee.StardewValleyAPI.FarmAnimals.Variations;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterFarmAnimalVariety
{
    public class ModConfig
    {
        public string Format;
        public bool IsEnabled;
        public VoidConfig.InShop VoidFarmAnimalsInShop;
        public bool RandomizeNewbornFromCategory;
        public bool RandomizeHatchlingFromCategory;
        public bool IgnoreParentProduceCheck;
        public Dictionary<string, ConfigFarmAnimal> FarmAnimals;

        private readonly AppSettings AppSettings;

        public ModConfig()
        {
            Dictionary<string, string> settings = Properties.Settings.Default.Properties
              .Cast<System.Configuration.SettingsProperty>()
              .ToDictionary(x => x.Name.ToString(), x => x.DefaultValue.ToString());

            this.AppSettings = new AppSettings(settings);
            this.Format = null;
            this.IsEnabled = true;
            this.VoidFarmAnimalsInShop = VoidConfig.InShop.Never;
            this.RandomizeNewbornFromCategory = false;
            this.RandomizeHatchlingFromCategory = false;
            this.IgnoreParentProduceCheck = false;

            this.InitializeFarmAnimals();
        }
        
        public bool IsValidFormat(string targetFormat)
        {
            return this.Format != null && this.Format.Equals(targetFormat);
        }

        private List<string> GetFarmAnimalGroups()
        {
            return this.FarmAnimals.Keys.ToList<string>();
        }

        public Dictionary<string, List<string>> GetFarmAnimalTypes()
        {
            Dictionary<string, List<string>> farmAnimals = new Dictionary<string, List<string>>();

            foreach (KeyValuePair<string, ConfigFarmAnimal> entry in this.FarmAnimals)
            {
                farmAnimals.Add(entry.Key, new List<string>(entry.Value.Types));
            }

            return farmAnimals;
        }

        public List<string> GetFarmAnimalTypes(string category)
        {
            return this.FarmAnimals[category].GetTypes().ToList<string>();
        }

        public void InitializeFarmAnimals()
        {
            if (this.FarmAnimals == null)
            {
                this.FarmAnimals = new Dictionary<string, ConfigFarmAnimal>();
                this.UpdateFarmAnimalValuesFromAppSettings();
            }
            else
            {
                // Need to restore the categories because they are not in the config.json
                foreach(KeyValuePair<string, ConfigFarmAnimal> entry in this.FarmAnimals.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
                {
                    entry.Value.Category = entry.Key;
                    this.FarmAnimals[entry.Key] = entry.Value;
                }
            }
        }

        public void UpdateFarmAnimalValuesFromAppSettings()
        {
            List<AppSetting> appSettings = this.AppSettings.FindFarmAnimalAppSettings();

            foreach (AppSetting appSetting in appSettings)
            {
                ConfigFarmAnimal configFarmAnimal = new ConfigFarmAnimal(appSetting);
                  
                if (this.FarmAnimals.ContainsKey(configFarmAnimal.Category))
                {
                    // Preserve user preferences if the group already has data loaded from the user's config JSON
                    ConfigFarmAnimal custom = this.FarmAnimals[configFarmAnimal.Category];

                    configFarmAnimal.AnimalShop.Name = custom.AnimalShop.Name;
                    configFarmAnimal.AnimalShop.Description = custom.AnimalShop.Description;
                    configFarmAnimal.AnimalShop.Price = custom.AnimalShop.Price;
                    configFarmAnimal.AnimalShop.Icon = custom.AnimalShop.Icon;
                    configFarmAnimal.Types = custom.Types;
                    configFarmAnimal.Buildings = custom.Buildings;

                    // Add the configuration to the farm animals config
                    this.FarmAnimals[configFarmAnimal.Category] = configFarmAnimal;
                }
                else
                {
                    this.FarmAnimals.Add(configFarmAnimal.Category, configFarmAnimal);
                }
            }
        }

        public List<FarmAnimalForPurchase> GetFarmAnimalsForPurchase(Farm farm)
        {
            List<FarmAnimalForPurchase> purchaseAnimalStock = new List<FarmAnimalForPurchase>();
            FarmAnimalsData farmAnimalsData = new FarmAnimalsData();
            Dictionary<string, string> farmAnimalsDataEntries = farmAnimalsData.GetEntries();

            foreach (KeyValuePair<string, ConfigFarmAnimal> entry in this.FarmAnimals)
            {
                if (!entry.Value.CanBePurchased())
                {
                    continue;
                }

                string name = entry.Value.Category;
                string displayName = entry.Value.AnimalShop.Name;
                string description = entry.Value.AnimalShop.Description;
                int price = Convert.ToInt32(entry.Value.AnimalShop.Price) / 2; // Divide by two because of the weird functionality in Object.salePrice()
                List<string> farmAnimalTypes = this.GetFarmAnimalTypes(name);
                List<string> buildingsILiveIn = new List<string>(entry.Value.Buildings);

                FarmAnimalForPurchase farmAnimalForPurchase = new FarmAnimalForPurchase(name, displayName, description, price, buildingsILiveIn, farmAnimalTypes);

                purchaseAnimalStock.Add(farmAnimalForPurchase);
            }

            return purchaseAnimalStock;
        }
    }
}