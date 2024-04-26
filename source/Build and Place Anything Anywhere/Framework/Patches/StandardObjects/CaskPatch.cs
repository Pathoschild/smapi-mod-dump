/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley.Menus;
using System.Collections.Generic;
using System;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal class CaskPatch : PatchTemplate
    {
        private readonly Type _object = typeof(Cask);

        internal CaskPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }
        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_object, nameof(Cask.IsValidCaskLocation)), prefix: new HarmonyMethod(GetType(), nameof(IsValidCaskLocationPrefix)));
        }

        // Enable jukebox functionality outside of the farm
        private static bool IsValidCaskLocationPrefix(Cask __instance, ref bool __result)
        {
            if (ModEntry.modConfig.EnableCaskFunctionality)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
