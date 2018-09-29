using Harmony;

namespace NeatAdditions
{
	interface IPatch
	{
		void Patch(HarmonyInstance harmony);

		string GetPatchName();
	}
}
