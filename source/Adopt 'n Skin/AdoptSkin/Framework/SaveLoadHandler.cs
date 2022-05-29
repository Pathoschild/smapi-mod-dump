/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Gathouria/Adopt-Skin
**
*************************************************/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;

namespace AdoptSkin.Framework
{
    class SaveLoadHandler
    {
        /// <summary>The file extensions recognised by the mod.</summary>
        private static readonly HashSet<string> ValidExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ".png",
            ".xnb"
        };

        private static readonly IModHelper SHelper = ModEntry.SHelper;
        private static readonly IMonitor SMonitor = ModEntry.SMonitor;

        private static ModEntry Entry;

        private static List<string> InvalidExt = new List<string>();
        private static List<string> InvalidType = new List<string>();
        private static List<string> InvalidID = new List<string>();
        private static List<string> InvalidNum = new List<string>();
        private static List<string> InvalidRange = new List<string>();

        internal SaveLoadHandler(ModEntry entry)
        {
            Entry = entry;
        }






        /**************************
        ** Setup + Load/Save Logic
        ***************************/

        /// <summary>Sets up initial values needed for A&S.</summary>
        internal static void Setup(object sender, SaveLoadedEventArgs e)
        {
            // Register FarmAnimal Types
            Dictionary<string, string> farmAnimalData = ModEntry.SHelper.Content.Load<Dictionary<string, string>>("Data/FarmAnimals", ContentSource.GameContent);
            foreach (KeyValuePair<string, string> pair in farmAnimalData)
            {
                // Ignore unused FarmAnimal type in SDV code
                if (pair.Key.ToLower() == "hog" || pair.Key.ToLower() == "babyhog")
                    continue;

                string[] animalInfo = pair.Value.Split(new[] { '/' });
                string harvestTool = animalInfo[22];
                int maturedDay = int.Parse(animalInfo[1]);

                ModApi.RegisterType(pair.Key, typeof(FarmAnimal), maturedDay > 0, ModEntry.Sanitize(harvestTool) == "shears");
            }

            // Register default supported pet types
            ModApi.RegisterType("cat", typeof(Cat));
            ModApi.RegisterType("dog", typeof(Dog));

            // Register horse type
            ModApi.RegisterType("horse", typeof(Horse));

            LoadAssets();

            // Alert player that there are creatures with no skins loaded for them
            List<string> skinless = new List<string>();
            foreach (string type in ModEntry.Assets.Keys)
                if (ModEntry.Assets[type].Count == 0)
                    skinless.Add(type);
            if (skinless.Count > 0)
                ModEntry.SMonitor.Log($"NOTICE: The following creature types have no skins located in `/assets/skins`:\n" +
                    $"{string.Join(", ", skinless)}", LogLevel.Debug);

            // Remove the Setup from the loop, so that it isn't done twice when the player returns to the title screen and loads again
            SHelper.Events.GameLoop.SaveLoaded -= Setup;
        }


        /// <summary>Starts processes that Adopt & Skin checks at every update tick</summary>
        internal static void StartUpdateChecks()
        {
            SHelper.Events.GameLoop.UpdateTicked += Entry.OnUpdateTicked;
            SHelper.Events.Input.ButtonPressed += ModEntry.Creator.AdoptableInteractionCheck;
            SHelper.Events.Input.ButtonReleased += ModEntry.HotKeyCheck;

            SHelper.Events.World.NpcListChanged += ModEntry.HorseMountedCheck;
            SHelper.Events.World.NpcListChanged += Entry.CheckForFirstPet;
            SHelper.Events.World.NpcListChanged += Entry.CheckForFirstHorse;

            if (ModEntry.Config.PetAndHorseNameTags)
                SHelper.Events.Display.RenderingHud += ModEntry.ToolTip.RenderHoverTooltip;
        }


