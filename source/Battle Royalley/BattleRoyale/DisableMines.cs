using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyale
{
	class DisableMines_enterMine : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Game1), "enterMine");
		public static bool Prefix() {
			return !ModEntry.BRGame.IsGameInProgress;
		}

	}

	class DisableMines_nextMineLevel : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Game1), "nextMineLevel");
		public static bool Prefix()
		{
			return !ModEntry.BRGame.IsGameInProgress;
		}
	}
}
