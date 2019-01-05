using Harmony;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale
{
	class ClubCooldownIncrease1 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(MeleeWeapon), "doAnimateSpecialMove");

		public const int NewClubCooldown = 10000;

		private static int beforeClubcooldown = -1;

		public static void Prefix()
		{
			beforeClubcooldown = MeleeWeapon.clubCooldown;
		}

		public static void Postfix()
		{
			int currentCooldown = MeleeWeapon.clubCooldown;
			if (beforeClubcooldown < currentCooldown && currentCooldown == MeleeWeapon.clubCooldownTime)
			{
				MeleeWeapon.clubCooldown = NewClubCooldown;
			}
		}
	}

	class ClubCooldownIncrease2 : Patch
	{
		protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(MeleeWeapon), "drawInMenu");

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
		{
			var l = instr.ToArray();

			for (int i = 0; i < l.Length; i++)
			{
				if (l[i].operand != null && l[i].operand.ToString() == MeleeWeapon.clubCooldownTime.ToString())
				{
					l[i].operand = (System.Single)ClubCooldownIncrease1.NewClubCooldown;
				}
			}

			return l;
		}
	}
}
