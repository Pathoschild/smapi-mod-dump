/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BfavToFavrModConverter.Bfav;
using BfavToFavrModConverter.Favr;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Action = BfavToFavrModConverter.Favr.Action;

namespace BfavToFavrModConverter
{
    /// <summary>The application entry point.</summary>
    public class Program
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>The application entry point.</summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length < 2)
                {
                    Logger.WriteLine("Invalid args supplied", ConsoleColor.Red);
                    Logger.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }

                // the folder that contains the bfav mod folders
                var sourceFolder = Path.GetFullPath(args[0]);
                // the folder that will contain the converted favr mod folders
                var destinationFolder = Path.GetFullPath(args[1]);

                if (!Directory.Exists(sourceFolder))
                    Directory.CreateDirectory(sourceFolder);
                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                var modDirectorties = Directory.GetDirectories(sourceFolder);
                foreach (var modDirectory in modDirectorties)
                {
                    var bfavModFolderName = GetDirectoryRootName(modDirectory);
                    var favrModFolderName = bfavModFolderName.Replace("BFAV", "FAVR");

                    Logger.WriteLine($"Converting {GetDirectoryRootName(modDirectory)} to {favrModFolderName}");

                    ConvertBFAVModFolder(modDirectory, Path.Combine(destinationFolder, favrModFolderName));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Converter failed and couldn't recover: {ex}", ConsoleColor.Red);
            }

            Logger.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /*********
        ** Private Methods
        *********/
        /// <summary>Connvert the given BFAV mod folder into a FAVR mod folder.</summary>
        /// <param name="bfavFolderPath">The path to the BFAV mod folder to convert.</param>
        /// <param name="destinationFavrFolderPath">The path where the converted FAVR mod folder will go.</param>
        private static void ConvertBFAVModFolder(string bfavFolderPath, string destinationFavrFolderPath)
        {
            // deserialize, convert, and serialize manifest.json
            Logger.WriteLine("Converting manifest.json file", ConsoleColor.Gray);
            var bfavManifest = DeserializeJsonFile<ModManifest>(Path.Combine(bfavFolderPath, "manifest.json"));
            if (bfavManifest == null)
            {
                Logger.WriteLine("Couldn't find BFAV manifest.json, skipping", ConsoleColor.Red);
                return;
            }

            var favrManifest = ConvertBfavManifest(bfavManifest);
            SerializeObjectToJson(favrManifest, Path.Combine(destinationFavrFolderPath, "manifest.json"));

            // deserialize, convert, and serialize content.json files
            var bfavContent = DeserializeJsonFile<BfavContent>(Path.Combine(bfavFolderPath, "content.json"));
            if (bfavContent == null)
                return;
            var favrAnimals = ConvertBfavContent(bfavContent);
            foreach (var favrAnimal in favrAnimals)
            {
                Logger.WriteLine($"Creating content.json file for animal: {favrAnimal.Name}", ConsoleColor.Gray);
                SerializeObjectToJson(favrAnimal, Path.Combine(destinationFavrFolderPath, favrAnimal.Name, "content.json"));
            }

            // copy over sprite sheets
            foreach (var bfavCategory in bfavContent.Categories)
            {
                var favrContent = favrAnimals.Where(animal => animal.Name == (bfavCategory.AnimalShop?.Name ?? bfavCategory.Category)).FirstOrDefault();
                if (favrContent == null)
                {
                    Logger.WriteLine($"Couldn't find FAVR animal related to BFAV animal: {bfavCategory.Category}. Manual conversion required.", ConsoleColor.Red);
                    continue;
                }

                Logger.WriteLine($"Converting sprite sheets for animal: {favrContent.Name}", ConsoleColor.Gray);
                MoveSpriteSheets(bfavFolderPath, destinationFavrFolderPath, bfavCategory, favrContent);
            }
        }

        /// <summary>Convert the given <see cref="ModManifest"/> from the BFAV version to the FAVR version.</summary>
        /// <param name="bfavManifest">The <see cref="ModManifest"/> to convert.</param>
        /// <returns>The converted pased <see cref="ModManifest"/>.</returns>
        private static ModManifest ConvertBfavManifest(ModManifest bfavManifest)
        {
            bfavManifest.ContentPackFor["UniqueID"] = "Satozaki.FarmAnimalVarietyRedux";
            if (bfavManifest.ContentPackFor.ContainsKey("MinimumVersion"))
                bfavManifest.ContentPackFor.Remove("MinimumVersion");

            return new ModManifest(
                name: bfavManifest.Name.Replace("BFAV", "FAVR"),
                author: bfavManifest.Author,
                version: bfavManifest.Version,
                description: bfavManifest.Description,
                uniqueId: bfavManifest.UniqueId,
                updateKeys: bfavManifest.UpdateKeys,
                contentPackFor: bfavManifest.ContentPackFor
            );
        }

        /// <summary>Convert the given <see cref="BfavContent"/> to <see cref="FavrContent"/>.</summary>
        /// <param name="bfavContent">The <see cref="BfavContent"/> to convert.</param>
        /// <returns>The converted passed <see cref="BfavContent"/>.</returns>
        private static List<FavrCustomAnimal> ConvertBfavContent(BfavContent bfavContent)
        {
            var favrAnimals = new List<FavrCustomAnimal>();

            foreach (var category in bfavContent.Categories)
            {
                Logger.WriteLine($"Converting animal: {category.AnimalShop?.Name ?? category.Category}", ConsoleColor.Gray);

                // create the animal subtypes
                var subtypes = new List<FavrCustomAnimalType>();
                foreach (var type in category.Types)
                {
                    var splitDataString = type.Data.Split('/');

                    var productId = splitDataString[2];
                    var deluxeProductId = splitDataString[3];
                    var meatId = splitDataString[23];

                    // check if the (deluxe|meat)product ids are valid ids, or it they should have api tokens added to them
                    if (!int.TryParse(productId, out _))
                        productId = $"spacechase0.JsonAssets:GetObjectId:{productId}";
                    if (!int.TryParse(deluxeProductId, out _))
                        deluxeProductId = $"spacechase0.JsonAssets:GetObjectId:{deluxeProductId}";
                    if (!int.TryParse(meatId, out _))
                        meatId = $"spacechase0.JsonAssets:GetObjectId:{meatId}";

                    // convert produce
                    var toolName = splitDataString[22];

                    // determine tool harvest sound as FAVR requires packs to be explicit
                    string toolHarvestSound = null;
                    if (toolName.ToLower() == "shears")
                        toolHarvestSound = "scissors";
                    else if (toolName.ToLower() == "milk pail")
                        toolHarvestSound = "Milking";

                    var produce = new FavrAnimalProduce(defaultProductId: productId, upgradedProductId: deluxeProductId, harvestType: (HarvestType)Convert.ToInt32(splitDataString[13]), toolHarvestSound: toolHarvestSound, daysToProduce: Convert.ToInt32(splitDataString[0]), toolName: toolName);

                    subtypes.Add(new FavrCustomAnimalType(
                        action: Action.Add,
                        name: type.Type,
                        isBuyable: true,
                        isIncubatable: true,
                        produce: new List<FavrAnimalProduce> { produce },
                        daysTillMature: Convert.ToByte(splitDataString[1]),
                        soundId: splitDataString[4],
                        frontAndBackSpriteWidth: Convert.ToInt32(splitDataString[16]),
                        frontAndBackSpriteHeight: Convert.ToInt32(splitDataString[17]),
                        sideSpriteWidth: Convert.ToInt32(splitDataString[18]),
                        sideSpriteHeight: Convert.ToInt32(splitDataString[19]),
                        meatId: meatId,
                        happinessDrain: Convert.ToByte(splitDataString[21])
                    ));
                }

                var action = Action.Add;
                bool? buyable = category.AnimalShop != null;
                if (category.Action.ToLower() == "update")
                {
                    Logger.WriteLine("Manual audit required, when an animal has an action of 'Update' the internal name needs to be specified in the FAVR pack", ConsoleColor.Red);
                    action = Action.Edit;
                    buyable = null; // inherit buyablility from the pack who added the animal
                }

                favrAnimals.Add(new FavrCustomAnimal(
                    action: action,
                    internalName: action == Action.Add ? null : "input animal internal name here",
                    name: category.AnimalShop?.Name ?? category.Category,
                    buyable: (buyable.HasValue && !buyable.Value) ? false : null, // turn 'true' into null (so it gets ommited from the final favr file as that's the default value)
                    canSwim: false,
                    animalShopInfo: new FavrAnimalShopInfo(
                        description: category.AnimalShop?.Description,
                        buyPrice: category.AnimalShop?.Price ?? 0),
                    types: subtypes,
                    buildings: category.Buildings
                ));
            }

            return favrAnimals;
        }

        /// <summary>Move and rename all the spritesheets to the correct format.</summary>
        /// <param name="bfavFolderPath">The root folder path of the bfav mod being converted.</param>
        /// <param name="destinationFavrFolderPath">The destination folder path where the favr mod will be converted to.</param>
        /// <param name="bfavCategory">The animal whose sprite sheets to convert.</param>
        /// <param name="favrContent">The favrContent corrosponding to the passed bfavCategory.</param>
        private static void MoveSpriteSheets(string bfavFolderPath, string destinationFavrFolderPath, BfavCategory bfavCategory, FavrCustomAnimal favrContent)
        {
            try
            {
                // create asset folder in destination folder
                var favrAssetsFolder = Path.Combine(destinationFavrFolderPath, favrContent.Name, "assets");
                if (!Directory.Exists(favrAssetsFolder))
                    Directory.CreateDirectory(favrAssetsFolder);

                // shop icon
                if (bfavCategory.AnimalShop != null)
                {
                    File.Copy(
                        sourceFileName: Path.Combine(bfavFolderPath, bfavCategory.AnimalShop.Icon),
                        destFileName: Path.Combine(favrAssetsFolder, "..\\", "shopdisplay.png"),
                        overwrite: true
                    );
                }

                foreach (var subtype in bfavCategory.Types)
                {
                    // adult sprite sheet
                    var adultSpriteSheetPath = Path.Combine(bfavFolderPath, subtype.Sprites.Adult);

                    // baby sprite sheet
                    var babySpriteSheetPath = "";
                    if (!string.IsNullOrEmpty(subtype.Sprites.Baby) && subtype.Sprites.Baby.ToLower() != subtype.Sprites.Adult.ToLower()) // ensure it's not the same as the adult, some mods use the same sprite sheet. this would create duplicates of the sprite sheet otherwise
                        babySpriteSheetPath = Path.Combine(bfavFolderPath, subtype.Sprites.Baby);

                    // harvested sprite sheet
                    var harvestedSpriteSheetPath = "";
                    if (!string.IsNullOrEmpty(subtype.Sprites.ReadyForHarvest))
                        harvestedSpriteSheetPath = Path.Combine(bfavFolderPath, subtype.Sprites.ReadyForHarvest);

                    // if there is a harvested and adult sheet, swap them (as favr stored sheets as Harvested_ while bfav stores them as Ready to Harvest_)
                    if (!string.IsNullOrEmpty(adultSpriteSheetPath) && !string.IsNullOrEmpty(harvestedSpriteSheetPath))
                    {
                        var tempAdultSpriteSheetPath = adultSpriteSheetPath;
                        adultSpriteSheetPath = harvestedSpriteSheetPath;
                        harvestedSpriteSheetPath = tempAdultSpriteSheetPath;
                    }

                    // copy over sprite sheets
                    if (!string.IsNullOrEmpty(adultSpriteSheetPath))
                    {
                        File.Copy(
                            sourceFileName: adultSpriteSheetPath,
                            destFileName: Path.Combine(favrAssetsFolder, $"{subtype.Type}.png"),
                            overwrite: true
                        );
                    }

                    if (!string.IsNullOrEmpty(babySpriteSheetPath))
                    {
                        File.Copy(
                            sourceFileName: babySpriteSheetPath,
                            destFileName: Path.Combine(favrAssetsFolder, $"Baby {subtype.Type}.png"),
                            overwrite: true
                        );
                    }

                    if (!string.IsNullOrEmpty(harvestedSpriteSheetPath))
                    {
                        File.Copy(
                            sourceFileName: harvestedSpriteSheetPath,
                            destFileName: Path.Combine(favrAssetsFolder, $"Harvested {subtype.Type}.png"),
                            overwrite: true
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to move sprites: {ex}", ConsoleColor.Red);
            }
        }

        /// <summary>Deserialises a file at the given path.</summary>
        /// <typeparam name="T">The type to deserialise the file to.</typeparam>
        /// <param name="file">The file to deserialise.</param>
        /// <returns>The file deserialised.</returns>
        private static T DeserializeJsonFile<T>(string file)
        {
            try
            {
                using (var reader = File.OpenText(file))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (T)serializer.Deserialize(reader, typeof(T));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to deserialize object: {ex}", ConsoleColor.Red);
                return default;
            }
        }

        /// <summary>Serilises the given object to the given file.</summary>
        /// <param name="objectToSerialize">The object to serilise.</param>
        /// <param name="file">The file to serialisze the object to.</param>
        private static void SerializeObjectToJson(object objectToSerialize, string file)
        {
            try
            {
                if (!Directory.Exists(file))
                    Directory.CreateDirectory(Path.GetDirectoryName(file));

                using (StreamWriter streamWriter = new StreamWriter(file))
                using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.Formatting = Formatting.Indented;

                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
                    {
                        DefaultValueHandling = DefaultValueHandling.Ignore,
                        Converters = new List<JsonConverter>() { new StringEnumConverter() }
                    });
                    serializer.Serialize(jsonWriter, objectToSerialize);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine($"Failed to serialize object: {ex}", ConsoleColor.Red);
            }
        }

        /// <summary>Get the name of the directory root.</summary>
        /// <param name="path">The directory.</param>
        /// <returns>The name of the directory root.</returns>
        private static string GetDirectoryRootName(string path)
        {
            var splitDirectory = path.Split(Path.DirectorySeparatorChar);
            return splitDirectory[splitDirectory.Length - 1];
        }
    }
}
