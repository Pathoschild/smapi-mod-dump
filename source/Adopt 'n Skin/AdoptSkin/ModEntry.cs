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

    // ** The TODO List **
    // 
    // - Add support for custom animal types (Ento added an ExtraTypes to the Config, look there)
    // - Figure out pet spawn before moving maps (check to see if pet is already on map? Will this cause cuddle puddle?)
    // - Android control compat
    // - Is there a keyboard interact button to do compat for?
    // - Baby stage pets and horses

    //
    // TEST - RECENTLY ADDED:
    // 
    // - Strays/WildHorses are controller interactable
    // - Skins can be named non-continuously
    // - Randomize_skins >> Work as list_creatures does, use variables

    public class ModEntry : Mod, IAssetEditor
    {
        /************************
        ** Fields
        *************************/

        private static readonly Random Randomizer = new Random();
        public enum CreatureCategory { Horse, Pet, Animal, Null };
        internal static ModConfig Config;

        internal static readonly int PetSpriteStartFrame = 28;
        internal static readonly int HorseSpriteStartFrame = 7;
        internal static readonly int AnimalSpriteStartFrame = 0;

        internal static HoverBox ToolTip = new HoverBox();






        /************************
       ** Accessors
       *************************/

        // SMAPI Modding helpers
        internal static IModHelper SHelper;
        internal static IMonitor SMonitor;

        // Internal helpers
        internal static CommandHandler Commander;
        internal static CreationHandler Creator;
        internal static SaveLoadHandler SaverLoader;

        // Mod integration
        internal static BFAV226Integrator BFAV226Worker;
        internal static BFAV300Integrator BFAV300Worker;

        // Skin assets
        internal static Dictionary<string, Dictionary<int, AnimalSkin>> Assets = new Dictionary<string, Dictionary<int, AnimalSkin>>();

        // Skin map
        internal static Dictionary<long, int> SkinMap = new Dictionary<long, int>();

        // ID maps
        internal static Dictionary<long, CreatureCategory> IDToCategory = new Dictionary<long, CreatureCategory>();
        internal static Dictionary<long, int> AnimalLongToShortIDs = new Dictionary<long, int>();
        internal static Dictionary<int, long> AnimalShortToLongIDs = new Dictionary<int, long>();

        // Pet and Horse to type maps
        internal static Dictionary<string, Type> PetTypeMap = new Dictionary<string, Type>();
        internal static Dictionary<string, Type> HorseTypeMap = new Dictionary<string, Type>();
        // ** TODO: Would it benefit from making animal similar?

        // Ridden horse holder
        internal static List<Horse> BeingRidden = new List<Horse>();
        // Last known FarmAnimal count
        internal static int AnimalCount = 0;

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

            // Internal helpers
            Commander = new CommandHandler(this);
            Creator = new CreationHandler(this);
            SaverLoader = new SaveLoadHandler(this);

            // Event Listeners
            helper.Events.GameLoop.SaveLoaded += SaveLoadHandler.Setup;
            helper.Events.GameLoop.SaveLoaded += SaveLoadHandler.LoadData;
            helper.Events.GameLoop.Saving += SaveLoadHandler.SaveData;
            helper.Events.GameLoop.Saved += SaveLoadHandler.LoadData;
            helper.Events.GameLoop.ReturnedToTitle += SaveLoadHandler.StopUpdateChecks;

            helper.Events.GameLoop.DayStarted += Creator.ProcessNewDay;
            helper.Events.GameLoop.DayEnding += Creator.ProcessEndDay;

            // SMAPI Commands
            helper.ConsoleCommands.Add("list_creatures", $"Lists the creature IDs and skin IDs of the given type.\n(Options: '{string.Join("', '", CommandHandler.CreatureGroups)}', or a specific animal type (such as bluechicken))", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("randomize_skin", $"Randomizes the skin for the given group of creatures or the creature with the given ID. Call `randomize_skin <creature group or creature ID>`.\nCallable creature groups: {string.Join(",", CommandHandler.CreatureGroups)}, or an adult creature type\nTo find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("set_skin", "Sets the skin of the given creature to the given skin ID. Call `set_skin <skin ID> <creature ID>`. To find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("corral_horses", "Warp all horses to the farm's stable, giving you the honor of being a professional clown car chauffeur.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("horse_whistle", "Summons one of the player's horses to them. Can be called with a horse's ID to call a specific horse. To find a horse's ID, call `list_creatures horse`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("sell", "Used to give away one of your pets or horses. Call `sell <creature ID>`. To find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);

            // DEBUG
            if (Config.DebuggingMode)
            {
                helper.ConsoleCommands.Add("debug_reset", "DEBUG: ** WARNING ** Resets all skins and creature IDs, but ensures that all creatures are properly in the Adopt & Skin system.", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_skinmaps", "DEBUG: Prints all info in current skin maps", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_idmaps", "DEBUG: Prints AnimalLongToShortIDs", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_pets", "DEBUG: Print the information for every Pet instance on the map", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_horses", "DEBUG: Print the information for every Horse instance on the map", Commander.OnCommandReceived);
                helper.ConsoleCommands.Add("debug_find", "DEBUG: Locate the creature with the given ID. Call `debug_find <creature ID>`.", Commander.OnCommandReceived);
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









        /************************
        ** Skin Handling
        *************************/

        /// <param name="creature">The creature (Pet, Horse, or FarmAnimal) to set the skin for</param>
        /// <param name="skinID">The file ID of the skin to set.</param>
        internal static int SetSkin(Character creature, int skinID)
        {
            if (!ModApi.IsInDatabase(creature) || !ModApi.HasSkins(ModApi.GetInternalType(creature)))
                return 0;

            SkinMap[GetLongID(creature)] = skinID;
            UpdateSkin(creature);
            return skinID;
        }

        internal static int RandomizeSkin(Character creature)
        {
            return SetSkin(creature, GetRandomSkin(ModApi.GetInternalType(creature)));
        }

        internal static int GetRandomSkin(string type)
        {
            type = Sanitize(type);
            if (!ModApi.HasSkins(type))
                return 0;
            // ** TODO: Find out why erroring with stray spawn, issue in GetSkin likely.
            // Issue related to lack of cat skins? Fix this
            // Android: A Button works, but not right click
            int randomLookup = Randomizer.Next(0, Assets[type].Keys.Count);
            return Assets[type].ElementAt(randomLookup).Key;
        }

        internal static AnimalSkin GetSkin(Character creature)
        {
            if (!ModApi.HasSkins(ModApi.GetInternalType(creature)) || !ModApi.IsInDatabase(creature))
                return null;
                

            int skinID;
            string type = ModApi.GetInternalType(creature);
            
            // Take care of Strays and WildHorses
            if (Creator.StrayInfo != null && ModApi.IsStray(creature))
                skinID = Creator.StrayInfo.SkinID;
            else if (Creator.HorseInfo != null && ModApi.IsWildHorse(creature))
                skinID = Creator.HorseInfo.SkinID;
            // Take care of FarmAnimal subtypes
            else if (creature is FarmAnimal animal)
            {
                skinID = SkinMap[GetLongID(creature)];

                if (ModApi.HasBabySprite(type) && animal.age.Value < animal.ageWhenMature.Value)
                    type = "baby" + type;
                else if (ModApi.HasShearedSprite(type) && animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                    type = "sheared" + type;
            }
            // Take care of owned Pets and Horses
            else
                skinID = SkinMap[GetLongID(creature)];


            if (skinID == 0)
                return null;
            else if (!Assets[ModApi.GetInternalType(creature)].ContainsKey(skinID))
            {
                ModEntry.SMonitor.Log($"{creature.Name}'s skin ID no longer exists in `/assets/skins`. Skin will be randomized.", LogLevel.Alert);
                skinID = RandomizeSkin(creature);
            }

            return GetSkin(type, skinID);
        }

        internal static AnimalSkin GetSkin(string type, int skinID) { return Assets[Sanitize(type)][skinID]; }









        /************************
        ** ID Handling
        *************************/

        internal static long GetLongID(Character creature)
        {
            if (creature is Pet pet)
                return pet.Manners;
            else if (creature is Horse horse)
                return horse.Manners;
            else if (creature is FarmAnimal animal)
                return animal.myID.Value;
            return 0;
        }

        internal static int GetShortID(Character creature)
        {
            if (!ModApi.IsInDatabase(creature))
                return 0;

            if (creature is Pet pet)
                return pet.Manners;
            else if (creature is Horse horse)
                return horse.Manners;
            else if (creature is FarmAnimal animal && AnimalLongToShortIDs.ContainsKey(GetLongID(animal)))
                return AnimalLongToShortIDs[GetLongID(animal)];
            return 0;
        }

        private static void SetShortID(Character creature, int shortID)
        {
            switch (creature)
            {
                case Pet pet:
                    pet.Manners = shortID;
                    return;

                case Horse horse:
                    horse.Manners = shortID;
                    return;

                case FarmAnimal animal:
                    AnimalLongToShortIDs[animal.myID.Value] = shortID;
                    AnimalShortToLongIDs[shortID] = animal.myID.Value;
                    return;

                default:
                    return;
            }
        }

        /// <summary>Returns an unused Short ID for a new creature to use.</summary>
        private int GetUnusedShortID()
        {
            int newShortID = 1;

            // Gather all current ShortIDs
            List<int> usedIDs = new List<int>();
            foreach (Horse horse in ModApi.GetHorses())
                usedIDs.Add(GetShortID(horse));
            foreach (Pet pet in ModApi.GetPets())
                usedIDs.Add(GetShortID(pet));
            foreach (FarmAnimal animal in ModApi.GetAnimals())
                usedIDs.Add(GetShortID(animal));

            // Find an unused ShortID and return it
            while (usedIDs.Contains(newShortID))
                newShortID++;
            return newShortID;
        }

        private static Character GetCreature(long longID)
        {
            // Check that LongID belongs to a creature in the database, or is otherwise a Stray or WildHorse.
            if (IDToCategory.ContainsKey(longID))
            {
                switch (IDToCategory[longID])
                {
                    case CreatureCategory.Pet:
                        foreach (Pet pet in ModApi.GetPets())
                            if (GetLongID(pet) == longID)
                                return pet;
                        return null;

                    case CreatureCategory.Horse:
                        foreach (Horse horse in ModApi.GetHorses())
                            if (GetLongID(horse) == longID)
                                return horse;
                        return null;

                    case CreatureCategory.Animal:
                        foreach (FarmAnimal animal in ModApi.GetAnimals())
                            if (GetLongID(animal) == longID)
                                return animal;
                        return null;

                    default:
                        return null;
                }
            }
            else
            {
                if (ModApi.IsStray(longID))
                {
                    if (Creator.StrayInfo != null)
                        return Creator.StrayInfo.PetInstance;
                }
                else if (ModApi.IsWildHorse(longID))
                {
                    if (Creator.HorseInfo != null)
                        return Creator.HorseInfo.HorseInstance;
                }
                return null;
            }
        }


        internal static Character GetCreatureFromShortID(int shortID)
        {
            if (AnimalShortToLongIDs.ContainsKey(shortID))
                return GetCreature(AnimalShortToLongIDs[shortID]);
            // The ShortIDs of Pets and Horses are also LongIDs
            else
                return GetCreature(shortID);
        }






        /****************************
        ** Additional Functionality
        *****************************/

        /// <summary>Calls a horse that the player owns to the player's location</summary>
        /// <returns>Returns true if a horse was successfully called.</returns>
        internal static bool CallHorse(long id = 0)
        {
            // Make sure that the player is calling the horse while outside
            if (!Game1.player.currentLocation.IsOutdoors)
            {
                ModEntry.SMonitor.Log("You cannot call for a horse while indoors.", LogLevel.Alert);
                Game1.chatBox.addInfoMessage("You hear your Grandfather's voice echo in your head.. \"Now is not the time to use that.\"");
                return false;
            }

            // Teleport the first horse you find that the player actually owns
            foreach (Horse taxi in ModApi.GetHorses())
                if (ModApi.IsInDatabase(taxi) && (id == 0 || id == GetLongID(taxi)))
                {
                    Game1.warpCharacter(taxi, Game1.player.currentLocation, Game1.player.getTileLocation());
                    return true;
                }

            return false;
        }


        /// <summary>Calls all horses owned by the player to return to the player's stable</summary>
        internal static void CorralHorses()
        {
            // Find the farm's stable
            Stable horsehut = null;
            foreach (Building building in Game1.getFarm().buildings)
                if (building is Stable)
                    horsehut = building as Stable;

            if (horsehut != null)
            {
                // WARP THEM. WARP THEM ALL.
                int stableX = int.Parse(horsehut.tileX.ToString()) + 1;
                int stableY = int.Parse(horsehut.tileY.ToString()) + 1;
                Vector2 stableWarp = new Vector2(stableX, stableY);

                foreach (Horse horse in ModApi.GetHorses())
                    if (ModApi.IsInDatabase(horse))
                        Game1.warpCharacter(horse, "farm", stableWarp);

                ModEntry.SMonitor.Log("All horses have been warped to the stable.", LogLevel.Alert);
                return;
            }

            // Player does not own a stable
            ModEntry.SMonitor.Log("NOTICE: You don't have a stable to warp to!", LogLevel.Error);
            Game1.chatBox.addInfoMessage("Your Grandfather's voice echoes in your head.. \"You aren't yet ready for this gift.\"");
        }






        /************************
        ** Save/Load/Update logic
        *************************/

        internal void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Check if a horse needs to be re-added to the map
            HorseDismountedCheck();

            // Make sure A&S database is up-to-date
            AnimalListChangeCheck();

            // Display name tooltip if necessary
            ToolTip.HoverCheck();
        }


        /// <summary>Update BeingRidden, such that horses can be manually re-added on dismount, preventing the disappearence of dismounted multihorses.</summary>
        internal static void HorseMountedCheck(object sender, NpcListChangedEventArgs e)
        {
            foreach (NPC npc in e.Removed)
                if (npc is Horse horse && horse.rider != null && horse.Manners != 0)
                {
                    BeingRidden.Add(horse);
                    if (Config.OneTileHorse)
                        horse.squeezeForGate();
                }
        }


        /// <summary>Checks horses known to be currently ridden and re-adds them to the map if they've been dismounted.</summary>
        internal static void HorseDismountedCheck()
        {
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
            }
            // Remove any dismounted horses from the list of horses currently being ridden
            if (dismounted.Count > 0)
                foreach (Horse horse in dismounted)
                    BeingRidden.Remove(horse);
        }


        /// <summary>Determines whether an animal has been removed or added in the game, and either removes or adds it to the A&S database.</summary>
        internal void AnimalListChangeCheck()
        {
            if (Game1.getFarm() != null && AnimalCount != Game1.getFarm().getAllFarmAnimals().Count)
            {
                List<long> existingAnimalIDs = new List<long>();
                List<FarmAnimal> newAnimals = new List<FarmAnimal>();

                // Check for new animals and populate lists containing existing and new animals
                foreach (FarmAnimal animal in ModApi.GetAnimals().ToList())
                {
                    if (!ModApi.IsInDatabase(animal))
                        newAnimals.Add(animal);
                    else
                        existingAnimalIDs.Add(animal.myID.Value);
                }

                // Remove animals that no longer exist
                List<long> animalIDs = new List<long>(SkinMap.Keys);
                foreach (long id in animalIDs)
                    if (IDToCategory[id] == CreatureCategory.Animal && !existingAnimalIDs.Contains(id))
                        RemoveCreature(id);

                // Add new animals
                foreach (FarmAnimal animal in newAnimals)
                    AddCreature(animal);

                // Update last known animal count
                AnimalCount = Game1.getFarm().getAllFarmAnimals().Count;
            }
        }


        internal void CheckForFirstPet(object sender, NpcListChangedEventArgs e)
        {
            // FirstPetReceived status already known
            if (Creator.FirstPetReceived)
            {
                Helper.Events.World.NpcListChanged -= CheckForFirstPet;
                Creator.PlacePetBed();
            }

            // Check for the arrival of the vanilla first pet
            else if (e != null && e.Added != null)
                foreach (NPC npc in e.Added)
                    if (npc is Pet pet)
                    {
                        AddCreature(pet);
                        Creator.FirstPetReceived = true;
                        this.Helper.Events.World.NpcListChanged -= this.CheckForFirstPet;
                        this.Helper.Events.GameLoop.DayStarted += Creator.PlacePetBedTomorrow;
                        Game1.addMailForTomorrow("MarnieStrays");
                        return;
                    }

        }


        internal void CheckForFirstHorse(object sender, NpcListChangedEventArgs e)
        {
            // FirstHorseReceived status already known
            if (Creator.FirstHorseReceived)
                Helper.Events.World.NpcListChanged -= CheckForFirstHorse;

            // Check for the arrival of the vanilla horse
            else if (e != null && e.Added != null)
                foreach (NPC npc in e.Added)
                    if (npc is Horse horse)
                    {
                        if (ModApi.IsNotATractor(horse))
                        {
                            AddCreature(horse);
                            Creator.FirstHorseReceived = true;
                            this.Helper.Events.World.NpcListChanged -= this.CheckForFirstHorse;
                            return;
                        }
                    }
        }


        /// <summary>Check for the Whistle or Corral hotkey to be pressed, and execute the function if necessary</summary>
        internal static void HotKeyCheck(object sender, ButtonReleasedEventArgs e)
        {
            // Only check for hotkeys if the player is not in a menu
            if (!Context.IsPlayerFree)
                return;

            if (e.Button.ToString().ToLower() == Config.HorseWhistleKey.ToLower())
            {
                if (!CallHorse())
                {
                    ModEntry.SMonitor.Log("You do not own any horse that you can call.", LogLevel.Alert);
                    Game1.chatBox.addInfoMessage("Your Grandfather's voice echoes in your head.. \"You aren't yet ready for this gift.\"");
                }
            }
            if (e.Button.ToString().ToLower() == Config.CorralKey.ToLower())
            {
                CorralHorses();
            }
        }


        /// <summary>Refreshes the texture of the creature's sprite if the texture it has is different from the one in the skin mapping</summary>
        internal static void UpdateSkin(Character creature)
        {
            if (!ModApi.IsInDatabase(creature) && !ModApi.IsStray(creature) && !ModApi.IsWildHorse(creature))
                return;

            AnimalSkin skin = GetSkin(creature);
            if (skin != null && creature.Sprite.textureName.Value != skin.AssetKey)
            {
                int[] spriteInfo = ModApi.GetSpriteInfo(creature);
                creature.Sprite = new AnimatedSprite(skin.AssetKey, spriteInfo[0], spriteInfo[1], spriteInfo[2]);
            }
        }


        /// <summary>Adds the given Pet, Horse, or FarmAnimal into the A&S database</summary>
        internal void AddCreature(Character creature, int skinID = 0)
        {
            string type = ModApi.GetInternalType(creature);
            if (ModApi.IsInDatabase(creature) || !ModApi.IsRegisteredType(type))
                return;

            // Set internal IDs
            SetShortID(creature, GetUnusedShortID());
            IDToCategory[GetLongID(creature)] = ModApi.GetCreatureCategory(type);

            // Give a skin
            if (skinID != 0)
                SetSkin(creature, skinID);
            else
                RandomizeSkin(creature);
        }


        /// <summary>Removes the Pet, Horse, or FarmAnimal with the given LongID from the A&S database</summary>
        internal void RemoveCreature(long longID)
        {
            if (!ModApi.IsInDatabase(longID))
                return;

            // Ensure creature is not on map
            Character creature = GetCreature(longID);
            if (creature != null && (creature is Pet || creature is Horse))
                Game1.removeThisCharacterFromAllLocations(creature as NPC);

            // Scrub internal IDs and skin mapping from system
            IDToCategory.Remove(longID);
            SkinMap.Remove(longID);
        }
    }
}
