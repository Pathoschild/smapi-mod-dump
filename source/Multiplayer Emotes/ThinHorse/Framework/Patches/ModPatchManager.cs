/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Collections.Generic;
using HarmonyLib;
using StardewModdingAPI;

namespace ThinHorse.Framework.Patches {

  public class ModPatchManager {

    public List<IClassPatch> PatchList { get; set; } = new List<IClassPatch>();
    public static Harmony HarmonyInstance { get; set; }

    public ModPatchManager(IModHelper helper, List<IClassPatch> patchList = default) {
      HarmonyInstance = new Harmony(helper.ModRegistry.ModID);
      PatchList = patchList;
    }

    public void PatchAll() {
      HarmonyInstance.PatchAll();
    }

    public void ApplyPatch() {
      this.PatchList.ForEach(patch => patch.Register(HarmonyInstance));
    }

    public void RemovePatch() {
      this.PatchList.ForEach(patch => patch.Remove(HarmonyInstance));
    }

  }

}
