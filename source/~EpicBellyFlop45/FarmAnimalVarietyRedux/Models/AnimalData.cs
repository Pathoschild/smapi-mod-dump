using StardewModdingAPI;
using System.Collections.Generic;

namespace FarmAnimalVarietyRedux.Models
{
    /// <summary>Data related to an animal.</summary>
    public class AnimalData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The name of the animal.</summary>
        public string Name { get; set; }

        /// <summary>Whether the animal can be bought from Marnie's shop.</summary>
        public bool Buyable { get; set; } = true;

        /// <summary>The data about the animal in Marnie's shop.</summary>
        public AnimalShopInfo AnimalShopInfo { get; set; }

        /// <summary>The sub types of the animal.</summary>
        public List<AnimalSubType> Types { get; set; }

        /// <summary>The number of days it takes the animal to produce product.</summary>
        public int DaysToProduce { get; set; }

        /// <summary>The number of days it takes the animal to become an adult.</summary>
        public int DaysTillMature { get; set; }

        /// <summary>The id of the sound the animal will make.</summary>
        public string SoundId { get; set; }

        /// <summary>The width of the animal sprite when it's looking toward / away from the camera.</summary>
        public int FrontAndBackSpriteWidth { get; set; }

        /// <summary>The height of the animal sprite when it's looking toward / away from the camera.</summary>
        public int FrontAndBackSpriteHeight { get; set; }

        /// <summary>The width of the animal sprite when it's looking to the side.</summary>
        public int SideSpriteWidth { get; set; }

        /// <summary>The height of the animal sprite when it's looking to the side.</summary>
        public int SideSpriteHeight { get; set; }

        /// <summary>The amount the animal's hunger bar will drain each night.</summary>
        public byte FullnessDrain { get; set; }

        /// <summary>The amount the animal's happiness bar will drain each night.</summary>
        public byte HappinessDrain { get; set; }

        /// <summary>The name(s) of the building(s) the animal can be housed in.</summary>
        public List<string> Buildings { get; set; }

        /// <summary>The walk speed of the animal.</summary>
        public int WalkSpeed { get; set; } = 2;

        /// <summary>The time the animal will go to sleep.</summary>
        public int BedTime { get; set; } = 1900;

        /// <summary>The seasons the animal is able to go outside.</summary>
        public List<Season> SeasonsAllowedOutdoors { get; set; } = new List<Season> { Season.Spring, Season.Summer, Season.Fall };


        /*********
        ** Public Methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="name">The name of the animal.</param>
        /// <param name="buyable">Whether the animal can be bought from Marnie's shop.</param>
        /// <param name="animalShopInfo">The data about the animal in Marnie's shop.</param>
        /// <param name="types">The sub types of the animal.</param>
        /// <param name="daysToProduce">The number of days it takes the animal to produce product.</param>
        /// <param name="daysTillMature">The number of days it takes the animal to become an adult.</param>
        /// <param name="soundId">The id of the sound the animal will make.</param>
        /// <param name="frontAndBackSpriteWidth">The width of the animal sprite when it's looking toward / away from the camera.</param>
        /// <param name="frontAndBackSpriteHeight">The height of the animal sprite when it's looking toward / away from the camera.</param>
        /// <param name="sideSpriteWidth">The width of the animal sprite when it's looking to the side.</param>
        /// <param name="sideSpriteHeight">The height of the animal sprite when it's looking to the side.</param>
        /// <param name="fullnessDrain">The amount the animal's hunger bar will drain each night.</param>
        /// <param name="happinessDrain">The amount the animal's happiness bar will drain each night.</param>
        /// <param name="buildings">The name(s) of the building(s) the animal can be housed in.</param>
        /// <param name="walkSpeed">The walk speed multiple of the animal.</param>
        /// <param name="bedTime">The time the animal will go to sleep.</param>
        /// <param name="seasonsAllowedOutdoors">The seasons the animal is able to go outside.</param>
        public AnimalData(string name, bool buyable, AnimalShopInfo animalShopInfo, List<AnimalSubType> types, int daysToProduce, int daysTillMature, string soundId, int frontAndBackSpriteWidth, 
            int frontAndBackSpriteHeight, int sideSpriteWidth, int sideSpriteHeight, byte fullnessDrain, byte happinessDrain, List<string> buildings, int walkSpeed, int bedTime, 
            List<Season> seasonsAllowedOutdoors)
        {
            Name = name;
            Buyable = buyable;
            AnimalShopInfo = animalShopInfo;
            Types = types;
            DaysToProduce = daysToProduce;
            DaysTillMature = daysTillMature;
            SoundId = soundId;
            FrontAndBackSpriteWidth = frontAndBackSpriteWidth;
            FrontAndBackSpriteHeight = frontAndBackSpriteHeight;
            SideSpriteWidth = sideSpriteWidth;
            SideSpriteHeight = sideSpriteHeight;
            FullnessDrain = fullnessDrain;
            HappinessDrain = happinessDrain;
            Buildings = buildings;
            WalkSpeed = walkSpeed;
            BedTime = bedTime;
            SeasonsAllowedOutdoors = seasonsAllowedOutdoors;
        }

        /// <summary>Check whether all the properties of the animal data are valid.</summary>
        /// <param name="animalName">The name of the animal (This is only used for error logging).</param>
        /// <returns>Whether the animal is valid.</returns>
        public bool IsValid(string animalName)
        {
            bool isValid = true;

            if (DaysToProduce < 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, DaysToProduce was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (DaysTillMature < 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, DaysTillMature was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            // TODO: ensure the sound id exists
            //if (SoundId == null)
            //{
                //ModEntry.ModMonitor.Log($"Animal Data Validation failed, SoundId was not valid on Animal: {animalName}.", LogLevel.Error);
                //isValid = false;
            //}

            if (FrontAndBackSpriteWidth <= 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, FrontAndBackSpriteWidth was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (FrontAndBackSpriteHeight <= 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, FrontAndBackSpriteHeight was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (SideSpriteWidth <= 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, SideSpriteWidth was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (SideSpriteHeight <= 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, SideSpriteHeight was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (FullnessDrain < 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, FullnessDrain was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (HappinessDrain < 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, HappinessDrain was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (AnimalShopInfo != null && AnimalShopInfo.BuyPrice < 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, AnimalShopInfo.BuyPrice was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (Buildings == null)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, Buildings was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (Types == null)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, Types was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            if (WalkSpeed <= 0)
            {
                ModEntry.ModMonitor.Log($"Animal Data Validation failed, WalkSpeed was not valid on Animal: {animalName}.", LogLevel.Error);
                isValid = false;
            }

            //if (BedTime < 0)
            //{
                //ModEntry.ModMonitor.Log($"Animal Data Validation failed, BedTime was not valid on Animal: {animalName}.", LogLevel.Error);
                //isValid = false;
            //}

            //if (SeasonsAllowedOutdoors == null)
            //{
                //ModEntry.ModMonitor.Log($"Animal Data Validation failed, SeasonsAllowedOutdoors was not valid on Animal: {animalName}.", LogLevel.Error);
                //isValid = false;
            //}

            return isValid;
        }
    }
}
