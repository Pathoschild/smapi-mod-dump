/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Extensions;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;

namespace StardewArchipelago.GameModifications.CodeInjections
{
    public class AppearanceRandomizer
    {
        private static IMonitor _monitor;
        private static ArchipelagoClient _archipelago;

        public AppearanceRandomizer(IMonitor monitor, ArchipelagoClient archipelago)
        {
            _monitor = monitor;
            _archipelago = archipelago;
        }

        public void ShuffleCharacterAppearances()
        {
            try
            {
                if (_archipelago.SlotData.AppearanceRandomization == AppearanceRandomization.Disabled)
                {
                    return;
                }

                var spritePool = GetSpritePool();
                if (!spritePool.Any())
                {
                    return;
                }

                foreach (var gameLocation in Game1.locations)
                {
                    foreach (var character in gameLocation.characters)
                    {
                        var isVillager = character.isVillager() || character is Pet;

                        if (!isVillager && _archipelago.SlotData.AppearanceRandomization == AppearanceRandomization.Villagers)
                        {
                            continue;
                        }

                        var chosenSprite = GetRandomAppearance(character.Sprite, character.Name, spritePool);
                        character.Sprite = new AnimatedSprite(chosenSprite.Key, 0, chosenSprite.Value.X, chosenSprite.Value.Y);
                    }
                }

                return;
            }
            catch (Exception ex)
            {
                _monitor.Log($"Failed in {nameof(ShuffleCharacterAppearances)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        private static List<KeyValuePair<string, Point>> GetSpritePool()
        {
            var spritePool = new List<KeyValuePair<string, Point>>();
            if (_archipelago.SlotData.AppearanceRandomization != AppearanceRandomization.Disabled)
            {
                spritePool.AddRange(_characterSpritesAndSize.Select(x =>
                    new KeyValuePair<string, Point>($"{_characterTexturePrefix}{x.Key}", x.Value)));

                if (_archipelago.SlotData.AppearanceRandomization != AppearanceRandomization.Villagers)
                {
                    // spritePool.AddRange(_monsterSprites.Select(x => $"{_monsterTexturePrefix}{x}"));
                    // spritePool.AddRange(_animalSprites.Select(x => $"{_animalTexturePrefix}{x}"));
                }
            }

            return spritePool;
        }

        private static KeyValuePair<string, Point> GetRandomAppearance(AnimatedSprite originalSprite, string characterName, List<KeyValuePair<string, Point>> spritePool)
        {
            var acceptableSprites = spritePool;
            var characterSize = new Point(originalSprite.SpriteWidth, originalSprite.SpriteHeight);
            if (_archipelago.SlotData.AppearanceRandomization != AppearanceRandomization.Chaos)
            {
                acceptableSprites = spritePool.Where(x => x.Value.Equals(characterSize)).ToList();
            }

            var random = GetSeededRandom(characterName);
            var chosenSpriteIndex = random.Next(0, acceptableSprites.Count);
            var chosenSprite = acceptableSprites[chosenSpriteIndex];
            return chosenSprite;
        }

        private static Random GetSeededRandom(string originalName)
        {
            var originalNameHashForSeed = Math.Abs(originalName.GetHash()) / 10;
            var seed = (int)Game1.uniqueIDForThisGame + originalNameHashForSeed;
            if (_archipelago.SlotData.AppearanceRandomizationDaily)
            {
                seed += (int)Game1.stats.DaysPlayed;
            }

            var random = new Random(seed);
            return random;
        }


        private const string _characterTexturePrefix = "Characters\\";
        private static readonly Dictionary<string, Point> _characterSpritesAndSize = new()
        {
            { "Junimo", new Point(16, 16)},
            { "Dwarf", new Point(16, 24) },
            { "Krobus", new Point(16, 24) },
            { "Krobus_Trenchcoat", new Point(16, 24) },
            { "TrashBear", new Point(32, 32) },
            { "Bear", new Point(32, 32) },
            { "Gourmand", new Point(32, 32) },
            { "LeahExMale", new Point(16, 32) },
            { "LeahExFemale", new Point(16, 32) },
            { "Toddler", new Point(16, 32) },
            { "Toddler_dark", new Point(16, 32) },
            { "Toddler_girl", new Point(16, 32) },
            { "Toddler_girl_dark", new Point(16, 32) },
            { "Birdie", new Point(16, 32) },
            { "ParrotBoy", new Point(16, 32) },
            { "George", new Point(16, 32) },
            { "Evelyn", new Point(16, 32) },
            { "Alex", new Point(16, 32) },
            { "Emily", new Point(16, 32) },
            { "Haley", new Point(16, 32) },
            { "Jodi", new Point(16, 32) },
            { "Sam", new Point(16, 32) },
            { "Vincent", new Point(16, 32) },
            { "Kent", new Point(16, 32) },
            { "Clint", new Point(16, 32) },
            { "Lewis", new Point(16, 32) },
            { "Caroline", new Point(16, 32) },
            { "Abigail", new Point(16, 32) },
            { "Pierre", new Point(16, 32) },
            { "Gus", new Point(16, 32) },
            { "Pam", new Point(16, 32) },
            { "Penny", new Point(16, 32) },
            { "Harvey", new Point(16, 32) },
            { "Elliott", new Point(16, 32) },
            { "Maru", new Point(16, 32) },
            { "Robin", new Point(16, 32) },
            { "Demetrius", new Point(16, 32) },
            { "Sebastian", new Point(16, 32) },
            { "Linus", new Point(16, 32) },
            { "Wizard", new Point(16, 32) },
            { "Marnie", new Point(16, 32) },
            { "Shane", new Point(16, 32) },
            { "Jas", new Point(16, 32) },
            { "Leah", new Point(16, 32) },
            { "MrQi", new Point(16, 32) },
            { "Sandy", new Point(16, 32) },
            { "Gunther", new Point(16, 32) },
            { "Marlon", new Point(16, 32) },
            { "Willy", new Point(16, 32) },
            { "Bouncer", new Point(16, 32) },
            { "Henchman", new Point(16, 32) },
            { "SafariGuy", new Point(16, 32) },
            { "Morris", new Point(16, 32) },
            { "Mariner", new Point(16, 32) },
            // { "maleRival", new Point(16, 32) },
            // { "femaleRival", new Point(16, 32) },
        };


        private static readonly string[] _villagers = new[]
        {
            "Birdie",
            "ParrotBoy",
            "George",
            "Evelyn",
            "Alex",
            "Emily",
            "Haley",
            "Jodi",
            "Sam",
            "Vincent",
            "Kent",
            "Clint",
            "Lewis",
            "Caroline",
            "Abigail",
            "Pierre",
            "Gus",
            "Pam",
            "Penny",
            "Harvey",
            "Elliott",
            "Maru",
            "Robin",
            "Demetrius",
            "Sebastian",
            "Linus",
            "Wizard",
            "Marnie",
            "Shane",
            "Jas",
            "Leah",
            "Sandy",
            "Gunther",
            "Marlon",
            "Willy",
        };
        private const string _monsterTexturePrefix = _characterTexturePrefix + "Monsters\\";

        private static readonly string[] _monsterSprites = new[]
        {
            "Wilderness Golem", "Skeleton", "Ghost", "Bat", "Big Slime", "Blue Squid", "Bug", "Pepper Rex", "Duggy",
            "Dust Spirit", "Dwarvish Sentry", "Fly", "Green Slime", "Grub", "Lava Crab", "Lava Lurk", "Spider",
            "Metal Head", "Mummy", "Rock Crab", "Stone Golem", "Serpent", "Shadow Brute", "Shadow Girl", "Shadow Guy",
            "Shadow Shaman", "Shadow Sniper", "Spiker", "Squid Kid",
        };

        private const string _animalTexturePrefix = "Animals\\";

        private static readonly string[] _animalSprites = new[]
        {
            "horse", "Dog", "Dog1", "Dog2", "Dog3", "cat", "cat1", "cat2", "cat3", "White Chicken", "BabyWhite Chicken",
            "Brown Chicken", "BabyBrown Chicken", "Duck", "Rabbit", "BabyRabbit", "Cow", "BabyCow", "Sheep", "ShearedSheep", "BabySheep", "Pig", "BabyPig",
        };
    }
}
