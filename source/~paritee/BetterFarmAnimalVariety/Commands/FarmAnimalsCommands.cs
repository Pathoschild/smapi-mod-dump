using BetterFarmAnimalVariety.Models;
using Paritee.StardewValleyAPI.Buildings;
using Paritee.StardewValleyAPI.Buildings.AnimalHouses;
using Paritee.StardewValleyAPI.FarmAnimals;
using Paritee.StardewValleyAPI.FarmAnimals.Variations;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using PariteeAnimalHouse = Paritee.StardewValleyAPI.Buildings.AnimalHouses.AnimalHouse;

namespace BetterFarmAnimalVariety.Commands
{
    class FarmAnimalsCommands : BaseCommands
    {
        private const string ANIMAL_SHOP_AVAILABLE = "yes";
        private const string ANIMAL_SHOP_UNAVAILABLE = "no";

        public FarmAnimalsCommands(ModConfig config, IModHelper helper, IMonitor monitor) : base(config, helper, monitor)
        {
            this.Commands = new List<Command>()
            {
                new Command("bfav_fa", "List all farm animal commands.\nUsage: bfav_fa", this.ListCommands),
                new Command("bfav_fa_list", "List the farm animal categories and types.\nUsage: bfav_fa_list", this.ListFarmAnimals),
                new Command("bfav_fa_reset", "Reset the farm animals in config.json to vanilla default.\nUsage: bfav_fa_reset", this.Reset),
                new Command("bfav_fa_addcategory", $"Add a unique category.\nUsage: bfav_fa_addcategory <category> <types> <buildings> <animalshop>\n- category: the unique animal category.\n- types: a comma separated string in quotes (ex \"White Cow,Brown Cow\").\n- buildings: a comma separated string in quotes (ex \"Barn,Deluxe Coop\").\n- animalshop: {FarmAnimalsCommands.ANIMAL_SHOP_AVAILABLE} or {FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE}.", this.AddCategory),
                new Command("bfav_fa_removecategory", "Remove an existing category.\nUsage: bfav_fa_removecategory <category>\n- category: the unique animal category.", this.RemoveCategory),
                new Command("bfav_fa_addtypes", "Add at least one animal type to a category.\nUsage: bfav_fa_addtypes <category> <types>\n- category: the unique animal category.\n- types: a comma separated string in quotes (ex \"White Cow,Brown Cow\").", this.AddTypes),
                new Command("bfav_fa_removetypes", "Remove at least one animal type to a category.\nUsage: bfav_fa_removetypes <category> <types>\n- category: the unique animal category.\n- types: a comma separated string in quotes (ex \"White Cow,Brown Cow\").", this.RemoveTypes),
                new Command("bfav_fa_setbuildings", "Set the category's buildings.\nUsage: bfav_fa_setbuildings <category> <buildings>\n- category: the unique animal category.\n- buildings: a comma separated string in quotes (ex \"Barn,Deluxe Coop\").", this.SetBuildings),
                new Command("bfav_fa_setshop", $"Set the availability of this category in the animal shop.\nUsage: bfav_fa_setshop <category> <animalshop>\n- category: the unique animal category.\n- animalshop: {FarmAnimalsCommands.ANIMAL_SHOP_AVAILABLE} or {FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE}.", this.SetAnimalShop),
                new Command("bfav_fa_setshopname", "Set the category's animal shop name.\nUsage: bfav_fa_setshopname <category> <name>\n- category: the unique animal category.\n- name: the displayed name.", this.SetAnimalShopName),
                new Command("bfav_fa_setshopdescription", "Set the category's animal shop description.\nUsage: bfav_fa_setshopdescription <category> <description>\n- category: the unique animal category.\n- description: the description.", this.SetAnimalShopDescription),
                new Command("bfav_fa_setshopprice", "Set the category's animal shop price.\nUsage: bfav_fa_setshopprice <category> <price>\n- category: the unique animal category.\n- price: the integer amount.", this.SetAnimalShopPrice),
                new Command("bfav_fa_setshopicon", "Set the category's animal shop icon.\nUsage: bfav_fa_setshopicon <category> <filename>\n- category: the unique animal category.\n- filename: the name of the file (ex. filename.png).", this.SetAnimalShopIcon),
                new Command("bfav_fa_fix", "Substitutes broken farm animal types from premature patch removal with vanilla versions.\nUsage: bfav_fa_fix <savefile>\n- savefile: save file name (optional)", this.Fix),
            };
        }

