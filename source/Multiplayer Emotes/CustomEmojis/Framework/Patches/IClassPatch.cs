
using Harmony;
using StardewModdingAPI;
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
