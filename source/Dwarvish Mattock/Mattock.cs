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
using static System.Reflection.BindingFlags;

namespace DwarvishMattock
{
	public class Mattock : MeleeWeapon
	{
		// Keeps track of objects that are struck in a single swing so they don't get hit multiple times.
		public HashSet<Object> struckObjects = new HashSet<Object>();
		public HashSet<TerrainFeature> struckFeatures = new HashSet<TerrainFeature>();
		public Mattock() : base(70)
		{
		}

		public override int salePrice()
		{
			return -1;
		}

		public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
		{
			struckObjects.Clear();
			struckFeatures.Clear();
			base.DoFunction(location, x, y, power, who);
		}

		public Axe asAxe()
		{
			Axe standIn = new Axe();
			standIn.UpgradeLevel = 3;
			CopyEnchantments(this, standIn);

			// Set the "lastUser" field to the player.
			var lastUser = standIn.GetType().GetField("lastUser", NonPublic | Instance);
			lastUser.SetValue(standIn, Game1.player);

			return standIn;
		}

		public Pickaxe asPickaxe()
		{
			Pickaxe standIn = new Pickaxe();
			standIn.UpgradeLevel = 3;
			CopyEnchantments(this, standIn);

			// Set the "lastUser" field to the player.
			var lastUser = standIn.GetType().GetField("lastUser", NonPublic | Instance);
			lastUser.SetValue(standIn, Game1.player);

			return standIn;
		}
	}
}