        /// <summary>Substitutes broken farm animal types from premature patch removal with vanilla versions.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void Fix(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 1)
            {
                this.Monitor.Log($"incorrect arguments", LogLevel.Error);
                return;
            }

            // Need to manually parse the XML since casting to a FarmAnimal 
            // triggers the data search crash that this command aims to avoid
            if (!Directory.Exists(Constants.SavesPath))
            {
                this.Monitor.Log($"cannot find saves path directory", LogLevel.Error);
                return;
            }

            if (args.Length.Equals(0))
            {
                string[] saveFolders = Directory.GetDirectories(Constants.SavesPath);

                foreach (string saveFolder in saveFolders)
                {
                    // Scan only the most recent save if it can be found
                    this.FixSave(saveFolder);
                }
            }
            else
            {
                this.FixSave(Path.Combine(Constants.SavesPath, args[0]));
            }
        }

        private void FixSave(string saveFolder)
        {
            string saveFile = Path.Combine(saveFolder, Path.GetFileName(saveFolder));

            if (!File.Exists(saveFile))
            {
                this.Monitor.Log($"{saveFile} does not exist", LogLevel.Error);
                return;
            }

            // Baseline
            FarmAnimalsData data = new FarmAnimalsData();

            WhiteVariation whiteVariation = new WhiteVariation();
            string coopDwellerSubstitute = whiteVariation.ApplyPrefix(Paritee.StardewValleyAPI.FarmAnimals.Type.Base.Chicken.ToString());
            string barnDwellerSubstitute = whiteVariation.ApplyPrefix(Paritee.StardewValleyAPI.FarmAnimals.Type.Base.Cow.ToString());

            this.Monitor.Log($"Searching {saveFolder} for problematic farm animal types", LogLevel.Trace);

            // Replace barn animals with White Cows and coop animals with White Chickens
            XmlDocument doc = new XmlDocument();

            // Track the types to be substituted
            List<string> typesToBeSubstituted = new List<string>();

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            // Load the xml
            doc.Load(saveFile);

            XmlNodeList buildings = doc.SelectNodes("//GameLocation[@xsi:type='Farm']/buildings/Building[@xsi:type='Barn' or @xsi:type='Coop']", namespaceManager);

            this.Monitor.Log($"- Checking {buildings.Count} buildings", LogLevel.Trace);

            // Go through each building
            for (int i = 0; i < buildings.Count; i++)
            {
                XmlNode building = buildings[i];

                bool isCoop = building.Attributes["xsi:type"].Value.Equals("Coop");

                // Grab only the animal types
                XmlNodeList types = building.SelectNodes(".//FarmAnimal/type");

                for (int k = 0; k < types.Count; k++)
                {
                    XmlNode type = types[k];

                    // If the type can't be found in the data entries
                    // then substitute it with an appropriate basic animal
                    if (!data.GetEntries().ContainsKey(type.InnerText))
                    {
                        typesToBeSubstituted.Add(type.InnerText);

                        type.InnerText = isCoop ? coopDwellerSubstitute : barnDwellerSubstitute;
                    }
                }
            }

            if (typesToBeSubstituted.Count > 0)
            {
                // save the XmlDocument back to disk
                doc.Save(saveFile);

                this.Monitor.Log($"- Converted {typesToBeSubstituted.Count} farm animals to White Cows or White Chickens: {String.Join(", ", typesToBeSubstituted.Distinct())}", LogLevel.Trace);
            }
            else
            {
                this.Monitor.Log($"- No problematic farm animals found", LogLevel.Trace);
            }
        }

