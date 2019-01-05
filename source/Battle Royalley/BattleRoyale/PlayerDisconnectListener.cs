using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyale
{
	class PlayerDisconnectListener : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Multiplayer), "playerDisconnected");

		public static bool Prefix(long id, List<long> ___disconnectingFarmers)
		{
			if (Game1.IsServer && Game1.otherFarmers.ContainsKey(id) && !___disconnectingFarmers.Contains(id))
			//if (Game1.otherFarmers.ContainsKey(id) && !___disconnectingFarmers.Contains(id))
			{
				ModEntry.BRGame.HandleDeath(Game1.otherFarmers[id]);
			}
				
			return true;
		}
	}
}
