using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using AdoptSkin.Framework;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using StardewValley.Locations;
using StardewValley.Buildings;

namespace AdoptSkin
{

    /// <summary>The mod entry point.</summary>
    class ModEntry : Mod, IAssetEditor
    {
        /************************
        ** Fields
        *************************/

        private readonly Random Randomizer = new Random();

        internal enum CreatureCategory { Horse, Pet, Animal };
        
        internal static ModConfig Config;






        /************************
       ** Accessors
       *************************/

        // SMAPI Modding helpers
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;

        // SMAPI console command handler
        internal static CommandHandler Commander;
        // Pet and Horse creation handler
        internal static CreationHandler Creator;
        // Save/Load logic handler
        internal static SaveLoadHandler SaverLoader;

        // Mod integration
        internal static BFAV226Integrator BFAV226Worker;
        internal static BFAV300Integrator BFAV300Worker;

        // Skin assets
        internal static Dictionary<string, List<AnimalSkin>> AnimalAssets = new Dictionary<string, List<AnimalSkin>>();
        internal static Dictionary<string, List<AnimalSkin>> PetAssets = new Dictionary<string, List<AnimalSkin>>();
        internal static Dictionary<string, List<AnimalSkin>> HorseAssets = new Dictionary<string, List<AnimalSkin>>();

        // Skin mappings
        internal static Dictionary<long, int> AnimalSkinMap = new Dictionary<long, int>();
        internal static Dictionary<long, int> PetSkinMap = new Dictionary<long, int>();
        internal static Dictionary<long, int> HorseSkinMap = new Dictionary<long, int>();

        // Short ID mappings. Short IDs are small, user-friendly numbers for referencing specific creatures.
        internal static Dictionary<long, int> AnimalLongToShortIDs = new Dictionary<long, int>();
        internal static Dictionary<int, long> AnimalShortToLongIDs = new Dictionary<int, long>();

        // Pet and Horse string to type mappings
        internal static Dictionary<string, Type> PetTypeMap = new Dictionary<string, Type>();
        internal static Dictionary<string, Type> HorseTypeMap = new Dictionary<string, Type>();

        // Ridden horse holder
        internal static List<Horse> BeingRidden = new List<Horse>();
        // Last known FarmAnimal count
        internal static int AnimalCount = 0;
        // Test to display on tooltip for Pet or Horse, if any
        internal static string HoverText;

        internal static bool AssetsLoaded = false;




        /************************
        ** Public methods
        *************************/

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // SMAPI helpers
            ModEntry.SHelper = helper;
            ModEntry.SMonitor = this.Monitor;

            // Config settings
            Config = this.Helper.ReadConfig<ModConfig>();

            // SMAPI console command handler
            Commander = new CommandHandler(this);
            // Pet and Horse creation handler
            Creator = new CreationHandler(this);
            // Save/Load logic handler
            SaverLoader = new SaveLoadHandler(this);

            // Event Listeners
            helper.Events.GameLoop.SaveLoaded += SaveLoadHandler.Setup;
            helper.Events.GameLoop.SaveLoaded += SaveLoadHandler.LoadData;
            helper.Events.GameLoop.Saving += SaveLoadHandler.SaveData;
            helper.Events.GameLoop.Saved += SaveLoadHandler.LoadData;
            helper.Events.GameLoop.ReturnedToTitle += SaveLoadHandler.StopUpdateChecks;

            helper.Events.GameLoop.DayStarted += Creator.ProcessNewDay;
            helper.Events.GameLoop.DayEnding += Creator.ProcessEndDay;
            helper.Events.Display.RenderingHud += RenderHoverTooltip;


