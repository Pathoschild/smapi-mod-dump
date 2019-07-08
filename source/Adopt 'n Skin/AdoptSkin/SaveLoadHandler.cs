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

        /// <summary>Sets up initial values needed for Adopt & Skin.</summary>
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
            SHelper.Events.Input.ButtonPressed += ModEntry.Creator.WildHorseInteractionCheck;
            SHelper.Events.Input.ButtonPressed += ModEntry.Creator.StrayInteractionCheck;
            SHelper.Events.Input.ButtonReleased += ModEntry.HotKeyCheck;

            SHelper.Events.World.NpcListChanged += ModEntry.SaveTheHorse;
            SHelper.Events.World.NpcListChanged += Entry.CheckForFirstPet;
            SHelper.Events.World.NpcListChanged += Entry.CheckForFirstHorse;
        }


        /// <summary>Stops Adopt & Skin from updating at each tick</summary>
        internal static void StopUpdateChecks(object s, EventArgs e)
        {
            SHelper.Events.GameLoop.UpdateTicked -= Entry.OnUpdateTicked;
            SHelper.Events.Input.ButtonPressed -= ModEntry.Creator.WildHorseInteractionCheck;
            SHelper.Events.Input.ButtonPressed -= ModEntry.Creator.StrayInteractionCheck;
            SHelper.Events.Input.ButtonReleased -= ModEntry.HotKeyCheck;

            SHelper.Events.World.NpcListChanged -= ModEntry.SaveTheHorse;
        }


        internal static void LoadData(object s, EventArgs e)
        {
            // Only allow the host player to load Adopt & Skin data
            if (!Context.IsMainPlayer)
            {
                SMonitor.Log("Multiplayer Farmhand detected. Adopt & Skin has been disabled.", LogLevel.Debug);
                return;
            }

            // Load skin maps
            ModEntry.AnimalSkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("animal-skin-map") ?? new Dictionary<long, int>();
            ModEntry.PetSkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("pet-skin-map") ?? new Dictionary<long, int>();
            ModEntry.HorseSkinMap = SHelper.Data.ReadSaveData<Dictionary<long, int>>("horse-skin-map") ?? new Dictionary<long, int>();

            // Load Short ID maps
            ModEntry.AnimalLongToShortIDs = SHelper.Data.ReadSaveData<Dictionary<long, int>>("animal-long-to-short-ids") ?? new Dictionary<long, int>();
            ModEntry.AnimalShortToLongIDs = SHelper.Data.ReadSaveData<Dictionary<int, long>>("animal-short-to-long-ids") ?? new Dictionary<int, long>();

            // Load whether the first pet and/or first horse have been received
            LoadAdoptableVars();

            // Refresh skins via skinmap
            LoadCreatureSkins();

            // Make sure Marnie's cows put some clothes on
            foreach (GameLocation loc in Game1.locations)
            {
                if (loc is Forest forest)
                    foreach (FarmAnimal animal in forest.marniesLivestock)
                        animal.Sprite = new AnimatedSprite(ModEntry.GetSkinFromID(ModEntry.Sanitize(animal.type.Value),
                            Randomizer.Next(1, ModEntry.AnimalAssets[ModEntry.Sanitize(animal.type.Value)].Count)).AssetKey, 0, 32, 32);
            }

            // Set configuration for walk-through pets
            foreach (Pet pet in ModEntry.GetPets())
                if (pet.Manners != Stray.StrayID)
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


        /// <summary>Loads the information on whether the first pet and/or first horse have been received by the player</summary>
        internal static void LoadAdoptableVars()
        {
            // Load FirstPetReceived bool
            string petReceived = SHelper.Data.ReadSaveData<string>("first-pet-received") ?? null;
            if (petReceived != null)
            {
                ModEntry.Creator.FirstPetReceived = bool.Parse(petReceived);
                // Place pet bed onto map if player can adopt pets
                if (ModEntry.Creator.FirstPetReceived)
                    ModEntry.Creator.PlaceBetBed();
            }
            else
            {
                // File is new to A&S or is from an older version. Re-check for this variable best you can.
                if (ModEntry.PetSkinMap.Count > 0 || ModEntry.GetPets().ToList().Count > 0)
                {
                    ModEntry.Creator.FirstPetReceived = true;
                    Game1.addMailForTomorrow("MarnieStrays");
                    SHelper.Events.GameLoop.DayStarted += ModEntry.PlaceBedTomorrow;
                }
                else
                {
                    ModEntry.Creator.FirstPetReceived = false;
                    SHelper.Events.World.NpcListChanged += Entry.CheckForFirstPet;
                }
            }

            // Load FirstHorseReceived bool
            string horseReceived = SHelper.Data.ReadSaveData<string>("first-horse-received") ?? null;
            if (horseReceived != null)
                ModEntry.Creator.FirstHorseReceived = bool.Parse(horseReceived);
            else
            {
                // File is new to A&S or is from an older version. Re-check for this variable best you can.
                if (ModEntry.HorseSkinMap.Count > 0 || ModEntry.GetHorses().ToList().Count > 0)
                    ModEntry.Creator.FirstHorseReceived = true;
                else
                    ModEntry.Creator.FirstHorseReceived = false;
            }
        }


        /// <summary>Refreshes creature information based on how much information the save file contains</summary>
        internal static void LoadCreatureSkins()
        {
            // File is new to A&S. Add all creatures into the system
            if (ModEntry.AnimalSkinMap.Count == 0 && ModEntry.PetSkinMap.Count == 0 && ModEntry.HorseSkinMap.Count == 0)
            {
                foreach (FarmAnimal animal in ModEntry.GetAnimals())
                    Entry.AddCreature(animal);
                foreach (Pet pet in ModEntry.GetPets())
                    Entry.AddCreature(pet);
                foreach (Horse horse in ModEntry.GetHorses())
                    Entry.AddCreature(horse);
            }
            // Refresh skins on creatures + add creatures to system if the save is an older version
            else
            {
                foreach (FarmAnimal animal in ModEntry.GetAnimals())
                    if (!ModEntry.AnimalLongToShortIDs.ContainsKey(animal.myID.Value))
                        Entry.AddCreature(animal);
                    else
                        Entry.UpdateSkin(animal);
                foreach (Pet pet in ModEntry.GetPets())
                    if (pet.Manners == 0)
                        Entry.AddCreature(pet);
                    // Remove extra Strays left on map
                    else if (pet.Manners == Stray.StrayID && (ModEntry.Creator.StrayInfo == null || ModEntry.Creator.StrayInfo.PetInstance != pet))
                        Game1.removeThisCharacterFromAllLocations(pet);
                    else
                        Entry.UpdateSkin(pet);
                foreach (Horse horse in ModEntry.GetHorses())
                    // Don't add tractors to the system
                    if (horse.Manners == 0 && !horse.Name.StartsWith("tractor/"))
                        Entry.AddCreature(horse);
                    // Remove extra WildHorses left on the map
                    else if (horse.Manners == WildHorse.WildID && (ModEntry.Creator.HorseInfo == null || ModEntry.Creator.HorseInfo.HorseInstance != horse))
                        Game1.removeThisCharacterFromAllLocations(horse);
                    else if (!horse.Name.StartsWith("tractor/"))
                        Entry.UpdateSkin(horse);
            }
        }


        internal static void SaveData(object s, EventArgs e)
        {
            // Only allow the host player to save Adopt & Skin data
            if (!Context.IsMainPlayer)
                return;

            // Remove Adopt & Skin from update loop
            StopUpdateChecks(null, null);

            // Save skin maps
            SHelper.Data.WriteSaveData("animal-skin-map", ModEntry.AnimalSkinMap);
            SHelper.Data.WriteSaveData("pet-skin-map", ModEntry.PetSkinMap);
            SHelper.Data.WriteSaveData("horse-skin-map", ModEntry.HorseSkinMap);

            // Save Short ID maps
            SHelper.Data.WriteSaveData("animal-long-to-short-ids", ModEntry.AnimalLongToShortIDs);
            SHelper.Data.WriteSaveData("animal-short-to-long-ids", ModEntry.AnimalShortToLongIDs);

            // Save Stray and WildHorse spawn potential
            SHelper.Data.WriteSaveData("first-pet-received", ModEntry.Creator.FirstPetReceived.ToString());
            SHelper.Data.WriteSaveData("first-horse-received", ModEntry.Creator.FirstHorseReceived.ToString());
            // If player returns to title screen, ensure that they don't carry the variable to another file
            ModEntry.Creator.FirstPetReceived = false;
            ModEntry.Creator.FirstHorseReceived = false;

            // Save data version. May be used for reverse-compatibility for files.
            SHelper.Data.WriteSaveData("data-version", "3");

            //SHelper.Events.GameLoop.DayStarted -= ModEntry.Creator.ProcessNewDay;
            //SHelper.Events.GameLoop.DayEnding -= ModEntry.Creator.ProcessEndDay;
        }


        /// <summary>Load skin assets from the /assets/skins directory into the A&S database</summary>
        internal static void LoadAssets()
        {
            // Gather handled types
            string validTypes = string.Join(", ", ModApi.GetHandledAllTypes());

            foreach (FileInfo file in new DirectoryInfo(Path.Combine(SHelper.DirectoryPath, "assets", "skins")).EnumerateFiles())
            {
                // Check extension of file is handled by Adopt & Skin
                string extension = Path.GetExtension(file.Name);
                if (!ValidExtensions.Contains(extension))
                {
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid extension (extension must be one of type {string.Join(", ", ValidExtensions)})", LogLevel.Warn);
                    continue;
                }

                // Parse file name
                string[] nameParts = Path.GetFileNameWithoutExtension(file.Name).Split(new[] { '_' }, 2);
                string type = ModEntry.Sanitize(nameParts[0]);
                // Ensure creature type is handled by Adopt & Skin
                if (!ModEntry.PetAssets.ContainsKey(type) && !ModEntry.HorseAssets.ContainsKey(type) && !ModEntry.AnimalAssets.ContainsKey(type))
                {
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid naming convention (can't parse {nameParts[0]} as an animal, pet, or horse. Expected one of type: {validTypes})", LogLevel.Warn);
                    continue;
                }
                // Ensure both a type and skin ID can be found in the file name
                if (nameParts.Length != 2)
                {
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name} with invalid naming convention (no skin ID found)", LogLevel.Warn);
                    continue;
                }
                // Ensure the skin ID is a number
                int skinID = 0;
                if (nameParts.Length == 2 && !int.TryParse(nameParts[1], out skinID))
                {
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with invalid skin ID (can't parse {nameParts[1]} as a number)", LogLevel.Warn);
                    continue;
                }
                // Ensure the skin ID is not 0 or negative
                if (skinID <= 0)
                {
                    ModEntry.SMonitor.Log($"Ignored skin `assets/skins/{file.Name}` with skin ID of less than or equal to 0. Skins must have an ID of at least 1.");
                    continue;
                }

                // File naming is valid, add asset into system
                string assetKey = SHelper.Content.GetActualAssetKey(Path.Combine("assets", "skins", extension.Equals("xnb") ? Path.Combine(Path.GetDirectoryName(file.Name), Path.GetFileNameWithoutExtension(file.Name)) : file.Name));
                if (ModEntry.AnimalAssets.ContainsKey(type))
                    ModEntry.AnimalAssets[type].Add(new AnimalSkin(type, skinID, assetKey));
                else if (ModEntry.HorseAssets.ContainsKey(type))
                    ModEntry.HorseAssets[type].Add(new AnimalSkin(type, skinID, assetKey));
                else
                    ModEntry.PetAssets[type].Add(new AnimalSkin(type, skinID, assetKey));
            }


            // Sort each list by ID
            AnimalSkin.Comparer comp = new AnimalSkin.Comparer();
            foreach (string type in ModEntry.AnimalAssets.Keys)
                ModEntry.AnimalAssets[type].Sort((p1, p2) => comp.Compare(p1, p2));
            foreach (string type in ModEntry.PetAssets.Keys)
                ModEntry.PetAssets[type].Sort((p1, p2) => comp.Compare(p1, p2));
            foreach (string type in ModEntry.HorseAssets.Keys)
                ModEntry.HorseAssets[type].Sort((p1, p2) => comp.Compare(p1, p2));


            // Print loaded assets to console
            StringBuilder summary = new StringBuilder();
            summary.AppendLine(
                "Statistics:\n"
                + "\n  Registered types: " + validTypes
                + "\n  Animal Skins:"
            );
            foreach (KeyValuePair<string, List<AnimalSkin>> pair in ModEntry.AnimalAssets)
            {
                if (pair.Value.Count > 0)
                    summary.AppendLine($"    {pair.Key}: {pair.Value.Count} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
            }
            summary.AppendLine("  Pet Skins:");
            foreach (KeyValuePair<string, List<AnimalSkin>> pair in ModEntry.PetAssets)
            {
                if (pair.Value.Count > 0)
                    summary.AppendLine($"    {pair.Key}: {pair.Value.Count} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
            }
            summary.AppendLine("  Horse Skins:");
            foreach (KeyValuePair<string, List<AnimalSkin>> pair in ModEntry.HorseAssets)
            {
                if (pair.Value.Count > 0)
                    summary.AppendLine($"    {pair.Key}: {pair.Value.Count} skins ({string.Join(", ", pair.Value.Select(p => Path.GetFileName(p.AssetKey)).OrderBy(p => p))})");
            }


            ModEntry.SMonitor.Log(summary.ToString(), LogLevel.Trace);
            ModEntry.AssetsLoaded = true;
        }
    }
}
