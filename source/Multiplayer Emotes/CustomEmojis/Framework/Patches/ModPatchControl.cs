/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/


using HarmonyLib;
using StardewModdingAPI;
using System.Collections.Generic;

namespace CustomEmojis.Framework.Patches {

	public class ModPatchControl {

		public List<IClassPatch> PatchList { get; set; }
		public static Harmony HarmonyInstance { get; set; }

		public ModPatchControl(IModHelper helper) {
			HarmonyInstance = new Harmony(helper.ModRegistry.ModID);
			PatchList = new List<IClassPatch>();
		}

		public void ApplyPatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Register(HarmonyInstance);
			}
		}

		public void RemovePatch() {
			foreach(IClassPatch patch in PatchList) {
				patch.Remove(HarmonyInstance);
			}
		}

	}

}
