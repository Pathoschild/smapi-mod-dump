/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CrunchyDuck/Stardew-Skip-Text
**
*************************************************/

using StardewModdingAPI;
using Harmony;

namespace SkipText {
    public class ModEntry : Mod {
        public override void Entry(IModHelper helper) {
            //Patch.Init(this);
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            System.Reflection.MethodInfo original = AccessTools.Method(typeof(StardewValley.Menus.DialogueBox), "update"); // Handles the dialogue and text boxes
            HarmonyMethod patch = new HarmonyMethod(typeof(Patch), "Prefix");
            harmony.Patch(original, prefix: patch);
        }
    }

    class Patch {
  //      private static ModEntry Mod;
  //      public static void Init(ModEntry mod) {
  //          Mod = mod;
		//}

        public static void Prefix(StardewValley.Menus.DialogueBox __instance) {
            __instance.characterIndexInDialogue = __instance.getCurrentString().Length;
        }
    }
}