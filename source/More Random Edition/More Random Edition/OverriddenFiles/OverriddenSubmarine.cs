using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;
using System.Linq;
using SVOBject = StardewValley.Object;

namespace Randomizer
{
	/// <summary>
	/// The submarine GameLocation - identical to the original Submarine.cs file, but with a
	/// different getFish function to include our randomized fish instead of their fish
	/// </summary>
	public class OverriddenSubmarine : Submarine
	{
		public OverriddenSubmarine() : base("Maps\\Submarine", "Submarine") { }

		/// <summary>
		/// Gets the fish that can be found on the submarine
		/// </summary>
		/// <param name="millisecondsAfterNibble"></param>
		/// <param name="bait"></param>
		/// <param name="waterDepth"></param>
		/// <param name="who"></param>
		/// <param name="baitPotency"></param>
		/// <returns />
		public override SVOBject getFish(
			float millisecondsAfterNibble,
			int bait,
			int waterDepth,
			Farmer who,
			double baitPotency,
			Vector2 bobberTile,
			string locationName)
		{
			List<int> nightMarketFish = FishItem.Get(Locations.NightMarket).Select(x => x.Id).ToList();

			if (Game1.random.NextDouble() < 0.15)
				return new SVOBject(nightMarketFish[0], 1, false, -1, 0);
			if (Game1.random.NextDouble() < 0.23)
				return new SVOBject(nightMarketFish[1], 1, false, -1, 0);
			if (Game1.random.NextDouble() < 0.30)
				return new SVOBject(nightMarketFish[2], 1, false, -1, 0);
			if (Game1.random.NextDouble() < 0.01)
				return new SVOBject(797, 1, false, -1, 0);

			return new SVOBject(152, 1, false, -1, 0);
		}
	}
}
