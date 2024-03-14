/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models.Converted;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux.Models.Parsed
{
    /// <summary>Represents a custom animal.</summary>
    /// <remarks>This is a version of <see cref="CustomAnimal"/> that uses <see cref="ParsedAnimalShopInfo"/> and <see cref="ParsedCustomAnimalType"/>s that will be used for parsing content packs.</remarks>
    public class ParsedCustomAnimal
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the animal data should be interpreted.</summary>
        public Action Action { get; set; }

        /// <summary>The internal name of the animal.</summary>
        /// <remarks>This is only used when the <see cref="Action"/> is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/>.</remarks>
        public string InternalName { get; set; }

        /// <summary>The name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>Whether the animal is buyable.</summary>
        public bool? IsBuyable { get; set; }

        /// <summary>Whether the animal can swim.</summary>
        public bool? CanSwim { get; set; }

        /// <summary>The animal shop information.</summary>
        public ParsedAnimalShopInfo AnimalShopInfo { get; set; }

        /// <summary>The subtypes of the animal.</summary>
        public List<ParsedCustomAnimalType> Subtypes { get; set; }

        /// <summary>The name of the buildings the animal can be housed in.</summary>
        public List<string> Buildings { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        public ParsedCustomAnimal() { }

        /// <summary>Constructs an instance.</summary>
        /// <param name="action">How the animal type data should be interpreted.</param>
        /// <param name="internalName">The internal name of the animal type. This is only used when the <see cref="Action"/> is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/>.</param>
        /// <param name="name">The name of the animal.</param>
        /// <param name="isBuyable">Whether the animal is buyable.</param>
        /// <param name="canSwim">Whether the animal can swim.</param>
        /// <param name="animalShopInfo">The animal shop information.</param>
        /// <param name="types">The subtypes of the animal.</param>
        /// <param name="buildings">The name of the buildings the animal can be housed in.</param>
        public ParsedCustomAnimal(Action action, string internalName, string name, bool? isBuyable, bool? canSwim, ParsedAnimalShopInfo animalShopInfo, List<ParsedCustomAnimalType> types, List<string> buildings)
        {
            Action = action;
            InternalName = internalName;
            Name = name;
            IsBuyable = isBuyable;
            CanSwim = canSwim;
            AnimalShopInfo = animalShopInfo;
            Subtypes = types;
            Buildings = buildings;
        }

        /// <summary>Determines whether the animal is valid.</summary>
        /// <returns><see langword="true"/> if the animal is valid; otherwise, <see langword="false"/>.</returns>
        public bool IsValid()
        {
            // ensure animal name is valid
            if (string.IsNullOrEmpty(Name))
            {
                ModEntry.Instance.Monitor.Log("Animal doesn't have a name", LogLevel.Error);
                return false;
            }

            if (Name.Contains('.') || Name.Contains(','))
            {
                ModEntry.Instance.Monitor.Log($"Animal: {Name} cannot have a '.' or ',' in the name", LogLevel.Error);
                return false;
            }

            return true;
        }
    }
}
