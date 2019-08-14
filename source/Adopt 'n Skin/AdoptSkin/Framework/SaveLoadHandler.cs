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
        private static readonly Random Randomizer = new Random();

        private static readonly IModHelper SHelper = ModEntry.SHelper;
        private static readonly IMonitor SMonitor = ModEntry.SMonitor;

        private static ModEntry Entry;



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
            if (!Context.IsMainPlayer)
            {
                SMonitor.Log("Multiplayer Farmhand detected. Adopt & Skin has been disabled.", LogLevel.Debug);
                return;
            }

            // Load skin and category maps
            ModEntry.SkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("skin-map") ?? new Dictionary<long, int>();
            ModEntry.IDToCategory = SHelper.Data.ReadSaveData<Dictionary<long, ModEntry.CreatureCategory>>("id-to-category") ?? new Dictionary<long, ModEntry.CreatureCategory>();
            
            // Load Short ID maps
            ModEntry.AnimalLongToShortIDs = SHelper.Data.ReadSaveData<Dictionary<long, int>>("animal-long-to-short-ids") ?? new Dictionary<long, int>();
            ModEntry.AnimalShortToLongIDs = SHelper.Data.ReadSaveData<Dictionary<int, long>>("animal-short-to-long-ids") ?? new Dictionary<int, long>();

            // Set up maps if save data is from an older data format of A&S
            if (ModEntry.SkinMap.Count == 0)
                LoadSkinsOldVersion();

            // Load received first pet/horse status
            ModEntry.Creator.FirstHorseReceived = bool.Parse(SHelper.Data.ReadSaveData<string>("first-horse-received") ?? "false");
            ModEntry.Creator.FirstPetReceived = bool.Parse(SHelper.Data.ReadSaveData<string>("first-pet-received") ?? "false");
            Entry.CheckForFirstPet(null, null);
            Entry.CheckForFirstHorse(null, null);

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
                if (!ModApi.IsStray(pet))
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
                // Remove extra Strays left on the map
                if (ModApi.IsStray(pet))
                    Game1.removeThisCharacterFromAllLocations(pet);
                else if (ModEntry.GetLongID(pet) == 0)
                    Entry.AddCreature(pet);
                else
                    ModEntry.UpdateSkin(pet);

            foreach (Horse horse in ModApi.GetHorses())
                // Remove extra WildHorses left on the map
                if (ModApi.IsWildHorse(horse))
                    Game1.removeThisCharacterFromAllLocations(horse);
                else if (ModApi.IsNotATractor(horse) && ModEntry.GetLongID(horse) == 0)
                    Entry.AddCreature(horse);
                else if (ModApi.IsNotATractor(horse))
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
                if (ModApi.IsStray(pet))
                {
                    Game1.removeThisCharacterFromAllLocations(pet);
                    continue;
                }

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
                if (ModApi.IsWildHorse(horse))
                {
                    Game1.removeThisCharacterFromAllLocations(horse);
                    continue;
                }
                else if (!ModApi.IsNotATractor(horse))
                    continue;

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

            // Save skin and category maps
            SHelper.Data.WriteSaveData("skin-map", ModEntry.SkinMap);
            SHelper.Data.WriteSaveData("id-to-category", ModEntry.IDToCategory);

            // Save Short ID maps
            SHelper.Data.WriteSaveData("animal-long-to-short-ids", ModEntry.AnimalLongToShortIDs);
            SHelper.Data.WriteSaveData("animal-short-to-long-ids", ModEntry.AnimalShortToLongIDs);

            // Save Stray and WildHorse spawn potential
            SHelper.Data.WriteSaveData("first-pet-received", ModEntry.Creator.FirstPetReceived.ToString());
            SHelper.Data.WriteSaveData("first-horse-received", ModEntry.Creator.FirstHorseReceived.ToString());

            // Save data version. May be used for reverse-compatibility for files.
            SHelper.Data.WriteSaveData("data-version", "4");

            // Remove Adopt & Skin from update loop
            StopUpdateChecks(null, null);
        }


        /// <summary>Load skin assets from the /assets/skins directory into the A&S database</summary>
        internal static void LoadAssets()
        {
            // Gather handled types
            string validTypes = string.Join(", ", ModApi.GetHandledAllTypes());

            List<string> invalidExt = new List<string>();
            List<string> invalidType = new List<string>();
            List<string> invalidID = new List<string>();
            List<string> invalidNum = new List<string>();
            List<string> invalidRange = new List<string>();

            // Add custom sprites
            foreach (string path in Directory.EnumerateFiles(Path.Combine(SHelper.DirectoryPath, "assets", "skins"), "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(path);
                string fileName = Path.GetFileNameWithoutExtension(path);
                string[] nameParts = fileName.Split(new[] { '_' }, 2);
                string type = ModEntry.Sanitize(nameParts[0]);
                int skinID = 0;

                if (!ValidExtensions.Contains(extension))
                    invalidExt.Add(fileName);
                else if (!ModEntry.Assets.ContainsKey(type))
                    invalidType.Add(fileName);
                else if (nameParts.Length != 2)
                    invalidID.Add(fileName);
                else if (nameParts.Length == 2 && !int.TryParse(nameParts[1], out skinID))
                    invalidNum.Add(fileName);
                else if (skinID <= 0)
                    invalidRange.Add(fileName);
                else
                {
                    // File naming is valid, get the asset key
                    string assetKey = SHelper.Content.GetActualAssetKey(Path.Combine(Path.GetDirectoryName(path), extension.Equals("xnb") ? Path.GetFileNameWithoutExtension(path) : Path.GetFileName(path)));

                    // User has duplicate skin names. Only keep the first skin found with the identifier and number ID
                    if (ModEntry.Assets[type].ContainsKey(skinID))
                        ModEntry.SMonitor.Log($"Ignored skin `{fileName}` with duplicate type and ID (more than one skin named `{fileName}` exists in `/assets/skins`)", LogLevel.Debug);
                    // Skin is valid, add into system
                    else
                        ModEntry.Assets[type].Add(skinID, new AnimalSkin(type, skinID, assetKey));
                }
            }

            // Warn for invalid files
            if (invalidExt.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid extension:\n`{string.Join("`, `", invalidExt)}`\nExtension must be one of type {string.Join(", ", ValidExtensions)}", LogLevel.Warn);
            if (invalidType.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid naming convention:\n`{string.Join("`, `", invalidType)}`\nCan't parse as an animal, pet, or horse. Expected one of type: {validTypes}", LogLevel.Warn);
            if (invalidID.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid naming convention (no skin ID found):\n`{string.Join("`, `", invalidID)}`", LogLevel.Warn);
            if (invalidNum.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with invalid ID (can't parse ID number):\n`{string.Join("`, `", invalidNum)}`", LogLevel.Warn);
            if (invalidRange.Count > 0)
                ModEntry.SMonitor.Log($"Ignored skins with ID of less than or equal to 0 (Skins must have an ID of at least 1):\n`{string.Join("`, `", invalidRange)}`", LogLevel.Warn);

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

        /// <summary>
        /// Checks the list of loaded assets, removes incomplete skin sets
        /// (i.e. a "sheared" or "baby" skin exists, but not the typical skin, or vice versa where applicable)
        /// </summary>
        private static void EnforceSpriteSets()
        {
            Dictionary<string, int> skinsToRemove = new Dictionary<string, int>();
            foreach (KeyValuePair<string, Dictionary<int, AnimalSkin>> pair in ModEntry.Assets)
            {
                if (pair.Key.StartsWith("sheared"))
                {
                    // Look at the creature type that comes after "sheared"
                    if (ModEntry.Assets.ContainsKey(pair.Key.Substring(7)))
                    {
                        // Make sure every sheared skin has a normal skin variant for its ID
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            if (!ModEntry.Assets[pair.Key.Substring(7)].ContainsKey(id) && !skinsToRemove.Contains(new KeyValuePair<string, int>(pair.Key, id)))
                                skinsToRemove.Add(pair.Key, id);

                        // Since the normal skin has a sheared version, make sure all normal versions have sheared skins
                        foreach (int id in ModEntry.Assets[pair.Key.Substring(7)].Keys)
                            if (!ModEntry.Assets[pair.Key].ContainsKey(id) && !skinsToRemove.Contains(new KeyValuePair<string, int>(pair.Key, id)))
                                skinsToRemove.Add(pair.Key.Substring(7), id);
                    }
                    // This sheared skin has no normal skins at all; remove them all
                    else
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            skinsToRemove.Add(pair.Key, id);
                }
                else if (pair.Key.StartsWith("baby"))
                {
                    // Look at the creature type that comes after "baby"
                    if (ModEntry.Assets.ContainsKey(pair.Key.Substring(4)))
                    {
                        // Make sure every baby skin has a normal skin variant for its ID
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            if (!ModEntry.Assets[pair.Key.Substring(4)].ContainsKey(id) && !skinsToRemove.Contains(new KeyValuePair<string, int>(pair.Key, id)))
                                skinsToRemove.Add(pair.Key, id);

                        // Since the normal skin has a baby version, make sure all normal versions have baby skins
                        foreach (int id in ModEntry.Assets[pair.Key.Substring(4)].Keys)
                            if (!ModEntry.Assets[pair.Key].ContainsKey(id) && !skinsToRemove.Contains(new KeyValuePair<string, int>(pair.Key, id)))
                                skinsToRemove.Add(pair.Key.Substring(4), id);
                    }
                    // This baby skin has no normal skins at all; remove them all
                    else
                        foreach (int id in ModEntry.Assets[pair.Key].Keys)
                            if (!skinsToRemove.Contains(new KeyValuePair<string, int>(pair.Key, id)))
                                skinsToRemove.Add(pair.Key, id);
                }
            }

            // Warn player of any incomplete sets and remove them from the Assets dictionary
            if (skinsToRemove.Count > 0)
            {
                ModEntry.SMonitor.Log($"The following skins are incomplete skin sets, and will be removed (missing a paired sheared, baby, or adult skin):\n{string.Join(", ", skinsToRemove)}", LogLevel.Warn);

                foreach (KeyValuePair<string, int> removing in skinsToRemove)
                    ModEntry.Assets[removing.Key].Remove(removing.Value);
            }


            // ** TODO: Is there a way to check for types, so adults with no baby *or* sheared can be caught? Just make grab adult skin?
            // -- Cycle through FarmAnimal typing list and check while in there
        }
    }
}
