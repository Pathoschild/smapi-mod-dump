using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarpToFriends.Helpers
{
	public static class PlayerHelper
	{

		public static List<Farmer> GetAllCreatedFarmers()
		{
			return Game1.getAllFarmers().Where(f => !string.IsNullOrEmpty(f.name)).ToList<Farmer>();
		}

	}
}
