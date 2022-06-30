/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using ContentPatcher.Framework.Conditions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Profiler.ContentPatcher
{
    internal static class Patches
    {
        public static IProfilerAPI API { get; private set; }

        public const string CONTENT_PATCHER_ID = "Pathoschild.ContentPatcher";

        public static void Initialize(IProfilerAPI api, Harmony harmony)
        {
            API = api;
            PublicPatches.AddAssemblyMap("ContentPatcher", CONTENT_PATCHER_ID);

            var screenManager = Type.GetType("ContentPatcher.Framework.ScreenManager,ContentPatcher");
            harmony.Patch(
                original: AccessTools.Method(screenManager, "UpdateContext", new[] { typeof(ContextUpdateType) }),
                prefix: new HarmonyMethod(typeof(Patches), nameof(ScreenManager_UpdateContext__Prefix)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(GenericDurationPostfix))
            );
            API.AddGenericDurationPatch("ContentPatcher.Framework.TokenManager,ContentPatcher", "UpdateContext");
            API.AddGenericDurationPatch("ContentPatcher.Framework.PatchManager,ContentPatcher", "UpdateContext");
        }


        public static void ScreenManager_UpdateContext__Prefix(out Stopwatch __state, ContextUpdateType updateType)
        {
            __state = Stopwatch.StartNew();
            API.Push(new EventDurationMetadata(CONTENT_PATCHER_ID, String.Join('/', CONTENT_PATCHER_ID, "ScreenManager.UpdateContext"), updateType.ToString(), -1, new()));
        }

        // __state only works when its intra class, so we need to effectively proxy the generic one
        public static void GenericDurationPostfix(Stopwatch __state)
        {
            PublicPatches.GenericDurationPostfix(__state);
        }
    }
}
