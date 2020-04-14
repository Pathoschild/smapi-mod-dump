using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Metadata about an animal.</summary>
    public class Animal
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>The data of the animal.</summary>
        public AnimalData Data { get; set; }

        /// <summary>The shop icon for the animal.</summary>
        public Texture2D ShopIcon { get; set; }

        /// <summary>The subtypes of the animal.</summary>
        public List<AnimalSubType> SubTypes { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the animal.</param>
        /// <param name="data">The data of the animal.</param>
        /// <param name="shopIcon">The shop icon for the animal.</param>
        /// <param name="subTypes">The subtypes of the animal.</param>
        public Animal(string name, AnimalData data, Texture2D shopIcon, List<AnimalSubType> subTypes)
        {
            Name = name;
            Data = data;
            ShopIcon = shopIcon;
            SubTypes = subTypes;
        }

        /// <summary>Get whether the animal is valid.</summary>
        /// <returns>Whether the animal is valid.</returns>
        public bool IsValid()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(Name))
            {
                ModEntry.ModMonitor.Log("Animal Validation failed, Name was not valid.", LogLevel.Error);
                isValid = false;
            }

            if (Data == null)
            {
                ModEntry.ModMonitor.Log($"Animal Validation failed, Data was not valid on Animal: {Name}.", LogLevel.Error);
                isValid = false;
            }

            if (ShopIcon == null)
            {
                ModEntry.ModMonitor.Log($"Animal Validation failed, Shop Icon was not valid on Animal: {Name}.", LogLevel.Error);
                isValid = false;
            }

            if (SubTypes == null)
            {
                ModEntry.ModMonitor.Log($"Animal Validation failed, Sub Types were not valid on Animal: {Name}.", LogLevel.Error);
                isValid = false;
            }

            return isValid;
        }
    }
}
