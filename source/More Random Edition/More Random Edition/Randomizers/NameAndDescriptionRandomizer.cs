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
using System.Runtime.CompilerServices;

namespace Randomizer
{
	public class NameAndDescriptionRandomizer
	{
		public static List<string> GenerateVegetableNames(int numberOfNames)
		{
			List<string> adjectives = new()
			{
				"African",
				"Asian",
				"Better",
				"Bitter",
				"Bok",
				"Bright",
				"Bygone",
				"Cold",
				"Common",
				"European",
				"Funky",
				"Giant",
				"Hot",
				"Juicy",
				"Miracle",
				"Moist",
				"Pelican",
				"Prickly",
				"Primordial",
				"Salty",
				"Savory",
				"Sour",
				"Spicy",
				"Stardew",
				"Sunny",
				"Super",
				"Sweet",
				"Tiny",
				"Whovian",
				"Wild",
				"Young"
            };

			List<string> prefixes = new()
			{
				"Aarti",
				"Alfa",
				"Apri",
				"Bana",
				"Boysen",
				"Cabba",
				"Carro",
				"Cauli",
				"Choco",
				"Clementi",
				"Coco",
				"Coffe",
				"Crann",
				"Cucu",
				"Dai",
				"Dew",
				"Drago",
				"Egg",
				"Honey",
				"Huckle",
				"Joja",
				"Kiwi",
				"Lemo",
				"Lime",
				"Luv",
				"Mang",
				"Mc",
				"Passion",
				"Pear",
				"Pome",
				"Pota",
				"Pump",
				"Pyne",
				"Radi",
				"Rasp",
				"Rue",
				"Squa",
				"Star",
				"Tange",
				"Tama",
				"Toma",
				"Vege",
				"Water",
				"Zucchi"
            };

			List<string> suffixes = new()
			{
                "apple",
				"ate",
				"barb",
				"bean",
				"berry",
				"cado",
				"ccio",
				"choke",
				"dew",
				"dropp",
				"fig",
				"flour",
				"froot",
				"granite",
				"iander",
				"jube",
				"korn",
				"lait",
				"lli",
				"loupe",
				"melon",
				"nana",
				"go",
				"onion",
				"paya",
				"pear",
				"pepper",
				"plum",
				"quat",
				"rant",
				"ranth",
				"rene",
				"rillo",
				"rind",
				"root",
				"rry",
				"sai",
				"siini",
				"snip",
				"tato",
				"to",
				"trout",
				"y",
				"yam",
				"zap"
            };

			return CreateNameFromPieces(numberOfNames, adjectives, prefixes, suffixes);
		}

		public static List<string> GenerateFlowerNames(int numberOfNames)
		{
			List<string> adjectives = new()
			{
				"Aromatic",
				"Common",
				"Creeping",
				"Fairy",
				"False",
				"Field",
				"Flowering",
				"Fragrant",
				"Giant",
				"Lesser",
				"Lovely",
				"Morning",
				"Rough",
				"Soft",
				"Stinky",
				"Sweet",
				"True",
				"Ugly",
				"Wild"
            };

			List<string> prefixes = new()
			{
				"Aza",
				"Bell",
				"Bella",
				"Canna",
				"Cro",
				"Daffo",
				"Forget-me-",
				"Frangi",
				"Hem",
				"Hibi",
				"Hya",
				"Jasmi",
				"Lili",
				"Mary",
				"Night",
				"Olea",
				"Poi",
				"Rhodo",
				"Snap",
				"Sun",
				"Vio"
            };

			List<string> suffixes = new()
			{
				"bane",
				"cissus",
				"dendrite",
				"dragon",
				"drop",
				"flower",
				"fodil",
				"hock",
				"iris",
				"kite",
				"laurel",
				"lilac",
				"lily",
				"lip",
				"mellia",
				"nettle",
				"pad",
				"rose",
				"ster",
				"suckle",
				"synth",
                "turtium",
                "weed",
				"wort"
            };

			return CreateNameFromPieces(numberOfNames, adjectives, prefixes, suffixes);
		}

