/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/DwarvishMattock
**
*************************************************/

using System.Collections.Generic;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace DwarvishMattock
{
	public class Mattock : MeleeWeapon
	{
		// Keeps track of objects that are struck in a single swing so they don't get hit multiple times.
		public HashSet<Object> struckObjects = new();
		public HashSet<TerrainFeature> struckFeatures = new();
		public Mattock() : base(ModEntry.MATTOCK_WEAPON_ID)
		{
		}

		public override int salePrice(bool ignoreProfitMargins = false)
		{
			return -1;
		}

		public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
		{
			struckObjects.Clear();
			struckFeatures.Clear();
			base.DoFunction(location, x, y, power, who);
		}

		public override void setFarmerAnimating(Farmer who)
		{
			struckObjects.Clear();
			struckFeatures.Clear();
			base.setFarmerAnimating(who);
		}

		public Axe AsAxe()
		{
			Axe standIn = new() { UpgradeLevel = 3, lastUser = Game1.player };

			CopyEnchantments(this, standIn);

			return standIn;
		}

		public Pickaxe AsPickaxe()
		{
			Pickaxe standIn = new() { UpgradeLevel = 3, lastUser = Game1.player };

			CopyEnchantments(this, standIn);

			return standIn;
		}
	}
}