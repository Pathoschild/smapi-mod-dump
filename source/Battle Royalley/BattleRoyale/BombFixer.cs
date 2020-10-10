/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/BattleRoyalley
**
*************************************************/

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
