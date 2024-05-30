/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using StardewValley.Objects;

namespace AnythingAnywhere.Framework.Patches.StandardObjects
{
    internal sealed class CaskPatch : PatchHelper
    {
        internal CaskPatch() : base(typeof(Cask)) { }
        internal void Apply()
        {
            Patch(PatchType.Prefix, nameof(Cask.IsValidCaskLocation), nameof(IsValidCaskLocationPrefix));
        }

        // Enable cask functionality outside of the farm
        private static bool IsValidCaskLocationPrefix(Cask __instance, ref bool __result)
        {
            if (!ModEntry.Config.EnableCaskFunctionality) return true;

            __result = true;
            return false;
        }
    }
}
