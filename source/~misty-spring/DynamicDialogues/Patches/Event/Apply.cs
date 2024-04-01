/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace DynamicDialogues.Patches;

internal partial class EventPatches
{
    private static LogLevel Level => ModEntry.Config.Verbose ? LogLevel.Debug : LogLevel.Trace;
    private static IModHelper Helper => ModEntry.Help;
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": postfixing SDV method \"Event.receiveActionPress\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.Event), nameof(Event.receiveActionPress)),
            postfix: new HarmonyMethod(typeof(EventPatches), nameof(Post_receiveActionPress))
        );

        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": postfixing SDV method \"Event.festivalUpdate\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.festivalUpdate)),
            prefix: new HarmonyMethod(typeof(EventPatches), nameof(Post_festivalUpdate))
        );

        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": prefixing SDV method \"Event.endBehaviors\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.endBehaviors), parameters: new[] { typeof(string[]), typeof(GameLocation) }),
            prefix: new HarmonyMethod(typeof(EventPatches), nameof(Pre_endBehaviors))
        );

        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": postfixing SDV method \"Event.exitEvent\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.exitEvent)),
            postfix: new HarmonyMethod(typeof(EventPatches), nameof(Post_exitEvent))
            );

        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": prefixing SDV method \"Event.DefaultCommands.Speak\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.DefaultCommands.Speak)),
            prefix: new HarmonyMethod(typeof(EventPatches), nameof(Pre_Speak))
        );
        Log($"Applying Harmony patch \"{nameof(EventPatches)}\": prefixing SDV method \"Event.DefaultCommands.Message\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(Event), nameof(Event.DefaultCommands.Message)),
            prefix: new HarmonyMethod(typeof(EventPatches), nameof(Pre_Message))
        );
    }

    private static void Post_exitEvent()
    {
        var eventQueue = ModEntry.EventQueue;

        if (!Game1.game1.IsActive || Game1.newDay || Game1.gameMode != Game1.playingGameMode || !Context.IsPlayerFree)
        {
            //if day ended, reset every trigger that wasn't run AND requires this event
            if (eventQueue.Count <= 0) return;
            
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var e in eventQueue)
            {
                if (e.ResetIfUnseen)
                    Game1.player.triggerActionsRun.Remove(e.TriggerKey);
            }

            eventQueue?.Clear();
        }
        else if (eventQueue.Count > 0)
        {
            //setup new event, remove from list
            var e = eventQueue[0];
            eventQueue.RemoveAt(0);
            var where = Utility.fuzzyLocationSearch(e.Location);
            Game1.PlayEvent(e.Key, where, out _, e.CheckPreconditions, e.CheckSeen);
        }
        else
        {
            Log("No more events in queue.");
        }
    }
}
