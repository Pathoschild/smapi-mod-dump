/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models.Converted;
using FarmAnimalVarietyRedux.Models.Parsed;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Provides basic animal apis.</summary>
    public interface IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <summary>Adds tools to a list to hide the error message when using said tool on an animal.</summary>
        /// <param name="toolNames">The names of the tools </param>
        public void SkipErrorMessagesForTools(params string[] toolNames);

        /// <summary>Adds a custom animal.</summary>
        /// <param name="modUniqueId">The unique id of the mod adding the animal.</param>
        /// <param name="animal">The parsed custom animal data.</param>
        /// <param name="customSound">The custom sound of the animal.</param>
        public void AddAnimal(string modUniqueId, ParsedCustomAnimal animal, SoundEffect customSound);

        /// <summary>Edits a custom animal.</summary>
        /// <param name="animal">The parsed custom animal data.</param>
        /// <param name="customSound">The custom sound of the animal, if should be replaced; otherwise, <see langword="null"/>.</param>
        public void EditAnimal(string modUniqueId, ParsedCustomAnimal animal, SoundEffect customSound);

        /// <summary>Deletes an animal.</summary>
        /// <param name="internalName">The internal name of the animal to delete.</param>
        public void DeleteAnimal(string internalName);

        /// <summary>Adds a custom incubator recipe.</summary>
        /// <param name="recipe">The parsed recipe data.</param>
        /// <returns><see langword="true"/> if the incubator recipe was successfully added; otherwise, <see langword="false"/>.</returns>
        public void AddIncubatorRecipe(ParsedIncubatorRecipe recipe);

        /// <summary>Gets all the <see cref="CustomAnimal"/>s as a list of <see cref="StardewValley.Object"/>.</summary>
        /// <returns>All the <see cref="CustomAnimal"/>s as a list of <see cref="StardewValley.Object"/>.</returns>
        public List<StardewValley.Object> GetAllAnimalObjects();

        /// <summary>Gets all the buyable <see cref="CustomAnimal"/>s as a list of <see cref="StardewValley.Object"/>.</summary>
        /// <returns>All the buyable <see cref="CustomAnimal"/>s as a lsit of <see cref="StardewValley.Object"/>.</returns>
        public List<StardewValley.Object> GetAllBuyableAnimalObjects();


        /*********
        ** Internal Methods
        *********/
        /// <summary>Gets a <see cref="CustomAnimal"/> by internal animal name.</summary>
        /// <param name="internalAnimalName">The internal name of the animal to get data for.</param>
        /// <returns>The <see cref="CustomAnimal"/> with the passed internal name.</returns>
        internal CustomAnimal GetAnimalByInternalName(string internalAnimalName);

        /// <summary>Gets a <see cref="CustomAnimal"/> by internal subtype name.</summary>
        /// <param name="internalSubtypeName">The internal name of a subtype of the animal to get data for.</param>
        /// <returns>The <see cref="CustomAnimal"/> with a subtype with the passed internal name.</returns>
        internal CustomAnimal GetAnimalByInternalSubtypeName(string internalSubtypeName);

        /// <summary>Gets a <see cref="CustomAnimalType"/> by internal subtype name.</summary>
        /// <param name="internalSubtypeName">The internal name of the subtype of the animal subtype to get data for.</param>
        /// <returns>The <see cref="CustomAnimalType"/> with the passed internal name.</returns>
        internal CustomAnimalType GetAnimalSubtypeByInternalName(string internalSubtypeName);

        /// <summary>Gets a <see cref="CustomAnimalType"/> by subtype name.</summary>
        /// <param name="subtypeName">The name of the subtype of the animal to get data for.</param>
        /// <returns>The <see cref="CustomAnimalType"/> with the passed name.</returns>
        /// <remarks>This shouldn't be used except for determining animal data to convert BFAV animals to FAVR animals, this api falls apart when there are multiple animal subtypes with the same name (which FAVR supports but BFAV doesn't.)<br/>When retrieving animal data normally, use <see cref="GetAnimalByInternalName(string)"/>, <see cref="GetAnimalByInternalSubtypeName(string)"/>, or <see cref="GetAnimalSubtypeByInternalName(string)"/>.</remarks>
        internal CustomAnimalType GetAnimalSubtypeByName(string subtypeName);
    }
}
