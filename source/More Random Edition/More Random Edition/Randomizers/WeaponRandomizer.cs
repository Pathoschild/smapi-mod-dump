/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
    /// <summary>
    /// Modifies weapons
    /// </summary>
    public class WeaponRandomizer
	{
		public readonly static Dictionary<string, WeaponData> Weapons = new();
		private static RNG Rng { get; set; }

		/// <summary>
		/// Returns the object use to modify the weapons
		/// </summary>
		/// <returns />
		public static Dictionary<string, WeaponData> Randomize()
		{
			Weapons.Clear();
			Rng = RNG.GetFarmRNG(nameof(WeaponRandomizer));
			WeaponAndArmorNameRandomizer nameRandomizer = new(nameof(WeaponRandomizer));

			// Exclude slingshots and scythes for now
			Dictionary<string, WeaponData> weaponReplacements = Game1.weaponData
				.Where(keyValuePair => {
					var weaponName = keyValuePair.Value.Name;
					return !weaponName.Contains("Slingshot") && !weaponName.Contains("Scythe");
                })
				.ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var weaponData in weaponReplacements)
			{
				// If this fails to be an int, it is a modded weapon, so just skip it
				if (int.TryParse(weaponData.Key, out int weaponIndex))
				{
					// In this case, we're checking the setting in RandomizeWeapon instead of existing early
					// since WeaponImageBuilder relies on Weapons to be populated
					RandomizeWeapon(weaponData.Value, (WeaponIndexes)weaponIndex, nameRandomizer);
					Weapons.Add(weaponData.Key, weaponData.Value);
				}
			}

			WriteToSpoilerLog(weaponReplacements);
			return weaponReplacements;
		}

		/// <summary>
		/// Randomizes the values on the given weapon
		/// </summary>
		/// <param name="weapon">The weapon to randomize</param>
		/// <param name="nameRandomizer">The name randomizer</param>
		private static void RandomizeWeapon(
			WeaponData weapon,
            WeaponIndexes weaponIndex, 
			WeaponAndArmorNameRandomizer nameRandomizer)
		{
			if (!Globals.Config.Weapons.Randomize)
			{
				return;
			}

            if (weapon.Type == (int)WeaponType.Slingshot)
			{
				//TODO: assign the name here after we deal with the slingshot name hardcoding issue
				// Doing this to advance the RNG so we don't affect seeds when we do actually
				// assign one for the slingshot - don't actually assign the name yet, though
				nameRandomizer.GenerateRandomWeaponName((WeaponType)weapon.Type, weaponIndex);
				return;
			}

			RandomizeWeaponType(weapon);
			RandomizeWeaponDamage(weapon);
			RandomizeWeaponCrits(weapon);
			RandomizeWeaponKnockback(weapon);
			RandomizeWeaponSpeed(weapon);
			RandomizeWeaponAOE(weapon);
			RandomizeWeaponPrecision(weapon);
			RandomizeWeaponDefense(weapon);
			RandomizeWeaponDropInfo(weapon, weaponIndex);
			SetWeaponDescription(weapon, weaponIndex);

			string weaponName = nameRandomizer.GenerateRandomWeaponName((WeaponType)weapon.Type);
			weapon.DisplayName = weaponName;
		}

		/// <summary>
		/// Randomizes the weapon type
		/// - 1/4 chance of each type that isn't a slingshot
		/// </summary>
		/// <param name="weapon">The weapon to randomize</param>
		private static void RandomizeWeaponType(WeaponData weapon)
		{
			weapon.Type = Rng.NextIntWithinRange(0, 3);
			if ((WeaponType)weapon.Type == WeaponType.StabbingSword)
			{
				weapon.Type = (int)WeaponType.SlashingSword;
			}
		}

		/// <summary>
		/// Randomizes weapon damage based on the original max damage
		/// - if the max damage is under 10 - has a 50% variance
		/// - if the max damage is under 50 - has a 30% variance
		/// - if the max damage is over 50 - has a 20% variance
		/// </summary>
		/// <param name="weapon">The weapon to randomize</param>
		private static void RandomizeWeaponDamage(WeaponData weapon)
		{
			const int percentageUnder10 = 50;
			const int percentageUnder50 = 30;
			const int percentageOver50 = 20;

			int originalMinDamage = weapon.MinDamage;
			int originalMaxDamage = weapon.MaxDamage;
			int variancePercentage;

			if (originalMaxDamage < 10) { variancePercentage = percentageUnder10; }
			else if (originalMaxDamage < 50) { variancePercentage = percentageUnder50; }
			else { variancePercentage = percentageOver50; }

			int minValueToUse = Rng.NextIntWithinPercentage(originalMinDamage, variancePercentage);
			int maxValueToUse = Rng.NextIntWithinPercentage(originalMaxDamage, variancePercentage);

			// Use a range to ensure that min is less than max
			var weaponDamage = new Range(minValueToUse, maxValueToUse);
			weapon.MinDamage = weaponDamage.MinValue;
			weapon.MaxDamage = weaponDamage.MaxValue;
        }

		/// <summary>
		/// Randomize the weapon crit stats
		/// - 1% chance of a 0.1% crit with a multiplier of 100
		/// - 4% chance of 8-12% crit with a multiplier of 3 - 3.1x
		/// - Else, 2-3% crit with a multiplier of 3 - 4x
		/// </summary>
		/// <param name="weapon">The weapon to randomize</param>
		private static void RandomizeWeaponCrits(WeaponData weapon)
		{
			if (Rng.NextBoolean(1))
			{
				weapon.CritChance = 0.001f;
				weapon.CritMultiplier = 100;
			}

			else if (Rng.NextBoolean(4))
			{
				weapon.CritChance = Rng.NextIntWithinRange(8, 12) / 100f;
				weapon.CritMultiplier = Rng.NextIntWithinRange(30, 31) / 10f;
			}

			else
			{
				weapon.CritChance = Rng.NextIntWithinRange(20, 30) / 1000f;
				weapon.CritMultiplier = Rng.NextIntWithinRange(30, 40) / 10f;
			}
		}

		/// <summary>
		/// Assigns a random weapon knockback
		/// - 5% chance of 1.6 - 2.0
		/// - else 0.5 - 1.6
		/// </summary>
		/// <param name="weapon">The weapon to set the knockback for</param>
		private static void RandomizeWeaponKnockback(WeaponData weapon)
		{
			weapon.Knockback = Rng.NextBoolean(5)
				? Rng.NextIntWithinRange(16, 20) / 10f
				: Rng.NextIntWithinRange(5, 16) / 10f;
		}

		/// <summary>
		/// Assigns the weapon's speed
		/// - 5% chance of max speed
		/// - 10% chance of slow speed (-16 to -1)
		/// - 50% chance of 0
		/// - Else, value from -8 to 8
		/// </summary>
		/// <param name="weapon">The weapon to set the speed for</param>
		private static void RandomizeWeaponSpeed(WeaponData weapon)
		{
			if (Rng.NextBoolean(5))
			{
				weapon.Speed = 308;
			}

			else if (Rng.NextBoolean(10))
			{
				weapon.Speed = Rng.NextIntWithinRange(-16, -1);
			}

			else if (Rng.NextBoolean())
			{
				weapon.Speed = 0;
			}

			else
			{
				weapon.Speed = Rng.NextIntWithinRange(-8, 8);
			}
		}

		/// <summary>
		/// Assigns a random AOE value to the weapon
		/// - 80% chance of 0
		/// - Else, value from 1 - 4
		/// </summary>
		/// <param name="weapon">The weapon to assign the AOE to</param>
		private static void RandomizeWeaponAOE(WeaponData weapon)
		{
			weapon.AreaOfEffect = Rng.NextBoolean(80)
				? 0
				: Rng.NextIntWithinRange(1, 4);
		}

		/// <summary>
		/// Assigns a random precision value to the weapon
		/// - 80% chance of 0
		/// - else 1 - 10
		/// </summary>
		/// <param name="weapon">The weapon to assign the precision value</param>
		private static void RandomizeWeaponPrecision(WeaponData weapon)
		{
			weapon.Precision = Rng.NextBoolean(80)
				? 0
				: Rng.NextIntWithinRange(1, 10);
		}

		/// <summary>
		/// Assigns a random defense value to the weapon based on its base defense
		/// This will be a value of + or - 50% of the defense, rounded up
		/// 
		/// If no defense, then a 5% chance of getting a value from 1-5
		/// </summary>
		/// <param name="weapon">The weapon to add the defense value</param>
		private static void RandomizeWeaponDefense(WeaponData weapon)
		{
			if (weapon.Defense > 0)
			{
                weapon.Defense = Rng.NextIntWithinPercentage(weapon.Defense, 50);
            }
			else if (Rng.NextBoolean(5))
			{
				weapon.Defense = Rng.NextIntWithinRange(1, 5);
            }
		}

        /// <summary>
        /// Randomizes the weapon drop info (where you receive weapons in mine containers).
        /// This does not affect galaxy items.
        /// - If you currently can't receive the weapon in the mines, set its base floor based on its max damage
        ///   - less than 10: floor between 1 and 20
        ///   - less than 30: floor between 21 and 60
        ///   - less than 50: floor between 61 and 100
        ///   - else: floor between 110 and 110
        /// - else, set the base floor to be + or - 10 floors of the original value
        /// 
        /// In either case, set the min floor to be between 10 and 30 floors lower than the base
        /// </summary>
        /// <param name="weapon">The weapon to set drop info for</param>
        /// <param name="weaponIndex">The weapon's index</param>
        private static void RandomizeWeaponDropInfo(WeaponData weapon, WeaponIndexes weaponIndex)
		{
			if (!ShouldRandomizeDropInfo(weaponIndex)) { return; }

			int baseMineLevel = weapon.MineBaseLevel;
			if (baseMineLevel == -1)
			{
				int maxDamage = weapon.MaxDamage;
				if (maxDamage < 10) { baseMineLevel = Rng.NextIntWithinRange(1, 20); }
				else if (maxDamage < 30) { baseMineLevel = Rng.NextIntWithinRange(21, 60); }
				else if (maxDamage < 50) { baseMineLevel = Rng.NextIntWithinRange(61, 100); }
				else { baseMineLevel = Rng.NextIntWithinRange(101, 110); }
			}

			else
			{
				baseMineLevel = FixMineLevelValue(baseMineLevel + Rng.NextIntWithinRange(-10, 10));
			}

			weapon.MineBaseLevel = baseMineLevel;
			weapon.MineMinLevel = FixMineLevelValue(baseMineLevel - Rng.NextIntWithinRange(10, 30), true);
		}

        /// <summary>
        /// Whether to randomize drop info - currently based on whether it is an endgame weapon
        /// </summary>
        /// <returns />
        private static bool ShouldRandomizeDropInfo(WeaponIndexes weaponIndex)
        {
            return weaponIndex != WeaponIndexes.GalaxyDagger &&
                weaponIndex != WeaponIndexes.GalaxyHammer &&
                weaponIndex != WeaponIndexes.GalaxySlingshot &&
                weaponIndex != WeaponIndexes.GalaxySword &&
                weaponIndex < WeaponIndexes.DwarfSword;
        }

        /// <summary>
        /// Ensures the mine level is a value from 1 to 110
        /// </summary>
        /// <param name="mineLevel">The mine level</param>
        /// <param name="allowMinusOne">Whether to set values less than 1 to -1</param>
        /// <returns>If less than 1, 1 or -1; if greater than 110, 110; else, the value given</returns>
        private static int FixMineLevelValue(int mineLevel, bool allowMinusOne = false)
		{
			if (mineLevel < 1)
			{
				return allowMinusOne ? -1 : 1;
			}
			else if (mineLevel > 110) { return 110; }
			else { return mineLevel; }
		}

        /// <summary>
        /// Sets a weapon's description based on its attributes
        /// </summary>
        /// <param name="weapon">The weapon to set the description for</param>
        /// <param name="weaponIndex">The weapon index - used to track the dark sword</param>
        private static void SetWeaponDescription(WeaponData weapon, WeaponIndexes weaponIndex)
		{
			string description = "";
			switch ((WeaponType)weapon.Type)
			{
				case WeaponType.Dagger:
				case WeaponType.StabbingSword:
					description = Globals.GetTranslation("weapon-description-stabbing");
					break;
				case WeaponType.SlashingSword:
					description = Globals.GetTranslation("weapon-description-slashing");
					break;
				case WeaponType.ClubOrHammer:
					description = Globals.GetTranslation("weapon-description-crushing");
					break;
				default:
					Globals.ConsoleError($"Assigning description to an invalid weapon type: {weapon}");
					break;
			}

			if (weapon.CritMultiplier == 100)
			{
				description += $" {Globals.GetTranslation("weapon-description-crit-high-damage")}";
			}

			else if (weapon.CritChance >= 8)
			{
				description += $" {Globals.GetTranslation("weapon-description-crit-high-frequency")}";
			}

			if (weapon.Knockback >= 15)
			{
				description += $" {Globals.GetTranslation("weapon-description-high-knockback")}";
			}

			if (weapon.Speed > 100)
			{
				description += $" {Globals.GetTranslation("weapon-description-fast")}";
			}

			if (weapon.AreaOfEffect > 0)
			{
				description += $" {Globals.GetTranslation("weapon-description-aoe")}";
			}

			if (weapon.Precision > 4)
			{
				description += $" {Globals.GetTranslation("weapon-description-accurate")}";
			}

			if (weapon.Defense > 0)
			{
				description += $" {Globals.GetTranslation("weapon-description-defense")}";
			}

			if (weaponIndex == WeaponIndexes.DarkSword)
			{
				description += $" {Globals.GetTranslation("weapon-description-heals")}";
			}

			weapon.Description = description;
		}

		/// <summary>
		/// Writes the changed weapon info to the spoiler log
		/// </summary>
		/// <param name="modifiedWeaponDictionary">The dictionary with changed info</param>
		private static void WriteToSpoilerLog(Dictionary<string, WeaponData> modifiedWeaponDictionary)
		{
			if (!Globals.Config.Weapons.Randomize) { return; }

			Globals.SpoilerWrite("==== WEAPONS ====");
			foreach (var weaponData in modifiedWeaponDictionary)
			{
				WeaponData weapon = weaponData.Value;

                Globals.SpoilerWrite($"{weaponData.Key}: {weapon.DisplayName}");
				Globals.SpoilerWrite($"Type: {Enum.GetName(typeof(WeaponType), weapon.Type)}");
				Globals.SpoilerWrite($"Damage: {weapon.MinDamage} - {weapon.MaxDamage}");
				Globals.SpoilerWrite($"Crit Chance / Multiplier: {weapon.CritChance} / {weapon.CritMultiplier}");
				Globals.SpoilerWrite($"Knockback / Speed / AOE: {weapon.Knockback} / {weapon.Speed} / {weapon.AreaOfEffect}");
				Globals.SpoilerWrite($"Added Precision / Defense: {weapon.Precision} / {weapon.Defense}");
				Globals.SpoilerWrite($"Base / Min Mine Level Drop: {weapon.MineBaseLevel} / {weapon.MineMinLevel}");
				Globals.SpoilerWrite("---");
			}
			Globals.SpoilerWrite("");
		}
	}
}
