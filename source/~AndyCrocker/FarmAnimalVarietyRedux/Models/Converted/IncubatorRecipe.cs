/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

namespace FarmAnimalVarietyRedux.Models.Converted
{
    /// <summary>Represents an incubator recipe.</summary>
    public class IncubatorRecipe
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The type of incubator the recipe will apply to.</summary>
        public IncubatorType IncubatorType { get; }

        /// <summary>The id of the input item.</summary>
        public int InputId { get; }

        /// <summary>The chance this recipe will get picked compared to others that have the same <see cref="InputId"/>.</summary>
        public float Chance { get; }

        /// <summary>The number of minutes it takes for the incubator to finish.</summary>
        public int MinutesTillDone { get; }

        /// <summary>The internal name of the animal that will get created.</summary>
        public string InternalAnimalName { get; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="incubatorType">The type of incubator the recipe will apply to.</param>
        /// <param name="inputId">The id of the input item.</param>
        /// <param name="chance">The chance this recipe will get picked compared to others that have the same <see cref="InputId"/>.</param>
        /// <param name="minutesTillDone">The number of minutes it takes for the incubator to finish.</param>
        /// <param name="internalAnimalName">The internal name of the animal that will get created.</param>
        public IncubatorRecipe(IncubatorType incubatorType, int inputId, float chance, int minutesTillDone, string internalAnimalName)
        {
            IncubatorType = incubatorType;
            InputId = inputId;
            Chance = chance;
            MinutesTillDone = minutesTillDone;
            InternalAnimalName = internalAnimalName;
        }
    }
}
