using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Reflection;

namespace Randomizer
{
	/// <summary>
	/// Fixes the buy prices of the items in the adventure shop
	/// </summary>
	public class OverriddenAdventureShop
	{
		/// <summary>
		/// A copy of the original "getAdventureShopStock" function in Utility.cs, which changes
		/// the prices to be dynamic such that it can't sell for more than you can buy it for
		/// </summary>
		/// <returns />
		public static Dictionary<ISalable, int[]> NewGetAdventureShopStock()
		{
			Dictionary<ISalable, int[]> dictionary = new Dictionary<ISalable, int[]>();
			int maxValue = int.MaxValue;
			dictionary.Add((ISalable)new MeleeWeapon(12), new int[2]
			{
				WeaponRandomizer.Weapons[12].GetBuyPrice(),
				maxValue
			});
			if (MineShaft.lowestLevelReached >= 15)
				dictionary.Add((ISalable)new MeleeWeapon(17), new int[2]
				{
				  WeaponRandomizer.Weapons[17].GetBuyPrice(),
				  maxValue
				});
			if (MineShaft.lowestLevelReached >= 20)
				dictionary.Add((ISalable)new MeleeWeapon(1), new int[2]
				{
				   WeaponRandomizer.Weapons[1].GetBuyPrice(),
				  maxValue
				});
			if (MineShaft.lowestLevelReached >= 25)
			{
				dictionary.Add((ISalable)new MeleeWeapon(43), new int[2]
				{
				   WeaponRandomizer.Weapons[43].GetBuyPrice(),
				  maxValue
				});
				dictionary.Add((ISalable)new MeleeWeapon(44), new int[2]
				{
				  WeaponRandomizer.Weapons[44].GetBuyPrice(),
				  maxValue
				});
			}
			if (MineShaft.lowestLevelReached >= 40)
				dictionary.Add((ISalable)new MeleeWeapon(27), new int[2]
				{
				   WeaponRandomizer.Weapons[27].GetBuyPrice(),
				  maxValue
				});
			if (MineShaft.lowestLevelReached >= 45)
				dictionary.Add((ISalable)new MeleeWeapon(10), new int[2]
				{
					 WeaponRandomizer.Weapons[10].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 55)
				dictionary.Add((ISalable)new MeleeWeapon(7), new int[2]
				{
					WeaponRandomizer.Weapons[7].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 75)
				dictionary.Add((ISalable)new MeleeWeapon(5), new int[2]
				{
					WeaponRandomizer.Weapons[5].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 90)
				dictionary.Add((ISalable)new MeleeWeapon(50), new int[2]
				{
					WeaponRandomizer.Weapons[50].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 120)
				dictionary.Add((ISalable)new MeleeWeapon(9), new int[2]
				{
					WeaponRandomizer.Weapons[9].GetBuyPrice(),
					maxValue
				});
			if (Game1.player.mailReceived.Contains("galaxySword"))
			{
				dictionary.Add((ISalable)new MeleeWeapon(4), new int[2]
				{
					WeaponRandomizer.Weapons[4].GetBuyPrice(),
					maxValue
				});
				dictionary.Add((ISalable)new MeleeWeapon(23), new int[2]
				{
					WeaponRandomizer.Weapons[23].GetBuyPrice(),
					maxValue
				});
				dictionary.Add((ISalable)new MeleeWeapon(29), new int[2]
				{
					WeaponRandomizer.Weapons[29].GetBuyPrice(),
					maxValue
				});
			}
			dictionary.Add((ISalable)new Boots(504), new int[2]
			{
				BootRandomizer.Boots[504].GetBuyPrice(),
				maxValue
			});
			if (MineShaft.lowestLevelReached >= 10)
				dictionary.Add((ISalable)new Boots(506), new int[2]
				{
					BootRandomizer.Boots[506].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 50)
				dictionary.Add((ISalable)new Boots(509), new int[2]
				{
					BootRandomizer.Boots[509].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 40)
				dictionary.Add((ISalable)new Boots(508), new int[2]
				{
					BootRandomizer.Boots[508].GetBuyPrice(),
					maxValue
				});
			if (MineShaft.lowestLevelReached >= 80)
			{
				dictionary.Add((ISalable)new Boots(512), new int[2]
				{
					BootRandomizer.Boots[512].GetBuyPrice(),
					maxValue
				});
				dictionary.Add((ISalable)new Boots(511), new int[2]
				{
					BootRandomizer.Boots[511].GetBuyPrice(),
					maxValue
				});
			}
			if (MineShaft.lowestLevelReached >= 110)
				dictionary.Add((ISalable)new Boots(514), new int[2]
				{
					BootRandomizer.Boots[514].GetBuyPrice(),
					maxValue
				});
			dictionary.Add((ISalable)new Ring(529), new int[2]
			{
				1000,
				maxValue
			});
			dictionary.Add((ISalable)new Ring(530), new int[2]
			{
				1000,
				maxValue
			});
			if (MineShaft.lowestLevelReached >= 40)
			{
				dictionary.Add((ISalable)new Ring(531), new int[2]
				{
					2500,
					maxValue
				});
				dictionary.Add((ISalable)new Ring(532), new int[2]
				{
					2500,
					maxValue
				});
			}
			if (MineShaft.lowestLevelReached >= 80)
			{
				dictionary.Add((ISalable)new Ring(533), new int[2]
				{
					5000,
					maxValue
				});
				dictionary.Add((ISalable)new Ring(534), new int[2]
				{
					5000,
					maxValue
				});
			}
			int lowestLevelReached = MineShaft.lowestLevelReached;
			if (Game1.player.craftingRecipes.ContainsKey("Explosive Ammo"))
				dictionary.Add((ISalable)new Object(441, int.MaxValue, false, -1, 0), new int[2]
				{
					300,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Slime Charmer Ring"))
				dictionary.Add((ISalable)new Ring(520), new int[2]
				{
					25000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Savage Ring"))
				dictionary.Add((ISalable)new Ring(523), new int[2]
				{
					25000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Burglar's Ring"))
				dictionary.Add((ISalable)new Ring(526), new int[2]
				{
					20000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Vampire Ring"))
				dictionary.Add((ISalable)new Ring(522), new int[2]
				{
					15000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Crabshell Ring"))
				dictionary.Add((ISalable)new Ring(810), new int[2]
				{
					15000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Napalm Ring"))
				dictionary.Add((ISalable)new Ring(811), new int[2]
				{
					30000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Skeleton Mask"))
				dictionary.Add((ISalable)new Hat(8), new int[2]
				{
					20000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Hard Hat"))
				dictionary.Add((ISalable)new Hat(27), new int[2]
				{
					20000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Arcane Hat"))
				dictionary.Add((ISalable)new Hat(60), new int[2]
				{
					20000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Knight's Helmet"))
				dictionary.Add((ISalable)new Hat(50), new int[2]
				{
					20000,
					maxValue
				});
			if (Game1.player.mailReceived.Contains("Gil_Insect Head"))
				dictionary.Add((ISalable)new MeleeWeapon(13), new int[2]
				{
					WeaponRandomizer.Weapons[13].GetBuyPrice(),
					maxValue
				});
			return dictionary;
		}

		/// <summary>
		/// Replaces the getAdventureShopStock method in Utility.cs with this file's NewGetAdventureShopStock method
		/// and the getItemlevel method in MeleeWeapon.cs with this file's NewGetItemLevel method
		/// NOTE: THIS IS UNSAFE CODE, CHANGE WITH EXTREME CAUTION
		/// </summary>
		public static void FixAdventureShopBuyAndSellPrices()
		{
			if (Globals.Config.RandomizeWeapons || Globals.Config.RandomizeBoots)
			{
				MethodInfo methodToReplace = typeof(Utility).GetMethod("getAdventureShopStock");
				MethodInfo methodToInject = typeof(OverriddenAdventureShop).GetMethod("NewGetAdventureShopStock");
				Globals.RepointMethod(methodToReplace, methodToInject);
			}
		}
	}
}