		public static List<string> GenerateFishNames(int numberOfNames)
		{
			List<string> adjectives = new()
			{
				"Armored",
				"Atlantic",
				"Bigeye",
				"Bristlemouth",
				"Cute",
				"Cutthroat",
				"Deepwater",
				"Dwarf",
				"Electric",
				"Fat",
				"Flat",
				"Fiery",
				"Flathead",
				"Fresh",
				"Freshwater",
				"Giant",
				"Glass",
				"Ice",
				"Jumping",
				"Largemouth",
				"Lava",
				"Northern",
				"Mature",
				"Pacific",
				"Rainbow",
				"Ribbon",
				"Round",
				"Sabertooth",
				"Saltwater",
				"Salty",
				"Sandy",
				"Sixgill",
				"Smallmouth",
				"Southern",
				"Super",
				"Thorn",
				"Toxic",
				"Ugly",
				"Void",
				"Whale",
				"Whitetip"
            };

			List<string> prefixes = new()
			{
				"Alba",
				"Ancho",
				"Ange",
				"Ba",
				"Bara",
				"Bone",
				"Box",
				"Bram",
				"Brea",
				"Bri",
				"Bull",
				"Can",
				"Car",
				"Chu",
				"Dab",
				"Dart",
				"Dog",
				"Dour",
				"Drago",
				"Ee",
				"Floun",
				"Gob",
				"Gost",
				"Grou",
				"Had",
				"Halli",
				"Ho",
				"Ina",
				"Kelp",
				"Koi",
				"Lem",
				"Linc",
				"Lun",
				"Ma",
				"Mar",
				"Milk",
				"Moon",
				"Mor",
				"Mud",
				"Mul",
				"Octop",
				"Per",
				"Pi",
				"Piran",
				"Pol",
				"Puffer",
				"Quill",
				"Rain",
				"Rock",
				"Sal",
				"Sha",
				"Sol",
				"Son",
				"Squea",
				"Squi",
				"Star",
				"Sting",
				"Tilla",
				"Trou",
				"Tu",
				"Un",
				"Vel",
				"Wal",
				"Wo"
            };

			List<string> suffixes = new List<string>
			{
				"ace",
				"ad",
				"ado",
				"ag",
				"ail",
				"apia",
				"apper",
				"arp",
				"ass",
				"b",
				"bu",
				"ch",
				"chore",
				"cod",
				"cuda",
				"d",
				"dine",
				"diru",
				"enn",
				"et",
				"eye",
				"f",
				"fin",
				"g",
				"gel",
				"geon",
				"ghoti",
				"h",
				"ha",
				"io",
				"ion",
				"ish",
				"jack",
				"k",
				"kerel",
				"ki",
				"le",
				"let",
				"ley",
				"ling",
				"luga",
				"m",
				"m",
				"ma",
				"ng",
				"now",
				"oach",
				"ock",
				"opus",
				"out",
				"ovy",
				"phish",
				"ray",
				"rring",
				"sh",
				"shark",
				"skip",
				"skipper",
				"sucker",
				"t",
				"ta",
				"tongue",
				"ub",
				"umber"
            };

			return CreateNameFromPieces(numberOfNames, adjectives, prefixes, suffixes);
		}

