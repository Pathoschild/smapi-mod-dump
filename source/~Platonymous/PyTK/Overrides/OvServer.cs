/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Harmony;
using PyTK.CustomElementHandler;
using StardewValley.Network;
using System.Reflection;
using System;
using StardewValley;
using StardewModdingAPI.Events;

namespace PyTK.Overrides
{
    internal class OvServer
    {
        [HarmonyPatch]
        internal class ServerFix
        {
            internal static MethodInfo TargetMethod()
            {
                return AccessTools.Method(PyUtils.getTypeSDV("Network.GameServer"), "sendServerIntroduction");
            }

            internal static void Prefix()
            {
               if (Game1.IsMasterGame)
                   SaveHandler.Replace();
            }

            internal static void Postfix()
            {
                if (Game1.IsMasterGame)
                    SaveHandler.RebuildFromActions();
            }

        }
      
        [HarmonyPatch]
        internal class ServerFix4
        {
            internal static MethodInfo TargetMethod()
            {
                return AccessTools.Method(PyUtils.getTypeSDV("Network.GameServer"), "sendAvailableFarmhands");
            }

            internal static void Prefix()
            {
                if (Game1.IsMasterGame)
                    SaveHandler.Replace();
            }

            internal static void Postfix()
            {
                if (Game1.IsMasterGame)
                    SaveHandler.RebuildFromActions();
            }
        }
    }
}
