/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	public class WeaponAndArmorNameRandomizer
	{
        private class WeaponAndArmorNamePool
        {
            public string Name { get; set; }
            public List<string> Pool { get; set; }
            public List<string> UsedPool { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="name">The name of the pool - for logging only</param>
            /// <param name="defaultValues">The default values of the pool - will copy the given list</param>
            public WeaponAndArmorNamePool(string name, List<string> defaultValues)
            {
                Name = name;
                Pool = new(defaultValues);
                UsedPool = new();
            }

			/// <summary>
			/// Gets a random name from the pool and adds it to the "used" pool
			/// If the pool is empty, refreshes it and shows a message to the user
			/// </summary>
			/// <returns>The random name</returns>
			public string TryGetRandomValue()
			{
				if (!Pool.Any())
				{
					Pool.AddRange(UsedPool);
					UsedPool.Clear();
					Globals.ConsoleWarn($"Had to refresh the {Name} pool when randomizing weapon and armor names. This may be okay if mods are being used that add content.");
				}

				string randomName = Rng.GetAndRemoveRandomValueFromList(Pool);
                UsedPool.Add(randomName);

                return randomName;
			}
		}

        private static RNG Rng { get; set; }

        private static readonly List<string> Adjectives = new()
		{
            "Abyssal",
            "Amazing",
            "Arcane",
            "Awesome",
            "Bad",
            "Bleeding",
            "Brittle",
            "Brutal",
            "Cleansing",
            "Cool",
            "Counterfeit",
            "Crunchy",
            "Deathly",
            "Dirty",
            "Discount",
            "Double",
            "Dreamy",
            "Dull",
            "Euphoric",
            "Fabulous",
            "Fangorious",
            "Fluffy",
            "Gaudy",
            "Gelatinous",
            "Giga",
            "Godly",
            "Golden",
            "Gutsy",
            "Holy",
            "Hot",
            "Hyper",
            "Inconvenient",
            "Legendary",
            "Lethal",
            "Magicant",
            "Mean",
            "Mega",
            "Musical",
            "Mystical",
            "Odd",
            "Outrageous",
            "Painful",
            "Plastic",
            "Prickly",
            "Pulsing",
            "Putrid",
            "Radical",
            "Radioactive",
            "Royal",
            "Rusty",
            "Sacred",
            "Salty",
            "Serial",
            "Sharp",
            "Shiny",
            "Special",
            "Spicy",
            "Spinning",
            "Stabby",
            "Stoned",
            "Strange",
            "Straw",
            "Stupid",
            "Super",
            "Sweaty",
            "Tempered",
            "Tubular",
            "Ultima",
            "Ultra",
            "Unusual",
            "Unwieldy",
            "Vibrating",
            "Void",
            "Wannabe",
            "Wicked",
            "Wooden"
        };
		private static readonly List<string> Nouns = new()
		{
            "Adamant",
            "Anshin",
            "Aquarius",
            "Aries",
            "Atlas",
            "Australium",
            "Barbarian",
            "Biggoron's",
            "Black",
            "Bronze",
            "Butter",
            "Cabbage",
            "Cancer",
            "Capricorn",
            "Car",
            "Cat",
            "Cereal",
            "Coconono",
            "Coral",
            "Corn",
            "Crystal",
            "Dahl",
            "Deku",
            "Device",
            "Dragon",
            "Eridian",
            "Flame",
            "Gemini",
            "Grandma's",
            "Gun",
            "Hyperion",
            "Ice",
            "Iron",
            "Jakobs",
            "Joja",
            "Kangaroo",
            "Kebab",
            "Kokiri",
            "Leo",
            "Libra",
			"Maliwan",
            "Mantle",
            "Master",
            "Mithril",
            "Pangolin",
            "Pickle",
            "Pie",
            "Pisces",
            "Plastic",
            "Potato",
            "Pudding",
            "Ranger",
            "Rune",
            "S&S Munitions",
            "Sagittarius",
            "Scorpio",
            "Snom",
            "Steak",
            "Steel",
            "Sun",
            "Taurus",
            "Tediore",
            "Torgue",
            "Virgo",
            "Vladof",
            "White"
        };
		private static readonly List<string> Swords = new()
		{
            "Axe",
            "Blade",
            "Braveheart",
            "Brisingr",
            "Broadsword",
            "Chance",
            "Chopper",
            "Claymore",
            "Cleaver",
            "Coinspinner",
            "Cutlass",
            "Deathbringer",
            "Defender",
            "Doomgiver",
            "Dragnipur",
            "Dragonslicer",
            "Edge",
            "Enhancer",
            "Excalibur",
            "Falchion",
            "Greatsword",
            "Harbringer",
            "Honedge",
            "Katana",
            "Kikuichimonji",
            "Kukri",
            "Lathe",
            "Lifegiver",
            "Lightbringer",
            "Lightsaber",
            "Longsword",
            "Masamune",
            "Mindsword",
            "Pitchfork",
            "Ragnarok",
            "Rapier",
            "Razer",
            "Saber",
            "Samehada",
            "Saw",
            "Scimitar",
            "Shichishito",
            "Shieldbreaker",
            "Sickle",
            "Sightblinder",
            "Sikanda",
            "Sol Blade",
            "Soulcutter",
            "Spire",
            "Stinger",
            "Stonecutter",
            "Sword",
            "Sword-chucks",
            "Swryd",
            "Thing",
            "Tool",
            "Townsaver",
            "Trident",
            "Vengeance",
            "Vorpal",
            "Wayfinder",
            "Werebuster",
            "Wouldhealer",
            "Wyrmkiller",
            "Zar'roc"
        };
		private static readonly List<string> Daggers = new()
		{
            "Awl",
            "Bayonet",
            "Branch",
            "Cactus",
            "Carnwennan",
            "Cheese Grater",
            "Cleaver",
            "Dagger",
            "Defender",
            "Dirk",
            "Drill",
            "Fencepost",
            "Fork",
            "Harpoon",
            "Horn",
            "Icicle",
            "Javelin",
            "Knife",
            "Kunai",
            "Letter Opener",
            "Nail",
            "Needle",
            "Object",
            "Peeler",
            "Pen Nib",
            "Pen",
            "Pencil",
            "Pin",
            "Pizza Cutter",
            "Poker",
            "Pricker",
            "Pushpin",
            "Razor",
            "Scissors",
            "Screwdriver",
            "Secateurs",
            "Shank",
            "Shard",
            "Sharktooth",
            "Shiv",
            "Skewer",
            "Slap Chop",
            "Spear",
            "Spike",
            "Spoke",
            "Spoon",
            "Spork",
            "Stabber",
            "Stake",
            "Stalactite",
            "Stalagmite",
            "Stick",
            "Switchblade",
            "Syringe",
            "Toothpick",
            "Twig"
        };
		private static readonly List<string> HammersAndClubs = new()
		{
            "2x4",
            "Anvil",
            "Bar",
            "Barstool",
            "Bat",
            "Baton",
            "Battering Ram",
            "Beatstick",
            "Board",
            "Bonecrusher",
            "Bopper",
            "Bottle",
            "Bowling Pin",
            "Brick",
            "Broom",
            "Chair",
            "Club",
            "Column",
            "Door",
            "Drum",
            "Drumstick",
            "Frying Pan",
            "Gadderhammer",
            "Gavel",
            "Golf Club",
            "Hammer",
            "Hamshank",
            "Iron",
            "Kaleidoscope",
            "Lamp",
            "Lightpost",
            "Log",
            "Mace",
            "Mallet",
            "Mop",
            "Morningstar",
            "Pipe",
            "Plank",
            "Pole",
            "Polearm",
            "Post",
            "Rake",
            "Remote",
            "Rod",
            "Rolling Pin",
            "Shovel",
            "Sign",
            "Skateboard",
            "Ski",
            "Sledgehammer",
            "Snowboard",
            "Spade",
            "Staff",
            "Streetlight",
            "Surfboard",
            "Table",
            "Tire Iron",
            "Weapon",
            "Wrench"
        };
		private static readonly List<string> Suffixes = new()
		{
            "Ancients",
            "Arceuus",
            "Baked Goods",
            "Battle",
            "Belly Aches",
            "Bluntness",
            "Bravery",
            "Brotherhood",
            "Burnination",
            "Chance",
            "Childhood",
            "Cleanliness",
            "Clouds",
            "Conquest",
            "Courage",
            "Darkness",
            "Death",
            "Deliciousness",
            "Despair",
            "Destiny",
            "Doom",
            "Envy",
            "Fakeness",
            "Famine",
            "Fate",
            "Fatherhood",
            "Femininity",
            "Force",
            "Friendship",
            "Fury",
            "Generosity",
            "Glory",
            "Gluttony",
            "Greed",
            "Griffindor",
            "Harmony",
            "Heroes",
            "Honesty",
            "Hosidius",
            "Hufflepuff",
            "Hunger",
            "Instability",
            "Juiciness",
            "Justice",
            "Kindness",
            "Laughter",
            "Life",
            "Lovakengj",
            "Loyalty",
            "Lust",
            "Magic",
            "Manliness",
            "Mercy",
            "Motherhood",
            "My Mom",
            "Old",
            "Pain",
            "Peace",
            "Perfection",
            "Perspiration",
            "Piscarilius",
            "Pointlessness",
            "Politeness",
            "Poop",
            "Popularity",
            "Power",
            "Pride",
            "Randomness",
            "Ravenclaw",
            "Royalty",
            "Sadness",
            "Shayzien",
            "Siege",
            "Sisterhood",
            "Slytherin",
            "Stealth",
            "the Forest",
            "the Mountains",
            "the Ocean",
            "Time",
            "Triumph",
            "Truth",
            "Valor",
            "Vengeance",
            "War",
            "Wisdom",
            "Wrath"
        };
		private static readonly List<string> Boots = new()
		{
            "Ankle Boots",
            "Army Boots",
            "Boots",
            "Bowling Shoes",
            "Brogues",
            "Cleats",
            "Clogs",
            "Cowboy Boots",
            "Crocs",
            "Elevator Shoes",
            "Espadrilles",
            "Flip-Flops",
            "Flippers",
            "Footwear",
            "Galoshes",
            "Gold Shoes",
            "Gumboots",
            "Heels",
            "High Heels",
            "Ice Skates",
            "Kamiks",
            "Loafers",
            "Moccasins",
            "Mukluks",
            "Mules",
            "Pumps",
            "Rain Boots",
            "Rollerskates",
            "Sandles",
            "Shoes",
            "Slides",
            "Slippers",
            "Sneakers",
            "Snowshoes",
            "Socks",
            "Tap Shoes",
            "Waders",
            "Zoris"
        };
		private static readonly List<string> Tier1Slingshots = new()
		{
			"Ice Cream",
			"Terrible",
			"Bandit",
			"Pea Shooter",
			"Spongy",
			"Sticky",
			"Messy",
			"Inaccurate",
			"Weak",
			"Erratic"
		};
		private static readonly List<string> Tier2Slingshots = new()
		{
			"Moderate",
			"Okay",
			"Average",
			"Existant",
			"Accurate",
			"Boring",
			"A",
			"Secondhand",
			"Better"
		};
		private static readonly List<string> Tier3Slingshots = new()
		{
			"Fairy",
			"Ultimate",
			"Powerful",
			"Precise",
			"S-Tier",
			"The",
			"Golden",
			"Amethyst",
			"Superb"
		};

        private readonly WeaponAndArmorNamePool AdjectivePool = new("Adjective", Adjectives);
		private readonly WeaponAndArmorNamePool NounPool = new("Noun", Nouns);
		private readonly WeaponAndArmorNamePool SwordPool = new("Sword", Swords);
		private readonly WeaponAndArmorNamePool DaggerPool = new("Dagger", Daggers);
		private readonly WeaponAndArmorNamePool HammerAndClubPool = new("Hammer and Club", HammersAndClubs);
		private readonly WeaponAndArmorNamePool SuffixPool = new("Suffix", Suffixes);
		private readonly WeaponAndArmorNamePool BootPool = new("Boot", Boots);
		private readonly WeaponAndArmorNamePool Tier1SlingshotPool = new("Tier 1 Slingshots", Tier1Slingshots);
		private readonly WeaponAndArmorNamePool Tier2SlingshotPool = new("Tier 2 Slingshots", Tier2Slingshots);
		private readonly WeaponAndArmorNamePool Tier3SlingshotPool = new("Tier 3 Slingshots", Tier3Slingshots);

		/// <summary>
		/// The constuctor - uses it's own name and a seed addendum so that the seeds are
		/// not always the same when its RNG is used
		/// </summary>
		/// <param name="seedAddendum">The seed addendum - probably the calling class name</param>
		public WeaponAndArmorNameRandomizer(string seedAddendum)
        {
            Rng = RNG.GetFarmRNG($"{nameof(WeaponAndArmorNameRandomizer)}.{seedAddendum}");
        }

		/// <summary>
		/// Generates a random name for the given weapon type
		/// </summary>
		/// <param name="type">The weapon type</param>
		/// <param name="slingshotId">
		///		The slingshot tier, from 0 - 2 = worst to best
		/// </param>
		/// <returns></returns>
		public string GenerateRandomWeaponName(WeaponType type, WeaponIndexes slingshotId = 0)
		{
			string typeString;
			switch (type)
			{
				case WeaponType.SlashingSword:
				case WeaponType.StabbingSword:
					typeString = SwordPool.TryGetRandomValue();
					break;
				case WeaponType.Dagger:
					typeString = DaggerPool.TryGetRandomValue();
					break;
				case WeaponType.ClubOrHammer:
                    typeString = HammerAndClubPool.TryGetRandomValue();
					break;
				case WeaponType.Slingshot:
					return GenerateRandomSlingshotName(slingshotId);
				default:
					Globals.ConsoleError($"Trying to generate weapon name for invalid type: {type}");
					return "ERROR";
			}

			return GenerateRandomNonSlingshotName(typeString);
		}

		/// <summary>
		/// Generates a random slingshot name
		/// </summary>
		/// <param name="slingshotId">The id of the slingshot weapon</param>
		/// <returns></returns>
		private string GenerateRandomSlingshotName(WeaponIndexes slingshotId)
		{
			string typeString;
			switch (slingshotId)
			{
				case WeaponIndexes.Slingshot:
                    typeString = Tier1SlingshotPool.TryGetRandomValue();
					break;
				case WeaponIndexes.MasterSlingshot:
					typeString = Tier2SlingshotPool.TryGetRandomValue();
					break;
				case WeaponIndexes.GalaxySlingshot:
					typeString = Tier3SlingshotPool.TryGetRandomValue();
					break;
				default:
					Globals.ConsoleError($"Trying to generate slingshot name for invalid id: {slingshotId}");
					return "ERROR";
			}

			return $"{typeString} Slingshot";
		}

		/// <summary>
		/// Generates a random boot name
		/// </summary>
		/// <returns>The name in the format: [adjective] (noun) (boot string) [of suffix]</returns>
		public string GenerateRandomBootName()
		{
            string bootName = BootPool.TryGetRandomValue();
			return GenerateRandomNonSlingshotName(bootName);
		}

		/// <summary>
		/// Generates a random name for a non-slingshot item (weapon or boots), given the already generated type string
        /// Will not include an adjective AND a suffix, cause it's way too long
		/// </summary>
		/// <param name="typeString">The type string</param>
		/// <returns>The name in the format: [adjective] (noun) (typeString) [of suffix]</returns>
		private string GenerateRandomNonSlingshotName(string typeString)
		{
            string adjective = "";
            string suffix = "";

            bool useAdjectiveOrSuffix = Rng.NextBoolean();
            if (useAdjectiveOrSuffix)
            {
                bool useAdjective = Rng.NextBoolean();
                if (useAdjective)
                {
                    adjective = AdjectivePool.TryGetRandomValue();
                }
                else
                {
                    suffix = $"of {SuffixPool.TryGetRandomValue()}";
                }
            }

			string noun = NounPool.TryGetRandomValue();
			return $"{adjective} {noun} {typeString} {suffix}".Trim();
		}
	}
}