		public static List<string> GenerateCropDescriptions(int numberOfDescriptions)
		{
			List<string> descriptionBases = new()
			{
				"Loved by Lord [name] for its [adjective] taste.",
				"The favorite food of the Marquis de [noun].",
				"Tastes like [noun].",
				"Very [adjective], but not [adjective2].",
				"Like [noun], not wholly unpleasant.",
				"First cultivated by the [adjective] Dr. [name].",
				"Would be great with slices of [noun].",
				"Part of a [adjective] breakfast!",
				"Your Aunt [name]'s favorite as a child.",
				"Back in 1945, these were called [noun].",
				"It reeks of [adjective]!",
				"A staple food of the Isle of [noun].",
				"Tastes like [noun] mixed with [noun2].",
				"Also sold at [name]'s Grocery.",
				"Like licking [adjective] plastic.",
				"Mc[name]'s new burger uses these as a topping.",
				"An amazing topping for [noun]!",
				"A popular frosting for [name] Cake.",
				"Adds a [adjective] flavor to dishes.",
				"Closely related to [noun].",
				"Makes a great side to entrees of [noun].",
				"Popular with [name], Ruler of Parsnandia.",
				"It's [adjective] with a hint of sweetness.",
				"The popular alcohol, [name]'s [noun], is fermented from this crop.",
				"Its [adjective] flavor may make some go blind when tasted.",
				"Can be used medicinally to cure [adjective] [noun] disease.",
				"It's [noun]-licking good!",
				"Often used in [adjective] desserts.",
				"An extremely [adjective] crop that loves to be watered.",
				"Slightly [adjective] with an aftertaste of [noun]",
				"Your friend [name] is allergic to these.",
				"Nobody knows whether it's a fruit, vegetable, or none of the above.",
				"Professional chefs never cook this with [noun].",
				"The renowned chef, [name] Ramsay, always eats it fresh.",
				"Its sweet aroma may attract [noun].",
				"Can be carved into a festive decoration for St. [name]'s Day.",
				"One of the most [adjective] things you've ever smelled.",
				"The skin has a high concentration of [noun].",
				"Discovered falling from the sky during Hurricane [name].",
				"If your name is [name] you will love this crop!",
				"A key ingredient in [name] brand energy drinks.",
				"Beloved by the masses.",
				"Always served at the [name] Fan Club's annual conventions.",
				"It's a mystery to everyone.",
				"Hated by lovers of [noun] everywhere.",
				"Some people find it too [adjective] for them.",
				"Tastes [adjective] when steeped into tea.",
				"Karate master [name] Lee first cultivated this crop.",
				"Perfect for tea with Queen [name].",
				"Not recommented to consume with [noun].",
                "Invented by [name], the genius behind this [adjective] [noun] crop.",
                "In [name]'s garden, the [adjective] [noun] bloom, spreading laughter across the fields.",
                "Legend speaks of [name], whose [adjective] [noun] bring a touch of magic to the ordinary fields.",
				"Don't let [name] see it.",
				"[name] would love this for their birthday!",
				"The main ingredient in the popular cereal, Cap'n [name]."
            };
			List<string> nouns = new()
			{
				"aprons",
				"backs",
				"balloons",
				"beans",
				"bears",
				"belts",
				"bermudas",
				"blackboards",
				"boxer shorts",
				"brains",
				"bread",
				"brinjals",
				"buckles",
				"bulbs",
				"butter",
				"cats",
				"cauliflowers",
				"chairs",
				"cheeks",
				"chests",
				"crows",
				"desks",
				"dogs",
				"drumsticks",
				"earbuds",
				"earrings",
				"ears",
				"eggs",
				"eyes",
				"fans",
				"feet",
				"foxes",
				"freezers",
				"frocks",
				"frogs",
				"ginger",
				"goats",
				"gowns",
				"grains",
				"grapes",
				"guavas",
				"hairs",
				"hands",
				"hats",
				"iron boxes",
				"jackets",
				"jaws",
				"koel birds",
				"ladyfingers",
				"leggings",
				"legs",
				"lighters",
				"lips",
				"lungs",
				"mangoes",
				"mice",
				"milk",
				"monkeys",
				"music players",
				"necks",
				"noses",
				"oranges",
				"ostriches",
				"ovens",
				"palms",
				"pandas",
				"panthers",
				"parrots",
				"pasta",
				"peacocks",
				"pen drives",
				"pencils",
				"pigeons",
				"pocket watches",
				"pomegranates",
				"ribs",
				"rice",
				"sandals",
				"sharks",
				"sheep",
				"shirts",
				"spoons",
				"stairs",
				"stoves",
				"suits",
				"swans",
				"sweaters",
				"tables",
				"tailor birds",
				"ties",
				"tigers",
				"toes",
				"tongues",
				"trousers",
				"turkeys",
				"underarms",
				"vests",
				"watermelons",
				"whales",
				"wolves",
				"wrists"
            };
			List<string> adjectives = new()
			{
				"abashed",
				"abject",
				"acid",
				"acidic",
				"alcoholic",
				"angry",
				"astonishing",
				"available",
				"blue-eyed",
				"blue",
				"bright",
				"bumpy",
				"careless",
				"caring",
				"chivalrous",
				"chunky",
				"cold",
				"common",
				"cowardly",
				"cruel",
				"curious",
				"debonair",
				"deep",
				"defeated",
				"defective",
				"delicate",
				"domineering",
				"dusty",
				"earthy",
				"eatable",
				"economic",
				"enchanting",
				"encouraging",
				"evanescent",
				"evasive",
				"faint",
				"fair",
				"festive",
				"first",
				"fluffy",
				"friendly",
				"general",
				"gentle",
				"gleaming",
				"healthy",
				"holistic",
				"hungry",
				"ill-informed",
				"inquisitive",
				"insidious",
				"jobless",
				"kaput",
				"lewd",
				"loving",
				"lucky",
				"marvelous",
				"muddled",
				"mundane",
				"nippy",
				"nostalgic",
				"old",
				"plain",
				"possessive",
				"psychotic",
				"public",
				"quickest",
				"quiet",
				"real",
				"rebel",
				"receptive",
				"reflective",
				"relieved",
				"romantic",
				"rude",
				"scientific",
				"silent",
				"silky",
				"spooky",
				"square",
				"squealing",
				"striped",
				"succinct",
				"synonymous",
				"temporary",
				"tiresome",
				"toothsome",
				"tough",
				"unnatural",
				"useless",
				"wandering",
				"warlike",
				"watery",
				"whispering",
				"witty",
				"wooden",
				"woozy",
				"worried",
				"wretched",
				"wrists",
				"wry",
				"youthful"
            };
			List<string> names = new()
			{
				"Abigail",
				"Adele",
				"Alberto",
				"Alden",
				"Alex",
				"Alison",
				"Alvin",
				"Amanda",
				"Antonio",
				"Armando",
				"Ava",
				"Barney",
				"Barry",
				"Benita",
				"Bradley",
				"Brock",
				"Cameron",
				"Cara",
				"Caroline",
				"Chester",
				"Clare",
				"Clint",
				"Colin",
				"Cornell",
				"Daphne",
				"David",
				"Deanne",
				"Demetrius",
				"Dorthy",
				"Dwarf",
				"Edwina",
				"Elliott",
				"Elvin",
				"Emily",
				"Enrique",
				"Evelyn",
				"Ezra",
				"Fannie",
				"Fran",
				"Freddy",
				"Fritz",
				"Gay",
				"George",
				"Gerardo",
				"Gus",
				"Haley",
				"Harold",
				"Harvey",
				"Helene",
				"Hollie",
				"Hong",
				"Howard",
				"Hunter",
				"Jas",
				"Jean",
				"Jerome",
				"Jerri",
				"Jesse",
				"Joanna",
				"Jodi",
				"Joesph",
				"Juana",
				"Julia",
				"Kara",
				"Karin",
				"Kent",
				"Kip",
				"Krobus",
				"Lara",
				"Lavern",
				"Leah",
				"Leif",
				"Leo",
				"Leonardo",
				"Lewis",
				"Lillian",
				"Lillie",
				"Linus",
				"Lionel",
				"Lola",
				"Loretta",
				"Lourdes",
				"Lydia",
				"Mariano",
				"Marnie",
				"Maru",
				"Maryanne",
				"Matt",
				"Mattie",
				"Maximo",
				"Michelle",
				"Miranda",
				"Moises",
				"Morris",
				"Myrna",
				"Nicholas",
				"Nicky",
				"Nigel",
				"Norma",
				"Normand",
				"Pam",
				"Patsy",
				"Penny",
				"Pete",
				"Pierre",
				"Ramiro",
				"Raymond",
				"Reed",
				"Rene",
				"Reva",
				"Rhoda",
				"Rickie",
                "Rasmodius",
                "Roberta",
				"Robin",
				"Robt",
				"Rosanna",
				"Sam",
				"Sandy",
				"Saundra",
				"Sebastian",
				"Selma",
				"Shana",
				"Shane",
				"Shelton",
				"The Wizard",
				"Vincent",
				"Walker",
				"Wayne",
				"Wiley",
				"Willy",
				"Wilmer",
				"Zelma"
            };

			return CreateDescriptionFromPieces(numberOfDescriptions, descriptionBases, nouns, adjectives, names);
		}

