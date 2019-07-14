using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

using StardewValley;
using StardewValley.Characters;

namespace AdoptSkin.Framework
{
    public class ModApi
    {
        /************************
        ** Fields
        *************************/

        private static List<string> HandledPetTypes = new List<string>();
        private static List<string> HandledHorseTypes = new List<string>();
        private static List<string> HandledFarmAnimalTypes = new List<string>();






        /************************
        ** Public methods
        *************************/

        /// <summary>Registers a creature type to handle skin support for. Must inherit from one of the classes: Pet, Horse, FarmAnimal.</summary>
        /// <param name="id">The filename for the animal. This will also be its internal ID within Adopt & Skin.</param>
        /// <param name="hasBaby">If this animal type has a baby skin.</param>
        /// <param name="canShear">If this animal type has a sheared skin.</param>
        public static void RegisterType(string id, Type type, bool hasBaby = true, bool canShear = false)
        {
            id = ModEntry.Sanitize(id);
            if (ModEntry.Assets.ContainsKey(id))
            {
                ModEntry.SMonitor.Log("Unable to register type, type already registered: " + id, LogLevel.Debug);
                return;
            }

            // Ensure inherits from one of the accepted classes and register class-specific information
            if (typeof(Pet).IsAssignableFrom(type))
            {
                HandledPetTypes.Add(id);
                ModEntry.PetTypeMap.Add(id, type);
            }
            else if (typeof(Horse).IsAssignableFrom(type))
            {
                HandledHorseTypes.Add(id);
                ModEntry.HorseTypeMap.Add(id, type);
            }
            else if (typeof(FarmAnimal).IsAssignableFrom(type))
            {
                HandledFarmAnimalTypes.Add(id);

                // Registers the Baby and Sheared sprites, if given
                if (hasBaby)
                    RegisterType("Baby" + id, typeof(FarmAnimal), false, false);
                if (canShear)
                    RegisterType("Sheared" + id, typeof(FarmAnimal), false, false);
            }
            else
            {
                ModEntry.SMonitor.Log("Unable to register type, type does not inherit from any of the classes Pet, Horse, or FarmAnimal: " + id, LogLevel.Debug);
                return;
            }

            // Create an entry for assets to be added in ModEntry
            ModEntry.Assets.Add(id, new Dictionary<int, AnimalSkin>());
        }

        /// <summary>Registers Stardew Valley's default animal and pet types for skin support.</summary>
        public static void RegisterDefaultTypes()
        {
            // Register default supported animal types
            RegisterType("Blue Chicken", typeof(FarmAnimal));
            RegisterType("Brown Chicken", typeof(FarmAnimal));
            RegisterType("Brown Cow", typeof(FarmAnimal));
            RegisterType("Dinosaur", typeof(FarmAnimal), false, false);
            RegisterType("Duck", typeof(FarmAnimal));
            RegisterType("Goat", typeof(FarmAnimal));
            RegisterType("Pig", typeof(FarmAnimal));
            RegisterType("Rabbit", typeof(FarmAnimal));
            RegisterType("Sheep", typeof(FarmAnimal), true, true);
            RegisterType("Void Chicken", typeof(FarmAnimal));
            RegisterType("White Chicken", typeof(FarmAnimal));
            RegisterType("White Cow", typeof(FarmAnimal));

            // Register default supported pet types
            RegisterType("cat", typeof(Cat));
            RegisterType("dog", typeof(Dog));

            // Register horse type
            RegisterType("horse", typeof(Horse));
        }


        /// <summary>Returns all handled types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledAllTypes()
        {
            List<string> defaultTypes = new List<string>();
            defaultTypes.AddRange(GetHandledAnimalTypes());
            defaultTypes.AddRange(GetHandledPetTypes());
            defaultTypes.AddRange(GetHandledHorseTypes());

            return defaultTypes;
        }

        /// <summary>Returns all pet types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledPetTypes() { return new List<string>(HandledPetTypes); }

        /// <summary>Returns all horse types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledHorseTypes() { return new List<string>(HandledHorseTypes); }

        /// <summary>Returns all animal types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledAnimalTypes() { return new List<string>(HandledFarmAnimalTypes); }

        /// <summary>Returns true if the given creature subtype (i.e. Dog, Cat, WhiteChicken) is being handled by A&S</summary>
        public static bool IsRegisteredType(string type) { return ModEntry.Assets.ContainsKey(ModEntry.Sanitize(type)); }

        /// <summary>Returns true if the given creature subtype (i.e. Dog, Cat, WhiteChicken) has at least one skin loaded for it in A&S.</summary>
        public static bool HasSkins(string type) { return (ModEntry.Assets.ContainsKey(ModEntry.Sanitize(type)) && (ModEntry.Assets[ModEntry.Sanitize(type)]).Count != 0); }



        /// <summary>Returns an enumerable list of all Horse instances that exist currently, including owned horses, wild horses, or otherwise.
        /// Note: Horses technically don't exist while they are being ridden.</summary>
        public static IEnumerable<Horse> GetHorses()
        {
            foreach (NPC npc in Utility.getAllCharacters())
                if (npc is Horse horse)
                    yield return horse;
        }

