using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BattleRoyale
{
	class BombFixer : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(TemporaryAnimatedSprite), "update");

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
		{
			//5 to 12 is 
			/*if (bombRadius > 0 && !Game1.shouldTimePass())
			{
				return false;
			}*/

			var a = instr.ToArray();
			
			for (int i = 5; i <= 12; i++)
			{
				a[i].opcode = OpCodes.Nop;
			}

			return a;
		}
	}
}
