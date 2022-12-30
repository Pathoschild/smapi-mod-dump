/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using AeroCore.Models;
using AeroCore.Utils;
using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using SObject = StardewValley.Object;

namespace AeroCore.Patches
{
    [HarmonyPatch]
    internal class UseObject
    {
        internal static event Action<IUseObjectEventArgs> OnUseObject;
        internal static IEnumerable<MethodInfo> TargetMethods()
        {
            return new[] { 
                typeof(Fence).MethodNamed(nameof(Fence.checkForAction)), 
                typeof(SObject).MethodNamed(nameof(SObject.checkForAction))
                };
        }
        internal static void Postfix(Farmer who, bool justCheckingForActivity, SObject __instance, ref bool __result)
        {
            var ev = new UseObjectEventArgs(who, justCheckingForActivity, __instance, __result);
            OnUseObject?.Invoke(ev);
            __result = ev.IsHandled;
        }
    }
}
