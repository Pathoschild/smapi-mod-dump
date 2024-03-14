/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.EqualityComparers;
using FarmAnimalVarietyRedux.Models.Converted;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace FarmAnimalVarietyRedux.Models.Parsed
{
    /// <summary>Represents an animal subtype.</summary>
    /// <remarks>This is a version of <see cref="CustomAnimalType"/> that uses <see cref="ParsedAnimalProduce"/> that will be used for parsing content packs.</remarks>
    public class ParsedCustomAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the subtype data should be interpreted.</summary>
        public Action Action { get; set; }

        /// <summary>The internal name of the subtype.</summary>
        /// <remarks>This is only used when the <see cref="Action"/> is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/>.</remarks>
        public string InternalName { get; set; }

        /// <summary>The name of the subtype.</summary>
        public string Name { get; set; }

        /// <summary>Whether the subtype is buyable.</summary>
        /// <remarks>If the <see cref="ParsedCustomAnimal"/> isn't buyable then this is ignored.</remarks>
        public bool? IsBuyable { get; set; }

        /// <summary>Whether the subtype can be born from an incubator recipe when the animal name is specified as the result (the incubator can still drop this animal type if the animal type itself was set as the result).</summary>
        public bool? IsIncubatable { get; set; }

        /// <summary>The produce for the subtype.</summary>
        public List<ParsedAnimalProduce> Produce { get; set; }

        /// <summary>Whether the same forage produce can be found multiple times in the same day.</summary>
        public bool? AllowForageRepeats { get; set; }

        /// <summary>The number of days it takes the subtype to become an adult.</summary>
        public byte? DaysTillMature { get; set; }

        /// <summary>The id of the sound the subtype will make.</summary>
        public string SoundId { get; set; }

        /// <summary>The width of the subtype sprite when it's looking toward / away from the camera.</summary>
        public int? FrontAndBackSpriteWidth { get; set; }

        /// <summary>The height of the subtype sprite when it's looking toward / away from the camera.</summary>
        public int? FrontAndBackSpriteHeight { get; set; }

        /// <summary>The width of the subtype sprite when it's looking to the side.</summary>
        public int? SideSpriteWidth { get; set; }

        /// <summary>The height of the subtype sprite when it's looking to the side.</summary>
        public int? SideSpriteHeight { get; set; }

        /// <summary>The id of the meat of the subtype.</summary>
        public string MeatId { get; set; }

        /// <summary>The amount of the subtype's happiness bar will drain each night, when they are not petted, or not fed.</summary>
        public byte? HappinessDrain { get; set; }

        /// <summary>The amount of the subtype's hunger bar will fill up each time they eat a piece of grass.</summary>
        public byte? FullnessGain { get; set; }

        /// <summary>The amount of extra happiness an animal will get when being pet when the player has either the Coop Master or Shepherd profession (which ever correlates to the type of building an animal lives in).</summary>
        /// <remarks>If <see langword="null"/> is specified then the default value is 40 - <see cref="HappinessDrain"/>.</remarks>
        public byte? HappinessGain { get; set; }

        /// <summary>The amount of the subtype's friendship bar will fill up each time they get petted by an auto petter.</summary>
        public int? AutoPetterFriendshipGain { get; set; }

        /// <summary>The amount of the subtype's friendship bar will full up each time they get petted by hand.</summary>
        public int? HandPetFriendshipGain { get; set; }

        /// <summary>The walk speed of the subtype.</summary>
        public int? WalkSpeed { get; set; }

        /// <summary>The sell price of the subtype when it's a baby.</summary>
        public int? BabySellPrice { get; set; }

        /// <summary>The sell price of the subtype when it's an adult.</summary>
        public int? AdultSellPrice { get; set; }

        /// <summary>Whether the subtype is always a male.</summary>
        /// <remarks>If <see langword="false"/> is specified the animal will always be female, if <see langword="null"/> is specified there's a 50% chance of either gender.</remarks>
        public bool? IsMale { get; set; }

        /// <summary>The seasons the subtype is able to go outside.</summary>
        public List<string> SeasonsAllowedOutdoors { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        public ParsedCustomAnimalType() { }

        /// <summary>Constructs an instance.</summary>
        /// <param name="action">How the subtype data should be interpreted.</param>
        /// <param name="internalName">The internal name of the subtype. This is only used when the <see cref="Action"/> is either <see cref="Action.Edit"/> or <see cref="Action.Delete"/>.</param>
        /// <param name="name">The name of the subtype.</param>
        /// <param name="isBuyable">Whether the subtype is buyable.</param>
        /// <param name="isIncubatable">Whether the subtype can be born from an incubator recipe when the animal name is specified as the result (the incubator can still drop this animal type if the animal type itself was set as the result).</param>
        /// <param name="produce">The produce for the subtype.</param>
        /// <param name="daysTillMature">The number of days it takes the subtype to become an adult.</param>
        /// <param name="soundId">The id of the sound the subtype will make.</param>
        /// <param name="frontAndBackSpriteWidth">The width of the subtype sprite when it's looking toward / away from the camera.</param>
        /// <param name="frontAndBackSpriteHeight">The height of the subtype sprite when it's looking toward / away from the camera.</param>
        /// <param name="sideSpriteWidth">The width of the subtype sprite when it's looking to the side.</param>
        /// <param name="sideSpriteHeight">The height of the subtype sprite when it's looking toward / away from the camera.</param>
        /// <param name="meatId">The id of the meat of the subtype.</param>
        /// <param name="happinessDrain">The amount of the subtype's happiness bar will drain each night.</param>
        /// <param name="fullnessGain">The amount of the subtype's hunger bar will fill up each time they eat a piece of grass.</param>
        /// <param name="happinessGain">The amount of extra happiness an animal will get when being pet when the player has either the Coop Master or Shepherd profession (which ever correlates to the type of building an animal lives in).</param>
        /// <param name="autoPetterFriendshipGain">The amount of the subtype's friendship bar will fill up each time they get petted by an auto petter.</param>
        /// <param name="handPetFriendshipGain">The amount of the subtype's friendship bar will full up each time they get petted by hand.</param>
        /// <param name="walkSpeed">The walk speed of the subtype.</param>
        /// <param name="babySellPrice">The sell price of the subtype when it's a baby.</param>
        /// <param name="adultSellPrice">The sell price of the subtype when it's an adult.</param>
        /// <param name="isMale">Whether the subtype is always a male.</param>
        /// <param name="seasonsAllowedOutdoors">The seasons the subtype is able to go outside.</param>
        /// <param name="allowForageRepeats">Whether the same forage produce can be found multiple times in the same day.</param>
        public ParsedCustomAnimalType(Action action, string internalName, string name, bool isBuyable, bool isIncubatable, List<ParsedAnimalProduce> produce, byte daysTillMature, string soundId, int frontAndBackSpriteWidth, int frontAndBackSpriteHeight, int sideSpriteWidth, int sideSpriteHeight, string meatId, byte happinessDrain, byte fullnessGain = 255, byte? happinessGain = null, int autoPetterFriendshipGain = 7, int handPetFriendshipGain = 15, int walkSpeed = 2, int? babySellPrice = null, int? adultSellPrice = null, bool? isMale = null, List<string> seasonsAllowedOutdoors = null, bool allowForageRepeats = true)
        {
            Action = action;
            InternalName = internalName;
            Name = name;
            IsBuyable = isBuyable;
            IsIncubatable = isIncubatable;
            Produce = produce;
            AllowForageRepeats = allowForageRepeats;
            DaysTillMature = daysTillMature;
            SoundId = soundId;
            FrontAndBackSpriteWidth = frontAndBackSpriteWidth;
            FrontAndBackSpriteHeight = frontAndBackSpriteHeight;
            SideSpriteWidth = sideSpriteWidth;
            SideSpriteHeight = sideSpriteHeight;
            MeatId = meatId;
            HappinessDrain = happinessDrain;
            FullnessGain = fullnessGain;
            HappinessGain = happinessGain;
            AutoPetterFriendshipGain = autoPetterFriendshipGain;
            HandPetFriendshipGain = handPetFriendshipGain;
            WalkSpeed = walkSpeed;
            BabySellPrice = babySellPrice;
            AdultSellPrice = adultSellPrice;
            IsMale = isMale;
            SeasonsAllowedOutdoors = seasonsAllowedOutdoors;
        }

        /// <summary>Determines whether the animal is valid.</summary>
        /// <returns><see langword="true"/> if the animal is valid; otherwise, <see langword="false"/>.</returns>
        public bool IsValid()
        {
            // ensure animal name is valid
            if (string.IsNullOrEmpty(Name))
            {
                ModEntry.Instance.Monitor.Log("Animal subtype doesn't have a name", LogLevel.Error);
                return false;
            }

            if (Name.Contains('.') || Name.Contains(','))
            {
                ModEntry.Instance.Monitor.Log($"Animal subtype: {Name} cannot have a '.' or ',' in the name", LogLevel.Error);
                return false;
            }

            // remove any duplicate produce from each subtype
            var oldCount = Produce.Count;
            Produce = Produce.Distinct(new ParsedAnimalProduceEqualityComparer()).ToList();
            if (Produce.Count != oldCount)
                ModEntry.Instance.Monitor.Log($"Atleast one produce has been removed from animal subtype: {InternalName} as it had the same {nameof(AnimalProduce.UniqueName)} as another produce in the same subtype", LogLevel.Error);

            return true;
        }
    }
}
