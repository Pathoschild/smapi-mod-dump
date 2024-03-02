/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Creates an image file for the given type of animal
    /// </summary>
    public class AnimalRandomizer : ImageBuilder
    {
        /// <summary>
        /// The type of animal this is randomizing
        /// </summary>
        private AnimalTypes AnimalTypeToRandomize { get; set; }

        /// <summary>
        /// A dictionary containing the last hue shift value chosen for the
        /// given animal type
        /// 
        /// This is used by the animal icon patcher to shift the matching icon colors
        /// when loading a farm
        /// </summary>
        public static Dictionary<AnimalTypes, int> LastHueShiftValue { get; private set; } = new();

        public AnimalRandomizer(AnimalTypes animalTypeToRandomize)
        {
            AnimalTypeToRandomize = animalTypeToRandomize;
            SubDirectory = Path.Combine("Animals", animalTypeToRandomize.ToString());
            StardewAssetPath = GetStardewAssetPath();
        }

        /// <summary>
        /// Get the path to the Stardew-equivalent asset based on the animal type
        /// </summary>
        /// <returns>The path of the xnb file to replace</returns>
        private string GetStardewAssetPath()
        {
            switch(AnimalTypeToRandomize)
            {
                case AnimalTypes.Horses:
                    return "Animals/horse";
                case AnimalTypes.Pets:
                    return "Animals/cat";
                default:
                    Globals.ConsoleWarn($"Stardew asset path undefined for animal type: {AnimalTypeToRandomize}");
                    return "";
            }
        }

        /// <summary>
        /// Build the image - hue shift it if the base file name ends with "-hue-shift"
        /// </summary>
        protected override Texture2D BuildImage()
        {
            string randomAnimalFileName = GetRandomAnimalFileName();
            string imageLocation = Path.Combine(ImageDirectory, randomAnimalFileName);
            Texture2D animalImage = Texture2D.FromFile(Game1.graphics.GraphicsDevice, imageLocation);

            LastHueShiftValue.Remove(AnimalTypeToRandomize);
            if (randomAnimalFileName[..^4].EndsWith("-hue-shift"))
            {
                Random rng = Globals.GetFarmRNG(nameof(AnimalRandomizer));
                int hueShiftValue = Range.GetRandomValue(0, 359, rng);
                animalImage = ImageManipulator.ShiftImageHue(animalImage, hueShiftValue);

                LastHueShiftValue[AnimalTypeToRandomize] = hueShiftValue;
            }

            if (ShouldSaveImage() && Globals.Config.SaveRandomizedImages)
            {
                using FileStream stream = File.OpenWrite(OutputFileFullPath);
                animalImage.SaveAsPng(stream, animalImage.Width, animalImage.Height);

                Globals.SpoilerWrite($"{AnimalTypeToRandomize} replaced with {randomAnimalFileName[..^4]}");
            }

            return animalImage;
        }

        /// <summary>
        /// Gets a random animal file name from the randomizers current directory
        /// This will use a new RNG seed so that we can get the pet name out of order
        /// </summary>
        /// <returns></returns>
        private string GetRandomAnimalFileName()
        {
            var animalImages = Directory.GetFiles($"{ImageDirectory}")
                .Where(x => x.EndsWith(".png") && !x.EndsWith(OutputFileName))
                .Select(x => Path.GetFileName(x))
                .OrderBy(x => x)
                .ToList();

            Random rng = Globals.GetFarmRNG(nameof(AnimalRandomizer));
            return Globals.RNGGetRandomValueFromList(animalImages, rng);

            // Uncomment to debug/test images, and comment out the other return
            // Change the animal type to the one you're testing
            //return AnimalTypeToRandomize == AnimalTypes.Horses
            //    ? "Horse-hue-shift.png"
            //    : Globals.RNGGetRandomValueFromList(animalImages, rng);
        }

        /// <summary>
        /// Gets the random pet type for the farm
        /// Returns it without the file extension, and all lowercase
        /// This should always be the same value since the seed should start in the same spot
        /// </summary>
        /// <param name="getOriginalName">Whether to get the original file name, with extension and capatalized</param>
        /// <returns></returns>
        public static string GetRandomAnimalName(AnimalTypes animalType, bool getOriginalName = false)
        {
            string originalFileName = new AnimalRandomizer(animalType).GetRandomAnimalFileName();
            return getOriginalName
                ? originalFileName
                : originalFileName[..^4].Replace("-hue-shift", "").ToLower();
        }

        /// <summary>
        /// Whether we should save the image
        /// Based on the appriate Animal randomize setting
        /// </summary>
        /// <returns>True if we should save; false otherwise</returns>
        public override bool ShouldSaveImage()
        {
            switch (AnimalTypeToRandomize)
            {
                case AnimalTypes.Horses: 
                    return Globals.Config.Animals.RandomizeHorses;
                case AnimalTypes.Pets:
                    return Globals.Config.Animals.RandomizePets;
                default:
                    Globals.ConsoleError($"Tried to save randomized image of unrecognized Animal type: {AnimalTypeToRandomize}");
                    return false;
            }
        }
    }
}
