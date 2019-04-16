using Harmony;
using StardewModdingAPI;
using System.Collections.Generic;

namespace MultiplayerEmotes.Framework.Patches {

	public class ModPatchManager {

		public List<IClassPatch> PatchList { get; set; }
		public static HarmonyInstance Harmony { get; set; }

		public ModPatchManager(IModHelper helper) {
			Harmony = HarmonyInstance.Create(helper.ModRegistry.ModID);
			PatchList = new List<IClassPatch>();
		}

		public void ApplyPatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Register(Harmony);
			}
		}

		public void RemovePatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Remove(Harmony);
			}
		}

	}

}
