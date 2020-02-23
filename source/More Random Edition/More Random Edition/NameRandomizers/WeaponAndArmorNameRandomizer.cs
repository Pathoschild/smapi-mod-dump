using System.Collections.Generic;

namespace Randomizer
{
	public class WeaponAndArmorNameRandomizer
	{
		private readonly List<string> Adjectives = new List<string>
		{
			"Ultra",
			"Stupid",
			"Bad",
			"Amazing",
			"Euphoric",
			"Dreamy",
			"Stoned",
			"Shiny",
			"Dull",
			"Dirty",
			"Putrid",
			"Deathly",
			"Cleansing",
			"Royal",
			"Sharp",
			"Painful",
			"Spicy",
			"Fabulous",
			"Salty",
			"Crunchy",
			"Radical",
			"Tubular",
			"Awesome",
			"Fluffy",
			"Outrageous",
			"Lethal",
			"Mean",
			"Golden",
			"Void",
			"Abyssal",
			"Prickly",
			"Vibrating",
			"Pulsing",
			"Spinning",
			"Unwieldy",
			"Hot",
			"Inconvenient",
			"Odd",
			"Serial",
			"Plastic",
			"Cool",
			"Sacred",
			"Wannabe",
			"Counterfeit",
			"Discount",
			"Special",
			"Fangorious",
			"Gelatinous",
			"Arcane",
			"Mystical",
			"Ultima",
			"Mega",
			"Super",
			"Giga",
			"Musical",
			"Hyper",
			"Gutsy",
			"Legendary",
			"Magicant",
			"Sweaty",
			"Holy",
			"Stabby",
			"Double",
			"Strange",
			"Unusual",
			"Gaudy",
			"Godly",
			"Wicked",
			"Radioactive",
			"Tempered",
			"Rusty",
			"Straw",
			"Wooden"
		};
		private readonly List<string> Nouns = new List<string>
		{
			"Maliwan",
			"Jakobs",
			"Hyperion",
			"Torgue",
			"Vladof",
			"Dahl",
			"Eridian",
			"Tediore",
			"S&S Munitions",
			"Device",
			"Bronze",
			"Iron",
			"Steel",
			"Black",
			"Mithril",
			"Adamant",
			"Rune",
			"Dragon",
			"White",
			"Coconono",
			"Aquarius",
			"Pisces",
			"Aries",
			"Taurus",
			"Gemini",
			"Cancer",
			"Leo",
			"Virgo",
			"Libra",
			"Scorpio",
			"Sagittarius",
			"Capricorn",
			"Master",
			"Kokiri",
			"Biggoron's",
			"Pickle",
			"Cereal",
			"Plastic",
			"Car",
			"Gun",
			"Grandma's",
			"Cat",
			"Kebab",
			"Cabbage",
			"Pie",
			"Pudding",
			"Mantle",
			"Potato",
			"Butter",
			"Flame",
			"Ice",
			"Coral",
			"Barbarian",
			"Sun",
			"Corn",
			"Kangaroo",
			"Australium",
			"Crystal",
			"Deku",
			"Steak",
			"Pangolin",
			"Atlas",
			"Anshin",
			"Snom",
			"Ranger",
			"Joja"
		};
		private readonly List<string> Swords = new List<string>
		{
			"Claymore",
			"Broadsword",
			"Sword",
			"Masamune",
			"Vorpal",
			"Katana",
			"Kikuichimonji",
			"Sol Blade",
			"Samehada",
			"Shichishito",
			"Scimitar",
			"Longsword",
			"Blade",
			"Spire",
			"Cutlass",
			"Falchion",
			"Sickle",
			"Rapier",
			"Edge",
			"Kukri",
			"Axe",
			"Cleaver",
			"Honedge",
			"Swryd",
			"Thing",
			"Chopper",
			"Deathbringer",
			"Harbringer",
			"Lifegiver",
			"Saw",
			"Lathe",
			"Tool",
			"Saber",
			"Wyrmkiller",
			"Greatsword",
			"Ragnarok",
			"Lightbringer",
			"Razer",
			"Enhancer",
			"Werebuster",
			"Excalibur",
			"Defender",
			"Braveheart",
			"Sword-chucks",
			"Pitchfork",
			"Trident",
			"Sikanda",
			"Dragnipur",
			"Chance",
			"Vengeance",
			"Stinger",
			"Brisingr",
			"Zar'roc",
			"Doomgiver",
			"Dragonslicer",
			"Stonecutter",
			"Mindsword",
			"Shieldbreaker",
			"Townsaver",
			"Wayfinder",
			"Soulcutter",
			"Coinspinner",
			"Sightblinder",
			"Wouldhealer",
			"Lightsaber"
		};
		private readonly List<string> Daggers = new List<string>
		{
			"Dirk",
			"Dagger",
			"Shiv",
			"Shard",
			"Nail",
			"Pencil",
			"Pushpin",
			"Needle",
			"Pin",
			"Knife",
			"Scissors",
			"Awl",
			"Shank",
			"Cactus",
			"Icicle",
			"Object",
			"Letter Opener",
			"Pen Nib",
			"Pen",
			"Spear",
			"Harpoon",
			"Javelin",
			"Bayonet",
			"Syringe",
			"Fork",
			"Spork",
			"Spoon",
			"Skewer",
			"Peeler",
			"Secateurs",
			"Kunai",
			"Switchblade",
			"Razor",
			"Drill",
			"Screwdriver",
			"Defender",
			"Toothpick",
			"Pricker",
			"Stabber",
			"Spike",
			"Stalagmite",
			"Stalactite",
			"Sharktooth",
			"Horn",
			"Carnwennan",
			"Slap Chop",
			"Cheese Grater",
			"Pizza Cutter",
			"Poker",
			"Stick",
			"Branch",
			"Twig",
			"Spoke",
			"Fencepost",
			"Stake"
		};
		private readonly List<string> HammersAndClubs = new List<string>
		{
			"Hammer",
			"Club",
			"Beatstick",
			"Pipe",
			"Mallet",
			"Board",
			"2x4",
			"Sign",
			"Gavel",
			"Bopper",
			"Gadderhammer",
			"Baton",
			"Bat",
			"Iron",
			"Brick",
			"Golf Club",
			"Frying Pan",
			"Rolling Pin",
			"Weapon",
			"Bowling Pin",
			"Wrench",
			"Post",
			"Bar",
			"Pole",
			"Log",
			"Column",
			"Battering Ram",
			"Door",
			"Kaleidoscope",
			"Drum",
			"Tire Iron",
			"Mop",
			"Broom",
			"Lamp",
			"Lightpost",
			"Streetlight",
			"Plank",
			"Rod",
			"Staff",
			"Polearm",
			"Mace",
			"Morningstar",
			"Anvil",
			"Bottle",
			"Hamshank",
			"Drumstick",
			"Remote",
			"Surfboard",
			"Skateboard",
			"Ski",
			"Snowboard",
			"Table",
			"Chair",
			"Barstool",
			"Shovel",
			"Spade",
			"Rake",
			"Bonecrusher"
		};
		private readonly List<string> Suffixes = new List<string>
		{
			"Pain",
			"Death",
			"Clouds",
			"Time",
			"Cleanliness",
			"Royalty",
			"Popularity",
			"Destiny",
			"Hunger",
			"Valor",
			"Bravery",
			"Courage",
			"Pointlessness",
			"Fate",
			"Battle",
			"War",
			"Triumph",
			"Famine",
			"Envy",
			"Greed",
			"Lust",
			"Gluttony",
			"Politeness",
			"Peace",
			"Conquest",
			"Pride",
			"Wrath",
			"Poop",
			"Magic",
			"the Forest",
			"the Ocean",
			"the Mountains",
			"Darkness",
			"Fakeness",
			"Old",
			"Burnination",
			"Ancients",
			"Doom",
			"Life",
			"Manliness",
			"Femininity",
			"Deliciousness",
			"Belly Aches",
			"Bluntness",
			"Harmony",
			"Kindness",
			"Loyalty",
			"Friendship",
			"Laughter",
			"Generosity",
			"Honesty",
			"Perfection",
			"Perspiration",
			"Sadness",
			"Motherhood",
			"Fatherhood",
			"Childhood",
			"Sisterhood",
			"Brotherhood",
			"Juiciness",
			"Instability",
			"Randomness",
			"Truth",
			"Griffindor",
			"Slytherin",
			"Hufflepuff",
			"Ravenclaw",
			"Chance",
			"Justice",
			"Heroes",
			"Vengeance",
			"Glory",
			"Force",
			"Stealth",
			"Despair",
			"Siege",
			"Fury",
			"Wisdom",
			"Mercy",
			"Power"
		};
		private readonly List<string> Boots = new List<string>
		{
			"Shoes",
			"Boots",
			"Sneakers",
			"Clogs",
			"Cleats",
			"Crocs",
			"Sandles",
			"High Heels",
			"Footwear",
			"Slippers",
			"Socks",
			"Moccasins",
			"Flip-Flops",
			"Snowshoes",
			"Flippers",
			"Rain Boots",
			"Galoshes",
			"Tap Shoes",
			"Rollerskates",
			"Ice Skates",
			"Elevator Shoes",
			"Bowling Shoes",
			"Ankle Boots",
			"Army Boots",
			"Cowboy Boots",
			"Gold Shoes",
			"Gumboots",
			"Brogues",
			"Espadrilles",
			"Heels",
			"Loafters",
			"Kamiks",
			"Mules",
			"Mukluks",
			"Pumps",
			"Slides",
			"Waders",
			"Zoris"
		};
		private readonly List<string> Tier1Slingshots = new List<string>
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
		private readonly List<string> Tier2Slingshots = new List<string>
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
		private readonly List<string> Tier3Slingshots = new List<string>
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
			string typeString = "";
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
			string typeString = "";
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
		/// </summary>
		/// <param name="typeString">The type string</param>
		/// <returns>The name in the format: [adjective] (noun) (typeString) [of suffix]</returns>
		private string GenerateRandomNonSlingshotName(string typeString)
		{
			string adjective = Globals.RNGGetNextBoolean() ? "" : Globals.RNGGetAndRemoveRandomValueFromList(Adjectives);
			string noun = Globals.RNGGetAndRemoveRandomValueFromList(Nouns);
			string suffix = Globals.RNGGetNextBoolean() ? "" : Globals.RNGGetAndRemoveRandomValueFromList(Suffixes);
			suffix = string.IsNullOrEmpty(suffix) ? "" : $"of {suffix}";

			return $"{adjective} {noun} {typeString} {suffix}".Trim();
		}
	}
}
