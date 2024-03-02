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
    public class WeaponData
	{
		private static Dictionary<int, string> DefaultWeaponData;

		/// <summary>
		/// Initializes the weapon data to the values in Data/weapons.xnb
		/// </summary>
		private static void Initialize()
		{
            DefaultWeaponData = Globals.ModRef.Helper.GameContent
				.Load<Dictionary<int, string>>("Data/weapons");
        }

		public enum WeaponFields
		{
			Name,
			Description,
			MinDamage,
			MaxDamage,
			Knockback,
			Speed,
			AddedPrecision,
			AddedDefense,
			Type,
			BaseMineLevelDrop,
			MinMineLevelDrop,
			AddedAOE,
			CritChance,
			CritMultiplier
		}

		/// <summary>
		/// Populates the given weapon with its default info
		/// </summary>
		/// <param name="weapon">The weapon</param>
		private static void FillDefaultWeaponInfo(WeaponItem weapon)
		{
			string input = DefaultWeaponData[weapon.Id];
			string[] fields = input.Split('/');
			if (fields.Length < 14)
			{
				Globals.ConsoleError($"Incorrect number of fields when parsing weapons with input: {input}");
				return;
			}

			// Name/Description
			weapon.OverrideName = fields[(int)WeaponFields.Name];
			weapon.Description = fields[(int)WeaponFields.Description];

			// Damage
			if (!int.TryParse(fields[(int)WeaponFields.MinDamage], out int minDamage))
			{
				Globals.ConsoleError($"Could not parse the min damage when parsing weapon with input: {input}");
				return;
			}

			if (!int.TryParse(fields[(int)WeaponFields.MaxDamage], out int maxDamage))
			{
				Globals.ConsoleError($"Could not parse the max damage when parsing weapon with input: {input}");
				return;
			}

			weapon.Damage = new Range(minDamage, maxDamage);

			// Knockback
			if (!double.TryParse(fields[(int)WeaponFields.Knockback], out double knockback))
			{
				Globals.ConsoleError($"Could not parse knockback when parsing weapon with input: {input}");
				return;
			}
			weapon.Knockback = knockback;

			// Speed
			if (!int.TryParse(fields[(int)WeaponFields.Speed], out int speed))
			{
				Globals.ConsoleError($"Could not parse speed when parsing weapon with input: {input}");
				return;
			}
			weapon.Speed = speed;

			// Added Precision
			if (!int.TryParse(fields[(int)WeaponFields.AddedPrecision], out int addedPrecision))
			{
				Globals.ConsoleError($"Could not parse the added precision when parsing weapon with input: {input}");
				return;
			}
			weapon.AddedPrecision = addedPrecision;

			// Added Defense
			if (!int.TryParse(fields[(int)WeaponFields.AddedDefense], out int addedDefense))
			{
				Globals.ConsoleError($"Could not parse the added defense when parsing weapon with input: {input}");
				return;
			}
			weapon.AddedDefense = addedDefense;

			// Type
			if (!int.TryParse(fields[(int)WeaponFields.Type], out int type))
			{
				Globals.ConsoleError($"Could not parse the weapon type when parsing weapon with input: {input}");
				return;
			}
			weapon.Type = (WeaponType)type;

			// Base Mine Level Drop
			if (!int.TryParse(fields[(int)WeaponFields.BaseMineLevelDrop], out int baseMineLevelDrop))
			{
				Globals.ConsoleError($"Could not parse the base mine level drop when parsing weapon with input: {input}");
				return;
			}
			weapon.BaseMineLevelDrop = baseMineLevelDrop;

			// Minimum Mine Level Drop
			if (!int.TryParse(fields[(int)WeaponFields.MinMineLevelDrop], out int minMineLevelDrop))
			{
				Globals.ConsoleError($"Could not parse the minimum mine level drop when parsing weapon with input: {input}");
				return;
			}
			weapon.MinMineLevelDrop = minMineLevelDrop;

			// Added AOE 
			if (!int.TryParse(fields[(int)WeaponFields.AddedAOE], out int addedAOE))
			{
				Globals.ConsoleError($"Could not parse the added AOE value when parsing weapon with input: {input}");
				return;
			}
			weapon.AddedAOE = addedAOE;

			// Crit Chance
			if (!double.TryParse(fields[(int)WeaponFields.CritChance], out double critChance))
			{
				Globals.ConsoleError($"Could not parse crit chance when parsing weapon with input: {input}");
				return;
			}
			weapon.CritChance = critChance;

			// Crit Multiplier
			if (!double.TryParse(fields[(int)WeaponFields.CritMultiplier], out double critMultiplier))
			{
				Globals.ConsoleError($"Could not parse the crit multiplier when parsing weapon with input: {input}");
				return;
			}
			weapon.CritMultiplier = critMultiplier;
		}

		/// <summary>
		/// The weapon items in dictionary form - data taken from Data/weapons.xnb
		/// Excludes slingshots and scythes
		/// </summary>
		/// <returns />
		public static Dictionary<int, WeaponItem> Items()
		{
            if (DefaultWeaponData == null)
            {
                Initialize();
            }

            Dictionary<int, WeaponItem> weaponItemMap = new();
			foreach (KeyValuePair<int, string> data in DefaultWeaponData)
			{
				int id = data.Key;

				// We don't want to include slingshots or scythes
                string weaponName = data.Value.Split("/")[(int)WeaponFields.Name];
                if (weaponName.Contains("Slingshot") || weaponName.Contains("Scythe"))
                {
					continue;
                }

                WeaponItem weapon = new(id);
				FillDefaultWeaponInfo(weapon);
				weaponItemMap.Add(id, weapon);
			}

			return weaponItemMap;
		}
	}
}