		public static List<string> GenerateBootDescriptions(int numberOfDescriptions)
		{
			List<string> descriptionBases = new()
			{
				"A little [adjective]... but [adjective2]!",
				"Protection from the [noun].",
				"The [noun] are very [adjective].",
				"They're [adjective] for extra [noun].",
				"Reinforced with [adjective] [noun].",
				"The [adjective] lining keeps your [noun] so [adjective2].",
				"Designed with extreme [noun] in mind.",
				"Made from [adjective] black [noun].",
				"It's said these can withstand the [adjective] [noun].",
				"The [adjective] [noun] permeate the fabric.",
				"The [adjective] [noun] give them a [adjective2] sheen.",
				"It's the height of country [noun].",
				"Made with [noun] by [name]. 100% [adjective]!",
				"The [noun] are made of [adjective] [noun2].",

				"Worn by people who like [noun].",
				"The [noun] are easy to tie.",
				"Some [adjective] people say you should never wear these while it's raining.",
				"In ancient times, these were made out of [noun].",
				"King [name] liked to wear these while sitting on his [noun].",
				"They're [adjective] and [adjective2] to wear.",
				"They cost as much as 5 [noun]!",
				"Made in [noun].",
				"Renowned by the famous [adjective] musician, [name].",
				"Reminiscent of the color of [noun].",
				"Best when worn with [adjective] socks.",
				"Worn by [noun] everywehre.",
				"Great for walking in the [noun].",
				"The [noun] agree! These shoes are [adjective]!",

                "Crafted from enchanted [noun], these [adjective] boots guarantee a [adjective2] journey.",
				"Known for their [adjective] tread, these turn [name] into a stealthy shadow.",
				"Wielding the power of [adjective] crystals, these make even [name] an unstoppable force.",
				"Imbued with [adjective] charm, these bring a touch of [noun] to every step.",
				"Forged in dragonfire, these [adjective] boots are [name]'s signature fashion statement."
            };
			List<string> nouns = new()
			{
				"aprons",
				"backs",
				"balloons",
				"beans",
				"bears",
				"bells",
				"belts",
				"bermudas",
				"blackboards",
				"boxer shorts",
				"brains",
				"bread",
				"brinjals",
				"buckles",
				"bulbs",
				"butter",
				"cats",
				"cauliflowers",
				"chairs",
				"cheeks",
				"chests",
				"crows",
				"desks",
				"dogs",
				"drumsticks",
				"earbuds",
				"earrings",
				"ears",
				"eels",
				"eggs",
				"eyes",
				"fangs",
				"fans",
				"feet",
				"foxes",
				"freezers",
				"frocks",
				"frogs",
				"ginger",
				"goats",
				"gowns",
				"grains",
				"grapes",
				"guavas",
				"hairs",
				"hands",
				"hats",
				"iron boxes",
				"jackets",
				"jaws",
				"koel birds",
				"ladyfingers",
				"leggings",
				"legs",
				"lighters",
				"lips",
				"lungs",
				"mangoes",
				"mice",
				"milk",
				"monkeys",
				"music players",
				"necks",
				"noses",
				"oranges",
				"ostriches",
				"ovens",
				"palms",
				"pandas",
				"panthers",
				"parrots",
				"pasta",
				"peacocks",
				"pen drives",
				"pencils",
				"pigeons",
				"pocket watches",
				"pomegranates",
				"ribs",
				"rice",
				"risotto",
				"sandals",
				"sharks",
				"sheep",
				"shirts",
				"spoons",
				"stairs",
				"stoves",
				"suits",
				"swans",
				"sweaters",
				"tables",
				"tailor birds",
				"ties",
				"tigers",
				"toes",
				"tongues",
				"trousers",
				"turkeys",
				"underarms",
				"vests",
				"watermelons",
				"whales",
				"wolves",
				"wrists"
            };
			List<string> adjectives = new()
			{
                "abashed",
                "abject",
                "acid",
                "acidic",
                "alcoholic",
                "angry",
                "astonishing",
                "available",
                "blue",
                "blue-eyed",
                "bright",
                "bumpy",
                "careless",
                "caring",
                "chivalrous",
                "chunky",
                "cold",
                "common",
                "cowardly",
				"chronic",
                "cruel",
                "curious",
                "debonair",
                "deep",
                "defeated",
                "defective",
                "delicate",
                "domineering",
                "dusty",
                "earthy",
                "eatable",
                "economic",
                "enchanting",
                "encouraging",
                "evanescent",
                "evasive",
                "faint",
                "fair",
                "festive",
                "first",
				"flaky",
                "fluffy",
                "friendly",
                "general",
                "gentle",
                "gleaming",
                "healthy",
                "holistic",
                "hungry",
                "ill-informed",
                "inquisitive",
                "insidious",
                "jobless",
                "kaput",
                "lewd",
                "loving",
                "lucky",
                "marvelous",
				"moist",
                "muddled",
                "mundane",
                "nippy",
                "nostalgic",
                "old",
                "plain",
                "possessive",
                "psychotic",
                "public",
                "quickest",
                "quiet",
				"radical",
                "real",
                "rebel",
                "receptive",
                "reflective",
                "relieved",
                "romantic",
                "rude",
                "scientific",
				"shaky",
                "silent",
                "silky",
                "spooky",
                "square",
                "squealing",
                "striped",
                "succinct",
                "synonymous",
                "temporary",
                "tiresome",
                "toothsome",
                "tough",
                "unnatural",
                "useless",
                "wandering",
                "warlike",
                "watery",
				"wet",
                "whispering",
                "witty",
                "wooden",
                "woozy",
                "worried",
                "wretched",
                "wry",
                "youthful"
            };
			List<string> names = new()
			{
                "Abigail",
                "Adele",
                "Alberto",
                "Alden",
                "Alex",
                "Alison",
                "Alvin",
                "Amanda",
                "Antonio",
                "Armando",
                "Ava",
                "Barney",
                "Barry",
                "Benita",
                "Bradley",
                "Brock",
                "Cameron",
                "Cara",
                "Caroline",
                "Chester",
                "Clare",
                "Clint",
                "Colin",
                "Cornell",
                "Daphne",
                "David",
                "Deanne",
                "Demetrius",
                "Dorthy",
                "Dwarf",
                "Edwina",
                "Elliott",
                "Elvin",
                "Emily",
                "Enrique",
                "Evelyn",
                "Ezra",
                "Fannie",
                "Fran",
                "Freddy",
                "Fritz",
                "Gay",
                "George",
                "Gerardo",
                "Gus",
                "Haley",
                "Harold",
                "Harvey",
                "Helene",
                "Hollie",
                "Hong",
                "Howard",
                "Hunter",
                "Jas",
                "Jean",
                "Jerome",
                "Jerri",
                "Jesse",
                "Joanna",
                "Jodi",
                "Joesph",
                "Juana",
                "Julia",
                "Kara",
                "Karin",
                "Kent",
                "Kip",
                "Krobus",
                "Lara",
                "Lavern",
                "Leah",
                "Leif",
                "Leo",
                "Leonardo",
                "Lewis",
                "Lillian",
                "Lillie",
                "Linus",
                "Lionel",
                "Lola",
                "Loretta",
                "Lourdes",
                "Lydia",
                "Mariano",
                "Marnie",
                "Maru",
                "Maryanne",
                "Matt",
                "Mattie",
                "Maximo",
                "Michelle",
                "Miranda",
                "Moises",
                "Morris",
                "Myrna",
                "Nicholas",
                "Nicky",
                "Nigel",
                "Norma",
                "Normand",
                "Pam",
                "Patsy",
                "Penny",
                "Pete",
                "Pierre",
                "Ramiro",
                "Raymond",
                "Reed",
                "Rene",
                "Reva",
                "Rhoda",
                "Rickie",
                "Rasmodius",
                "Roberta",
                "Robin",
                "Robt",
                "Rosanna",
                "Sam",
                "Sandy",
                "Saundra",
                "Sebastian",
                "Selma",
                "Shana",
                "Shane",
                "Shelton",
                "The Wizard",
                "Vincent",
                "Walker",
                "Wayne",
                "Wiley",
                "Willy",
                "Wilmer",
                "Zelma"
            };

			return CreateDescriptionFromPieces(numberOfDescriptions, descriptionBases, nouns, adjectives, names);
		}

