using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ModApi.RegisterDefaultTypes();
            IntegrateMods();

            LoadAssets();

            // Remove the Setup from the loop, so that it isn't done twice when the player returns to the title screen and loads again
            SHelper.Events.GameLoop.SaveLoaded -= Setup;
        }


        /// <summary>Loads handlers for integration of other mods</summary>
        private static void IntegrateMods()
        {
            if (SHelper.ModRegistry.IsLoaded("Paritee.BetterFarmAnimalVariety"))
            {
                ISemanticVersion bfavVersion = SHelper.ModRegistry.Get("Paritee.BetterFarmAnimalVariety").Manifest.Version;

                if (bfavVersion.IsNewerThan("2.2.6"))
                    ModEntry.BFAV300Worker = new BFAV300Integrator();
                else
                    ModEntry.BFAV226Worker = new BFAV226Integrator();
            }
        }


        /// <summary>Starts processes that Adopt & Skin checks at every update tick</summary>
        internal static void StartUpdateChecks()
        {
            SHelper.Events.GameLoop.UpdateTicked += Entry.OnUpdateTicked;
            SHelper.Events.Input.ButtonReleased += ModEntry.Creator.AdoptableInteractionCheck;
            SHelper.Events.Input.ButtonReleased += ModEntry.HotKeyCheck;

            SHelper.Events.World.NpcListChanged += ModEntry.HorseMountedCheck;
            SHelper.Events.World.NpcListChanged += Entry.CheckForFirstPet;
            SHelper.Events.World.NpcListChanged += Entry.CheckForFirstHorse;

            SHelper.Events.Display.RenderingHud += ModEntry.ToolTip.RenderHoverTooltip;
        }


        /// <summary>Stops Adopt & Skin from updating at each tick</summary>
        internal static void StopUpdateChecks(object s, EventArgs e)
        {
            SHelper.Events.GameLoop.UpdateTicked -= Entry.OnUpdateTicked;
            SHelper.Events.Input.ButtonReleased -= ModEntry.Creator.AdoptableInteractionCheck;
            SHelper.Events.Input.ButtonReleased -= ModEntry.HotKeyCheck;

            SHelper.Events.World.NpcListChanged -= ModEntry.HorseMountedCheck;

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

            // Set up maps if save data is from an older A&S
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
                ModEntry.UpdateSkin(animal);

            foreach (Pet pet in ModApi.GetPets())
                // Remove extra Strays left on the map
                if (ModApi.IsStray(pet))
                    Game1.removeThisCharacterFromAllLocations(pet);
                else
                    ModEntry.UpdateSkin(pet);

            foreach (Horse horse in ModApi.GetHorses())
                // Remove extra WildHorses left on the map
                if (ModApi.IsWildHorse(horse))
                    Game1.removeThisCharacterFromAllLocations(horse);
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

            // Parse file name. Ignore if using an invalid name or file extension.
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(SHelper.DirectoryPath, "assets", "skins")).EnumerateFiles())
            {
                string extension = Path.GetExtension(file.Name);
                string[] nameParts = Path.GetFileNameWithoutExtension(file.Name).Split(new[] { '_' }, 2);
                string type = ModEntry.Sanitize(nameParts[0]);
                int skinID = 0;

                if (!ValidExtensions.Contains(extension))
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid extension (extension must be one of type {string.Join(", ", ValidExtensions)})", LogLevel.Warn);
                else if (!ModEntry.Assets.ContainsKey(type))
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse {nameParts[0]} as an animal, pet, or horse. Expected one of type: {validTypes})", LogLevel.Warn);
                else if (nameParts.Length != 2)
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name} with invalid naming convention (no skin ID found)", LogLevel.Warn);
                else if (nameParts.Length == 2 && !int.TryParse(nameParts[1], out skinID))
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid skin ID (can't parse {nameParts[1]} as a number)", LogLevel.Warn);
                else if (skinID <= 0)
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with skin ID of less than or equal to 0. Skins must have an ID of at least 1.", LogLevel.Warn);
                else
                {
                    // File naming is valid, add asset into system
                    string assetKey = SHelper.Content.GetActualAssetKey(Path.Combine("assets", "skins", extension.Equals("xnb") ? Path.Combine(Path.GetDirectoryName(file.Name), Path.GetFileNameWithoutExtension(file.Name)) : file.Name));
                    ModEntry.Assets[type].Add(skinID, new AnimalSkin(type, skinID, assetKey));
                }
            }
            /*
            // Sort each skin list by ID
            AnimalSkin.Comparer comp = new AnimalSkin.Comparer();
            foreach (string type in ModEntry.Assets.Keys)
                ModEntry.Assets[type]
            */

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
    }
}
