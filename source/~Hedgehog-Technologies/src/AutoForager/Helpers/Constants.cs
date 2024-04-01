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
            { "18", "Spring" },     // Daffodil
            { "22", "Spring" },     // Dandelion
            { "20", "Spring" },     // Leek
            { "16", "Spring" },     // Wild Horseradish

            { "259", "Summer" },    // Fiddlehead Fern
            { "398", "Summer" },    // Grapes
            { "396", "Summer" },    // Spice Berry
            { "402", "Summer" },    // Sweet Pea

            { "410", "Fall" },      // Blackberry
            { "408", "Fall" },      // Hazelnut
            { "406", "Fall" },      // Wild Plum

            { "418", "Winter" },    // Crocus
            { "414", "Winter" },    // Crystal Fruit
            { "283", "Winter" },    // Holly
            { "416", "Winter" },    // Snow Yam
            { "412", "Winter" },    // Winter Root

            { "281", "Mushrooms" }, // Chanterelle
            { "404", "Mushrooms" }, // Common Mushrooms
            { "851", "Mushrooms" }, // Magma Cap
            { "257", "Mushrooms" }, // Morel
            { "422", "Mushrooms" }, // Purple Mushroom
            { "420", "Mushrooms" }, // Red Mushroom

            { "372", "Beach" },     // Clam
            { "718", "Beach" },     // Cockle
            { "393", "Beach" },     // Coral
            { "719", "Beach" },     // Mussel
            { "392", "Beach" },     // Nautilus Shell
            { "723", "Beach" },     // Oyster
            { "394", "Beach" },     // Rainbow Shell
            { "397", "Beach" },     // Sea Urchin
            { "152", "Beach" },     // Seaweed

            { "90", "Desert" },     // Cactus Fruit
            { "88", "Desert" },     // Coconut

            { "829", "Special" },   // Ginger
            { "Moss", "Special" },  // Moss
            { "399", "Special" },   // Sping Onion
            { "430", "Special" },   // Truffle
        };
        public static Dictionary<string, string> KnownCategoryLookup => _knownCategoryLookup;

        private static readonly List<string> _sveForageables = new()
        {
            "(O)FlashShifter.StardewValleyExpandedCP_Bearberrys",
            "(O)FlashShifter.StardewValleyExpandedCP_Big_Conch",
            "(O)FlashShifter.StardewValleyExpandedCP_Dried_Sand_Dollar",
            "(O)FlashShifter.StardewValleyExpandedCP_Ferngill_Primrose",
            "(O)FlashShifter.StardewValleyExpandedCP_Goldenrod",
            "(O)FlashShifter.StardewValleyExpandedCP_Lucky_Four_Leaf_Clover",
            "(O)FlashShifter.StardewValleyExpandedCP_Mushroom_Colony",
            "(O)FlashShifter.StardewValleyExpandedCP_Poison_Mushroom",
            "(O)FlashShifter.StardewValleyExpandedCP_Red_Baneberry",
            "(O)FlashShifter.StardewValleyExpandedCP_Smelly_Rafflesia",
            "(O)FlashShifter.StardewValleyExpandedCP_Thistle",
            "(O)FlashShifter.StardewValleyExpandedCP_Winter_Star_Rose"
        };
        public static List<string> SVEForageables => _sveForageables;

        private static readonly Dictionary<string, string> _knownModPrefixes = new()
        {
            { "Cornucopia", "Cornucopia" },
            { "FlashShifter.StardewValleyExpandedCP", "Stardew Valley Expanded" },
            { "FlashShifter.SVE-FTM", "Stardew Valley Expanded" },
            { "FlashShifter.SVECode", "Stardew Valley Expanded" },
            { "Lumisteria.MtVapius", "Lumisteria - Mt. Vapius" },
            { "Lumisteria.SereneMeadow", "Lumisteria - Serene Meadow" }
        };
        public static Dictionary<string, string> KnownModPrefixes => _knownModPrefixes;
    }
}
