/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    internal class SaveGamePatch : IPatch
    {
        public readonly Type PATCH_TYPE = typeof(SaveGame);
        public SaveGamePatch() { }

        public static void deleteEmergencySaveIfCalled(object obj)
        {
            ModUtilities.Helper.Reflection.GetMethod(typeof(SaveGame), "deleteEmergencySaveIfCalled").Invoke(obj);
        }
        public static bool newerBackUpExists(string obj)
        {
            return ModUtilities.Helper.Reflection.GetMethod(typeof(SaveGame), "newerBackUpExists").Invoke<bool>(obj);
        }
        public static void Load(string filename, bool loadEmergencySave = false, bool loadBackupSave = false)
        {
            ModUtilities.Helper.Reflection.GetMethod(typeof(SaveGame), "Load").Invoke(filename, loadEmergencySave, loadBackupSave);
        }

        private static void postfix_getLoadEnumerator(string file)
        {
            if (Game1.multiplayerMode == 2)
            {
                Game1.options.setServerMode("friends");
            }
        }

        public void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(PATCH_TYPE, "getLoadEnumerator"), postfix: new HarmonyMethod(this.GetType() , nameof(postfix_getLoadEnumerator)));
        }
    }
}