        /// <summary>Stops Adopt & Skin from updating at each tick</summary>
        internal static void StopUpdateChecks(object s, EventArgs e)
        {
            SHelper.Events.GameLoop.UpdateTicked -= Entry.OnUpdateTicked;
            SHelper.Events.Input.ButtonPressed -= ModEntry.Creator.AdoptableInteractionCheck;
            SHelper.Events.Input.ButtonReleased -= ModEntry.HotKeyCheck;

            SHelper.Events.World.NpcListChanged -= ModEntry.HorseMountedCheck;

            if (ModEntry.Config.PetAndHorseNameTags)
                SHelper.Events.Display.RenderingHud -= ModEntry.ToolTip.RenderHoverTooltip;

            // Ensure variables don't carry-over between saves if leave to title screen
            FlatlineAllVariables();
        }


        /// <summary>Clears all file-specific variables</summary>
        internal static void FlatlineAllVariables()
        {
            ModEntry.SkinMap = new Dictionary<long, int>();
            ModEntry.IDToCategory = new Dictionary<long, ModEntry.CreatureCategory>();

            ModEntry.AnimalLongToShortIDs = new Dictionary<long, int>();
            ModEntry.AnimalShortToLongIDs = new Dictionary<int, long>();

            ModEntry.Creator.FirstPetReceived = false;
            ModEntry.Creator.FirstHorseReceived = false;
        }


        internal static void LoadData(object s, EventArgs e)
        {
            // Only allow the host player to load Adopt & Skin data
            if (Context.IsMainPlayer)
            {
                //SMonitor.Log("Multiplayer Farmhand detected. Adopt & Skin has been disabled.", LogLevel.Debug);
                SMonitor.Log("Host detected! Let's WRECK IT", LogLevel.Debug);

                // Load skin and category maps
                ModEntry.SkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("skin-map") ?? new Dictionary<long, int>();
                ModEntry.IDToCategory = SHelper.Data.ReadSaveData<Dictionary<long, ModEntry.CreatureCategory>>("id-to-category") ?? new Dictionary<long, ModEntry.CreatureCategory>();

                // Load Short ID maps
                ModEntry.AnimalLongToShortIDs = SHelper.Data.ReadSaveData<Dictionary<long, int>>("animal-long-to-short-ids") ?? new Dictionary<long, int>();
                ModEntry.AnimalShortToLongIDs = SHelper.Data.ReadSaveData<Dictionary<int, long>>("animal-short-to-long-ids") ?? new Dictionary<int, long>();

                // Set up maps if save data is from an older data format of A&S
                if (ModEntry.SkinMap.Count == 0)
                    LoadSkinsOldVersion();

                // Load Pet ownership map
                // TODO: Similar to horse ownership loading

                // Load Horse ownership map
                Dictionary<long, long> horseOwnerMap = SHelper.Data.ReadSaveData<Dictionary<long, long>>("horse-ownership-map") ?? new Dictionary<long, long>();
                ModEntry.HorseOwnershipMap = new Dictionary<long, Farmer>();

                foreach (KeyValuePair<long, long> pair in horseOwnerMap)
                {
                    foreach (Farmer farmer in Game1.getAllFarmers())
                        if (pair.Key == ModApi.FarmerToID(farmer))
                            ModEntry.HorseOwnershipMap[pair.Key] = farmer;
                }

                // If no horses are mapped to owners, or there are some unmapped horses, assign them all to the host player
                if (ModEntry.HorseOwnershipMap.Count != ModApi.GetHorses().Count())
                    foreach (Horse horse in ModApi.GetHorses())
                    {
                        long horseID = ModEntry.GetLongID(horse);
                        if (horseID != 0 && !ModEntry.HorseOwnershipMap.ContainsKey(horseID))
                            ModEntry.HorseOwnershipMap.Add(horseID, Game1.MasterPlayer);
                    }
                // If there are extra horses on the mapping, remove them. This deal with a bug in older versions of A&S
                if (ModEntry.HorseOwnershipMap.Count > ModApi.GetHorses().Count())
                {
                    List<Horse> horses = ModApi.GetHorses().ToList();
                    List<long> existingHorses = new List<long>();
                    // Find the IDs of all existing, owned Horses
                    foreach (Horse horse in horses)
                        existingHorses.Add(ModEntry.GetLongID(horse));

                    // Find the stowaways.
                    List<long> idsToRemove = new List<long>();
                    foreach (long horseID in ModEntry.HorseOwnershipMap.Keys)
                    {
                        if (!existingHorses.Contains(horseID))
                            idsToRemove.Add(horseID);
                    }

                    // Yeet them from the mapping.
                    foreach (long id in idsToRemove)
                        ModEntry.HorseOwnershipMap.Remove(id);
                }

                // Load received first pet/horse status
                ModEntry.Creator.FirstHorseReceived = bool.Parse(SHelper.Data.ReadSaveData<string>("first-horse-received") ?? "false");
                ModEntry.Creator.FirstPetReceived = bool.Parse(SHelper.Data.ReadSaveData<string>("first-pet-received") ?? "false");
                Entry.CheckForFirstPet(null, null);
                Entry.CheckForFirstHorse(null, null);
                //return;
            }


            // Refresh skins via skinmap
            LoadCreatureSkins();

            // Make sure Marnie's cows put some clothes on
            foreach (GameLocation loc in Game1.locations)
            {
                if (loc is Forest forest)
                    foreach (FarmAnimal animal in forest.marniesLivestock)
                    {
                        string type = ModApi.GetInternalType(animal);
                        if (ModApi.HasSkins(type))
                        {
                            int[] spriteInfo = ModApi.GetSpriteInfo(animal);
                            AnimalSkin skin = ModEntry.GetSkin(type, ModEntry.GetRandomSkin(type));
                            animal.Sprite = new AnimatedSprite(skin.AssetKey, spriteInfo[0], spriteInfo[1], spriteInfo[2]);
                        }
                    }
            }

            // Set configuration for walk-through pets
            foreach (Pet pet in ModApi.GetPets())
            {
                if (ModEntry.Config.WalkThroughPets)
                    pet.farmerPassesThrough = true;
                else
                    pet.farmerPassesThrough = false;
            }

            // Set last known animal count
            ModEntry.AnimalCount = Game1.getFarm().getAllFarmAnimals().Count;

            // Add Adopt & Skin to the update loop
            StartUpdateChecks();
        }


