
using Harmony;
using System.Reflection;

namespace MultiplayerEmotes.Framework.Patches {

	public interface IClassPatch {

		MethodInfo Original { get; }
		MethodInfo Prefix { get; }
		MethodInfo Postfix { get; }
		MethodInfo Transpiler { get; }

		bool PrefixEnabled { get; set; }
		bool PostfixEnabled { get; set; }
		bool TranspilerEnabled { get; set; }

		void Register(HarmonyInstance harmony);
		void Remove(HarmonyInstance harmony, HarmonyPatchType patchType = HarmonyPatchType.All);

	}

}