        /// <summary>List the farm animal categories and types when the 'bfav_fa_list' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void ListFarmAnimals(string command, string[] args)
        {
            string output = "Listing BFAV farm animals\n";

            foreach (KeyValuePair<string, ConfigFarmAnimal> entry in this.Config.FarmAnimals)
            {
                output += this.DescribeFarmAnimalCategory(entry);
            }

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Reset the farm animals in config.json to vanilla default when the 'bfav_fa_reset' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void Reset(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            ModConfig config = new ModConfig();

            this.Config.FarmAnimals = config.FarmAnimals;

            this.HandleUpdatedConfig();

            this.Helper.ConsoleCommands.Trigger("bfav_fa_list", new string[] { });
        }

        /// <summary>Add a unique category when the 'bfav_fa_addcategory' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void AddCategory(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 4)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} already exists in config.json", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"type is required", LogLevel.Error);
                return;
            }

            List<string> types = args[1].Split(',').Select(i => i.Trim()).ToList();
            FarmAnimalsData farmAnimalsData = new FarmAnimalsData();

            // Check if these new types are valid
            foreach (string key in types)
            {
                if (!farmAnimalsData.Exists(key))
                {
                    this.Monitor.Log($"{key} does not exist in Data/FarmAnimals", LogLevel.Error);
                    return;
                }
            }

            string building = args.Length < 3 ? Barn.BARN : args[2].Trim();
            List<string> buildings = new List<string>();

            if (building.ToLower().Equals(Coop.COOP.ToLower()))
            {
                foreach (PariteeAnimalHouse.Size size in Enum.GetValues(typeof(Coop.Size)))
                {
                    buildings.Add(PariteeAnimalHouse.FormatBuilding(Coop.COOP, size));
                }
            }
            else if (building.ToLower().Equals(Barn.BARN.ToLower()))
            {
                foreach (PariteeAnimalHouse.Size size in Enum.GetValues(typeof(Barn.Size)))
                {
                    buildings.Add(PariteeAnimalHouse.FormatBuilding(Barn.BARN, size));
                }
            }
            else
            {
                buildings = building.Split(',').Select(i => i.Trim()).ToList();

                BlueprintsData blueprintsData = new BlueprintsData();

                // Check if these new types are valid
                foreach (string key in buildings)
                {
                    if (!blueprintsData.Exists(key))
                    {
                        this.Monitor.Log($"{key} does not exist in Data/Blueprints", LogLevel.Error);
                        return;
                    }
                }
            }

            string animalShop = args.Length < 4 ? FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE : args[3].Trim().ToLower();

