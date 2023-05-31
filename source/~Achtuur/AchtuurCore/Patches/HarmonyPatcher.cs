/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchtuurCore.Patches
{
    /// <summary>
    /// Can take in a number of GenericPatches and apply them using Harmony
    /// </summary>
    public static class HarmonyPatcher
    {
        public static void ApplyPatches(Mod instance, params GenericPatcher[] patches)
        {
            Harmony harmony = new Harmony(instance.ModManifest.UniqueID);
            foreach (GenericPatcher patch in patches)
            {
                try
                {
                    patch.Patch(harmony, instance.Monitor);
                }
                catch(Exception e)
                {
                    instance.Monitor.Log($"Applying patch {patch} failed:\n{e}", LogLevel.Error);
                }
            }
        }
    }
}
