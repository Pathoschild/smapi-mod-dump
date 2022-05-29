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
    // - Is there a keyboard interact button to do compat for for Android?
    // - Baby stage pets and horses

    //
    // TEST - RECENTLY ADDED:
    // 
    // - Strays/WildHorses are controller interactable
    // - Skins can be named non-continuously
    // - Randomize_skins >> Work as list_creatures does, use variables
    //IAssetEditor >>>> Helper.Events.Content
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
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

        // Skin assets
        internal static Dictionary<string, Dictionary<int, AnimalSkin>> Assets = new Dictionary<string, Dictionary<int, AnimalSkin>>();

        // Skin map
        internal static Dictionary<long, int> SkinMap = new Dictionary<long, int>();

        // ID maps
        internal static Dictionary<long, CreatureCategory> IDToCategory = new Dictionary<long, CreatureCategory>();
        internal static Dictionary<long, int> AnimalLongToShortIDs = new Dictionary<long, int>();
        internal static Dictionary<int, long> AnimalShortToLongIDs = new Dictionary<int, long>();

        // Pet to Owner map
        //internal static Dictionary<long, Farmer> OwnerMap = new Dictionary<long, Farmer>();
        // Horse to Owner map
        internal static Dictionary<long, Farmer> HorseOwnershipMap = new Dictionary<long, Farmer>();

        // Pet and Horse to type maps
        internal static Dictionary<string, Type> PetTypeMap = new Dictionary<string, Type>();
        internal static Dictionary<string, Type> HorseTypeMap = new Dictionary<string, Type>();
        // ** TODO: Would it benefit from making FarmAnimal similar?

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

            var loaders = Helper.Content.AssetLoaders;
            loaders.Add(this);

            WildHorse.PlayerSpecifiedSpawnMaps = new List<string>(Config.WildHorseSpawnLocations);
            // Check that Wild Horse spawn maps set in the Config file are all valid
            foreach (string loc in Config.WildHorseSpawnLocations)
            {
                if (!WildHorse.SpawningMaps.Contains(Sanitize(loc)))
                {
                    SMonitor.Log($"Invalid map \"{loc}\" is listed in Wild Horse spawning locations and will be ignored.\nMaps must all be one of: Forest, BusStop, Mountain, Town, Railroad, or Beach.", LogLevel.Warn);
                    WildHorse.PlayerSpecifiedSpawnMaps.Remove(loc);
                }
            }

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
            helper.ConsoleCommands.Add("rename", "Renames the pet or horse of the given ID to the given name. Call `rename <creature ID> \"new name\"`.", Commander.OnCommandReceived);
            helper.ConsoleCommands.Add("sell", "Used to give away one of your pets or horses. Call `sell <creature ID>`. To find a creature's ID, call `list_creatures`.", Commander.OnCommandReceived);

            // DEBUG
            if (Config.DebuggingMode)
            {
                helper.ConsoleCommands.Add("debug_asreset", "DEBUG: ** WARNING ** Resets all skins and creature IDs, but ensures that all creatures are properly in the Adopt & Skin system.", Commander.OnCommandReceived);
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

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if ((asset.AssetName.ToLower()).Contains("assets/skins"))
                return true;
            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if ((asset.AssetName.ToLower()).Contains("assets/skins"))
            {
                return SHelper.Content.Load<T>(asset.AssetName, ContentSource.ModFolder);
            }
            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
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

        /// <summary>Initiate A&S data on farm</summary>
        public void UpdateCreatureCount()
        {
            // TODO: Put this where it belongs. Use to load game, then check in UpdateTicked to make sure all farmhands and host have animals and skins known
            if (Game1.getFarm() != null)
            {
                Game1.getFarm().modData[$"{this.ModManifest.UniqueID}/count-farmanimals"] = Game1.getFarm().getAllFarmAnimals().Count.ToString();
                Game1.getFarm().modData[$"{this.ModManifest.UniqueID}/count-pets"] = ModApi.GetPets().Count().ToString();
                Game1.getFarm().modData[$"{this.ModManifest.UniqueID}/count-horses"] = ModApi.GetHorses().Count().ToString();
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
                if (SkinMap.ContainsKey(GetLongID(creature)))
                    skinID = SkinMap[GetLongID(creature)];
                else
                    skinID = RandomizeSkin(creature);

                if (ModApi.HasBabySprite(type) && animal.age.Value < animal.ageWhenMature.Value)
                    type = "baby" + type;
                else if (ModApi.HasShearedSprite(type) && animal.showDifferentTextureWhenReadyForHarvest.Value && animal.currentProduce.Value <= 0)
                    type = "sheared" + type;
            }
            // Take care of owned Pets and Horses
            else
            {
                if (SkinMap.ContainsKey(GetLongID(creature)))
                    skinID = SkinMap[GetLongID(creature)];
                else
                    skinID = RandomizeSkin(creature);
            }


            if (skinID == 0)
                return null;
            else if (!Assets[ModApi.GetInternalType(creature)].ContainsKey(skinID))
            {
                ModEntry.SMonitor.Log($"{creature.Name}'s skin ID no longer exists in `/assets/skins`. Skin will be randomized.", LogLevel.Warn);
                skinID = RandomizeSkin(creature);
            }

            return GetSkin(type, skinID);
        }

        internal static AnimalSkin GetSkin(string type, int skinID)
        {
            type = Sanitize(type);
            if (Assets.ContainsKey(type) && Assets[type].ContainsKey(skinID))
                return Assets[Sanitize(type)][skinID];
            else if (Assets.ContainsKey(type))
                return Assets[type][GetRandomSkin(type)];
            else
                return null;
        }









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
        internal static bool CallHorse(int id = 0)
        {
            // Make sure that the player is calling the horse while outside
            if (!Game1.player.currentLocation.IsOutdoors)
            {
                Game1.chatBox.addInfoMessage("You hear your Grandfather's voice echo in your head.. \"Now is not the time to use that.\"");
                return false;
            }

            // Teleport the first horse you find that the player actually owns
            List<Horse> taxis = ModApi.GetHorses().ToList();
            taxis.Reverse();
            foreach (Horse taxi in taxis)
            {
                long longID = GetLongID(taxi);

                // If the player called for a specific horse
                if (id != 0 && GetShortID(taxi) == id)
                {
                    // Ensure that the player owns this horse
                    if (HorseOwnershipMap[longID] != Game1.player)
                    {
                        SMonitor.Log($"Horse {id} ({taxi.Name}) does not belong to this player. (Belongs to ({HorseOwnershipMap[longID].Name}))", LogLevel.Error);
                        return false;
                    }

                    Game1.warpCharacter(taxi, Game1.player.currentLocation, Game1.player.getTileLocation());
                    return true;
                }
                // Otherwise
                else if (id == 0)
                {
                    // Ensure that the player owns this horse
                    if (HorseOwnershipMap[longID] != Game1.player)
                        continue;
                    else
                    {
                        Game1.warpCharacter(taxi, Game1.player.currentLocation, Game1.player.getTileLocation());
                        return true;
                    }
                }
            }

            SMonitor.Log("This player does not own any horses to call.", LogLevel.Debug);
            return false;
        }


        /// <summary>Calls all horses owned by the player to return to the player's stable</summary>
        internal static void CorralHorses()
        {
            // Gather the taxis
            List<Horse> horses = new List<Horse>();
            foreach (Horse horse in ModApi.GetHorses())
                horses.Add(horse);

            // Ensure you own at least one taxi
            if (horses.Count == 0)
            {
                ModEntry.SMonitor.Log("NOTICE: You do not own any horses", LogLevel.Error);
                Game1.chatBox.addInfoMessage("Your Grandfather's voice echoes in your head.. \"You aren't yet ready for this gift.\"");
                return;
            }

            // Find the farm's stable(s)
            List<Stable> horsehuts = new List<Stable>();
            foreach (Building building in Game1.getFarm().buildings)
                if (building is Stable stable)
                    foreach (Horse horse in horses)
                        if (stable.HorseId == horse.HorseId)
                        {
                            horsehuts.Add(building as Stable);
                            break;
                        }

            // Player does not own a stable
            if (horsehuts.Count == 0)
            {
                ModEntry.SMonitor.Log("NOTICE: You don't have a stable to warp to!", LogLevel.Error);
                Game1.chatBox.addInfoMessage("Your Grandfather's voice echoes in your head.. \"You aren't yet ready for this gift.\"");
                return;
            }

            // WARP THEM. WARP THEM ALL.
            foreach (Horse horse in horses)
                foreach (Stable stable in horsehuts)
                    if (horse.HorseId == stable.HorseId)
                    {
                        int stableX = int.Parse(stable.tileX.ToString()) + 1;
                        int stableY = int.Parse(stable.tileY.ToString()) + 1;
                        Vector2 stableWarp = new Vector2(stableX, stableY);
                        Game1.warpCharacter(horse, "farm", stableWarp);
                        break;
                    }


            ModEntry.SMonitor.Log("All horses have been warped to the stable.", LogLevel.Info);
        }






        /************************
        ** Save/Load/Update logic
        *************************/

        internal void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Check if a horse needs to be re-added to the map
            HorseDismountedCheck();
            // One-tile-horse for horses being ridden
            foreach (Horse horse in BeingRidden)
                if (Config.OneTileHorse)
                    horse.squeezeForGate();

            // Make sure A&S database is up-to-date
            AnimalListChangeCheck();

            // Display name tooltip if necessary
            if (Config.PetAndHorseNameTags)
                ToolTip.HoverCheck();
        }


        /// <summary>Update BeingRidden, such that horses can be manually re-added on dismount, preventing the disappearence of dismounted multihorses.</summary>
        internal static void HorseMountedCheck(object sender, NpcListChangedEventArgs e)
        {
            foreach (NPC npc in e.Removed)
                if (npc is Horse horse && horse.rider != null && !ModApi.IsWildHorse(horse))
                    BeingRidden.Add(horse);
        }


        /// <summary>Checks horses known to be currently ridden and re-adds them to the map if they've been dismounted.</summary>
        internal static void HorseDismountedCheck()
        {
            // Only allow the host player to alter horse locations
            if (!Context.IsMainPlayer)
                return;

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
                        if (ModApi.IsNotATractorOrCart(horse))
                        {
                            // The first horse is given to the host player
                            AddCreature(horse, 0, Game1.MasterPlayer);
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

            if (Config.HorseWhistleKey != null && e.Button.ToString().ToLower() == Config.HorseWhistleKey.ToLower())
                CallHorse();
            if (Config.CorralKey != null && e.Button.ToString().ToLower() == Config.CorralKey.ToLower())
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
        internal void AddCreature(Character creature, int skinID = 0, Farmer owner = null)
        {
            string type = ModApi.GetInternalType(creature);
            if (ModApi.IsInDatabase(creature) || !ModApi.IsRegisteredType(type))
                return;

            // Set internal IDs
            SetShortID(creature, GetUnusedShortID());
            IDToCategory[GetLongID(creature)] = ModApi.GetCreatureCategory(type);

            // Set ownership
            if (creature is Horse)
            {
                if (owner != null)
                {
                    HorseOwnershipMap.Add(GetLongID(creature), owner);
                    // DEBUG
                    Monitor.Log($"Horse ID: {GetShortID(creature)}\nOwner: {owner.Name}", LogLevel.Debug);
                }
                else
                {
                    Monitor.Log($"No adopter able to be detected. Horse {GetShortID(creature)} will be owned by the host player.", LogLevel.Debug);
                    HorseOwnershipMap.Add(GetLongID(creature), Game1.MasterPlayer);
                }
            }

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
            // Remove FarmAnimal-specific ID markers from lists
            else if (AnimalLongToShortIDs.ContainsKey(longID))
            {
                int shortID = AnimalLongToShortIDs[longID];
                AnimalLongToShortIDs.Remove(longID);
                AnimalShortToLongIDs.Remove(shortID);
            }

            // Scrub internal IDs and skin mapping from system
            IDToCategory.Remove(longID);
            SkinMap.Remove(longID);
        }
    }
}
