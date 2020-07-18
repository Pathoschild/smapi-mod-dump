using FarmAnimalVarietyRedux.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>A collection of sprites corresponding to each animal type. This includes sprite sheets for adult, harvest, and baby animal spritesheets per season.</summary>
    public class AnimalSprites
    {
        /*********
        ** Accessors
        *********/
        /****
        ** Non Seasonal
        ****/
        /// <summary>The non seasonal adult sprite sheet for the animal.</summary>
        public Texture2D AdultSpriteSheet { get; set; }

        /// <summary>The non seasonal sprite sheet for the animal when it's ready for harvest.</summary>
        public Texture2D HarvestedSpriteSheet { get; set; }

        /// <summary>The non seasonal baby sprite sheet for the animal.</summary>
        public Texture2D BabySpriteSheet { get; set; }

        /****
        ** Spring
        ****/
        /// <summary>The spring adult sprite sheet for the animal.</summary>
        public Texture2D SpringAdultSpriteSheet { get; set; }

        /// <summary>The spring ready to harvest sprite sheet for the animal.</summary>
        public Texture2D SpringHarvestedSpriteSheet { get; set; }

        /// <summary>The spring baby sprite sheet for the animal.</summary>
        public Texture2D SpringBabySpriteSheet { get; set; }

        /****
        ** Summer
        ****/
        /// <summary>The summer adult sprite sheet of the animal.</summary>
        public Texture2D SummerAdultSpriteSheet { get; set; }

        /// <summary>The summer ready to harvest sprite sheet for the animal.</summary>
        public Texture2D SummerHarvestedSpriteSheet { get; set; }

        /// <summary>The summer baby sprite sheet of the animal.</summary>
        public Texture2D SummerBabySpriteSheet { get; set; }

        /****
        ** Fall
        ****/
        /// <summary>The fall adult sprite sheet of the animal.</summary>
        public Texture2D FallAdultSpriteSheet { get; set; }

        /// <summary>The fall ready to harvest sprite sheet for the animal.</summary>
        public Texture2D FallHarvestedSpriteSheet { get; set; }

        /// <summary>The fall baby sprite sheet of the animal.</summary>
        public Texture2D FallBabySpriteSheet { get; set; }

        /****
        ** Winter
        ****/
        /// <summary>The winter adult sprite sheet of the animal.</summary>
        public Texture2D WinterAdultSpriteSheet { get; set; }

        /// <summary>The winter ready to harvest sprite sheet for the animal.</summary>
        public Texture2D WinterHarvestedSpriteSheet { get; set; }

        /// <summary>The winter baby sprite sheet of the animal.</summary>
        public Texture2D WinterBabySpriteSheet { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="adultSpriteSheet">The non seasonal adult sprite sheet for the animal.</param>
        /// <param name="babySpriteSheet">The non seasonal baby sprite sheet for the animal.</param>
        /// <param name="harvestedSpriteSheet">The non seasonal sprite sheet for the animal when it's ready for harvest.</param>
        /// <param name="springAdultSpriteSheet">The spring adult sprite sheet for the animal.</param>
        /// <param name="springHarvestedSpriteSheet">The spring ready to harvest sprite sheet for the animal.</param>
        /// <param name="springBabySpriteSheet">The spring baby sprite sheet for the animal.</param>
        /// <param name="summerAdultSpriteSheet">The summer adult sprite sheet of the animal.</param>
        /// <param name="summerHarvestedSpriteSheet">The summer ready to harvest sprite sheet for the animal.</param>
        /// <param name="summerBabySpriteSheet">The summer baby sprite sheet of the animal.</param>
        /// <param name="fallAdultSpriteSheet">The fall adult sprite sheet of the animal.</param>
        /// <param name="fallHarvestedSpriteSheet">The fall ready to harvest sprite sheet for the animal.</param>
        /// <param name="fallBabySpriteSheet">The fall baby sprite sheet of the animal.</param>
        /// <param name="winterAdultSpriteSheet">The winter adult sprite sheet of the animal.</param>
        /// <param name="winterHarvestedSpriteSheet">The winter ready to harvest sprite sheet for the animal.</param>
        /// <param name="winterBabySpriteSheet">The winter baby sprite sheet of the animal.</param>
        public AnimalSprites(Texture2D adultSpriteSheet, Texture2D babySpriteSheet = null, Texture2D harvestedSpriteSheet = null, Texture2D springAdultSpriteSheet = null, Texture2D springHarvestedSpriteSheet = null,
            Texture2D springBabySpriteSheet = null, Texture2D summerAdultSpriteSheet = null, Texture2D summerHarvestedSpriteSheet = null, Texture2D summerBabySpriteSheet = null, 
            Texture2D fallAdultSpriteSheet = null, Texture2D fallHarvestedSpriteSheet = null, Texture2D fallBabySpriteSheet = null, Texture2D winterAdultSpriteSheet = null, Texture2D winterHarvestedSpriteSheet = null,
            Texture2D winterBabySpriteSheet = null)
        {
            AdultSpriteSheet = adultSpriteSheet;
            BabySpriteSheet = babySpriteSheet;
            HarvestedSpriteSheet = harvestedSpriteSheet;
            SpringAdultSpriteSheet = springAdultSpriteSheet;
            SpringHarvestedSpriteSheet = springHarvestedSpriteSheet;
            SpringBabySpriteSheet = springBabySpriteSheet;
            SummerAdultSpriteSheet = summerAdultSpriteSheet;
            SummerHarvestedSpriteSheet = summerHarvestedSpriteSheet;
            SummerBabySpriteSheet = summerBabySpriteSheet;
            FallAdultSpriteSheet = fallAdultSpriteSheet;
            FallHarvestedSpriteSheet = fallHarvestedSpriteSheet;
            FallBabySpriteSheet = fallBabySpriteSheet;
            WinterAdultSpriteSheet = winterAdultSpriteSheet;
            WinterHarvestedSpriteSheet = winterHarvestedSpriteSheet;
            WinterBabySpriteSheet = winterBabySpriteSheet;
        }

        /// <summary>Get whether the sprites are valid, meaning there is atleast a sprite sheet for the baby and adult for each season.</summary>
        /// <returns>Whether the sprites are valid.</returns>
        public bool IsValid()
        {
            // non seasonal
            if (AdultSpriteSheet != null)
                return true;
            
            // seasonal
            if (SpringAdultSpriteSheet != null && SummerAdultSpriteSheet != null && FallAdultSpriteSheet != null && WinterAdultSpriteSheet != null)
                return true;

            return false;
        }

        /// <summary>Get a valid sprite sheet.</summary>
        /// <param name="isBaby">Whether the sprite sheet should be the baby version.</param>
        /// <param name="isHarvested">Whether the sprite sheet should be the harvested version.</param>
        /// <param name="season">The season the sprite sheet should be in.</param>
        /// <returns>The sprite sheet </returns>
        public Texture2D GetSpriteSheet(bool isBaby, bool isHarvested, Season season)
        {
            return (isBaby, isHarvested, season) switch
            {
                (false, false, Season.Spring) => SpringAdultSpriteSheet ?? AdultSpriteSheet,
                (false, false, Season.Summer) => SummerAdultSpriteSheet ?? AdultSpriteSheet,
                (false, false, Season.Fall) => FallAdultSpriteSheet ?? AdultSpriteSheet,
                (false, false, Season.Winter) => WinterAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Spring) => SpringHarvestedSpriteSheet ?? HarvestedSpriteSheet ?? SpringAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Summer) => SummerHarvestedSpriteSheet ?? HarvestedSpriteSheet ?? SummerAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Fall) => FallHarvestedSpriteSheet ?? HarvestedSpriteSheet ?? FallAdultSpriteSheet ?? AdultSpriteSheet,
                (false, true, Season.Winter) => WinterHarvestedSpriteSheet ?? HarvestedSpriteSheet ?? WinterAdultSpriteSheet ?? AdultSpriteSheet,
                (true, false, Season.Spring) => SpringBabySpriteSheet ?? BabySpriteSheet ?? SpringAdultSpriteSheet ?? AdultSpriteSheet,
                (true, false, Season.Summer) => SummerBabySpriteSheet ?? BabySpriteSheet ?? SummerAdultSpriteSheet ?? AdultSpriteSheet,
                (true, false, Season.Fall) => FallBabySpriteSheet ?? BabySpriteSheet ?? FallAdultSpriteSheet ?? AdultSpriteSheet,
                (true, false, Season.Winter) => WinterBabySpriteSheet ?? BabySpriteSheet ?? WinterAdultSpriteSheet ?? AdultSpriteSheet,
                (true, _, _) => BabySpriteSheet ?? AdultSpriteSheet
            };
        }

        /// <summary>Get whether the animal has valid harvested sprite sheets.</summary>
        /// <returns>Whether the animal has valid harvested sprite sheets.</returns>
        public bool HasDifferentSpriteSheetWhenHarvested()
        {
            if (HarvestedSpriteSheet != null)
                return true;

            if (SpringHarvestedSpriteSheet != null && SummerHarvestedSpriteSheet != null && FallHarvestedSpriteSheet != null && WinterHarvestedSpriteSheet != null)
                return true;

            return false;
        }
    }
}
