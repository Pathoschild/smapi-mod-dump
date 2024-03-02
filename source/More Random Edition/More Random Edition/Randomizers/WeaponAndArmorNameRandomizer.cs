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

namespace Randomizer
{
	public class WeaponAndArmorNameRandomizer
	{
		private readonly List<string> Adjectives = new()
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
		private readonly List<string> Nouns = new()
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
		private readonly List<string> Swords = new()
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
		private readonly List<string> Daggers = new()
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
		private readonly List<string> HammersAndClubs = new()
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
		private readonly List<string> Suffixes = new()
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
		private readonly List<string> Boots = new()
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
		private readonly List<string> Tier1Slingshots = new()
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
		private readonly List<string> Tier2Slingshots = new()
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
		private readonly List<string> Tier3Slingshots = new()
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

		/// <summary>
		/// Generates a random name for the given weapon type
		/// </summary>
		/// <param name="type">The weapon type</param>
		/// <param name="slingshotTier">
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
					typeString = Globals.RNGGetAndRemoveRandomValueFromList(Swords);
					break;
				case WeaponType.Dagger:
					typeString = Globals.RNGGetAndRemoveRandomValueFromList(Daggers);
					break;
				case WeaponType.ClubOrHammer:
					typeString = Globals.RNGGetAndRemoveRandomValueFromList(HammersAndClubs);
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
					typeString = Globals.RNGGetAndRemoveRandomValueFromList(Tier1Slingshots);
					break;
				case WeaponIndexes.MasterSlingshot:
					typeString = Globals.RNGGetAndRemoveRandomValueFromList(Tier2Slingshots);
					break;
				case WeaponIndexes.GalaxySlingshot:
					typeString = Globals.RNGGetAndRemoveRandomValueFromList(Tier3Slingshots);
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
			string bootName = Globals.RNGGetAndRemoveRandomValueFromList(Boots);
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

            bool useAdjectiveOrSuffix = Globals.RNGGetNextBoolean();
            if (useAdjectiveOrSuffix)
            {
                bool useAdjective = Globals.RNGGetNextBoolean();
                if (useAdjective)
                {
                    adjective = Globals.RNGGetAndRemoveRandomValueFromList(Adjectives);
                }
                else
                {
                    suffix = $"of {Globals.RNGGetAndRemoveRandomValueFromList(Suffixes)}";
                }
            }

			string noun = Globals.RNGGetAndRemoveRandomValueFromList(Nouns);
			return $"{adjective} {noun} {typeString} {suffix}".Trim();
		}
	}
}
