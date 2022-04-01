/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;

#endregion using directives

[UsedImplicitly]
internal class DebugPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal DebugPatch()
    {
#if DEBUG
        //Original = RequireMethod<>(nameof(.));
#endif
    }

    #region harmony patches

    [HarmonyPrefix]
    private static bool DebugPrefix()
    {
        Log.D("DebugPatch called!");

        return false;
    }

    #endregion harmony patches
}