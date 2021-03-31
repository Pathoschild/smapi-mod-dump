/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Models.Converted
{
    /// <summary>Represents a custom animal.</summary>
    public class CustomAnimal
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The internal name of the animal, this is the mod that added the animal's unique id prefixed the <see cref="Name"/>.</summary>
        public string InternalName { get; }

        /// <summary>The display name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>Whether the animal is buyable.</summary>
        public bool IsBuyable { get; set; }

        /// <summary>Whether the animal can swim.</summary>
        public bool CanSwim { get; set; }

        /// <summary>The animal shop information.</summary>
        public AnimalShopInfo AnimalShopInfo { get; }

        /// <summary>The subtypes of the animal.</summary>
        public List<CustomAnimalType> Subtypes { get; }

        /// <summary>The custom sound of the animal.</summary>
        public SoundEffect CustomSound { get; set; }

        /// <summary>The name of the buildings the animal can be housed in.</summary>
        public List<string> Buildings { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="internalName">The internal name of the animal, this is the mod that added the animal's unique id prefixed the <see cref="Name"/>.</param>
        /// <param name="name">The name of the animal.</param>
        /// <param name="isBuyable">Whether the animal is buyable.</param>
        /// <param name="canSwim">Whether the animal can swim.</param>
        /// <param name="animalShopInfo">The animal shop information.</param>
        /// <param name="types">The subtypes of the animal.</param>
        /// <param name="customSound">The custom sound of the animal.</param>
        /// <param name="buildings">The name of the buildings the animal can be housed in.</param>
        public CustomAnimal(string internalName, string name, bool isBuyable, bool canSwim, AnimalShopInfo animalShopInfo, List<CustomAnimalType> types, SoundEffect customSound, List<string> buildings)
        {
            InternalName = internalName;
            Name = name;
            IsBuyable = isBuyable;
            CanSwim = canSwim;
            AnimalShopInfo = animalShopInfo;
            Subtypes = types;
            CustomSound = customSound;
            Buildings = buildings ?? new List<string>();
        }

        /// <inheritdoc/>
        public override string ToString() => $"InternalName: {InternalName}, Name: {Name}, Buyable: {IsBuyable}, CanSwim: {CanSwim}, HasCustomSound: {CustomSound != null}, Buildings: {string.Join(", ", Buildings)}";
    }
}