        /// <summary>Refreshes creature information based on how much information the save file contains</summary>
        internal static void LoadCreatureSkins()
        {
            foreach (FarmAnimal animal in ModApi.GetAnimals())
                if (!ModEntry.AnimalLongToShortIDs.ContainsKey(ModEntry.GetLongID(animal)))
                    Entry.AddCreature(animal);
                else
                    ModEntry.UpdateSkin(animal);

            foreach (Pet pet in ModApi.GetPets())
                if (ModEntry.GetLongID(pet) == 0)
                    Entry.AddCreature(pet);
                else
                    ModEntry.UpdateSkin(pet);

            foreach (Horse horse in ModApi.GetHorses())
                if (ModEntry.GetLongID(horse) == 0)
                    Entry.AddCreature(horse);
                else
                    ModEntry.UpdateSkin(horse);
        }


        /// <summary>Adds creatures into the system based on the data from older versions' save files or for files new to A&S.</summary>
        internal static void LoadSkinsOldVersion()
        {
            Dictionary<Character, int> creaturesToAdd = new Dictionary<Character, int>();

            // Load pet information stored from older version formats
            Dictionary<long, int> petSkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("pet-skin-map") ?? new Dictionary<long, int>();
            foreach (Pet pet in ModApi.GetPets())
            {
                long longID = ModEntry.GetLongID(pet);
                // Pet unregistered
                if (longID == 0)
                    creaturesToAdd.Add(pet, 0);
                // Pet registered in previous system
                else if (petSkinMap.ContainsKey(longID))
                    creaturesToAdd.Add(pet, petSkinMap[longID]);
                // Reset any previous known ShortID
                pet.Manners = 0;
            }

            // Load horse information stored from older version formats
            Dictionary<long, int> horseSkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("horse-skin-map") ?? new Dictionary<long, int>();
            foreach (Horse horse in ModApi.GetHorses())
            {
                long longID = ModEntry.GetLongID(horse);
                // Horse unregistered
                if (longID == 0)
                    creaturesToAdd.Add(horse, 0);
                // Horse registered in previous system
                else if (horseSkinMap.ContainsKey(longID))
                    creaturesToAdd.Add(horse, horseSkinMap[longID]);
                // Reset any previous known ShortID
                horse.Manners = 0;
            }

            // Load animal information stored from older version formats
            Dictionary<long, int> animalSkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("animal-skin-map") ?? new Dictionary<long, int>();
            ModEntry.AnimalLongToShortIDs = new Dictionary<long, int>();
            ModEntry.AnimalShortToLongIDs = new Dictionary<int, long>();
            foreach (FarmAnimal animal in ModApi.GetAnimals())
            {
                long longID = ModEntry.GetLongID(animal);
                // Animal registered in previous system
                if (animalSkinMap.ContainsKey(longID))
                    creaturesToAdd.Add(animal, animalSkinMap[longID]);
                // Animal unregistered
                else
                    creaturesToAdd.Add(animal, 0);
            }

            // Add in all creatures from older systems
            foreach (KeyValuePair<Character, int> creatureInfo in creaturesToAdd)
                Entry.AddCreature(creatureInfo.Key, creatureInfo.Value);
        }


