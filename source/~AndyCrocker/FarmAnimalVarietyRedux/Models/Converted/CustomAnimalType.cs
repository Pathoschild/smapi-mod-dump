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

namespace FarmAnimalVarietyRedux.Models.Converted
{
    /// <summary>Represents an animal subtype.</summary>
    public class CustomAnimalType
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The internal name of the animal, this is the mod that added the animal type's unique id prefixed the <see cref="Name"/>.</summary>
        public string InternalName { get; }

        /// <summary>The name of the animal type.</summary>
        public string Name { get; set; }

        /// <summary>Whether the animal type is buyable.</summary>
        /// <remarks>If the <see cref="CustomAnimal"/> isn't buyable then this is ignored.</remarks>
        public bool IsBuyable { get; set; }

        /// <summary>Whether the animal type can be born from an incubator recipe when the animal name is specified as the result (the incubator can still drop this animal type if the animal type itself was set as the result).</summary>
        public bool IsIncubatable { get; set; }

        /// <summary>The produce for the animal type.</summary>
        public List<AnimalProduce> Produce { get; set; }

        /// <summary>Whether the same forage produce can be found multiple times in the same day.</summary>
        public bool AllowForageRepeats { get; set; }

        /// <summary>The number of days it takes the animal type to become an adult.</summary>
        public byte DaysTillMature { get; set; }

        /// <summary>The id of the sound the animal type will make.</summary>
        public string SoundId { get; set; }

        /// <summary>The width of the animal type sprite when it's looking toward / away from the camera.</summary>
        public int FrontAndBackSpriteWidth { get; set; }

        /// <summary>The height of the animal type sprite when it's looking toward / away from the camera.</summary>
        public int FrontAndBackSpriteHeight { get; set; }

        /// <summary>The width of the animal type sprite when it's looking to the side.</summary>
        public int SideSpriteWidth { get; set; }

        /// <summary>The height of the animal type sprite when it's looking to the side.</summary>
        public int SideSpriteHeight { get; set; }

        /// <summary>The id of the meat of the animal type.</summary>
        public int MeatId { get; set; }

        /// <summary>The amount of the animal type's happiness bar will drain each night, when they are not petted, or not fed.</summary>
        public byte HappinessDrain { get; set; }

        /// <summary>The amount of the animal type's hunger bar will fill up eat time they eat a piece of grass.</summary>
        public byte FullnessGain { get; set; }

        /// <summary>The amount of extra happiness an animal will get when being pet when the player has either the Coop Master or Shepherd profession (which ever correlates to the type of building an animal lives in).</summary>
        public byte HappinessGain { get; set; }

        /// <summary>The amount of the animal type's friendship bar will fill up each time they get petted by an auto petter.</summary>
        public int AutoPetterFriendshipGain { get; set; }

        /// <summary>The amount of the animal type's friendship bar will full up each time they get petted by hand.</summary>
        public int HandPetFriendshipGain { get; set; }

        /// <summary>The walk speed of the animal type.</summary>
        public int WalkSpeed { get; set; }

        /// <summary>The sell price of the animal when it's a baby.</summary>
        public int? BabySellPrice { get; set; }

        /// <summary>The sell price of the animal when it's an adult.</summary>
        public int? AdultSellPrice { get; set; }

        /// <summary>Whether the animal is always a male.</summary>
        /// <remarks>If <see langword="false"/> is specified the animal will always be female, if <see langword="null"/> is specified there's a 50% chance of either gender.</remarks>
        public bool? IsMale { get; set; }

