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
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

namespace IPv6.Patch
{
    internal partial class MyPatch
    {
        public static void LogInfo(string msg)
        {
#if DEBUG
            FileLog.Log(msg);
#endif
        }

        public static void GameLog(string msg)
        {
#if DEBUG
            FileLog.Log(msg);
#endif
            log.Info(msg);
        }

        public static void CheckSomeTypes(this CodeInstruction code)
        {
#if DEBUG
            var codestr = code.ToString();
            foreach (var t in new Type[] {
            typeof(StardewValley.Network.LidgrenClient),
            typeof(StardewValley.Network.LidgrenServer),
        })
            {
                if (codestr.Contains(t.FullName!))
                {
                    LogInfo($"! check type | {t.FullName} in {codestr}");
                }
            }
#endif
        }

        public static void PatchingLoadFunction(this CodeInstruction code, HarmonyMethod transpiler)
        {
            if (code.opcode == OpCodes.Ldftn)
            {
                var m = (MethodBase)code.operand;
                LogInfo($"! load function | {m.ReflectedType!.FullName}:{m.Name}");
                Harmony.Patch(m, transpiler: transpiler);
            }
        }


        public static void Patching(IModHelper helper, string harmonyID)
        {
            InitRefValues(helper, harmonyID);
            //Harmony.DEBUG = true;

            foreach (var m in new MethodBase[] {
           AccessTools.Method(typeof(StardewValley.Game1), "UpdateTitleScreen"),
           AccessTools.Method(typeof(StardewValley.Menus.CoopMenu), "enterIPPressed"),
           AccessTools.Method(typeof(StardewValley.Multiplayer), "LogDisconnect")})
            {
                Harmony.Patch(m, transpiler: ClientTranspiler);
            }

            Harmony.Patch(AccessTools.Constructor(typeof(StardewValley.Network.GameServer), new Type[] { typeof(bool) }), transpiler: ServerTranspiler);

            Harmony.Patch(AccessTools.Method(typeof(StardewValley.Network.GameServer), "UpdateLocalOnlyFlag"), transpiler: UpdateLocalOnlyFlagTranspiler);
        }
    }
}
