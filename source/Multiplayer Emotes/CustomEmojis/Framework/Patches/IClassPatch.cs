/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using Harmony;
using System.Reflection;

namespace CustomEmojis.Framework.Patches {


	public interface IClassPatch {

		MethodInfo Original { get; }
		MethodInfo Prefix { get; }
		MethodInfo Postfix { get; }
		MethodInfo Transpiler { get; }

		void Register(HarmonyInstance harmony);
		void Remove(HarmonyInstance harmony, HarmonyPatchType patchType = HarmonyPatchType.All);

	}

}
