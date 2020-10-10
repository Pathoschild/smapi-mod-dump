/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using StardewModdingAPI;

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


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the animal.</param>
        /// <param name="data">The data of the animal.</param>
        public Animal(string name, AnimalData data)
        {
            Name = name;
            Data = data;
        }

        /// <summary>Get whether the animal is valid.</summary>
        /// <returns>Whether the animal is valid.</returns>
        public bool IsValid()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(Name))
            {
                ModEntry.ModMonitor.Log($"Animal Validation failed, Name was not valid on Animal: {Name}.", LogLevel.Error);
                isValid = false;
            }

            if (Data == null)
            {
                ModEntry.ModMonitor.Log($"Animal Validation failed, Data was not valid on Animal: {Name}.", LogLevel.Error);
                isValid = false;
            }

            return isValid;
        }
    }
}