        internal static void SaveData(object s, EventArgs e)
        {
            // Only allow the host player to save Adopt & Skin data
            if (!Context.IsMainPlayer)
                return;

            // Ensure strays and wild horses don't persist
            ModApi.ClearUnownedPets();

            // Save skin and category maps
            SHelper.Data.WriteSaveData("skin-map", ModEntry.SkinMap);
            SHelper.Data.WriteSaveData("id-to-category", ModEntry.IDToCategory);

            // Save Short ID maps
            SHelper.Data.WriteSaveData("animal-long-to-short-ids", ModEntry.AnimalLongToShortIDs);
            SHelper.Data.WriteSaveData("animal-short-to-long-ids", ModEntry.AnimalShortToLongIDs);

            // Save Pet to owner map
            /*Dictionary<long, long> petOwnership = new Dictionary<long, long>();
            foreach (KeyValuePair<long, Farmer> pair in ModEntry.OwnerMap)
            {
                petOwnership[pair.Key] = ModApi.FarmerToID(pair.Value);
            }
            SHelper.Data.WriteSaveData("pet-ownership-map", petOwnership); */
            // Save Horse to owner map
            Dictionary<long, long> horseOwnership = new Dictionary<long, long>();
            foreach (KeyValuePair<long, Farmer> pair in ModEntry.HorseOwnershipMap)
            {
                horseOwnership[pair.Key] = ModApi.FarmerToID(pair.Value);
            }
            SHelper.Data.WriteSaveData("horse-ownership-map", horseOwnership);

            // Save Stray and WildHorse spawn potential
            SHelper.Data.WriteSaveData("first-pet-received", ModEntry.Creator.FirstPetReceived.ToString());
            SHelper.Data.WriteSaveData("first-horse-received", ModEntry.Creator.FirstHorseReceived.ToString());

            // Save data version. May be used for reverse-compatibility for files.
            SHelper.Data.WriteSaveData("data-version", "5");

            // Remove Adopt & Skin from update loop
            StopUpdateChecks(null, null);
        }


