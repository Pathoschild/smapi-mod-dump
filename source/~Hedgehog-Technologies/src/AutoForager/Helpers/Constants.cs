/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hedgehog-Technologies/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace AutoForager.Helpers
{
	public static class Constants
	{
		#region Custom Field Properties

		private const string _customFieldPrefix = "HedgehogTechnologies.AutoForager.";

		private const string _customFieldForageableKey = _customFieldPrefix + "Forageable";
		public static string CustomFieldForageableKey => _customFieldForageableKey;

		private const string _customFieldCategoryKey = _customFieldPrefix + "Category";
		public static string CustomFieldCategoryKey => _customFieldCategoryKey;

		private const string _customFieldBushKey = _customFieldPrefix + "Bush";
		public static string CustomFieldBushKey => _customFieldBushKey;

		private const string _customFieldBushBloomCategory = _customFieldPrefix + "BushBloomCategory";
		public static string CustomFieldBushBloomCategory => _customFieldBushBloomCategory;

		private const string _customFieldCustomBushCategory = _customFieldPrefix + "CustomBushCategory";
		public static string CustomFieldCustomBushCategory => _customFieldCustomBushCategory;

		#endregion Custom Field Properties

		#region Asset Properties

		private const string _fruitTreesAssetName = "Data/FruitTrees";
		public static string FruitTreesAssetName => _fruitTreesAssetName;

		private const string _locationsAssetName = "Data/Locations";
		public static string LocationsAssetName => _locationsAssetName;

		private const string _machinesAssetName = "Data/Machines";
		public static string MachinesAssetName => _machinesAssetName;

		private const string _objectsAssetName = "Data/Objects";
		public static string ObjectsAssetName => _objectsAssetName;

		private const string _wildTreesAssetName = "Data/WildTrees";
		public static string WildTreesAssetName => _wildTreesAssetName;

		#endregion Asset Properties

		#region Tracker Key Properties

		private const string _keyPrefix = "Key.";

		private const string _bushKey = _keyPrefix + "Bushes";
		public static string BushKey => _bushKey;

		private const string _forageableKey = _keyPrefix + "Forageables";
		public static string ForageableKey => _forageableKey;

		private const string _fruitTreeKey = _keyPrefix + "FruitTrees";
		public static string FruitTreeKey => _fruitTreeKey;

		private const string _wildTreeKey = _keyPrefix + "WildTrees";
		public static string WildTreeKey => _wildTreeKey;

		#endregion Tracker Key Properties

		#region Config Properties

		private const string _togglesSuffix = "Toggles";

		private const string _bushToggleKey = "Bush" + _togglesSuffix;
		public static string BushToggleKey => _bushToggleKey;

		private const string _foragingToggleKey = "Foraging" + _togglesSuffix;
		public static string ForagingToggleKey => _foragingToggleKey;

		private const string _fruitTreeToggleKey = "FruitTree" + _togglesSuffix;
		public static string FruitTreeToggleKey => _fruitTreeToggleKey;

		private const string _wildTreeToggleKey = "WildTree" + _togglesSuffix;
		public static string WildTreeToggleKey => _wildTreeToggleKey;

		private const string _salmonberryBushKey = "Salmonberry";
		public static string SalmonBerryBushKey => _salmonberryBushKey;

		private const string _blackberryBushKey = "Blackberry";
		public static string BlackberryBushKey => _blackberryBushKey;

		private const string _teaBushKey = "Tea";
		public static string TeaBushKey => _teaBushKey;

		private const string _walnutBushKey = "Walnut";
		public static string WalnutBushKey => _walnutBushKey;

		private const string _fieldIdPrefix = "AutoForager.";

		private const string _isForagerActiveId = _fieldIdPrefix + "IsForagerActive";
		public static string IsForagerActiveId => _isForagerActiveId;

		private const string _toggleForagerId = _fieldIdPrefix + "ToggleForager";
		public static string ToggleForagerId => _toggleForagerId;

		private const string _usePlayerMagnetismId = _fieldIdPrefix + "UsePlayerMagnetism";
		public static string UsePlayerMagnetismId => _usePlayerMagnetismId;

		private const string _shakeDistanceId = _fieldIdPrefix + "ShakeDistance";
		public static string ShakeDistanceId => _shakeDistanceId;

		private const string _requireHoeId = _fieldIdPrefix + "RequireHoe";
		public static string RequireHoeId => _requireHoeId;

		private const string _requireToolMossId = _fieldIdPrefix + "RequireToolMoss";
		public static string RequireToolMossId => _requireToolMossId;

		private const string _ignoreMushroomLogTreesId = _fieldIdPrefix + "IgnoreMushroomLogTrees";
		public static string IgnoreMushroomLogTreesId => _ignoreMushroomLogTreesId;

		private const string _bushesPageId = _fieldIdPrefix + "BushesPage";
		public static string BushesPageId => _bushesPageId;

		private const string _forageablesPageId = _fieldIdPrefix + "ForageablesPage";
		public static string ForageablesPageId => _forageablesPageId;

		private const string _fruitTreesPageId = _fieldIdPrefix + "FruitTreesPage";
		public static string FruitTreesPageId => _fruitTreesPageId;

		private const string _wildTreesPageId = _fieldIdPrefix + "WildTreesPage";
		public static string WildTreesPageId => _wildTreesPageId;

		private const string _fruitsReadyToShakeId = _fieldIdPrefix + "FruitsReadyToShake";
		public static string FruitsReadyToShakeId => _fruitsReadyToShakeId;

		private const string _shakeSalmonberriesId = _fieldIdPrefix + "ShakeSalmonberries";
		public static string ShakeSalmonberriesId => _shakeSalmonberriesId;

		private const string _shakeBlackberriesId = _fieldIdPrefix + "ShakeBlackberries";
		public static string ShakeBlackberriesId => _shakeBlackberriesId;

		private const string _shakeTeaBushesId = _fieldIdPrefix + "ShakeTeaBushes";
		public static string ShakeTeaBushesId => _shakeTeaBushesId;

		private const string _shakeWalnutBushesId = _fieldIdPrefix + "ShakeWalnutBushes";
		public static string ShakeWalnutBushesId => _shakeWalnutBushesId;

		private const int _minFruitsReady = 1;
		public static int MinFruitsReady => _minFruitsReady;

		private const int _maxFruitsReady = 3;
		public static int MaxFruitsReady => _maxFruitsReady;

		#endregion

		private static readonly Dictionary<string, string> _knownCategoryLookup = new()
		{
			{ "107", "Category.Animal" },    // Dinosaur Egg
			{ "442", "Category.Animal" },    // Duck Egg
			{ "444", "Category.Animal" },    // Duck Feather
			{ "180", "Category.Animal" },    // Egg (brown)
			{ "176", "Category.Animal" },    // Egg (white)
			{ "928", "Category.Animal" },    // Golden Egg
			{ "182", "Category.Animal" },    // Large Egg (brown)
			{ "174", "Category.Animal" },    // Large Egg (white)
			{ "289", "Category.Animal" },    // Ostrich Egg
			{ "446", "Category.Animal" },    // Rabbit's Foot
			{ "430", "Category.Animal" },    // Truffle
			{ "305", "Category.Animal" },    // Void Egg
			{ "440", "Category.Animal" },    // Wool

			{ "18", "Category.Spring" },     // Daffodil
			{ "22", "Category.Spring" },     // Dandelion
			{ "20", "Category.Spring" },     // Leek
			{ "16", "Category.Spring" },     // Wild Horseradish

			{ "259", "Category.Summer" },    // Fiddlehead Fern
			{ "398", "Category.Summer" },    // Grapes
			{ "396", "Category.Summer" },    // Spice Berry
			{ "402", "Category.Summer" },    // Sweet Pea

			{ "410", "Category.Fall" },      // Blackberry
			{ "408", "Category.Fall" },      // Hazelnut
			{ "406", "Category.Fall" },      // Wild Plum

			{ "418", "Category.Winter" },    // Crocus
			{ "414", "Category.Winter" },    // Crystal Fruit
			{ "283", "Category.Winter" },    // Holly
			{ "416", "Category.Winter" },    // Snow Yam 
			{ "412", "Category.Winter" },    // Winter Root 

			{ "281", "Category.Mushrooms" }, // Chanterelle
			{ "404", "Category.Mushrooms" }, // Common Mushrooms
			{ "851", "Category.Mushrooms" }, // Magma Cap
			{ "257", "Category.Mushrooms" }, // Morel
			{ "422", "Category.Mushrooms" }, // Purple Mushroom
			{ "420", "Category.Mushrooms" }, // Red Mushroom

			{ "372", "Category.Beach" },     // Clam
			{ "718", "Category.Beach" },     // Cockle
			{ "393", "Category.Beach" },     // Coral
			{ "719", "Category.Beach" },     // Mussel
			{ "392", "Category.Beach" },     // Nautilus Shell
			{ "723", "Category.Beach" },     // Oyster
			{ "394", "Category.Beach" },     // Rainbow Shell
			{ "397", "Category.Beach" },     // Sea Urchin
			{ "152", "Category.Beach" },     // Seaweed

			{ "90", "Category.Desert" },     // Cactus Fruit
			{ "88", "Category.Desert" },     // Coconut

			{ "613",  "Category.Special" },  // Apple
			{ "634",  "Category.Special" },  // Apricot
			{ "638",  "Category.Special" },  // Cherry
			{ "829",  "Category.Special" },  // Ginger
			{ "Moss", "Category.Special" },  // Moss
			{ "635",  "Category.Special" },  // Orange
			{ "636",  "Category.Special" },  // Peach
			{ "637",  "Category.Special" },  // Pomegranate
			{ "296",  "Category.Special" },  // Salmonberry
			{ "399",  "Category.Special" },  // Sping Onion
		};
		public static Dictionary<string, string> KnownCategoryLookup => _knownCategoryLookup;

		private static readonly List<string> _vanillaFruitTrees = new()
		{
			"628", // Cherry
			"629", // Apricot
			"630", // Orange
			"631", // Peach
			"632", // Pomegranate
			"633", // Apple
			"69",  // Banana
			"835", // Mango
		};
		public static List<string> VanillaFruitTrees => _vanillaFruitTrees;

		private static readonly List<string> _vanillaWildTrees = new()
		{
			"1",  // Acorn
			"2",  // Maple
			"3",  // Pine
			"7",  // Mushroom
			"8",  // Mahogany
			"6",  // Coconut
			"9",  // Coconut
			"10", // Mossy
			"11", // Mossy
			"12", // Mossy
			"13"  // Mystic
		};
		public static List<string> VanillaWildTrees => _vanillaWildTrees;

		private static readonly List<string> _vanillaBushBlooms = new()
		{
			"296", // Salmonberry
			"410"  // Blackberry
		};
		public static List<string> VanillaBushBlooms => _vanillaBushBlooms;

		private static readonly Dictionary<string, int> _bigCraftableXpLookup = new()
		{
			{ "(BC)128", 5 },
			{ "(BC)MushroomLog", 5 }
		};
		public static Dictionary<string, int> BigCraftableXpLookup => _bigCraftableXpLookup;
	}
}