		private static List<string> CreateDescriptionFromPieces(
			int numberOfDescriptions, 
			List<string> descriptionBases, 
			List<string> nouns, 
			List<string> adjectives, 
			List<string> names,
            [CallerMemberName] string caller = default)
		{
            // Create an rng unique to to this class and the method that's creating the name
            RNG rng = RNG.GetFarmRNG($"{nameof(NameAndDescriptionRandomizer)}.{caller}");

			List<string> descriptionBasePool = new(descriptionBases);
			List<string> nounPool = new(nouns);
			List<string> adjectivePool = new(adjectives);
			List<string> namePool = new(names);

			List<string> createdDescriptions = new();
			for (int i = 0; i < numberOfDescriptions; i++)
			{
				string description = TryGetValueFromPool("Descrption Base", rng, descriptionBasePool, descriptionBases);
				string noun1 = TryGetValueFromPool("Noun", rng, nounPool, nouns);
				string noun2 = TryGetValueFromPool("Noun", rng, nounPool, nouns);
				string adjective1 = TryGetValueFromPool("Adjective", rng, adjectivePool, adjectives);
				string adjective2 = TryGetValueFromPool("Adjective", rng, adjectivePool, adjectives);
				string name = TryGetValueFromPool("Name", rng, namePool, names);

				description = description.Replace("[noun]", noun1);
				description = description.Replace("[noun2]", noun2);
				description = description.Replace("[adjective]", adjective1);
				description = description.Replace("[adjective2]", adjective2);
				description = description.Replace("[name]", name);
				createdDescriptions.Add(description);
			}

			return createdDescriptions;
		}