        /// <summary>The seasons the animal type is able to go outside.</summary>
        public List<string> SeasonsAllowedOutdoors { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Constructs an instance.</summary>
        /// <param name="internalName">The internal name of the animal, this is the mod that added the animal type's unique id prefixed the <see cref="Name"/>.</param>
        /// <param name="name">The name of the animal type.</param>
        /// <param name="isBuyable">Whether the animal type is buyable.</param>
        /// <param name="isIncubatable">Whether the animal type can be born from an incubator recipe when the animal name is specified as the result (the incubator can still drop this animal type if the animal type itself was set as the result).</param>
        /// <param name="produce">The produce for the animal type.</param>
        /// <param name="allowForageRepeats">Whether the same forage produce can be found multiple times in the same day.</param>
        /// <param name="daysTillMature">The number of days it takes the animal type to become an adult.</param>
        /// <param name="soundId">The id of the sound the animal type will make.</param>
        /// <param name="frontAndBackSpriteWidth">The width of the animal type sprite when it's looking toward / away from the camera.</param>
        /// <param name="frontAndBackSpriteHeight">The height of the animal type sprite when it's looking toward / away from the camera.</param>
        /// <param name="sideSpriteWidth">The width of the animal type sprite when it's looking to the side.</param>
        /// <param name="sideSpriteHeight">The height of the animal type sprite when it's looking toward / away from the camera.</param>
        /// <param name="meatId">The id of the meat of the animal type.</param>
        /// <param name="happinessDrain">The amount of the animal type's happiness bar will drain each night.</param>
        /// <param name="fullnessGain">The amount of the animal type's hunger bar will fill up each time they eat a piece of grass.</param>
        /// <param name="happinessGain">The amount of extra happiness an animal will get when being pet when the player has either the Coop Master or Shepherd profession (which ever correlates to the type of building an animal lives in).</param>
        /// <param name="autoPetterFriendshipGain">The amount of the animal type's friendship bar will fill up each time they get petted by an auto petter.</param>
        /// <param name="handPetFriendshipGain">The amount of the animal type's friendship bar will full up each time they get petted by hand.</param>
        /// <param name="walkSpeed">The walk speed of the animal type.</param>
        /// <param name="babySellPrice">The sell price of the animal when it's a baby.</param>
        /// <param name="adultSellPrice">The sell price of the animal when it's an adult.</param>
        /// <param name="isMale">Whether the animal is always a male.</param>
        /// <param name="seasonsAllowedOutdoors">The seasons the animal type is able to go outside.</param>
        public CustomAnimalType(string internalName, string name, bool isBuyable, bool isIncubatable, List<AnimalProduce> produce, bool allowForageRepeats, byte daysTillMature, string soundId, int frontAndBackSpriteWidth, int frontAndBackSpriteHeight, int sideSpriteWidth, int sideSpriteHeight, int meatId, byte happinessDrain, byte fullnessGain, byte? happinessGain, int autoPetterFriendshipGain, int handPetFriendshipGain, int walkSpeed, int? babySellPrice, int? adultSellPrice, bool? isMale, List<string> seasonsAllowedOutdoors)
        {
            if (seasonsAllowedOutdoors == null || seasonsAllowedOutdoors.Count == 0)
                seasonsAllowedOutdoors = new List<string> { "spring", "summer", "fall" };

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
            HappinessGain = happinessGain ?? (byte)(40 - happinessDrain);
            AutoPetterFriendshipGain = autoPetterFriendshipGain;
            HandPetFriendshipGain = handPetFriendshipGain;
            WalkSpeed = walkSpeed;
            BabySellPrice = babySellPrice;
            AdultSellPrice = adultSellPrice;
            IsMale = isMale;
            SeasonsAllowedOutdoors = seasonsAllowedOutdoors;
        }

        /// <inheritdoc/>
        public override string ToString() => $"InternalName: {InternalName}, Name: {Name}, IsBuyable: {IsBuyable}, IsIncubatable: {IsIncubatable}, DaysTillMature: {DaysTillMature}, SoundsId: {SoundId}, FrontAndBackSpriteWidth: {FrontAndBackSpriteWidth}, FrontAndBackSpriteHeight: {FrontAndBackSpriteHeight}, SideSpriteWidth: {SideSpriteWidth}, SideSpriteHeight: {SideSpriteHeight}, MeatId: {MeatId}, HappinessDrain: {HappinessDrain}, FullnessGain: {FullnessGain}, HappinessGain: {HappinessGain}, AutoPetterFriendshipGain: {AutoPetterFriendshipGain}, HandPetFriendshipGain: {HandPetFriendshipGain}, WalkSpeed: {WalkSpeed}, BabySellPrice: {BabySellPrice}, AdultSellPrice: {AdultSellPrice}, IsMale: {IsMale}, SeasonsAllowedOutdoors: {string.Join(", ", SeasonsAllowedOutdoors)}";
    }
}
