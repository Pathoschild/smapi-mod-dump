using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace AdoptSkin.Framework
{
    /// <summary>The SMAPI console command handler for Adopt & Skin</summary>
    class CommandHandler
    {
        /// <summary>Allowable custom creature group denotations for use in commands</summary>
        internal static readonly List<string> CreatureGroups = new List<string>() { "all", "animal", "coop", "barn", "chicken", "cow", "pet", "horse" };

        internal ModEntry Earth;

        internal CommandHandler(ModEntry modEntry)
        {
            Earth = modEntry;
        }


        /*****************************
        ** Console Command Handlers
        ******************************/

        /// <summary>Handles commands for the SMAPI console</summary>
        internal void OnCommandReceived(string command, string[] args)
        {
            switch (command)
            {
                case "debug_reset":
                    if (!EnforceArgCount(args, 0))
                        return;

                    ModEntry.SkinMap = new Dictionary<long, int>();
                    ModEntry.IDToCategory = new Dictionary<long, ModEntry.CreatureCategory>();
                    ModEntry.AnimalLongToShortIDs = new Dictionary<long, int>();
                    ModEntry.AnimalShortToLongIDs = new Dictionary<int, long>();
                    foreach (Pet pet in ModApi.GetPets())
                        if (!ModApi.IsStray(pet))
                            pet.Manners = 0;
                    foreach (Horse horse in ModApi.GetHorses())
                        if (!ModApi.IsWildHorse(horse) && ModApi.IsNotATractor(horse))
                            horse.Manners = 0;

                    // Re-add all creatures
                    foreach (Pet pet in ModApi.GetPets())
                        if (!ModApi.IsStray(pet))
                            Earth.AddCreature(pet);
                    foreach (Horse horse in ModApi.GetHorses())
                        if (!ModApi.IsWildHorse(horse) && ModApi.IsNotATractor(horse))
                            Earth.AddCreature(horse);
                    foreach (FarmAnimal animal in ModApi.GetAnimals())
                        Earth.AddCreature(animal);
                    return;


                case "debug_idmaps":
                    if (!EnforceArgCount(args, 0))
                        return;
                    ModEntry.SMonitor.Log($"Animals Long to Short:\n{string.Join("\n", ModEntry.AnimalLongToShortIDs)}", LogLevel.Info);
                    ModEntry.SMonitor.Log($"Animals Short to Long equal length: {ModEntry.AnimalLongToShortIDs.Count == ModEntry.AnimalShortToLongIDs.Count}", LogLevel.Alert);
                    return;


                case "debug_skinmaps":
                    if (!EnforceArgCount(args, 0))
                        return;
                    ModEntry.SMonitor.Log($"{string.Join("\n", ModEntry.SkinMap)}", LogLevel.Info);
                    return;


                case "debug_pets":
                    if (!EnforceArgCount(args, 0))
                        return;
                    foreach (Pet pet in ModApi.GetPets())
                    {
                        int petSkin = 0;
                        if (ModApi.IsStray(pet))
                            petSkin = ModEntry.Creator.StrayInfo.SkinID;
                        else if (ModApi.IsInDatabase(pet))
                            petSkin = ModEntry.SkinMap[pet.Manners];

                        ModEntry.SMonitor.Log($"[{pet.Name}] | Manners: {pet.Manners} | Skin: {petSkin} | Stray: {pet.Manners == Stray.StrayID}", LogLevel.Info);
                    }
                    return;


                case "debug_horses":
                    if (!EnforceArgCount(args, 0))
                        return;

                    foreach (Horse horse in ModApi.GetHorses())
                    {
                        int horseSkin = 0;
                        if (ModApi.IsWildHorse(horse))
                            horseSkin = ModEntry.Creator.HorseInfo.SkinID;
                        else if (ModApi.IsInDatabase(horse))
                            horseSkin = ModEntry.SkinMap[horse.Manners];

                        ModEntry.SMonitor.Log($"[{horse.Name}] | Manners: {horse.Manners} | Skin: {horseSkin} | Wild: {horse.Manners == WildHorse.WildID}", LogLevel.Info);
                    }
                    return;


                case "summon_stray":
                    if (!EnforceArgCount(args, 0))
                        return;

                    // Get rid of any previous stray still on the map
                    if (ModEntry.Creator.StrayInfo != null)
                        ModEntry.Creator.StrayInfo.RemoveFromWorld();
                    // Create a gosh darn new stray
                    ModEntry.Creator.StrayInfo = new Stray();
                    return;


                case "summon_horse":
                    if (!EnforceArgCount(args, 0))
                        return;

                    // Get rid of any previous horse still on the map
                    if (ModEntry.Creator.HorseInfo != null)
                        ModEntry.Creator.HorseInfo.RemoveFromWorld();
                    // Create a gosh darn new horse
                    ModEntry.Creator.HorseInfo = new WildHorse();
                    return;


                case "debug_clearunowned":
                    if (!EnforceArgCount(args, 0))
                        return;

                    foreach (NPC npc in Utility.getAllCharacters())
                    {
                        if (npc is Horse horse && ModApi.IsWildHorse(horse))
                            Game1.removeThisCharacterFromAllLocations(horse);
                        else if (npc is Pet pet && ModApi.IsStray(pet))
                            Game1.removeThisCharacterFromAllLocations(pet);
                    }
                    return;


                // Expected arguments: <creature ID>
                case "debug_find":
                    // Enforce argument constraints
                    if (!EnforceArgCount(args, 1) ||
                        !EnforceArgTypeInt(args[0], 1))
                        return;

                    Character creatureToFind = ModEntry.GetCreatureFromShortID(int.Parse(args[0]));
                    if (creatureToFind != null)
                        ModEntry.SMonitor.Log($"{creatureToFind.Name} located at: {creatureToFind.currentLocation} | {creatureToFind.getTileX()}, {creatureToFind.getTileY()}", LogLevel.Info);
                    else
                        ModEntry.SMonitor.Log($"No creature exists with the given ID: {args[0]}", LogLevel.Error);
                    return;


                case "horse_whistle":
                    if (args.ToList().Count == 1)
                    {
                        // Enforce argument constraints
                        if (!EnforceArgTypeInt(args[0], 1))
                            return;

                        int callID = int.Parse(args[0]);
                        if (ModEntry.CallHorse(callID))
                            return;
                        // Horse with given ID wasn't found
                        ModEntry.SMonitor.Log($"No horse exists with the given ID: {callID}", LogLevel.Error);
                    }
                    else
                    {
                        // Enforce argument constraints
                        if (!EnforceArgCount(args, 0))
                            return;

                        // Call the first horse found
                        ModEntry.CallHorse();
                    }
                    return;


                case "corral_horses":
                    ModEntry.CorralHorses();
                    return;


                case "sell":
                    // Enforce argument constraints
                    if (!EnforceArgCount(args, 1) ||
                        !EnforceArgTypeInt(args[0], 1))
                        return;

                    Character sellCreature = ModEntry.GetCreatureFromShortID(int.Parse(args[0]));
                    if (sellCreature == null)
                    {
                        ModEntry.SMonitor.Log($"No creature exists with the given ID: {args[0]}", LogLevel.Error);
                        return;
                    }
                    else if ((sellCreature is Pet || sellCreature is Horse) && !ModApi.IsStray(sellCreature) && !ModApi.IsWildHorse(sellCreature))
                    {
                        Game1.activeClickableMenu = new ConfirmationDialog($"Are you sure you want to sell your {ModApi.GetInternalType(sellCreature)}, {sellCreature.Name}?", (who) =>
                        {
                            if (Game1.activeClickableMenu is StardewValley.Menus.ConfirmationDialog cd)
                                cd.cancel();

                            Earth.RemoveCreature(ModEntry.GetLongID(sellCreature));
                            Game1.removeThisCharacterFromAllLocations((NPC)sellCreature);
                            return;
                        });
                    }
                    else
                        ModEntry.SMonitor.Log("You may only sell pets and horses via the A&S console. The ID given is for a farm animal or an unknown type of creature.", LogLevel.Error);

                    return;


                // Expected arguments: <creature type/category/group>
                case "list_creatures":
                    // Enforce argument constraints
                    if (!EnforceArgCount(args, 1) ||
                        !EnforceArgTypeGroup(args[0]))
                        return;

                    PrintRequestedCreatures(ModEntry.Sanitize(args[0]));
                    return;


                // Expected arguments: None.
                case "randomize_all_skins":
                    // Enforce argument constraints
                    if (!EnforceArgCount(args, 0))
                        return;

                    // Randomize all skins
                    foreach (Pet pet in ModApi.GetPets())
                        if (!ModApi.IsStray(pet))
                            ModEntry.RandomizeSkin(pet);
                    foreach (Horse horse in ModApi.GetHorses())
                        if (!ModApi.IsWildHorse(horse) && ModApi.IsNotATractor(horse))
                            ModEntry.RandomizeSkin(horse);
                    foreach (FarmAnimal animal in ModApi.GetAnimals())
                        ModEntry.RandomizeSkin(animal);

                    ModEntry.SMonitor.Log("All animal, pet, and horse skins have been randomized.", LogLevel.Alert);
                    return;


                // Expected arguments: <creature group or creature ID>
                case "randomize_skin":
                    // Enforce argument constraints
                    if (!EnforceArgCount(args, 1))
                        return;

                    string call = ModEntry.Sanitize(args[0]);
                    if (CreatureGroups.Contains(call) || ModApi.GetHandledAllTypes().Contains(call))
                    {
                        List<Character> group = GetCreaturesFromGroup(call);
                        foreach (Character creature in group)
                        {
                            ModEntry.RandomizeSkin(creature);
                        }
                        ModEntry.SMonitor.Log($"All creatures in group `{call}` have been randomized.", LogLevel.Alert);
                    }
                    else if (EnforceArgTypeInt(args[0], 1))
                    {
                        // Find associated creature instance
                        int creatureID = int.Parse(args[0]);
                        Character creature = ModEntry.GetCreatureFromShortID(creatureID);

                        // A creature was able to be located with the given category and ID
                        if (creature != null && ModApi.IsInDatabase(creature))
                        {
                            if (ModEntry.RandomizeSkin(creature) == 0)
                                ModEntry.SMonitor.Log($"No skins are located in `/assets/skins` for {creature.Name}'s type: {ModEntry.Sanitize(creature.GetType().Name)}", LogLevel.Error);
                            else
                                ModEntry.SMonitor.Log($"{creature.Name}'s skin has been randomized.", LogLevel.Alert);
                        }
                        else
                            ModEntry.SMonitor.Log($"Creature with given ID does not exist: {creatureID}", LogLevel.Error);
                    }
                    return;


                // Expected arguments: <skin ID>, <creature ID>
                case "set_skin":
                    // Enforce argument constraints
                    if (!EnforceArgCount(args, 2) ||
                        !EnforceArgTypeInt(args[0], 1) ||
                        !EnforceArgTypeInt(args[1], 2))
                        return;

                    int skinID = int.Parse(args[0]);
                    int shortID = int.Parse(args[1]);
                    Character creatureToSkin = ModEntry.GetCreatureFromShortID(shortID);

                    if (creatureToSkin == null)
                    {
                        ModEntry.SMonitor.Log($"No creature is registered with the given ID: {shortID}", LogLevel.Error);
                        return;
                    }

                    // Enforce argument range to the range of the available skins for this creature's type
                    if (!ModEntry.Assets[ModApi.GetInternalType(creatureToSkin)].ContainsKey(skinID))
                    {
                        ModEntry.SMonitor.Log($"{creatureToSkin.Name}'s type ({ModApi.GetInternalType(creatureToSkin)}) has no skin with ID {skinID}", LogLevel.Error);
                        return;
                    }

                    // Successfully found given creature to set skin for
                    ModEntry.SetSkin(creatureToSkin, skinID);
                    ModEntry.SMonitor.Log($"{creatureToSkin.Name}'s skin has been set to skin {skinID}", LogLevel.Alert);
                    return;


                default:
                    return;
            }
        }






        /// <summary>Checks that the correct number of arguments are given for a console command.</summary>
        /// <param name="args">Arguments given to the command</param>
        /// <param name="number">Correct number of arguments to give to the command</param>
        /// <returns>Returns true if the correct number of arguments was given. Otherwise gives a console error report and returns false.</returns>
        internal static bool EnforceArgCount(string[] args, int number)
        {
            if (args.Length == number)
            {
                return true;
            }
            ModEntry.SMonitor.Log($"Incorrect number of arguments given. The command requires {number} arguments, {args.Length} were given.", LogLevel.Error);
            return false;
        }


        /// <summary>Checks that the given int argument is less than or equal to the given maxValue, and at least of value 1</summary>
        /// <param name="arg">The argument being checked</param>
        /// <param name="argNumber">The numbered order of the argument for the command (e.g. the first argument would be argNumber = 1)</param>
        /// <param name="maxValue">The maximum value that the given argument is allowed to be</param>
        /// <returns>Returns true if the given argument is within the range of 1 to maxValue. Otherwise gives a console error report and returns false.</returns>
        internal static bool EnforceArgRange(int arg, int maxValue, int argNumber)
        {
            if (arg < 1 || arg > maxValue)
            {
                ModEntry.SMonitor.Log($"Incorrect argument range. Argument {argNumber} should be of a value between 1 and {maxValue} (inclusive)", LogLevel.Error);
                return false;
            }
            return true;
        }


        /// <summary>Checks that the argument given is able to be parsed into an integer.</summary>
        /// <param name="arg">The argument to be checked</param>
        /// <param name="argNumber">The numbered order of the argument for the command (e.g. the first argument would be argNumber = 1)</param>
        /// <returns>Returns true if the given argument can be parsed as an int. Otherwise gives a console error report and returns false.</returns>
        internal static bool EnforceArgTypeInt(string arg, int argNumber)
        {
            if (!int.TryParse(arg, out int parsedArg))
            {
                ModEntry.SMonitor.Log($"Incorrect argument type given for argument {argNumber}. Expected type: int", LogLevel.Error);
                return false;
            }
            return true;
        }


        /// <summary>Checks that the given argument is of a recognized creature category.</summary>
        /// <param name="arg">The argument to be checked</param>
        /// <param name="argNumber">The numbered order of the argument for the command (e.g. the first argument would be argNumber = 1)</param>
        /// <returns>Returns true if the given argument is one of the strings "animal", "pet", or "horse". Otherwise gives a console error report and returns false.</returns>
        internal static bool EnforceArgTypeCategory(string arg, int argNumber)
        {
            string type = ModEntry.Sanitize(arg);

            if (type != "animal" && type != "pet" && type != "horse")
            {
                ModEntry.SMonitor.Log($"Incorrect argument type given for argument {argNumber}. Expected one of creature categories: animal, pet, horse", LogLevel.Error);
                return false;
            }
            return true;
        }


        /// <summary>Checks that the given argument is of a recognized creature group or recognized creature type.</summary>
        /// <param name="arg">The argument to be checked</param>
        /// <returns>Returns true if the given argument is stored in CreatureGroups or is a known FarmAnimal, Pet, or Horse type. Otherwise gives a console error report and returns false.</returns>
        internal static bool EnforceArgTypeGroup(string arg)
        {
            string type = ModEntry.Sanitize(arg);
            List<string> handledTypes = ModApi.GetHandledAllTypes();

            if (!CreatureGroups.Contains(type) && !handledTypes.Contains(type))
            {
                ModEntry.SMonitor.Log($"Argument given isn't one of {string.Join(", ", CreatureGroups)}, or a handled creature type. Handled types:\n{string.Join(", ", handledTypes)}", LogLevel.Error);
                return false;
            }

            return true;
        }






        /*************************
        ** Miscellaneous Helpers
        *************************/

        /// <summary>Returns a List of Characters of all creatures of the specified creature type or custom grouping</summary>
        internal List<Character> GetCreaturesFromGroup(string group)
        {
            group = ModEntry.Sanitize(group);
            List<Character> calledGroup = new List<Character>();

            if (!CreatureGroups.Contains(group) && !ModApi.GetHandledAllTypes().Contains(group))
            {
                ModEntry.SMonitor.Log($"Specified grouping is not handled by Adopt & Skin: {group}", LogLevel.Error);
                return calledGroup;
            }

            // Add FarmAnimal types to the return list
            if (group == "all" || group == "animal")
                foreach (FarmAnimal animal in ModApi.GetAnimals())
                {
                    calledGroup.Add(animal);
                }
            else if (group == "coop")
                foreach (FarmAnimal animal in ModApi.GetAnimals())
                {
                    if (animal.isCoopDweller())
                        calledGroup.Add(animal);
                }
            else if (group == "barn")
                foreach (FarmAnimal animal in ModApi.GetAnimals())
                {
                    if (!animal.isCoopDweller())
                        calledGroup.Add(animal);
                }
            else if (group == "chicken")
                foreach (FarmAnimal animal in ModApi.GetAnimals())
                {
                    if (ModApi.IsChicken(animal))
                        calledGroup.Add(animal);
                }
            else if (group == "cow")
                foreach (FarmAnimal animal in ModApi.GetAnimals())
                {
                    if (ModApi.IsCow(animal))
                        calledGroup.Add(animal);
                }
            else if (ModApi.GetHandledAnimalTypes().Contains(group))
                foreach (FarmAnimal animal in ModApi.GetAnimals())
                {
                    if (ModApi.GetInternalType(animal) == group)
                        calledGroup.Add(animal);
                }


            // Add Pet types to the return list
            if (group == "all" || group == "pet")
                foreach (Pet pet in ModApi.GetPets())
                {
                    if (!ModApi.IsStray(pet))
                        calledGroup.Add(pet);
                }
            else if (ModApi.GetHandledPetTypes().Contains(group))
                foreach (Pet pet in ModApi.GetPets())
                {
                    if (!ModApi.IsStray(pet) && ModApi.GetInternalType(pet) == group)
                        calledGroup.Add(pet);
                }


            // Add Horse types to the return list
            if (group == "all" || ModApi.GetHandledHorseTypes().Contains(group))
                foreach (Horse horse in ModApi.GetHorses())
                {
                    if (!ModApi.IsWildHorse(horse) && ModApi.IsNotATractor(horse))
                        calledGroup.Add(horse);
                }


            return calledGroup;
        }






        /******************
        ** Print Strings
        ******************/

        /// <summary>Prints the the requested creature information from the list_animals console command.</summary>
        internal static void PrintRequestedCreatures(string arg)
        {
            string type = ModEntry.Sanitize(arg);

            // -- Handle FarmAnimal type arguments --
            if (type == "all" || type == "animal")
            {
                List<string> animalInfo = new List<string>();

                foreach (FarmAnimal animal in ModApi.GetAnimals())
                    animalInfo.Add(GetPrintString(animal));

                ModEntry.SMonitor.Log("Animals:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", animalInfo)}", LogLevel.Info);
            }
            // Handle coop animal types only
            else if (type == "coop")
            {
                List<string> coopInfo = new List<string>();

                foreach (FarmAnimal animal in ModApi.GetAnimals())
                    if (animal.isCoopDweller())
                        coopInfo.Add(GetPrintString(animal));

                ModEntry.SMonitor.Log("Coop Animals:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", coopInfo)}", LogLevel.Info);
            }
            // Handle barn animal types only
            else if (type == "barn")
            {
                List<string> barnInfo = new List<string>();

                foreach (FarmAnimal animal in ModApi.GetAnimals())
                    if (!animal.isCoopDweller())
                        barnInfo.Add(GetPrintString(animal));

                ModEntry.SMonitor.Log("Barn Animals:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", barnInfo)}", LogLevel.Info);
            }
            // Handle chicken type arguments
            else if (type == "chicken")
            {
                List<string> chickenInfo = new List<string>();

                foreach (FarmAnimal animal in ModApi.GetAnimals())
                    if (ModApi.IsChicken(ModApi.GetInternalType(animal)))
                        chickenInfo.Add(GetPrintString(animal));

                ModEntry.SMonitor.Log("Chickens:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", chickenInfo)}", LogLevel.Info);
            }
            // Handle cow type arguments
            else if (type == "cow")
            {
                List<string> cowInfo = new List<string>();

                foreach (FarmAnimal animal in ModApi.GetAnimals())
                    if (ModApi.IsCow(ModApi.GetInternalType(animal)))
                        cowInfo.Add(GetPrintString(animal));

                ModEntry.SMonitor.Log("Cows:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", cowInfo)}", LogLevel.Info);
            }
            // Handle other animal type arguments
            else if (ModApi.GetHandledAnimalTypes().Contains(type))
            {
                List<string> animalInfo = new List<string>();

                foreach (FarmAnimal animal in ModApi.GetAnimals())
                    if (type == ModEntry.Sanitize(animal.type.Value))
                        animalInfo.Add(GetPrintString(animal));

                ModEntry.SMonitor.Log($"{arg}s:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", animalInfo)}", LogLevel.Info);
            }


            // -- Handle Pet type arguments --
            if (type == "all" || type == "pet")
            {
                List<string> petInfo = new List<string>();

                foreach (Pet pet in ModApi.GetPets())
                    if (!ModApi.IsStray(pet))
                        petInfo.Add(GetPrintString(pet));

                ModEntry.SMonitor.Log("Pets:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", petInfo)}", LogLevel.Info);

            }
            else if (ModApi.GetHandledPetTypes().Contains(type))
            {
                List<string> petInfo = new List<string>();

                foreach (Pet pet in ModApi.GetPets())
                    if (type == ModEntry.Sanitize(pet.GetType().Name) && !ModApi.IsStray(pet))
                        petInfo.Add(GetPrintString(pet));

                ModEntry.SMonitor.Log($"{arg}s:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", petInfo)}", LogLevel.Info);
            }


            // -- Handle Horse type arguments --
            if (type == "all" || ModApi.GetHandledHorseTypes().Contains(type))
            {
                List<string> horseInfo = new List<string>();

                foreach (Horse horse in ModApi.GetHorses())
                    if (!ModApi.IsWildHorse(horse) && ModApi.IsNotATractor(horse))
                        horseInfo.Add(GetPrintString(horse));

                ModEntry.SMonitor.Log("Horses:", LogLevel.Alert);
                ModEntry.SMonitor.Log($"{string.Join(", ", horseInfo)}", LogLevel.Info);
            }
        }


        /// <summary>Return the information on a pet or horse that the list_animals console command uses.
        internal static string GetPrintString(Character creature)
        {
            string name = creature.Name;
            string type = ModApi.GetInternalType(creature);
            int shortID = ModEntry.GetShortID(creature);
            long longID = ModEntry.GetLongID(creature); 
            int skinID = 0;
            if (ModEntry.SkinMap.ContainsKey(longID))
                skinID = ModEntry.SkinMap[longID];

            return $"\n # {name}:  Type - {type}\n" +
                    $"Short ID:   {shortID}\n" +
                    $"Skin ID:    {skinID}";
        }
    }
}