		private static List<string> CreateNameFromPieces(
			int numberOfNames, 
			List<string> adjectives, 
			List<string> prefixes, 
			List<string> suffixes,
            [CallerMemberName] string caller = default)
		{
			// Create an rng unique to to this class and the method that's creating the name
			RNG rng = RNG.GetFarmRNG($"{nameof(NameAndDescriptionRandomizer)}.{caller}");

			List<string> adjectivePool = new(adjectives);
			List<string> prefixPool = new(prefixes);
			List<string> suffixPool = new(suffixes);

			List<string> createdNames = new();
			for (int i = 0; i < numberOfNames; i++)
			{
				string prefix = TryGetValueFromPool("Prefix", rng, prefixPool, prefixes);
				string suffix = TryGetValueFromPool("Suffix", rng, suffixPool, suffixes);
				string newName = $"{prefix}{suffix}";

				// Adjust so McName works correctly
				if (newName.StartsWith("Mc"))
				{
					newName = $"Mc{newName.Substring(2, 1).ToUpper()}{newName[3..]}";
				}

				// 10% chance of an adjective being used before the name
				if (rng.NextBoolean(10) && adjectives.Count > 0)
				{
					string adjective = TryGetValueFromPool("Adjective", rng, adjectivePool, adjectives);
					newName = $"{adjective} {newName}";
				}
				createdNames.Add(newName);
			}

			return createdNames;
		}

		/// <summary>
		/// Try to get a value from the given pool
		/// Refreshes the pool and shows a warning if there are no values left to grab
		/// </summary>
		/// <param name="poolName">The name of the pool (for logging)</param>
		/// <param name="rng">The rng to use to grab values out of the pool</param>
		/// <param name="pool">The pool itself to pull from</param>
		/// <param name="defaultValues">The list of default values</param>
		/// <returns></returns>
		private static string TryGetValueFromPool(
			string poolName, 
			RNG rng,
			List<string> pool, 
			List<string> defaultValues)
		{
			if (!pool.Any())
			{
				pool.AddRange(defaultValues);
				Globals.ConsoleWarn($"Had to refresh the {poolName} pool when randomizing names and descriptions. This may be okay if mods are being used that add content.");
			}

			return rng.GetAndRemoveRandomValueFromList(pool);
		}
	}
}
