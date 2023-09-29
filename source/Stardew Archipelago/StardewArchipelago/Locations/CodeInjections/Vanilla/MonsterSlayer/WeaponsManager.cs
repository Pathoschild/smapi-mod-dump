/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Diagnostics;
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
        private const double WEAPON_LEVELS_PER_TIER = 3.5;

        public Dictionary<string, Dictionary<int, List<StardewItem>>> WeaponsByCategoryByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> WeaponsByTier { get; private set; }
        public Dictionary<int, List<StardewItem>> BootsByTier { get; private set; }

        public WeaponsManager(StardewItemManager itemManager)
        {
            InitializeWeapons(itemManager);
            InitializeBoots(itemManager);
        }

        private void InitializeWeapons(StardewItemManager itemManager)
        {
            WeaponsByCategoryByTier = new Dictionary<string, Dictionary<int, List<StardewItem>>>();
            WeaponsByTier = new Dictionary<int, List<StardewItem>>();
            var weapons = itemManager.GetAllWeapons();
            foreach (var weapon in weapons)
            {
                if (weapon.PrepareForGivingToFarmer() is not MeleeWeapon stardewWeapon)
                {
                    continue;
                }

                var weaponLevel = stardewWeapon.getItemLevel();
                var type = weapon.Type switch
                {
                    1 => TYPE_DAGGER,
                    2 => TYPE_CLUB,
                    _ => TYPE_SWORD,
                };

                var tier = (int)(weaponLevel / WEAPON_LEVELS_PER_TIER) + 1;
                Debug.Assert(tier is >= 1 and <= 5);
                AddToWeapons(weapon, type, tier);
            }
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
