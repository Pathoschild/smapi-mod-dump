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

namespace HarmonyPatchLib.Framework.Patches {

  public interface IClassPatch {

    /// <summary>
    /// The original method.
    /// </summary>
    /// <seealso href="https://harmony.pardeike.net/articles/patching.html">Harmony patching.</seealso>
    MethodInfo Original { get; }

    /// <summary>
    /// The method that is executed before the <see cref="Original">Original</see> method.
    /// </summary>
    /// <seealso href="https://harmony.pardeike.net/articles/patching-prefix.html">Harmony prefix methods.</seealso>
    MethodInfo Prefix { get; }

    /// <summary>
    /// The method that is executed after the <see cref="Original">Original</see> method.
    /// </summary>
    /// <seealso href="https://harmony.pardeike.net/articles/patching-postfix.html">Harmony prefix methods.</seealso>
    MethodInfo Postfix { get; }

    /// <summary>
    /// The method that can alter IL code of the <see cref="Original">Original</see> method, but is executed at runtime.
    /// 
    /// <para>
    /// A transpiler is executed only once before the original is run. It can therefore not have access to any runtime state.
    /// <c>Harmony</c> will run it once when you patch the method and again every time someone else adds a transpiler for the
    /// same methods. Transpilers are chained to produce the final output.
    /// </para>
    /// </summary>
    /// <seealso href="https://harmony.pardeike.net/articles/patching-transpiler.html">Harmony transpiler methods.</seealso>
    MethodInfo Transpiler { get; }

    /// <summary>
    /// The method that makes <c>Harmony</c> wrap the <see cref="Original">Original</see> and all other patches in a try/catch block.
    /// It can receive a thrown exception and even suppress it or return a different one.
    /// </summary>
    /// <seealso href="https://harmony.pardeike.net/articles/patching-finalizer.html">Harmony prefix methods.</seealso>
    MethodInfo Finalizer { get; }

    bool PrefixEnabled { get; set; }
    bool PostfixEnabled { get; set; }
    bool TranspilerEnabled { get; set; }
    bool FinalizerEnabled { get; set; }

    void Register(Harmony harmony);
    void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All);

  }

}
