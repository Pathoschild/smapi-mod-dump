/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/FerMod/StardewMods
**
*************************************************/

using System;
using System.Reflection;
using HarmonyLib;

namespace CryptOfTheNecrodancerEnemies.Framework.Patches {

  public abstract class ClassPatch : IClassPatch {

    public abstract MethodInfo[] Original { get; }
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
      var prefix = PreparePatchMethod(Prefix);
      var postfix = PreparePatchMethod(Postfix);
      var transpiler = PreparePatchMethod(Transpiler);
      var finalizer = PreparePatchMethod(Finalizer);

      foreach (var originalMethod in Original) {
        harmony.Patch(
          original: originalMethod,
          prefix: prefix,
          postfix: postfix,
          transpiler: transpiler,
          finalizer: finalizer
        );
      }

    }

    public void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All) {
      foreach (var originalMethod in Original) {
        harmony.Unpatch(
          original: originalMethod,
          type: patchType,
          harmonyID: harmony.Id
        );
      }
    }

  }

}
