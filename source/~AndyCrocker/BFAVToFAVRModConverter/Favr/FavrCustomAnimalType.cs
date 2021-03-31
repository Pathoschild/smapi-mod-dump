/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;

namespace BfavToFavrModConverter.Favr
{
    /// <summary>Represents an FAVR animal subtype.</summary>
    public class FavrCustomAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>How the animal type data should be interpreted.</summary>
        [DefaultValue(Action.Add)]
        public Action Action { get; set; }

        /// <summary>The name of the animal type.</summary>
        public string Name { get; set; }

        /// <summary>Whether the animal type is buyable.</summary>
        /// <remarks>If the <see cref="ParsedCustomAnimal"/> isn't buyable then this is ignored.</remarks>
        [DefaultValue(true)]
        public bool? IsBuyable { get; set; } = true;

        /// <summary>Whether the animal type can be born from an incubator recipe when the animal name is specified as the result (the incubator can still drop this animal type if the animal type itself was set as the result).</summary>
        [DefaultValue(true)]
        public bool? IsIncubatable { get; set; } = true;

        /// <summary>The produce for the animal type.</summary>
        public List<FavrAnimalProduce> Produce { get; set; }

        /// <summary>The number of days it takes the animal type to become an adult.</summary>
        public byte? DaysTillMature { get; set; }

        /// <summary>The id of the sound the animal type will make.</summary>
        [DefaultValue(null)]
        public string SoundId { get; set; }

        /// <summary>The width of the animal type sprite when it's looking toward / away from the camera.</summary>
        public int? FrontAndBackSpriteWidth { get; set; }

        /// <summary>The height of the animal type sprite when it's looking toward / away from the camera.</summary>
        public int? FrontAndBackSpriteHeight { get; set; }

        /// <summary>The width of the animal type sprite when it's looking to the side.</summary>
        public int? SideSpriteWidth { get; set; }

        /// <summary>The height of the animal type sprite when it's looking to the side.</summary>
        public int? SideSpriteHeight { get; set; }

        /// <summary>The id of the meat of the animal type.</summary>
        [DefaultValue(null)]
        public string MeatId { get; set; }

        /// <summary>The amount of the animal type's happiness bar will drain each night, when they are not petted, or not fed.</summary>
        [DefaultValue(null)]
        public byte? HappinessDrain { get; set; }

        /// <summary>The amount of the animal type's hunger bar will fill up each time they eat a piece of grass.</summary>
        [DefaultValue(255)]
        public byte? FullnessGain { get; set; }

        /// <summary>The amount of the animal type's happiness bar goes up when they are pet, or go to sleep fed.</summary>
        [DefaultValue(null)]
        public byte? HappinessGain { get; set; }

        /// <summary>The amount of the animal type's friendship bar will fill up each time they get petted by an auto petter.</summary>
        [DefaultValue(7)]
        public int? AutoPetterFriendshipGain { get; set; }

        /// <summary>The amount of the animal type's friendship bar will full up each time they get petted by hand.</summary>
        [DefaultValue(15)]
        public int? HandPetFriendshipGain { get; set; }

        /// <summary>The walk speed of the animal type.</summary>
        [DefaultValue(2)]
        public int? WalkSpeed { get; set; }

        /// <summary>The sell price of the animal when it's a baby.</summary>
        [DefaultValue(null)]
        public int? BabySellPrice { get; set; }

        /// <summary>The sell price of the animal when it's an adult.</summary>
        [DefaultValue(null)]
        public int? AdultSellPrice { get; set; }

        /// <summary>Whether the animal is always a male.</summary>
        /// <remarks>If <see langword="false"/> is specified the animal will always be female, if <see langword="null"/> is specified there's a 50% chance of either gender.</remarks>
        [DefaultValue(null)]
        public bool? IsMale { get; set; }

        /// <summary>The seasons the animal type is able to go outside.</summary>
        [DefaultValue(null)]
        public List<string> SeasonsAllowedOutdoors { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="action">How the animal type data should be interpreted.</param>
        /// <param name="name">The name of the animal type.</param>
        /// <param name="isBuyable">Whether the animal type is buyable.</param>
        /// <param name="isIncubatable">Whether the animal type can be born from an incubator recipe when the animal name is specified as the result (the incubator can still drop this animal type if the animal type itself was set as the result).</param>
        /// <param name="produce">The produce for the animal type.</param>
        /// <param name="daysTillMature">The number of days it takes the animal type to become an adult.</param>
        /// <param name="soundId">The id of the sound the animal type will make.</param>
        /// <param name="frontAndBackSpriteWidth">The width of the animal type sprite when it's looking toward / away from the camera.</param>
        /// <param name="frontAndBackSpriteHeight">The height of the animal type sprite when it's looking toward / away from the camera.</param>
        /// <param name="sideSpriteWidth">The width of the animal type sprite when it's looking to the side.</param>
        /// <param name="sideSpriteHeight">The height of the animal type sprite when it's looking toward / away from the camera.</param>
        /// <param name="meatId">The id of the meat of the animal type.</param>
        /// <param name="happinessDrain">The amount of the animal type's happiness bar will drain each night.</param>
        /// <param name="fullnessGain">The amount of the animal type's hunger bar will fill up each time they eat a piece of grass.</param>
        /// <param name="happinessGain">The amount of the animal type's happiness bar will fill up each time they get petted by the player.</param>
        /// <param name="autoPetterFriendshipGain">The amount of the animal type's friendship bar will fill up each time they get petted by an auto petter.</param>
        /// <param name="handPetFriendshipGain">The amount of the animal type's friendship bar will full up each time they get petted by hand.</param>
        /// <param name="walkSpeed">The walk speed of the animal type.</param>
        /// <param name="babySellPrice">The sell price of the animal when it's a baby.</param>
        /// <param name="adultSellPrice">The sell price of the animal when it's an adult.</param>
        /// <param name="isMale">Whether the animal is always a male.</param>
        /// <param name="seasonsAllowedOutdoors">The seasons the animal type is able to go outside.</param>
        public FavrCustomAnimalType(Action action, string name, bool isBuyable, bool isIncubatable, List<FavrAnimalProduce> produce, byte daysTillMature, string soundId, int frontAndBackSpriteWidth, int frontAndBackSpriteHeight, int sideSpriteWidth, int sideSpriteHeight, string meatId, byte happinessDrain, byte fullnessGain = 255, byte? happinessGain = null, int autoPetterFriendshipGain = 7, int handPetFriendshipGain = 15, int walkSpeed = 2, int? babySellPrice = null, int? adultSellPrice = null, bool? isMale = null, List<string> seasonsAllowedOutdoors = null)
        {
            Action = action;
            Name = name;
            IsBuyable = isBuyable;
            IsIncubatable = isIncubatable;
            Produce = produce;
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
    }
}
