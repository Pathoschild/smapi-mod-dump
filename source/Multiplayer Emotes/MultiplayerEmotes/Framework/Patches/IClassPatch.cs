/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System.Reflection;
using HarmonyLib;

namespace MultiplayerEmotes.Framework.Patches {

  public interface IClassPatch {

    /// <summary>
    /// The original method.
    /// </summary>
    MethodInfo Original { get; }

    /// <summary>
    /// The prefix method.
    /// </summary>
    MethodInfo Prefix { get; }
    MethodInfo Postfix { get; }
    MethodInfo Transpiler { get; }
    MethodInfo Finalizer { get; }

    bool PrefixEnabled { get; set; }
    bool PostfixEnabled { get; set; }
    bool TranspilerEnabled { get; set; }
    bool FinalizerEnabled { get; set; }

    void Register(Harmony harmony);
    void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All);

  }

}