        /// <summary>Returns an enumerable list of all Pet instances that exist currently, including owned pets, stray pets, or otherwise.</summary>
        public static IEnumerable<Pet> GetPets()
        {
            foreach (NPC npc in Utility.getAllCharacters())
                if (npc is Pet pet)
                    yield return pet;
        }

        /// <summary>Returns an enumerable list of all FarmAnimal instances on the Farm.</summary>
        public static IEnumerable<FarmAnimal> GetAnimals()
        {
            Farm farm = Game1.getFarm();

            if (farm == null)
                yield break;

            foreach (FarmAnimal animal in farm.getAllFarmAnimals())
                yield return animal;
        }


        /// <summary>Returns the string used to reference the given creature's type within A&S</summary>
        public static string GetInternalType(Character creature)
        {
            if (creature is Pet || creature is Horse)
                return ModEntry.Sanitize(creature.GetType().Name);
            else if (creature is FarmAnimal animal)
                return ModEntry.Sanitize(animal.type.Value);
            return "";
        }


        /// <summary>Returns true if the given creature instance of Horse, Pet, or FarmAnimal is currently being tracked by the A&S database</summary>
        public static bool IsInDatabase(Character creature)
        {
            if (creature == null)
                return false;

            long longID = ModEntry.GetLongID(creature);
            if (creature is FarmAnimal && IsInDatabase(longID) && ModEntry.IDToCategory[longID] == ModEntry.CreatureCategory.Animal)
                return true;
            else if (creature is Pet && IsInDatabase(longID) && ModEntry.IDToCategory[longID] == ModEntry.CreatureCategory.Pet)
                return true;
            else if (creature is Horse && IsInDatabase(longID) && ModEntry.IDToCategory[longID] == ModEntry.CreatureCategory.Horse)
                return true;
            return false;
        }

        /// <summary>Returns true if the given LongID is in use in the A&S system</summary>
        public static bool IsInDatabase(long longID) { return ModEntry.IDToCategory.ContainsKey(longID); }



        /// <summary>Returns whether the given type contains the word "chicken"</summary>
        public static bool IsChicken(string type) { return ModEntry.Sanitize(type).Contains("chicken"); }
        public static bool IsChicken(FarmAnimal animal) { return IsChicken(animal.type.Value); }

        /// <summary>Returns whether the given type contains the word "cow"</summary>
        public static bool IsCow(string type) { return ModEntry.Sanitize(type).Contains("cow"); }
        public static bool IsCow(FarmAnimal animal) { return IsCow(animal.type.Value); }

        public static bool HasBabySprite(string type) { return ModEntry.Assets.ContainsKey("baby" + ModEntry.Sanitize(type)); }

        public static bool HasShearedSprite(string type) { return ModEntry.Assets.ContainsKey("sheared" + ModEntry.Sanitize(type)); }

        public static bool IsNotATractor(Horse horse) { return !horse.Name.StartsWith("tractor/"); }

        public static bool IsWildHorse(Character creature) { return (creature is Horse horse && IsWildHorse(horse.Manners)); }

        public static bool IsWildHorse(long longID) { return (longID == WildHorse.WildID); }

        public static bool IsStray(Character creature) { return (creature is Pet pet && IsStray(pet.Manners)); }

        public static bool IsStray(long longID) { return (longID == Stray.StrayID); }

        public static ModEntry.CreatureCategory GetCreatureCategory(Character creature) { return GetCreatureCategory(GetInternalType(creature)); }

        public static ModEntry.CreatureCategory GetCreatureCategory(string type)
        {
            type = ModEntry.Sanitize(type);
            if (HandledPetTypes.Contains(type))
                return ModEntry.CreatureCategory.Pet;
            else if (HandledHorseTypes.Contains(type))
                return ModEntry.CreatureCategory.Horse;
            else if (HandledFarmAnimalTypes.Contains(type))
                return ModEntry.CreatureCategory.Animal;
            else
                return ModEntry.CreatureCategory.Null;
        }

        /// <summary>Returns an array of the given creature's sprite's information: [StartFrame, Width, Height].</summary>
        public static int[] GetSpriteInfo(Character creature)
        {
            int[] info = new int[3];
            switch (GetCreatureCategory(creature))
            {
                case ModEntry.CreatureCategory.Pet:
                    info[0] = ModEntry.PetSpriteStartFrame;
                    info[1] = 32;
                    info[2] = 32;
                    return info;
                case ModEntry.CreatureCategory.Horse:
                    info[0] = ModEntry.HorseSpriteStartFrame;
                    info[1] = 32;
                    info[2] = 32;
                    return info;
                case ModEntry.CreatureCategory.Animal:
                    info[0] = ModEntry.AnimalSpriteStartFrame;
                    info[1] = (creature as FarmAnimal).frontBackSourceRect.Width;
                    info[2] = (creature as FarmAnimal).frontBackSourceRect.Height;
                    return info;
                default:
                    return info;
            }
        }
    }
}