            if (!animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_AVAILABLE) && !animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE))
            {
                this.Monitor.Log($"animalshop must be yes or no", LogLevel.Error);
                return;
            }

            ConfigFarmAnimalAnimalShop configFarmAnimalAnimalShop;

            try
            {
                configFarmAnimalAnimalShop = this.GetAnimalShopConfig(category, animalShop);
            }
            catch
            {
                // Messaging handled in function
                return;
            }

            this.Config.FarmAnimals.Add(category, new ConfigFarmAnimal());

            this.Config.FarmAnimals[category].Category = category;
            this.Config.FarmAnimals[category].Types = types.Distinct().ToArray();
            this.Config.FarmAnimals[category].Buildings = buildings.ToArray();
            this.Config.FarmAnimals[category].AnimalShop = configFarmAnimalAnimalShop;

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Remove an existing category when the 'bfav_fa_removecategory' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void RemoveCategory(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 1)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0];

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} is not a category in config.json", LogLevel.Error);
                return;
            }

            this.Config.FarmAnimals.Remove(category);

            this.HandleUpdatedConfig();

            this.Helper.ConsoleCommands.Trigger("bfav_fa_list", new string[] { });
        }

        /// <summary>Add at least one animal type to a category when the 'bfav_fa_addtypes' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>ary>
        private void AddTypes(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} is not a category in config.json", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"type is required", LogLevel.Error);
                return;
            }

            List<string> types = new List<string>(this.Config.FarmAnimals[category].Types);
            List<string> newTypes = args[1].Split(',').Select(i => i.Trim()).ToList();
            FarmAnimalsData farmAnimalsData = new FarmAnimalsData();

            // Check if these new types are valid
            foreach (string newType in newTypes)
            {
                if (!farmAnimalsData.Exists(newType))
                {
                    this.Monitor.Log($"{newType} does not exist in Data/FarmAnimals", LogLevel.Error);
                    return;
                }
            }

            this.Config.FarmAnimals[category].Types = types.Concat(newTypes).Distinct().ToArray();

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Remove at least one animal type to a category when the 'bfav_fa_removetypes' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void RemoveTypes(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} is not a category in config.json", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"type is required", LogLevel.Error);
                return;
            }

            List<string> types = new List<string>(this.Config.FarmAnimals[category].Types);
            List<string> typesToBeRemoved = args[1].Split(',').Select(i => i.Trim()).ToList();
            string[] newTypes = types.Except(typesToBeRemoved).ToArray();

            if (newTypes.Length < 1)
            {
                this.Monitor.Log($"categories must contain at least one type", LogLevel.Error);
                return;
            }

            this.Config.FarmAnimals[category].Types = newTypes;

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Set the category's buildings when the 'bfav_fa_setbuildings' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void SetBuildings(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} does not exist in config.json", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"buildings is required", LogLevel.Error);
                return;
            }

            List<string> buildings = args[1].Split(',').Select(i => i.Trim()).ToList();

            BlueprintsData blueprintsData = new BlueprintsData();

            // Check if these new types are valid
            foreach (string key in buildings)
            {
                if (!blueprintsData.Exists(key))
                {
                    this.Monitor.Log($"{key} does not exist in Data/Blueprints", LogLevel.Error);
                    return;
                }
            }

            this.Config.FarmAnimals[category].Buildings = buildings.ToArray();

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Set the availability of this category in the animal shop 'bfav_fa_setshop' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void SetAnimalShop(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} does not exist in config.json", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"animalshop is required", LogLevel.Error);
                return;
            }

            string animalShop = args[1].Trim().ToLower();

            if (!animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_AVAILABLE) && !animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE))
            {
                this.Monitor.Log($"animalshop must be yes or no", LogLevel.Error);
                return;
            }

            if (animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_AVAILABLE) && this.Config.FarmAnimals[category].CanBePurchased())
            {
                this.Monitor.Log($"{category} is already available in the animal shop", LogLevel.Error);
                return;
            }
            else if (animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE) && !this.Config.FarmAnimals[category].CanBePurchased())
            {
                this.Monitor.Log($"{category} is already not available in the animal shop", LogLevel.Error);
                return;
            }

            ConfigFarmAnimalAnimalShop configFarmAnimalAnimalShop;

            try
            {
                configFarmAnimalAnimalShop = this.GetAnimalShopConfig(category, animalShop);
            }
            catch
            {
                // Messaging handled in function
                return;
            }

            this.Config.FarmAnimals[category].AnimalShop = configFarmAnimalAnimalShop;

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Set the category's animal shop name when the 'bfav_fa_setshopname' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void SetAnimalShopName(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} does not exist in config.json", LogLevel.Error);
                return;
            }

            if (!this.Config.FarmAnimals[category].CanBePurchased())
            {
                this.Monitor.Log($"{category} is not available in the animal shop", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"name is required", LogLevel.Error);
                return;
            }

            this.Config.FarmAnimals[category].AnimalShop.Name = args[1].Trim();

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Set the category's animal shop description when the 'bfav_fa_setshopdescription' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void SetAnimalShopDescription(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} does not exist in config.json", LogLevel.Error);
                return;
            }

            if (!this.Config.FarmAnimals[category].CanBePurchased())
            {
                this.Monitor.Log($"{category} is not available in the animal shop", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"description is required", LogLevel.Error);
                return;
            }

            this.Config.FarmAnimals[category].AnimalShop.Description = args[1].Trim();

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Set the category's animal shop price when the 'bfav_fa_setshopprice' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void SetAnimalShopPrice(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} does not exist in config.json", LogLevel.Error);
                return;
            }

            if (!this.Config.FarmAnimals[category].CanBePurchased())
            {
                this.Monitor.Log($"{category} is not available in the animal shop", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"price is required", LogLevel.Error);
                return;
            }


            if (!int.TryParse(args[1], out int n))
            {
                this.Monitor.Log($"price must be a positive number", LogLevel.Error);
                return;
            }

            if (n < 0)
            {
                this.Monitor.Log($"price must be a positive number", LogLevel.Error);
                return;
            }

            this.Config.FarmAnimals[category].AnimalShop.Price = args[1].Trim();

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        /// <summary>Set the category's animal shop icon when the 'bfav_fa_setshopicon' command is invoked.</summary>
        /// <param name="command">The name of the command invoked.</param>
        /// <param name="args">The arguments received by the command. Each word after the command name is a separate argument.</param>
        private void SetAnimalShopIcon(string command, string[] args)
        {
            if (Game1.hasLoadedGame)
            {
                this.Monitor.Log($"this cannot be done after loading a save");
                return;
            }

            if (args.Length > 2)
            {
                this.Monitor.Log($"use quotation marks (\") around your text if you are using spaces", LogLevel.Error);
                return;
            }

            if (args.Length < 1)
            {
                this.Monitor.Log($"category is required", LogLevel.Error);
                return;
            }

            string category = args[0].Trim();

            if (!this.Config.FarmAnimals.ContainsKey(category))
            {
                this.Monitor.Log($"{category} does not exist in config.json", LogLevel.Error);
                return;
            }

            if (!this.Config.FarmAnimals[category].CanBePurchased())
            {
                this.Monitor.Log($"{category} is not available in the animal shop", LogLevel.Error);
                return;
            }

            if (args.Length < 2)
            {
                this.Monitor.Log($"icon is required", LogLevel.Error);
                return;
            }

            string filename = Path.GetFileName(args[1]);
            string pathToIcon = Path.Combine(Properties.Settings.Default.AssetsDirectory, filename);
            string fullPathToIcon = Path.Combine(this.Helper.DirectoryPath, pathToIcon);

            if (!File.Exists(fullPathToIcon))
            {
                this.Monitor.Log($"{fullPathToIcon} does not exist", LogLevel.Error);
                return;
            }

            if (!Path.GetExtension(fullPathToIcon).ToLower().Equals(".png"))
            {
                this.Monitor.Log($"{filename} must be a .png", LogLevel.Error);
                return;
            }

            this.Config.FarmAnimals[category].AnimalShop.Icon = args[1].Trim();

            this.HandleUpdatedConfig();

            string output = this.DescribeFarmAnimalCategory(new KeyValuePair<string, ConfigFarmAnimal>(category, this.Config.FarmAnimals[category]));

            this.Monitor.Log(output, LogLevel.Info);
        }

        private string DescribeFarmAnimalCategory(KeyValuePair<string, ConfigFarmAnimal> entry)
        {
            string output = "";

            output += $"{entry.Key}\n";
            output += $"- Types: {String.Join(",", entry.Value.Types)}\n";
            output += $"- Buildings: {String.Join(",", entry.Value.Buildings)}\n";
            output += $"- AnimalShop:\n";
            output += $"-- Name: {entry.Value.AnimalShop.Name}\n";
            output += $"-- Description: {entry.Value.AnimalShop.Description}\n";
            output += $"-- Price: {entry.Value.AnimalShop.Price}\n";
            output += $"-- Icon: {entry.Value.AnimalShop.Icon}\n";

            return output;
        }

        private ConfigFarmAnimalAnimalShop GetAnimalShopConfig(string category, string animalShop)
        {
            ConfigFarmAnimalAnimalShop configFarmAnimalAnimalShop = new ConfigFarmAnimalAnimalShop();

            if (animalShop.Equals(FarmAnimalsCommands.ANIMAL_SHOP_UNAVAILABLE))
            {
                return configFarmAnimalAnimalShop;
            }

            configFarmAnimalAnimalShop.Category = category;
            configFarmAnimalAnimalShop.Name = category;
            configFarmAnimalAnimalShop.Description = configFarmAnimalAnimalShop.GetDescriptionPlaceholder();
            configFarmAnimalAnimalShop.Price = ConfigFarmAnimalAnimalShop.PRICE_PLACEHOLDER;
            configFarmAnimalAnimalShop.Icon = configFarmAnimalAnimalShop.GetDefaultIconPath();

            string fullPathToIcon = Path.Combine(this.Helper.DirectoryPath, configFarmAnimalAnimalShop.Icon);

            if (!File.Exists(fullPathToIcon))
            {
                this.Monitor.Log($"{fullPathToIcon} does not exist", LogLevel.Error);
                throw new Exception();
            }

            return configFarmAnimalAnimalShop;
        }
    }
}
