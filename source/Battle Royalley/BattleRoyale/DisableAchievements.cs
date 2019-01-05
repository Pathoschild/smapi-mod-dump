using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyale
{
	class DisableAchievements : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Game1), "getAchievement");

		public static bool Prefix()
		{
			return !ModEntry.BRGame.IsGameInProgress;
		}
	}
}
