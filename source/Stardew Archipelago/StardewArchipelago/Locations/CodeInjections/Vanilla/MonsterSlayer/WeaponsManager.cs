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
using StardewArchipelago.Constants.Modded;
using StardewArchipelago.Stardew;
using StardewValley.Objects;
using StardewValley.Tools;

namespace StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer
{
    public class WeaponsManager
    {
        public const string TYPE_SWORD = "Sword";
        public const string TYPE_CLUB = "Club";
        public const string TYPE_DAGGER = "Dagger";
        private ModsManager _modsManager;

        private Dictionary<int, List<int>> _weaponWeightsByNumberOfTiers = new()
        {
            { 5, new List<int> { 4, 3, 2, 2, 2 } },
            { 6, new List<int> { 4, 3, 2, 2, 2, 2 } }
        };

        public Dictionary<string, Dictionary<int, List<StardewItem>>> WeaponsByCategoryByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> WeaponsByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> BootsByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> SlingshotsByTier { get; private set; }

        public WeaponsManager(StardewItemManager itemManager, ModsManager modsManager)
        {
            _modsManager = modsManager;
            InitializeWeapons(itemManager);
            InitializeBoots(itemManager);
            InitializeSlingshots(itemManager);
        }

        private void InitializeSlingshots(StardewItemManager itemManager)
        {
            SlingshotsByTier = new Dictionary<int, List<StardewItem>>()
            {
                { 1, new List<StardewItem> { itemManager.GetWeaponByName("Slingshot") } },
                { 2, new List<StardewItem> { itemManager.GetWeaponByName("Master Slingshot") } }
            };
        }

        private void InitializeWeapons(StardewItemManager itemManager)
        {
            WeaponsByCategoryByTier = new Dictionary<string, Dictionary<int, List<StardewItem>>>();
            WeaponsByTier = new Dictionary<int, List<StardewItem>>();
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
                PlaceWeaponsInTier(weaponsSortedByStrength, category, countInThisTier, placedWeaponsCount, tier+1);

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
            if (!WeaponsByTier.ContainsKey(tier))
            {
                WeaponsByTier.Add(tier, new List<StardewItem>());
            }

            if (!WeaponsByCategoryByTier.ContainsKey(category))
            {
                WeaponsByCategoryByTier.Add(category, new Dictionary<int, List<StardewItem>>());
            }

            if (!WeaponsByCategoryByTier[category].ContainsKey(tier))
            {
                WeaponsByCategoryByTier[category].Add(tier, new List<StardewItem>());
            }

            WeaponsByTier[tier].Add(weapon);
            WeaponsByCategoryByTier[category][tier].Add(weapon);
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
    }
}