        /// <summary>Load skin assets from the /assets/skins directory into the A&S database</summary>
        internal static void LoadAssets()
        {
            // Gather handled types
            string validTypes = string.Join(", ", ModApi.GetHandledAllTypes());



            // Add custom sprites
            foreach (string path in Directory.EnumerateFiles(Path.Combine(SHelper.DirectoryPath, "assets", "skins"), "*", SearchOption.AllDirectories))
                PullSprite(Path.GetRelativePath(SHelper.DirectoryPath, path)); // must be a relative path
            // Grab the directory for the /Mods folder from the /Mods/AdoptSkin

            //string modFolderPath = SHelper.DirectoryPath
            foreach (string path in Directory.EnumerateFiles(Path.Combine(SHelper.DirectoryPath)))

            // Warn for invalid files
            if (InvalidExt.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid extension:\n`{string.Join("`, `", InvalidExt)}`\nExtension must be one of type {string.Join(", ", ValidExtensions)}", LogLevel.Warn);
            if (InvalidType.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid naming convention:\n`{string.Join("`, `", InvalidType)}`\nCan't parse as an animal, pet, or horse. Expected one of type: {validTypes}", LogLevel.Warn);
            if (InvalidID.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid naming convention (no skin ID found):\n`{string.Join("`, `", InvalidID)}`", LogLevel.Warn);
            if (InvalidNum.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid ID (can't parse ID number):\n`{string.Join("`, `", InvalidNum)}`", LogLevel.Warn);
            if (InvalidRange.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with ID of less than or equal to 0 (Skins must have an ID of at least 1):\n`{string.Join("`, `", InvalidRange)}`", LogLevel.Warn);

            EnforceSpriteSets();

            // Print loaded assets to console
            StringBuilder summary = new StringBuilder();
            summary.AppendLine(
                "Statistics:\n"
                + "\n  Registered types: " + validTypes
                + "\n  Skins:"
            );
            foreach (KeyValuePair<string, Dictionary<int, AnimalSkin>> skinEntry in ModEntry.Assets)
            {
                if (skinEntry.Value.Count > 0)
                    summary.AppendLine($"    {skinEntry.Key}: {skinEntry.Value.Count} skins ({string.Join(", ", skinEntry.Value.Select(p => Path.GetFileName(p.Value.AssetKey)).OrderBy(p => p))})");
            }

            ModEntry.SMonitor.Log(summary.ToString(), LogLevel.Trace);
            ModEntry.AssetsLoaded = true;
        }


        /// <summary>Places the custom sprite at the given path into the A&S system for use.</summary>
        /// <param name="path">The full directory location of the sprite</param>
        private static void PullSprite(string path)
        {
            string extension = Path.GetExtension(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string[] nameParts = fileName.Split(new[] { '_' }, 2);
            string type = ModEntry.Sanitize(nameParts[0]);
            int skinID = 0;

            if (!ValidExtensions.Contains(extension))
                InvalidExt.Add(fileName);
            else if (!ModEntry.Assets.ContainsKey(type))
                InvalidType.Add(fileName);
            else if (nameParts.Length != 2)
                InvalidID.Add(fileName);
            else if (nameParts.Length == 2 && !int.TryParse(nameParts[1], out skinID))
                InvalidNum.Add(fileName);
            else if (skinID <= 0)
                InvalidRange.Add(fileName);
            else
            {
                // File naming is valid, get the asset key
                string assetKey = SHelper.Content.GetActualAssetKey(Path.Combine(Path.GetDirectoryName(path), extension.Equals("xnb") ? Path.GetFileNameWithoutExtension(path) : Path.GetFileName(path)));

                // User has duplicate skin names. Only keep the first skin found with the identifier and number ID
                if (ModEntry.Assets[type].ContainsKey(skinID))
                    ModEntry.SMonitor.Log($"Ignored skin `{fileName}` with duplicate type and ID (more than one skin named `{fileName}` exists in `/assets/skins`)", LogLevel.Debug);
                // Skin is valid, add into system
                else
                {
                    Texture2D texture = ModEntry.SHelper.Content.Load<Texture2D>(assetKey, ContentSource.ModFolder);
                    ModEntry.Assets[type].Add(skinID, new AnimalSkin(type, skinID, assetKey, texture));
                }
            }
        }


        /// <summary>
        /// Checks the list of loaded assets, removes incomplete skin sets
        /// (i.e. a "sheared" or "baby" skin exists, but not the typical skin, or vice versa where applicable)
        /// </summary>
        private static void EnforceSpriteSets()
        {
            Dictionary<string, List<int>> skinsToRemove = new Dictionary<string, List<int>>();
            // ** Make list of values added, when check is done, see if Key already exists- simply add to values if so

            //Dictionary<string, Dictionary<int, AnimalSkin>> assetCopy = new Dictionary<string, Dictionary<int, AnimalSkin>>(ModEntry.Assets);
            foreach (KeyValuePair<string, Dictionary<int, AnimalSkin>> pair in ModEntry.Assets)
            {
                if (pair.Key.StartsWith("sheared"))
                {
                    // Look at the creature type that comes after "sheared"
                    if (ModEntry.Assets.ContainsKey(pair.Key.Substring(7)))
                    {
                        // Make sure every sheared skin has a normal skin variant for its ID
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            if (!ModEntry.Assets[pair.Key.Substring(7)].ContainsKey(id))
                                skinsToRemove = AddToValueList(skinsToRemove, pair.Key, id);

                        // Since the normal skin has a sheared version, make sure all normal versions have sheared skins
                        foreach (int id in ModEntry.Assets[pair.Key.Substring(7)].Keys)
                            if (!ModEntry.Assets[pair.Key].ContainsKey(id))
                                skinsToRemove = AddToValueList(skinsToRemove, pair.Key.Substring(7), id);
                    }
                    // This sheared skin has no normal animal type registered to it; remove all sheared variants of this skin
                    else
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            skinsToRemove = AddToValueList(skinsToRemove, pair.Key, id);
                }
                else if (pair.Key.StartsWith("baby"))
                {
                    // Look at the creature type that comes after "baby"
                    if (ModEntry.Assets.ContainsKey(pair.Key.Substring(4)))
                    {
                        // Make sure every baby skin has a normal skin variant for its ID
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            if (!ModEntry.Assets[pair.Key.Substring(4)].ContainsKey(id))
                                skinsToRemove = AddToValueList(skinsToRemove, pair.Key, id);

                        // Since the normal skin has a baby version, make sure all normal versions have baby skins
                        foreach (int id in ModEntry.Assets[pair.Key.Substring(4)].Keys)
                            if (!ModEntry.Assets[pair.Key].ContainsKey(id))
                                skinsToRemove = AddToValueList(skinsToRemove, pair.Key.Substring(4), id);
                    }
                    // This baby skin has no normal skins at all; remove them all
                    else
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            skinsToRemove = AddToValueList(skinsToRemove, pair.Key, id);
                }
            }


            // Warn player of any incomplete sets and remove them from the Assets dictionary
            if (skinsToRemove.Count > 0)
            {
                string warnString = "";

                foreach (KeyValuePair<string, List<int>> removing in skinsToRemove)
                {
                    warnString += removing.Key.ToString() + ": IDs " + string.Join(", ", removing.Value) + "\n";
                    foreach (int id in removing.Value)
                        ModEntry.Assets[removing.Key].Remove(id);
                }

                ModEntry.SMonitor.Log($"The following skins are incomplete skin sets, and will not be loaded (missing a paired sheared, baby, or adult skin):\n{warnString}", LogLevel.Warn);
            }
                

            // ** TODO: Is there a way to check for types, so adults with no baby *or* sheared can be caught? Just make grab adult skin?
            // -- Cycle through FarmAnimal typing list and check while in there
        }

        private static Dictionary<string, List<int>> AddToValueList(Dictionary<string, List<int>> dict, string type, int id)
        {
            if (!dict.ContainsKey(type))
                dict.Add(type, new List<int> { id });
            else if (!dict[type].Contains(id))
                dict[type].Add(id);

            return dict;
        }
    }
}