            // SMAPI Commands
            helper.ConsoleCommands.Add("list_creatures", $"Lists the creature IDs and skin IDs of the given type.\n(Options: '{string.Join("', '", CommandHandler.CreatureGroups)}', or a specific animal type (such as bluechicken))", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("randomize_all_skins", "Randomizes the skins for every farm animal, pet, and horse on the farm.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("randomize_skin", "Randomizes the skin for the given creature. Call `randomize_skin <animal/pet/horse> <creature ID>`. To find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("set_skin", "Sets the skin of the given creature to the given skin ID. Call `set_skin <skin ID> <animal/pet/horse> <creature ID>`. To find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("corral_horses", "Warp all horses to the farm's stable, giving you the honor of being a clown car chauffeur.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("horse_whistle", "Summons one of the player's horses to them. Can be called with a horse's ID to call a specific horse. To find a horse's ID, call `list_creatures horse`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("sell", "Used to give away one of your pets or horses. Call `sell <pet/horse> <creature ID>`. To find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);

            // DEBUG
            if (Config.DebuggingMode)
            {
                helper.ConsoleCommands.Add("debug_reset", "DEBUG: ** WARNING ** Resets all skins and creature IDs, but ensures that all creatures are properly in the Adopt & Skin system.", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_skinmaps", "DEBUG: Prints all info in current skin maps", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_idmaps", "DEBUG: Prints AnimalLongToShortIDs", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_pets", "DEBUG: Print the information for every Pet instance on the map", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_horses", "DEBUG: Print the information for every Horse instance on the map", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_find", "DEBUG: Locate the creature with the given ID. Call `debug_find <horse/pet/animal> <creature ID>`.", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("summon_stray", "DEBUG: Summons a new stray at Marnie's.", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("summon_horse", "DEBUG: Summons a wild horse. Somewhere.", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_clearunowned", "DEBUG: Removes any wild horses or strays that exist, to clear out glitched extras", Commander.OnCommandReceived);
            }

        }


        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/mail"))
                return true;
            return false;
        }


        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            // Add the letter Marnie sends regarding the stray animals
            if (asset.AssetNameEquals("Data/mail"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data.Add("MarnieStrays", "Dear @,   ^   Since I came over with the stray that I found, I've been running across a lot more of them! Poor things, I think they're escapees from nearby Weatherwoods, that town that burned down a few weeks back.   ^   Anyway, I'm adopting out the ones that I happen across! Just stop by during my normal hours if you'd like to bring home a friend for your newest companion.   ^   -Marnie");
            }
        }


        /// <summary>Standardize internal types and file names to have no spaces and to be entirely lowercase. </summary>
        public static string Sanitize(string input)
        {
            input = input.ToLower().Replace(" ", "");
            return string.IsInterned(input) ?? input;
        }


        /// <summary>Returns an enumerable list containing all horses.</summary>
        public static IEnumerable<Horse> GetHorses()
        {
            foreach (NPC npc in Utility.getAllCharacters())
                if (npc is Horse horse)
                    yield return horse;
        }


        /// <summary>Returns an enumerable list containing all pets.</summary>
        public static IEnumerable<Pet> GetPets()
        {
            foreach (NPC npc in Utility.getAllCharacters())
                if (npc is Pet pet)
                    yield return pet;
        }


        /// <summary>Returns an enumerable list copy of the list containing all animals on the farm.</summary>
        public static IEnumerable<FarmAnimal> GetAnimals()
        {
            Farm farm = Game1.getFarm();

            if (farm == null)
                yield break;

            foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                yield return animal;
        }






        /************************
        ** Protected / Internal
        *************************/

        /// <summary>Calls a horse that the player owns to the player's location</summary>
        internal static void CallHorse()
        {
            // Make sure that the player is calling the horse while outside
            if (!Game1.player.currentLocation.IsOutdoors)
            {
                ModEntry.SMonitor.Log("You cannot call for a horse while indoors.", LogLevel.Alert);
                Game1.chatBox.addInfoMessage("You hear your Grandfather's voice echo in your head.. \"Now is not the time to use that.\"");
                return;
            }

            // Teleport the first horse you find that the player actually owns
            foreach (Horse taxi in GetHorses())
            {
                if (HorseSkinMap.ContainsKey(taxi.Manners))
                {
                    Game1.warpCharacter(taxi, Game1.player.currentLocation, Game1.player.getTileLocation());
                    return;
                }
            }

            // Player doesn't own a horse yet
            ModEntry.SMonitor.Log("You do not own any horse that you can call.", LogLevel.Alert);
            Game1.chatBox.addInfoMessage("Your Grandfather's voice echoes in your head.. \"You aren't yet ready for this gift.\"");
        }


        /// <summary>Calls all horses owned by the player to return to the player's stable</summary>
        internal static void CorralHorses()
        {
            // Find the farm's stable
            Stable horsehut = null;
            foreach (Building building in Game1.getFarm().buildings)
                if (building is Stable)
                    horsehut = building as Stable;

            // No stable was found on the farm
            if (horsehut == null)
            {
                ModEntry.SMonitor.Log("NOTICE: You don't have a stable to warp to!", LogLevel.Error);
                return;
            }

            // WARP THEM. WARP THEM ALL.
            int stableX = int.Parse(horsehut.tileX.ToString()) + 1;
            int stableY = int.Parse(horsehut.tileY.ToString()) + 1;
            Vector2 stableWarp = new Vector2(stableX, stableY);
            foreach (Horse horse in ModEntry.GetHorses())
            {
                if (ModEntry.HorseSkinMap.ContainsKey(horse.Manners))
                    Game1.warpCharacter(horse, "farm", stableWarp);
            }

            ModEntry.SMonitor.Log("All horses have been warped to the stable.", LogLevel.Alert);
        }


        /// <summary>Returns the creature of the given category and ID</summary>
        internal static Character GetCreature(CreatureCategory creatureCategory, long id)
        {
            switch (creatureCategory)
            {
                case CreatureCategory.Horse:
                    foreach (Horse horse in GetHorses())
                        if (id == horse.Manners)
                            return horse;
                    break;

                case CreatureCategory.Pet:
                    foreach (Pet pet in GetPets())
                        if (id == pet.Manners)
                            return pet;
                    break;

                case CreatureCategory.Animal:
                    foreach (FarmAnimal animal in GetAnimals())
                        if (id == animal.myID.Value)
                            return animal;
                    break;

                default:
                    return null;
            }

            return null;
        }


        /// <summary>Returns the animal skin associated with the skin ID. Assumes the ID is in range.</summary>
        internal static AnimalSkin GetSkinFromID(string type, int ID)
        {
            // Creature does not have a skin set for it
            if (ID == 0)
                return null;

            type = Sanitize(type);

            if (PetAssets.ContainsKey(type))
                return PetAssets[type][ID - 1];
            else if (HorseAssets.ContainsKey(type))
                return HorseAssets[type][ID - 1];
            else if (AnimalAssets.ContainsKey(type))
                return AnimalAssets[type][ID - 1];

            // Creature type not handled by Adopt & Skin. No skin to return.
            return null;
        }


        /// <summary>
        /// Given the ID for an animal, pet, or horse, and that creature's type, return the AnimalSkin mapped to that creature.
        /// Return null if the creature type isn't handled.
        /// </summary>
        internal AnimalSkin GetSkin(Character creature)
        {
            switch (creature)
            {
                case Horse horse:
                    // No horse skins are loaded
                    if (HorseAssets[Sanitize(horse.GetType().Name)].Count == 0)
                        return null;

                    // A wild horse is being checked
                    if (horse.Manners == WildHorse.WildID)
                        return GetSkinFromID(horse.GetType().Name, Creator.HorseInfo.SkinID);
                    // Horse is not in system
                    else if (horse.Manners == 0 || !HorseSkinMap.ContainsKey(horse.Manners))
                    {
                        this.Monitor.Log($"Horse not in system: {horse.Name}", LogLevel.Error);
                        return null;
                    }

                    // Ensure skin ID given is a valid number for the given horse type
                    int horseSkinID = HorseSkinMap[horse.Manners];
                    if (horseSkinID < 1 || horseSkinID > HorseAssets[Sanitize(horse.GetType().Name)].Count)
                    {
                        this.Monitor.Log($"{horse.Name}'s skin ID no longer exists in `/assets/skins`. Skin will be randomized.", LogLevel.Alert);
                        horseSkinID = SetRandomSkin(horse);
                    }

                    // Horse has a skin. Return it.
                    return GetSkinFromID(horse.GetType().Name, horseSkinID);

                case Pet pet:
                    string petType = Sanitize(pet.GetType().Name);

                    // Break out of unhandled types
                    if (!ModApi.GetHandledPetTypes().Contains(petType))
                        break;
                    else if (PetAssets[Sanitize(pet.GetType().Name)].Count == 0)
                        return null;

                    // A stray pet is being checked
                    if (pet.Manners == Stray.StrayID)
                        return GetSkinFromID(pet.GetType().Name, Creator.StrayInfo.SkinID);
                    // Pet is not in system
                    else if (pet.Manners == 0 || !PetSkinMap.ContainsKey(pet.Manners))
                    {
                        this.Monitor.Log($"Pet not in system: {pet.Name}", LogLevel.Error);
                        return null;
                    }

                    // Ensure skin ID given is a current valid number for the given pet type
                    int petSkinID = PetSkinMap[pet.Manners];
                    if (petSkinID < 1 || petSkinID > PetAssets[petType].Count)
                    {
                        this.Monitor.Log($"{pet.Name}'s skin ID no longer exists in `/assets/skins`. Skin will be randomized.", LogLevel.Alert);
                        petSkinID = SetRandomSkin(pet);
                    }
                    return GetSkinFromID(petType, petSkinID);

                case FarmAnimal animal:
                    string animalType = Sanitize(animal.type.Value);

                    // Break out of unhandled types
                    if (!ModApi.GetHandledAnimalTypes().Contains(animalType))
                        break;
                    else if (AnimalAssets[Sanitize(animal.type.Value)].Count == 0)
                        return null;

                    // Set sub-type if applicable
                    if (ModApi.HasBabySprite(animalType) && animal.age.Value < animal.ageWhenMature.Value)
                        animalType = "baby" + animalType;
                    else if (ModApi.HasShearedSprite(animalType) && animal.currentProduce.Value <= 0)
                        animalType = "sheared" + animalType;

                    // Animal is not in system
                    if (!AnimalSkinMap.ContainsKey(animal.myID.Value))
                        return null;

                    // Ensure skin ID given is a current valid number for the given animal type
                    int animalSkinID = AnimalSkinMap[animal.myID.Value];
                    if (animalSkinID < 1 || animalSkinID > AnimalAssets[animalType].Count)
                    {
                        this.Monitor.Log($"{animal.Name}'s skin ID is no longer exists in `/assets/skins`. Skin will be randomized.", LogLevel.Alert);
                        animalSkinID = SetRandomSkin(animal);
                    }
                    return GetSkinFromID(animalType, animalSkinID);

                default:
                    break;
            }
            return null;
        }


        /// <summary>Assigns a new random skin to the given creature.</summary>
        /// <param name="creature">The animal, pet, or horse to assign a new random skin.</param>
        internal int SetRandomSkin(Character creature)
        {
            switch (creature)
            {
                case Horse horse:
                    return SetSkin(horse, Randomizer.Next(1, HorseAssets[Sanitize(horse.GetType().Name)].Count + 1));

                case Pet pet:
                    return SetSkin(pet, Randomizer.Next(1, PetAssets[Sanitize(pet.GetType().Name)].Count + 1));

                case FarmAnimal animal:
                    return SetSkin(animal, Randomizer.Next(1, AnimalAssets[Sanitize(animal.type.Value)].Count + 1));

                default:
                    return 0;
            }
        }


        /// <summary>Sets the skin of the given creature with the given skin ID</summary>
        internal int SetSkin(Character creature, int skinID)
        {
            switch (creature)
            {
                case Horse horse:
                    if (HorseAssets[Sanitize(horse.GetType().Name)].Count == 0)
                        return 0;
                    HorseSkinMap[horse.Manners] = skinID;
                    break;

                case Pet pet:
                    if (PetAssets[Sanitize(pet.GetType().Name)].Count == 0)
                        return 0;
                    PetSkinMap[pet.Manners] = skinID;
                    break;

                case FarmAnimal animal:
                    if (AnimalAssets[Sanitize(animal.type.Value)].Count == 0)
                        return 0;
                    AnimalSkinMap[animal.myID.Value] = skinID;
                    break;

                default:
                    break;
            }

            UpdateSkin(creature);
            return skinID;
        }


        /// <summary>Returns an unused Short ID value for the given creature type to use.</summary>
        private int GetUnusedShortID(CreatureCategory creatureCategory)
        {
            int newShortID = 1;

            switch (creatureCategory)
            {
                case CreatureCategory.Horse:
                    List<int> usedHorseIDs = new List<int>();
                    foreach (Horse horse in GetHorses())
                        usedHorseIDs.Add(horse.Manners);

                    while (usedHorseIDs.Contains(newShortID))
                        newShortID++;

                    break;

                case CreatureCategory.Pet:
                    List<int> usedPetIDs = new List<int>();
                    foreach (Pet pet in GetPets())
                        usedPetIDs.Add(pet.Manners);

                    while (usedPetIDs.Contains(newShortID))
                        newShortID++;

                    break;

                case CreatureCategory.Animal:
                    while (AnimalShortToLongIDs.ContainsKey(newShortID))
                        newShortID++;
                    break;

                default:
                    break;
            }

            return newShortID;
        }






        /************************
        ** Save/Load/Update logic
        *************************/

        internal static void UpdateScream(object sender, UpdateTickedEventArgs e)
        {
            SMonitor.Log("UPDATE TICK", LogLevel.Alert);
        }

        internal void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {

            // -- Check if a horse being ridden has been dismounted. If so, re-add it to the map. --
            List<Horse> dismounted = new List<Horse>();
            foreach (Horse horse in BeingRidden)
            {
                if (horse.rider == null)
                {
                    GameLocation loc = horse.currentLocation;
                    Game1.removeThisCharacterFromAllLocations(horse);
                    loc.addCharacter(horse);
                    dismounted.Add(horse);
                }
                else if (Config.OneTileHorse)
                {
                    horse.squeezeForGate();
                }
            }
            // Remove any dismounted horses from the list of horses currently being ridden
            if (dismounted.Count > 0)
                foreach (Horse horse in dismounted)
                    BeingRidden.Remove(horse);


            // -- Check that animal list is up to date. If not, add/remove animal in system. --
            if (Game1.getFarm() != null && AnimalCount != Game1.getFarm().getAllFarmAnimals().Count)
            {
                List<long> existingAnimals = new List<long>();
                List<FarmAnimal> newAnimals = new List<FarmAnimal>();

                // Check for new animals and populate lists containing existing and new animals
                foreach (FarmAnimal animal in GetAnimals())
                {
                    if (!AnimalSkinMap.ContainsKey(animal.myID.Value))
                        newAnimals.Add(animal);
                    else
                        existingAnimals.Add(animal.myID.Value);
                }

                // Check for removed animals
                List<long> animalsToRemove = new List<long>();
                foreach (long id in AnimalSkinMap.Keys)
                    if (!existingAnimals.Contains(id))
                        animalsToRemove.Add(id);
                // Remove animals no longer on farm
                foreach (long id in animalsToRemove)
                    RemoveCreature(CreatureCategory.Animal, id);

                // Add new animals
                foreach (FarmAnimal animal in newAnimals)
                    AddCreature(animal);

                // Update last known animal count
                AnimalCount = Game1.getFarm().getAllFarmAnimals().Count;
            }


            // -- Display name tooltips when hovering over a pet or horse
            bool isHovering = false;
            Vector2 mousePos = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;

            // Show pet tooltip
            foreach (Pet pet in GetPets())
                if (IsWithinSpriteBox(mousePos, pet))
                {
                    isHovering = true;
                    HoverText = pet.displayName;
                }
            // Show horse tooltip
            foreach (Horse horse in GetHorses())
                if (IsWithinSpriteBox(mousePos, horse))
                {
                    isHovering = true;
                    HoverText = horse.displayName;
                }

            // Clear hover text when not hovering over a pet or horse
            if (!isHovering)
            {
                HoverText = null;
            }
        }


        /// <summary>Handler to remember the current horses being ridden, such that they can be manually re-added, preventing the disappearence of dismounted multihorses.</summary>
        internal static void SaveTheHorse(object sender, NpcListChangedEventArgs e)
        {
            foreach (NPC npc in e.Removed)
                if (npc is Horse horse && horse.rider != null && horse.Manners != 0)
                    BeingRidden.Add(horse);
        }


        /// <summary>Checks for the arrival of the player's first pet and adds it to the system.</summary>
        internal void CheckForFirstPet(object sender, NpcListChangedEventArgs e)
        {
            // Check for the arrival of the vanilla pet and add it to the system.
            if (PetSkinMap.Count == 0)
            {
                foreach (NPC npc in e.Added)
                    if (npc is Pet pet)
                    {
                        AddCreature(pet);
                        this.Helper.Events.World.NpcListChanged -= this.CheckForFirstPet;
                        this.Helper.Events.GameLoop.DayStarted += PlaceBedTomorrow;
                        Game1.addMailForTomorrow("MarnieStrays");
                        return;
                    }
            }
        }


        /// <summary>Checks for the arrival of the player's first horse and adds it to the system.</summary>
        internal void CheckForFirstHorse(object sender, NpcListChangedEventArgs e)
        {
            // Check for the arrival of the vanilla horse and add it to the system.
            if (HorseSkinMap.Count == 0)
            {
                foreach (NPC npc in e.Added)
                    if (npc is Horse horse)
                    {
                        // A tractor is not your first horse
                        if (horse.Name.StartsWith("tractor/"))
                            break;

                        AddCreature(horse);
                        Creator.FirstHorseReceived = true;
                        this.Helper.Events.World.NpcListChanged -= this.CheckForFirstHorse;
                        return;
                    }
            }
            // Horse already known
            else
            {
                Creator.FirstHorseReceived = true;
                this.Helper.Events.World.NpcListChanged -= this.CheckForFirstHorse;
                return;
            }
        }


        /// <summary>Helper to place the pet bed in Marnie's on the day after the first pet is received</summary>
        internal static void PlaceBedTomorrow(object sender, DayStartedEventArgs e)
        {
            Creator.PlaceBetBed();
            // Remove self from day update after bed has been placed
            SHelper.Events.GameLoop.DayStarted -= PlaceBedTomorrow;
        }


        /// <summary>Check for the Horse Whistle or Corral hotkey to be pressed, and execute the function if necessary</summary>
        internal static void HotKeyCheck(object sender, ButtonReleasedEventArgs e)
        {
            // Only check for hotkeys if the player is not in a menu
            if (!Context.IsPlayerFree)
                return;

            if (e.Button.ToString().ToLower() == Config.HorseWhistleKey.ToLower())
            {
                CallHorse();
            }
            if (e.Button.ToString().ToLower() == Config.CorralKey.ToLower())
            {
                CorralHorses();
            }
        }


        /// <summary>Refreshes the given animal, pet, or horse's skin texture with the one Adopt & Skin has saved for it.</summary>
        internal void UpdateSkin(Character creature)
        {
            switch (creature)
            {
                case Horse horse:
                    if (Creator.HorseInfo != null && horse.Manners == WildHorse.WildID)
                    {
                        horse.Sprite = new AnimatedSprite(GetSkinFromID(horse.GetType().Name, Creator.HorseInfo.SkinID).AssetKey, 0, 32, 32);
                        break;
                    }
                    AnimalSkin horseSkin = GetSkin(horse);
                    if (horseSkin != null && horse.Sprite.textureName.Value != horseSkin.AssetKey)
                        horse.Sprite = new AnimatedSprite(horseSkin.AssetKey, 7, 32, 32);
                    break;

                case Pet pet:
                    if (Creator.StrayInfo != null && pet.Manners == Stray.StrayID)
                    {
                        pet.Sprite = new AnimatedSprite(GetSkinFromID(pet.GetType().Name, Creator.StrayInfo.SkinID).AssetKey, 28, 32, 32);
                        break;
                    }
                    AnimalSkin petSkin = GetSkin(pet);
                    if (petSkin != null && pet.Sprite.textureName.Value != petSkin.AssetKey)
                        pet.Sprite = new AnimatedSprite(petSkin.AssetKey, 28, 32, 32);
                    break;

                case FarmAnimal animal:
                    AnimalSkin animalSkin = GetSkin(animal);
                    if (animalSkin != null && animal.Sprite.textureName.Value != animalSkin.AssetKey)
                        animal.Sprite = new AnimatedSprite(animalSkin.AssetKey, 0, animal.frontBackSourceRect.Width, animal.frontBackSourceRect.Height);
                    break;

                default:
                    break;
            }
        }


        /// <summary>Adds a creature into the Adopt & Skin system.</summary>
        /// <param name="creature">The StardewValley.Character type creature (animal, pet, or horse) to add to the system</param>
        /// <param name="skin">Optional parameter. Given when a creature is being created with a predetermined skin.</param>
        internal void AddCreature(Character creature, int skin = 0)
        {
            switch (creature)
            {
                case Horse horse:
                    // Horse is already in system
                    if (HorseSkinMap.ContainsKey(horse.Manners))
                        break;

                    // Assign a ShortID to the horse
                    horse.Manners = GetUnusedShortID(CreatureCategory.Horse);

                    // Set horse's skin
                    if (skin == 0)
                        HorseSkinMap[horse.Manners] = SetRandomSkin(horse);
                    else
                        SetSkin(horse, skin);
                    break;

                case Pet pet:
                    // Pet is already in the system
                    if (PetSkinMap.ContainsKey(pet.Manners))
                        break;

                    // Assign a ShortID to the pet
                    pet.Manners = GetUnusedShortID(CreatureCategory.Pet);

                    // Set pet's skin
                    if (skin == 0)
                        PetSkinMap[pet.Manners] = SetRandomSkin(pet);
                    else
                        SetSkin(pet, skin);
                    break;

                case FarmAnimal animal:
                    // Animal is already in system
                    if (AnimalSkinMap.ContainsKey(animal.myID.Value))
                        break;

                    // Assign a ShortID to the animal
                    int shortID = GetUnusedShortID(CreatureCategory.Animal);
                    AnimalLongToShortIDs[animal.myID.Value] = shortID;
                    AnimalShortToLongIDs[shortID] = animal.myID.Value;

                    // Set animal's skin
                    if (skin == 0)
                        AnimalSkinMap[animal.myID.Value] = SetRandomSkin(animal);
                    else
                        SetSkin(animal, skin);
                    break;

                default:
                    break;
            }
        }


        /// <summary>Removes a creature from the Adopt & Skin system.</summary>
        internal void RemoveCreature(CreatureCategory category, long id)
        {
            switch (category)
            {
                case CreatureCategory.Horse:
                    // Horse isn't in the system
                    if (!HorseSkinMap.ContainsKey(id))
                        return;

                    HorseSkinMap.Remove(id);
                    break;

                case CreatureCategory.Pet:
                    // Pet isn't in the system
                    if (!PetSkinMap.ContainsKey(id))
                        return;

                    PetSkinMap.Remove(id);
                    break;

                case CreatureCategory.Animal:
                    // Animal isn't in the system
                    if (!AnimalSkinMap.ContainsKey(id))
                        return;

                    // Remove from ShortID lists
                    int shortID = AnimalLongToShortIDs[id];
                    AnimalLongToShortIDs.Remove(id);
                    AnimalShortToLongIDs.Remove(shortID);

                    AnimalSkinMap.Remove(id);
                    break;

                default:
                    break;
            }
        }






        /*****************
        ** Name Tooltip
        ******************/

        /// <summary>Renders the name hover tooltip if a pet or horse is being hovered over</summary>
        private void RenderHoverTooltip(object sender, RenderingHudEventArgs e)
        {
            if (Context.IsPlayerFree && HoverText != null)
                this.DrawSimpleTooltip(Game1.spriteBatch, HoverText, Game1.smallFont);
        }


        /// <summary>Returns true if the given mouse cursor location is over the given pet or horse's location</summary>
        private bool IsWithinSpriteBox(Vector2 mousePos, Character creature)
        {
            // ** MAY NEED TO CHANGE FOR MULTIPLAYER **
            if (Game1.player.currentLocation == creature.currentLocation &&
                (int)mousePos.X >= creature.getLeftMostTileX().X && (int)mousePos.X <= creature.getRightMostTileX().X &&
                    (int)mousePos.Y <= creature.getTileY() && (int)mousePos.Y >= creature.getTileY() - 1)
                return true;

            return false;
        }


        /// <summary>Draw tooltip at the cursor position with the given message.</summary>
        /// <param name="b">The sprite batch to update.</param>
        /// <param name="hoverText">The tooltip text to display.</param>
        /// <param name="font">The tooltip font.</param>
        private void DrawSimpleTooltip(SpriteBatch b, string hoverText, SpriteFont font)
        {
            Vector2 textSize = font.MeasureString(hoverText);
            int width = (int)textSize.X + Game1.tileSize / 2;
            int height = Math.Max(60, (int)textSize.Y + Game1.tileSize / 2);
            int x = Game1.getOldMouseX() + Game1.tileSize / 2;
            int y = Game1.getOldMouseY() - Game1.tileSize / 2;
            if (x + width > Game1.viewport.Width)
            {
                x = Game1.viewport.Width - width;
                y += Game1.tileSize / 4;
            }
            if (y + height < 0)
            {
                x += Game1.tileSize / 4;
                y = Game1.viewport.Height + height;
            }
            IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            if (hoverText.Length > 1)
            {
                Vector2 tPosVector = new Vector2(x + (Game1.tileSize / 4), y + (Game1.tileSize / 4 + 4));
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(0f, 2f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector + new Vector2(2f, 0f), Game1.textShadowColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                b.DrawString(font, hoverText, tPosVector, Game1.textColor * 0.9f, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

    }
}
