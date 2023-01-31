/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using MoreFertilizers.Framework;
using Netcode;

namespace MoreFertilizers.HarmonyPatches.Compat;
internal static class SFTranspiler
{
    internal static void ApplyPatches(Harmony harmony)
    {
        Type genericBuilding = AccessTools.TypeByName("SolidFoundations.Framework.Models.ContentPack.GenericBuilding")
            ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("SF Generic Building");

        harmony.Patch(
            original: genericBuilding.InstanceMethodNamed("ProcessItemConversions"),
            transpiler: new HarmonyMethod(typeof(SFTranspiler), nameof(Transpiler)));
    }

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);

            helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling Automate:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}
