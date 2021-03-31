/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BfavToFavrModConverter.Favr
{
    /// <summary>Represents an FAVR animal.</summary>
    public class FavrCustomAnimal
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the animal data should be interpreted.</summary>
        [DefaultValue(Action.Add)]
        public Action Action { get; set; }

        /// <summary>The internal name of the animal.</summary>
        /// <remarks>This is only used when the <see cref="Action"/> is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/>.</remarks>
        [DefaultValue(null)]
        public string InternalName { get; set; }

        /// <summary>The name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>Whether the animal is buyable.</summary>
        [DefaultValue(null)]
        public bool? Buyable { get; set; }

        /// <summary>Whether the animal can swim.</summary>
        [DefaultValue(false)]
        public bool? CanSwim { get; set; }

        /// <summary>The animal shop information.</summary>
        [DefaultValue(null)]
        public FavrAnimalShopInfo AnimalShopInfo { get; set; }

        /// <summary>The subtypes of the animal.</summary>
        public List<FavrCustomAnimalType> Subtypes { get; set; }

        /// <summary>The name of the buildings the animal can be housed in.</summary>
        public List<string> Buildings { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="action">How the animal type data should be interpreted.</param>
        /// <param name="internalName">The internal name of the animal type. This is only used when the <see cref="Action"/> is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/>.</param>
        /// <param name="name">The name of the animal.</param>
        /// <param name="buyable">Whether the animal is buyable.</param>
        /// <param name="canSwim">Whether the animal can swim.</param>
        /// <param name="animalShopInfo">The animal shop information.</param>
        /// <param name="types">The subtypes of the animal.</param>
        /// <param name="buildings">The name of the buildings the animal can be housed in.</param>
        public FavrCustomAnimal(Action action, string internalName, string name, bool? buyable, bool? canSwim, FavrAnimalShopInfo animalShopInfo, List<FavrCustomAnimalType> types, List<string> buildings)
        {
            Action = action;
            InternalName = internalName;
            Name = name;
            Buyable = buyable;
            CanSwim = canSwim;
            AnimalShopInfo = animalShopInfo;
            Subtypes = types;
            Buildings = buildings;
        }
    }
}
