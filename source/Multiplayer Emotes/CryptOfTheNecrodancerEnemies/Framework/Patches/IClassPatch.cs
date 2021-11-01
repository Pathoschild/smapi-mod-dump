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

namespace CryptOfTheNecrodancerEnemies.Framework.Patches {

  public interface IClassPatch {

    /// <summary>
    /// The original method.
    /// </summary>
    /// <remarks>
    /// See <see href="https://harmony.pardeike.net/articles/patching.html">Harmony patching</see> for more information.
    /// </remarks>
    MethodInfo[] Original { get; }

    /// <summary>
    /// The method that is executed before the <see cref="Original">Original</see> method.
    /// </summary>
    /// <remarks>
    /// See <see href="https://harmony.pardeike.net/articles/patching-prefix.html">Harmony prefix methods</see> for more information.
    /// </remarks>
    MethodInfo Prefix { get; }

    /// <summary>
    /// The method that is executed after the <see cref="Original">Original</see> method.
    /// </summary>
    /// <remarks>
    /// See <see href="https://harmony.pardeike.net/articles/patching-postfix.html">Harmony postfix methods</see> for more information.
    /// </remarks>
    MethodInfo Postfix { get; }

    /// <summary>
    /// The method that can alter IL code of the <see cref="Original">Original</see> method, but is executed at runtime.
    /// </summary>
    /// <remarks>
    /// <para>  
    /// A transpiler is executed only once before the original is run. It can therefore not have access to any runtime state.
    /// <see cref="Harmony"/> will run it once when you patch the method and again every time someone else adds a transpiler for the
    /// same methods. Transpilers are chained to produce the final output.
    /// </para>
    /// See <see href="https://harmony.pardeike.net/articles/patching-transpiler.html">Harmony transpiler methods</see> for more information.
    /// </remarks>
    MethodInfo Transpiler { get; }

    /// <summary>
    /// The method that makes <see cref="Harmony"/> wrap the <see cref="Original">Original</see> and all other patches in a try/catch block.
    /// It can receive a thrown exception and even suppress it or return a different one.
    /// </summary>
    /// <remarks>
    /// See <see href="https://harmony.pardeike.net/articles/patching-finalizer.html">Harmony finalizer methods</see> for more information.
    /// </remarks>
    MethodInfo Finalizer { get; }

    /// <summary>Wheter the <see cref="Prefix"/> should be enabled.</summary>
    /// <remarks>To prevent calling the patch function use <see cref="Remove"/>, to remove the patch.</remarks>
    bool PrefixEnabled { get; set; }

    /// <summary>Wheter the <see cref="Postfix"/> should be enabled.</summary>
    /// <remarks>To prevent calling the patch function use <see cref="Remove"/>, to remove the patch.</remarks>
    bool PostfixEnabled { get; set; }

    /// <summary>Wheter the <see cref="Transpiler"/> should be enabled.</summary>
    /// <remarks>To prevent calling the patch function use <see cref="Remove"/>, to remove the patch.</remarks>
    bool TranspilerEnabled { get; set; }

    /// <summary>Wheter the <see cref="Finalizer"/> should be enabled.</summary>
    /// <remarks>To prevent calling the patch function use <see cref="Remove"/>, to remove the patch.</remarks>
    bool FinalizerEnabled { get; set; }

    /// <summary>
    /// Register the defined patches for the <paramref name="harmony"/> instance.
    /// </summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance. It is recommended to have an instance with an unique id.</param>
    void Register(Harmony harmony);

    /// <summary>
    /// Remove the patches for the <paramref name="harmony"/> instance, that match the given <paramref name="patchType"/> type.
    /// </summary>
    /// <remarks>
    /// Fully unpatching is not supported. A method is unpatched by patching it with zero patches.
    /// </remarks>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    /// <param name="patchType">The type of patch to remove, that matches the <see cref="HarmonyPatchType"/>.</param>
    void Remove(Harmony harmony, HarmonyPatchType patchType = HarmonyPatchType.All);
  }

}
