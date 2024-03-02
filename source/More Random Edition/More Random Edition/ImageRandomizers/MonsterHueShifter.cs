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
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Shifts the hues of the list of monsters defined
    /// </summary>
    public class MonsterHueShifter
    {
        /// <summary>
        /// The path to the randomized images directory
        /// This is only used if the setting is on to save the images
        /// </summary>
        private static string RandomizedImagesDirectory =>
            Globals.GetFilePath(Path.Combine("assets", "CustomImages", "HueShiftedMonsters"));

        /// <summary>
        /// The list of monsters to hue shift
        /// These are under Characters/Monsters
        /// </summary>
        private readonly static List<string> MonstersToHueShift = new()
        {
            // Mines
            // 1-29
            "Bug",
            "Fly",
            "Duggy",
            "Grub",
            "Rock Crab",

            // 31-39
            "Bat",
            "Stone Golem",

            // 41-79
            "Dust Spirit",
            "Frost Bat",
            "Ghost",
            "Skeleton",

            // 81-119
            "Lava Bat",
            "Lava Crab",
            "Metal Head",
            "Shadow Brute",
            "Shadow Shaman",
            "Squid Kid",

            // Skull Cavern
            "Armored Bug",
            "Carbon Ghost",
            "Iridium Bat",
            "Iridium Crab",
            "Mummy",
            "Pepper Rex",
            "Serpent",

            // Quarry Mine
            "Haunted Skull",

            // Mustant Bug Lair
            "Dwarvish Sentry",
            "False Magma Cap",
            "Hot Head",
            "Lava Lurk",
            "Magma Sprite",
            "Magma Sparker",
            "Magma Duggy",

            // Wilderness Farm
            "Wilderness Golem",

            // Dangerous Mines
            // All floors
            "Haunted Skull_dangerous",

            // 1-29
            "Blue Squid",
            "Bug_dangerous",
            "Duggy_dangerous",
            "Rock Crab_dangerous",

            // 31-39
            "Bat_dangerous",
            "Stone Golem_dangerous",

            // 41-69
            "Grub_dangerous",
            "Fly_dangerous",
            "Dust Spirit_dangerous",
            "Frost Bat_dangerous",
            "Putrid Ghost",
            "Spider",
            "Stick Bug",

            // 71-79
            "Skeleton_dangerous",
            "Skeleton Mage_dangerous",

            // 81-119
            "Lava Crab_dangerous",
            "Metal Head_dangerous",
            "Shadow Brute_dangerous",
            "Shadow Shaman_dangerous",
            "Shadow Sniper",
            "Squid Kid_dangerous",
            
            // Dangerous Skull Cavern
            "Armored Bug_dangerous",
            "Mummy_dangerous",
            "Royal Serpent"
        };

        /// <summary>
        /// Gets a list of data containing hue-shifted monster data and their corresponding
        /// Stardew asset names so they can be replaced
        /// </summary>
        /// <returns />
        public static List<MonsterHueShiftData> GetHueShiftedMonsterAssets()
        {
            CleanUpRandomizedImageDirectory();

            List<MonsterHueShiftData> monsterHueShiftData = new();
            MonstersToHueShift.ForEach(monsterName =>
            {
                string monsterAssetPath = GetStardewAssetPath(monsterName);
                MonsterHueShiftData hueShiftedData = GetHueShiftedMonsterAsset(monsterName);
                monsterHueShiftData.Add(hueShiftedData);

                TrySaveImage(hueShiftedData);
            });
            return monsterHueShiftData;
        }

        /// <summary>
        /// Hue shifts the given monster path and returns the data object with its info
        /// </summary>
        /// <param name="monsterName">The name of the monster</param>
        /// <returns>The hue-shifted data</returns>
        private static MonsterHueShiftData GetHueShiftedMonsterAsset(string monsterName)
        {
            Texture2D monsterImage = Globals.ModRef.Helper.GameContent
                .Load<Texture2D>(GetStardewAssetPath(monsterName));

            Random rng = Globals.GetFarmRNG($"{nameof(MonsterHueShifter)}{monsterName}");
            int hueShiftAmount = Range.GetRandomValue(0, Globals.Config.Monsters.HueShiftMax, rng);

            return new MonsterHueShiftData(
                monsterName,
                ImageManipulator.ShiftImageHue(monsterImage, hueShiftAmount)
            );
        }

        /// <summary>
        /// Clean up the randomized image directory
        /// This is so they're gone if you turn off the setting
        /// </summary>
        public static void CleanUpRandomizedImageDirectory()
        {
            Directory.CreateDirectory(RandomizedImagesDirectory);
            DirectoryInfo directoryInfo = new(RandomizedImagesDirectory);
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// If we're saving randomized images, and there was actually a hue shift,
        /// then write the images to a CustomImages/HueShiftedMonsters directory
        /// </summary>
        /// <param name="image">The image</param>
        private static void TrySaveImage(MonsterHueShiftData monsterHueShiftData)
        {
            if (Globals.Config.SaveRandomizedImages && Globals.Config.Monsters.HueShiftMax > 0)
            {
                Texture2D image = monsterHueShiftData.MonsterImage;
                using FileStream stream = File.OpenWrite(
                    Path.Combine(RandomizedImagesDirectory, $"{monsterHueShiftData.AssetName}.png"));
                image.SaveAsPng(stream, image.Width, image.Height);
            }
        }

        /// <summary>
        /// Gets the path to the stardew asset
        /// </summary>
        /// <param name="assetName">The name of the asset</param>
        /// <returns>The path to the asset</returns>
        private static string GetStardewAssetPath(string assetName) 
            => $"Characters/Monsters/{assetName}";
    }

    /// <summary>
    /// The data to return back to the asset loader
    /// </summary>
    public class MonsterHueShiftData
    {
        /// <summary>
        /// The path to the asset
        /// </summary>
        public string StardewAssetPath => $"Characters/Monsters/{AssetName}";

        /// <summary>
        /// The name of the modified asset
        /// </summary>
        public string AssetName { get; private set; }

        /// <summary>
        /// The modified image
        /// </summary>
        public Texture2D MonsterImage { get; private set; }

        public MonsterHueShiftData(string assetName, Texture2D monsterImage)
        {
            AssetName = assetName;
            MonsterImage = monsterImage;
        }
    }
}
