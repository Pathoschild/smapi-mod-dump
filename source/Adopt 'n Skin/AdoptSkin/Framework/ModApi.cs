using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewModdingAPI;

using StardewValley.Characters;

namespace AdoptSkin.Framework
{
    public class ModApi
    {
        /// <summary>Registers a pet type to handle skin support for.</summary>
        /// <param name="id">The filename for the pet. This will also be its internal ID within Adopt & Skin.</param>
        /// <param name="type">The Type for creating a new instance of this pet. This must inherit from Characters.Pet.</param>
        public static void RegisterPetType(string id, Type type)
        {
            // Ensure the type inherits from Pet
            if (!typeof(Pet).IsAssignableFrom(type))
                ModEntry.SMonitor.Log("Unable to register pet type, type does not inherit from Pet class: " + id, LogLevel.Debug);

            id = ModEntry.Sanitize(id);
            if (ModEntry.PetAssets.ContainsKey(id))
            {
                ModEntry.SMonitor.Log("Unable to register pet type, type already registered: " + id, LogLevel.Debug);
                return;
            }

            ModEntry.PetAssets.Add(id, new List<AnimalSkin>());
            ModEntry.PetTypeMap.Add(id, type);
        }

        /// <summary>Registers a horse type to handle skin support for.</summary>
        /// <param name="id">The filename for the horse. This will also be its internal ID within Adopt & Skin.</param>
        /// <param name="type">The Type for creating a new instance of this horse. This must inherit from Characters.Horse.</param>
        public static void RegisterHorseType(string id, Type type)
        {
            // Ensure the type inherits from Horse
            if (!typeof(Horse).IsAssignableFrom(type))
                ModEntry.SMonitor.Log("Unable to register horse type, type does not inherit from Horse class: " + id, LogLevel.Debug);

            id = ModEntry.Sanitize(id);
            if (ModEntry.HorseAssets.ContainsKey(id))
            {
                ModEntry.SMonitor.Log("Unable to register horse type, type already registered: " + id, LogLevel.Debug);
                return;
            }

            ModEntry.HorseAssets.Add(id, new List<AnimalSkin>());
            ModEntry.HorseTypeMap.Add(id, type);
        }

        /// <summary>Registers an animal type to handle skin support for.</summary>
        /// <param name="id">The filename for the animal. This will also be its internal ID within Adopt & Skin.</param>
        /// <param name="hasBaby">If this animal type has a baby skin.</param>
        /// <param name="canShear">If this animal type has a sheared skin.</param>
        public static void RegisterAnimalType(string id, bool hasBaby = true, bool canShear = false)
        {
            id = ModEntry.Sanitize(id);
            if (ModEntry.AnimalAssets.ContainsKey(id))
            {
                ModEntry.SMonitor.Log("Unable to register animal type, type already registered: " + id, LogLevel.Debug);
                return;
            }

            ModEntry.AnimalAssets.Add(id, new List<AnimalSkin>());

            // Registers the Baby and Sheared sprites, if given
            if (hasBaby)
                RegisterAnimalType("Baby" + id, false);
            if (canShear)
                RegisterAnimalType("Sheared" + id, false);
        }

        /// <summary>Registers Stardew Valley's default animal and pet types for skin support.</summary>
        public static void RegisterDefaultTypes()
        {
            // Register default supported animal types
            RegisterAnimalType("Blue Chicken");
            RegisterAnimalType("Brown Chicken");
            RegisterAnimalType("Brown Cow");
            RegisterAnimalType("Dinosaur", false, false);
            RegisterAnimalType("Duck");
            RegisterAnimalType("Goat");
            RegisterAnimalType("Pig");
            RegisterAnimalType("Rabbit");
            RegisterAnimalType("Sheep", true, true);
            RegisterAnimalType("Void Chicken");
            RegisterAnimalType("White Chicken");
            RegisterAnimalType("White Cow");

            // Register default supported pet types
            RegisterPetType("cat", typeof(Cat));
            RegisterPetType("dog", typeof(Dog));

            // Register horse type
            RegisterHorseType("horse", typeof(Horse));
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


        /// <summary>Returns all animal types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledAnimalTypes()
        {
            List<string> handledAnimals = new List<string>();

            foreach (string type in ModEntry.AnimalAssets.Keys)
                handledAnimals.Add($"{type}");

            return handledAnimals;
        }


        /// <summary>Returns all pet types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledPetTypes()
        {
            List<string> handledPets = new List<string>();

            foreach (string type in ModEntry.PetAssets.Keys)
                handledPets.Add($"{type}");

            return handledPets;
        }


        /// <summary>Returns all horse types Adopt & Skin is currently handling.</summary>
        public static List<string> GetHandledHorseTypes()
        {
            List<string> handledHorses = new List<string>();

            foreach (string type in ModEntry.HorseAssets.Keys)
                handledHorses.Add($"{type}");

            return handledHorses;
        }


        /// <summary>Returns whether the given type contains the word "chicken"</summary>
        public static bool IsChicken(string type)
        {
            type = ModEntry.Sanitize(type);
            return type.Contains("chicken");
        }


        /// <summary>Returns whether the given type contains the word "cow"</summary>
        public static bool IsCow(string type)
        {
            type = ModEntry.Sanitize(type);
            return type.Contains("cow");
        }


        public static bool HasBabySprite(string type)
        {
            if (ModEntry.AnimalAssets.ContainsKey("baby" + ModEntry.Sanitize(type)))
                return true;
            return false;
        }


        public static bool HasShearedSprite(string type)
        {
            if (ModEntry.AnimalAssets.ContainsKey("sheared" + ModEntry.Sanitize(type)))
                return true;
            return false;
        }


        public static bool IsRegisteredType(string type)
        {
            type = ModEntry.Sanitize(type);
            if (ModEntry.PetAssets.ContainsKey(type) || ModEntry.HorseAssets.ContainsKey(type) || ModEntry.AnimalAssets.ContainsKey(type))
                return true;
            return false;
        }
    }
}
