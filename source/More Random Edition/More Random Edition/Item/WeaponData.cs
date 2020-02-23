using System.Collections.Generic;

namespace Randomizer
{
	public class WeaponData
	{
		public static Dictionary<int, string> DefaultStringData = new Dictionary<int, string>
		{
			{0, "Rusty Sword/A rusty, dull old sword./2/5/1/0/0/0/3/-1/-1/0/.02/3" },
			{1, "Silver Saber/Plated with silver to deter rust./8/15/1/0/1/1/0/50/45/0/.02/3" },
			{2, "Dark Sword/It's glowing with vampire energy./30/45/1.5/-10/0/0/0/-1/-1/2/.04/3" },
			{3, "Holy Blade/It feels hopeful to wield./18/24/1.2/0/5/0/3/-1/-1/0/.02/3" },
			{4, "Galaxy Sword/It's unlike anything you've ever seen./60/80/1/8/0/0/0/-1/-1/0/.02/3" },
			{5, "Bone Sword/A very light piece of sharpened bone./20/30/.8/8/5/0/0/74/50/0/.02/3" },
			{6, "Iron Edge/A heavy broadsword./12/25/1.2/-4/0/1/3/44/-1/0/.02/3" },
			{7, "Templar's Blade/It once belonged to an honorable knight./22/29/1/0/10/1/3/90/50/0/0/3" },
			{8, "Obsidian Edge/It's incredibly sharp./30/45/1/-2/0/0/0/121/100/0/.02/3.2" },
			{9, "Lava Katana/A powerful blade forged in a pool of churning lava./55/64/1.2/0/0/3/0/-1/-1/2/.015/3.5" },
			{10, "Claymore/It's really heavy./20/32/1.3/-8/0/2/3/86/50/0/.02/3" },
			{11, "Steel Smallsword/A standard metal blade./4/8/1/4/0/0/0/26/-1/0/.02/3" },
			{12, "Wooden Blade/Not bad for a piece of carved wood./3/7/1/0/0/0/0/3/-1/0/.02/3" },
			{13, "Insect Head/Not very pleasant to wield./10/20/1/4/9/0/0/-1/-1/0/.04/3" },
			{14, "Neptune's Glaive/An heirloom from beyond the Gem Sea./18/35/1.4/-2/6/2/3/-1/-1/0/.02/3" },
			{15, "Forest Sword/Made powerful by forest magic./8/18/1/4/5/1/0/-1/-1/0/.02/3" },
			{16, "Carving Knife/A small, light blade./1/3/.5/0/0/0/1/18/-1/0/.04/3" },
			{17, "Iron Dirk/A common dagger./2/4/.5/0/0/0/1/62/50/0/.03/3" },
			{18, "Burglar's Shank/A weapon of choice for the swift and silent./7/12/.5/0/5/0/1/114/100/0/.04/3.5" },
			{19, "Shadow Dagger/When you hold the blade to your ear you can hear 1,000 souls shrieking./10/20/.5/0/0/0/1/80/50/0/.04/3" },
			{20, "Elf Blade/Only the nimble hands of an elf could craft this./3/5/.5/0/5/0/1/-1/-1/0/.04/3" },
			{21, "Crystal Dagger/The blade is made of purified quartz./4/10/1/0/10/0/1/-1/-1/0/.03/4" },
			{22, "Wind Spire/A swift little blade./1/5/1/0/0/0/1/-1/-1/0/.02/3.2" },
			{23, "Galaxy Dagger/It's unlike anything you've seen./30/40/1/3/0/0/1/-1/-1/0/.02/3" },
			{24, "Wood Club/A solid piece of wood, crudely chiseled into a club shape./9/16/1.5/-8/0/0/2/32/-1/0/.02/3" },
			{25, "Alex's Bat/The sweet spot is dented from Alex's famous Grand Slam./1/3/1/-8/0/0/2/-1/-1/0/.02/3" },
			{26, "Lead Rod/It's incredibly heavy./18/27/1.5/-16/0/0/2/56/50/1/.02/3" },
			{27, "Wood Mallet/The solid head packs a punch. Relatively light for a club./15/24/1.3/-4/1/0/2/68/50/0/.02/3" },
			{28, "The Slammer/An extremely heavy gavel that'll send foes flying./40/55/1.5/-12/0/0/2/128/100/2/.02/3" },
			{29, "Galaxy Hammer/It's made from an ultra-light material you've never seen before./70/90/1/-4/0/0/2/-1/-1/0/.02/3" },
			{30, "Sam's Old Guitar/It's seen better days./1/3/1/-6/0/0/2/-1/-1/0/.02/3" },
			{31, "Femur/An old, heavy bone caked in centuries of grime./6/11/1.5/-4/0/0/2/10/-1/0/.02/3" },
			{32, "Slingshot/Requires stones for ammo./1/3/1/308/0/0/4/-1/-1/0/.02/3" },
			{33, "Master Slingshot/Requires stones for ammo./1/3/1/308/0/0/4/-1/-1/0/.02/3" },
			{34, "Galaxy Slingshot/It looks really powerful./1/3/1/308/0/0/4/-1/-1/0/.02/3" },
			{35, "Elliott's Pencil/Elliott used this to write his book. It's sharp!/1/3/1/308/0/0/1/-1/-1/0/.02/3" },
			{36, "Maru's Wrench/A big, metal wrench. It smells like Maru./1/3/1/308/0/0/2/-1/-1/0/.02/3" },
			{37, "Harvey's Mallet/It brings back memories of Harvey's clinic./1/3/1/308/0/0/2/-1/-1/0/.02/3" },
			{38, "Penny's Fryer/Penny's favorite frying pan. There's some rubbery gunk stuck to the inside./1/3/1/308/0/0/2/-1/-1/0/.02/3" },
			{39, "Leah's Whittler/Leah's favorite tool for shaping driftwood./1/3/1/308/0/0/1/-1/-1/0/.02/3" },
			{40, "Abby's Planchette/It's made from fine marblewood./1/3/1/308/0/0/1/-1/-1/0/.02/3" },
			{41, "Seb's Lost Mace/One of Sebastian's medieval replicas./1/3/1/308/0/0/2/-1/-1/0/.02/3" },
			{42, "Haley's Iron/It's searing hot and smells like Haley's hair./1/3/1/308/0/0/0/-1/-1/0/.02/3" },
			{43, "Pirate's Sword/It looks like a pirate owned this once./8/14/1/4/0/0/3/40/-1/0/.02/3" },
			{44, "Cutlass/A finely crafted blade./9/17/1/4/0/0/0/-1/-1/0/.02/3" },
			{45, "Wicked Kris/The blade is made of an iridium alloy./24/30/.5/0/8/0/1/-1/-1/2/.06/3" },
			{46, "Kudgel/A brute's companion./27/40/1.6/-10/0/0/2/107/100/0/.02/3.1" },
			{48, "Yeti Tooth/It's icy cold to the touch./26/42/1/0/0/4/0/-1/-1/0/.02/3.2" },
			{49, "Rapier/An elegant blade./15/25/1/4/0/0/0/100/95/2/.02/3" },
			{50, "Steel Falchion/Light and powerful./28/46/1/8/5/0/0/142/100/0/.02/3.4" },
			{51, "Broken Trident/It came from the sea, but it's still sharp./15/26/.5/0/8/0/1/-1/-1/2/.02/3" },
			{52, "Tempered Broadsword/It looks like it could withstand anything./29/44/1.2/-6/0/3/0/135/100/1/.02/3" },
			{53, "Golden Scythe/It's more powerful than a normal scythe./13/13/1/0/0/0/0/-1/-1/2/.02/4" }
		};

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
			string input = DefaultStringData[weapon.Id];
			string[] fields = input.Split('/');
			if (fields.Length != 14)
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
			weapon.AddedPrecision = addedPrecision;

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
		/// The weapon items in dictionary form - data taken from weapons.xnb
		/// </summary>
		/// <returns />
		public static Dictionary<int, WeaponItem> Items()
		{
			Dictionary<int, WeaponItem> weaponItemMap = new Dictionary<int, WeaponItem>();
			foreach (int id in DefaultStringData.Keys)
			{
				WeaponItem weapon = new WeaponItem(id);
				FillDefaultWeaponInfo(weapon);
				weaponItemMap.Add(id, weapon);
			}

			return weaponItemMap;
		}
	}
}
