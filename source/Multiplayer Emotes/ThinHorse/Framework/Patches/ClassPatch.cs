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

namespace ThinHorse.Framework.Patches {

  public abstract class ClassPatch : IClassPatch {

    public abstract MethodInfo Original { get; }
    public virtual MethodInfo Prefix => default;
    public virtual MethodInfo Postfix => default;
    public virtual MethodInfo Transpiler => default;
    public virtual MethodInfo Finalizer => default;

    public bool PrefixEnabled { get; set; } = true;
    public bool PostfixEnabled { get; set; } = true;
    public bool TranspilerEnabled { get; set; } = true;
    public bool FinalizerEnabled { get; set; } = true;

    /// <summary>
    /// Prepares the <paramref name="method"/> that will be patched.
    /// </summary>
    /// <param name="method">The method to patch.</param>
    /// <returns>
    /// The <see cref="HarmonyMethod"/> wrapper to use as a patch, or <see langword="null"/>
    /// if <paramref name="method"/> is <see langword="null"/>.
    /// </returns>
    private HarmonyMethod PreparePatchMethod(MethodInfo method) {
      return method is null ? null : new HarmonyMethod(method);
    }

    public void Register(Harmony harmony) {
      harmony.Patch(
        original: Original,
        prefix: PreparePatchMethod(Prefix),
        postfix: PreparePatchMethod(Postfix),
        transpiler: PreparePatchMethod(Transpiler),
        finalizer: PreparePatchMethod(Finalizer)
      );

    }

    public void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All) {
      harmony.Unpatch(
        original: Original,
        type: patchType,
        harmonyID: harmony.Id
      );
    }

  }

}
