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
using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants;
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Stardew;
using StardewValley;
using StardewValley.Internal;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer
{
    public class WeaponsManager
    {
        public const string TYPE_WEAPON = "Weapon";
        public const string TYPE_SWORD = "Sword";
        public const string TYPE_CLUB = "Club";
        public const string TYPE_DAGGER = "Dagger";
        private readonly ArchipelagoClient _archipelago;
        private ModsManager _modsManager;

        private Dictionary<int, List<int>> _weaponWeightsByNumberOfTiers = new()
        {
            { 5, new List<int> { 4, 3, 2, 2, 2 } },
            { 6, new List<int> { 4, 3, 2, 2, 2, 2 } },
        };

        public Dictionary<string, Dictionary<int, List<StardewItem>>> WeaponsByCategoryByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> BootsByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> SlingshotsByTier { get; private set; }
        public List<StardewRing> Rings { get; private set; }

        public WeaponsManager(ArchipelagoClient archipelago, StardewItemManager itemManager, ModsManager modsManager)
        {
            _archipelago = archipelago;
            _modsManager = modsManager;
            InitializeWeapons(itemManager);
            InitializeBoots(itemManager);
            InitializeSlingshots(itemManager);
            InitializeRings(itemManager);
        }

        public IEnumerable<ItemQueryResult> GetEquipmentsForSale(string arguments)
        {
            var priceMultiplier = GetPriceMultiplier(arguments, out var isRecovery);
            var itemsToSell = new List<ItemQueryResult>();
            var random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);
            foreach (var (category, weaponsByTier) in WeaponsByCategoryByTier)
            {
                itemsToSell.AddRange(GetWeaponsToSell($"Progressive {category}", weaponsByTier, isRecovery, priceMultiplier, random));
            }

            itemsToSell.AddRange(GetBootsToSell(isRecovery, priceMultiplier, random));
            itemsToSell.AddRange(GetSlingshotsToSell(isRecovery, priceMultiplier, random));
            if (isRecovery)
            {
                itemsToSell.AddRange(GetRingsToSell());
            }

            return itemsToSell;
        }

        private static double GetPriceMultiplier(string arguments, out bool isRecovery)
        {
            var priceMultiplier = 1.0;
            isRecovery = true;
            if (arguments == IDProvider.ARCHIPELAGO_EQUIPMENTS_SALE)
            {
                priceMultiplier = 2.0;
                isRecovery = false;
            }

            if (arguments == IDProvider.ARCHIPELAGO_EQUIPMENTS_RECOVERY)
            {
                priceMultiplier = 4.0;
                isRecovery = true;
            }

            return priceMultiplier;
        }

        private IEnumerable<ItemQueryResult> GetWeaponsToSell(string archipelagoItemName, Dictionary<int, List<StardewItem>> weaponsByTier, bool isRecovery, double priceMultiplier, Random random)
        {
            var receivedTier = _archipelago.GetReceivedItemCount(archipelagoItemName);
            for (var i = 1; i <= weaponsByTier.Keys.Max(); i++)
            {
                var weaponsInTier = weaponsByTier.ContainsKey(i) ? weaponsByTier[i] : new List<StardewItem>();
                var randomWeaponIndex = random.Next(weaponsInTier.Count);
                for (var index = 0; index < weaponsInTier.Count; index++)
                {
                    var weaponToSell = weaponsInTier[index];

                    var isWeaponAllowed = i <= receivedTier || _archipelago.HasReceivedItem(weaponToSell.Name);
                    if (!isWeaponAllowed)
                    {
                        continue;
                    }

                    if (isRecovery && (i != receivedTier || index != randomWeaponIndex))
                    {
                        continue;
                    }

                    var shopItem = weaponToSell.PrepareForGivingToFarmer();
                    if (isRecovery)
                    {
                        shopItem.isLostItem = true;
                    }
                    yield return new ItemQueryResult(shopItem)
                    {
                        OverrideBasePrice = (int)Math.Round(i * 500 * priceMultiplier),
                        OverrideStackSize = 1,
                        OverrideShopAvailableStock = 1,
                    };
                }
            }
        }

        private IEnumerable<ItemQueryResult> GetBootsToSell(bool isRecovery, double priceMultiplier, Random random)
        {
            var receivedTier = _archipelago.GetReceivedItemCount("Progressive Boots");
            for (var i = 1; i <= BootsByTier.Keys.Max(); i++)
            {
                var bootsInTier = BootsByTier.ContainsKey(i) ? BootsByTier[i] : new List<StardewItem>();
                var randomBootsIndex = random.Next(bootsInTier.Count);
                for (var index = 0; index < bootsInTier.Count; index++)
                {
                    var bootsToSell = bootsInTier[index];

                    var isBootsAllowed = i <= receivedTier || _archipelago.HasReceivedItem(bootsToSell.Name);
                    if (!isBootsAllowed)
                    {
                        continue;
                    }

                    if (isRecovery && (i != receivedTier || index != randomBootsIndex))
                    {
                        continue;
                    }

                    var shopItem = bootsToSell.PrepareForGivingToFarmer();
                    if (isRecovery)
                    {
                        shopItem.isLostItem = true;
                    }
                    yield return new ItemQueryResult(shopItem)
                    {
                        OverrideBasePrice = (int)Math.Round(bootsToSell.SellPrice * priceMultiplier),
                        OverrideStackSize = 1,
                        OverrideShopAvailableStock = 1,
                    };
                }
            }
        }

        private IEnumerable<ItemQueryResult> GetSlingshotsToSell(bool isRecovery, double priceMultiplier, Random random)
        {
            var receivedTier = _archipelago.GetReceivedItemCount("Progressive Slingshot");
            for (var i = 1; i <= SlingshotsByTier.Keys.Max(); i++)
            {
                var slingShotsInTier = SlingshotsByTier.ContainsKey(i) ? SlingshotsByTier[i] : new List<StardewItem>();
                var randomSlingshotIndex = random.Next(slingShotsInTier.Count);
                for (var index = 0; index < slingShotsInTier.Count; index++)
                {
                    var slingshotToSell = slingShotsInTier[index];

                    var isSlingshotAllowed = i <= receivedTier || _archipelago.HasReceivedItem(slingshotToSell.Name);
                    if (!isSlingshotAllowed)
                    {
                        continue;
                    }

                    if (isRecovery && (i != receivedTier || index != randomSlingshotIndex))
                    {
                        continue;
                    }

                    var shopItem = slingshotToSell.PrepareForGivingToFarmer();
                    if (isRecovery)
                    {
                        shopItem.isLostItem = true;
                    }
                    yield return new ItemQueryResult(shopItem)
                    {
                        OverrideBasePrice = (int)Math.Round(((StardewSlingshot)slingshotToSell).GetDefaultSalePrice() * priceMultiplier),
                        OverrideStackSize = 1,
                        OverrideShopAvailableStock = 1,
                    };
                }
            }
        }

        private IEnumerable<ItemQueryResult> GetRingsToSell()
        {
            foreach (var stardewRing in Rings)
            {
                if (!_archipelago.HasReceivedItem(stardewRing.Name))
                {
                    continue;
                }


                var shopItem = stardewRing.PrepareForGivingToFarmer();
                yield return new ItemQueryResult(shopItem)
                {
                    OverrideBasePrice = stardewRing.SellPrice,
                    OverrideStackSize = 1,
                    OverrideShopAvailableStock = 1,
                };
            }
        }

        private void InitializeSlingshots(StardewItemManager itemManager)
        {
            SlingshotsByTier = new Dictionary<int, List<StardewItem>>()
            {
                { 1, new List<StardewItem> { itemManager.GetWeaponByName("Slingshot") } },
                { 2, new List<StardewItem> { itemManager.GetWeaponByName("Master Slingshot") } },
            };
        }

        private void InitializeWeapons(StardewItemManager itemManager)
        {
            WeaponsByCategoryByTier = new Dictionary<string, Dictionary<int, List<StardewItem>>>();
            var numberOfTiers = GetExpectedProgressiveWeapons();
            var weightList = _weaponWeightsByNumberOfTiers[numberOfTiers];
            var weightTotal = weightList.Sum();
            var weapons = itemManager.GetAllWeapons();
            var weaponLevelsByCategory = GetWeaponLevelsByCategory(weapons);

            foreach (var (category, weaponLevels) in weaponLevelsByCategory)
            {
                var weaponsSortedByStrength = GetWeaponsSortedByStrength(weaponLevels);
                var weaponsCountInEachTier = GetWeaponsCountInEachTier(weightList, weaponsSortedByStrength.Length, weightTotal);
                AddRoundingErrorIntoThirdEntry(weaponsCountInEachTier, weaponsSortedByStrength.Length);
                PlaceWeaponsInCorrectTiers(numberOfTiers, weaponsCountInEachTier, weaponsSortedByStrength, category);
            }
        }

        private static StardewWeapon[] GetWeaponsSortedByStrength(Dictionary<StardewWeapon, int> weaponLevels)
        {
            var weaponsSortedByStrength = weaponLevels.OrderBy(kvp => kvp.Value).Select(x => x.Key).ToArray();
            return weaponsSortedByStrength;
        }

        private static List<int> GetWeaponsCountInEachTier(List<int> weightList, int numberOfWeapons, int weightTotal)
        {
            var weaponsCountInEachTier = new List<int> { };
            foreach (var weight in weightList)
            {
                weaponsCountInEachTier.Add((int)Math.Round(((double)weight * numberOfWeapons) / weightTotal));
            }

            return weaponsCountInEachTier;
        }

        private static void AddRoundingErrorIntoThirdEntry(List<int> weaponsCountInEachTier, int numberOfWeapons)
        {
            var sum = weaponsCountInEachTier.Sum();
            if (sum < numberOfWeapons)
            {
                weaponsCountInEachTier[2] += numberOfWeapons - sum;
            }
            else if (sum > numberOfWeapons)
            {
                weaponsCountInEachTier[2] -= sum - numberOfWeapons;
            }
        }

        private void PlaceWeaponsInCorrectTiers(int numberOfTiers, List<int> weaponsCountInEachTier, StardewWeapon[] weaponsSortedByStrength, string category)
        {
            var placedWeaponsCount = 0;
            for (var tier = 0; tier < numberOfTiers; tier++)
            {
                var countInThisTier = weaponsCountInEachTier[tier];
                PlaceWeaponsInTier(weaponsSortedByStrength, category, countInThisTier, placedWeaponsCount, tier + 1);

                placedWeaponsCount += countInThisTier;
            }
        }

        private void PlaceWeaponsInTier(StardewWeapon[] weaponsSortedByStrength, string category, int countInThisTier, int previousTiersCount, int tier)
        {
            for (var i = 0; i < countInThisTier; i++)
            {
                var currentWeapon = weaponsSortedByStrength[previousTiersCount + i];
                AddToWeapons(currentWeapon, category, tier);
            }
        }

        private static Dictionary<string, Dictionary<StardewWeapon, int>> GetWeaponLevelsByCategory(StardewWeapon[] weapons)
        {
            var weaponLevelsByCategory = new Dictionary<string, Dictionary<StardewWeapon, int>>();
            foreach (var weapon in weapons)
            {
                if (weapon.PrepareForGivingToFarmer() is not MeleeWeapon stardewWeapon || stardewWeapon.Name.Contains("Scythe"))
                {
                    continue;
                }

                var weaponLevel = stardewWeapon.getItemLevel();
                if (stardewWeapon.speed.Value > 100)
                {
                    weaponLevel = weaponLevel / 2;
                }
                var type = weapon.Type switch
                {
                    1 => TYPE_DAGGER,
                    2 => TYPE_CLUB,
                    _ => TYPE_SWORD,
                };

                if (!weaponLevelsByCategory.ContainsKey(type))
                {
                    weaponLevelsByCategory.Add(type, new Dictionary<StardewWeapon, int>());
                }

                weaponLevelsByCategory[type][weapon] = weaponLevel;
            }

            return weaponLevelsByCategory;
        }

        private int GetExpectedProgressiveWeapons()
        {
            return _modsManager.HasMod(ModNames.SVE) ? 6 : 5;
        }

        private void AddToWeapons(StardewWeapon weapon, string category, int tier)
        {
            if (!WeaponsByCategoryByTier.ContainsKey(category))
            {
                WeaponsByCategoryByTier.Add(category, new Dictionary<int, List<StardewItem>>());
            }

            if (!WeaponsByCategoryByTier[category].ContainsKey(tier))
            {
                WeaponsByCategoryByTier[category].Add(tier, new List<StardewItem>());
            }

            WeaponsByCategoryByTier[category][tier].Add(weapon);
            if (category == TYPE_WEAPON)
            {
                return;
            }

            AddToWeapons(weapon, TYPE_WEAPON, tier);
        }

        private void InitializeBoots(StardewItemManager itemManager)
        {
            BootsByTier = new Dictionary<int, List<StardewItem>>();
            var boots = itemManager.GetAllBoots();
            foreach (var boot in boots)
            {
                var stardewBoots = (Boots)boot.PrepareForGivingToFarmer();
                var bootsLevel = stardewBoots.defenseBonus.Value + stardewBoots.immunityBonus.Value;
                var tier = bootsLevel switch
                {
                    <= 2 => 1,
                    <= 5 => 2,
                    <= 8 => 3,
                    _ => 4,
                };

                AddToBoots(boot, tier);
            }

            BootsByTier.Add(5, new List<StardewItem>());
        }

        private void AddToBoots(StardewBoots boot, int tier)
        {
            if (!BootsByTier.ContainsKey(tier))
            {
                BootsByTier.Add(tier, new List<StardewItem>());
            }

            BootsByTier[tier].Add(boot);
        }

        private void InitializeRings(StardewItemManager itemManager)
        {
            Rings = new List<StardewRing>(itemManager.GetAllRings());
        }
    }
}
