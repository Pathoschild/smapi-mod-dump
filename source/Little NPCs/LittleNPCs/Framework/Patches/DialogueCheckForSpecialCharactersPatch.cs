/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System.Linq;

using StardewValley;


namespace LittleNPCs.Framework.Patches {
    /// <summary>
    /// Prefix for <code>Dialogue.checkForSpecialCharacters</code>.
    /// Enables use of special variables %kid1 and %kid2 for LittleNPC objects.
    /// </summary>
    class DialogueCheckForSpecialCharactersPatch {
        public static bool Prefix(Dialogue __instance, ref string str) {
            // Here we provide names for LittleNPCs,
            // the original method is still used to provide children's names.
            if (ModEntry.LittleNPCsList.Count > 0) {
                str = str.Replace("%kid1", ModEntry.GetLittleNPC(0).displayName);
            }
            if (ModEntry.LittleNPCsList.Count > 1) {
                str = str.Replace("%kid2", ModEntry.GetLittleNPC(1).displayName);
            }

            // Enable original method.
            return true;
        }
    }
}
