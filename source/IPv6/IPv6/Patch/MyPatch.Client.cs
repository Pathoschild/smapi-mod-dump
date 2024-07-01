/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sunsst/Stardew-Valley-IPv6
**
*************************************************/

using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;

namespace IPv6.Patch;

internal static partial class MyPatch
{
    public static HarmonyMethod ClientTranspiler = new HarmonyMethod(typeof(MyPatch), "ClientTranspilerMethod");

    public static IEnumerable<CodeInstruction> ClientTranspilerMethod(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
    {
        LogInfo($"\n========== client | {__originalMethod.DeclaringType}:{__originalMethod.Name}() ==========");

        var t = typeof(StardewValley.Network.LidgrenClient);
        var c = AccessTools.Constructor(t, new Type[] { typeof(string) });
        foreach (CodeInstruction code in instructions)
        {
            if (code.Is(OpCodes.Isinst, t))
            {
                LogInfo($"< client | {code}");
                code.operand = typeof(Classes.LidgrenClient);
                LogInfo($"> client | {code}");
            }
            else if (code.Is(OpCodes.Newobj, c))
            {
                LogInfo($"< client | {code}");
                code.operand = AccessTools.Constructor(typeof(Classes.LidgrenClient), new Type[] { typeof(string) });
                LogInfo($"> client | {code}");
            }

            code.CheckSomeTypes();
            code.PatchingLoadFunction(ClientTranspiler);

            yield return code;
        }
    }
}
