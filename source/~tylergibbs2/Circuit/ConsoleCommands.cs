/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using System;
using System.Linq;
using Circuit.UI;
using StardewModdingAPI;
using StardewValley;

namespace Circuit
{
    internal static class ConsoleCommands
    {
        public static void HandleCommand(string command, string[] args)
        {
            if (command != "circuit")
                return;

            if (args.Length == 0)
            {
                Help();
                return;
            }

            string[] subcommandArgs = args.Skip(1).ToArray();
            switch (args[0])
            {
                case "start":
                    Start();
                    break;
                case "ff":
                case "fastforward":
                    FastForward(subcommandArgs);
                    break;
                case "help":
                    Help();
                    break;
                case "event":
                    EventGroup(subcommandArgs);
                    break;
                default:
                    Logger.Log("Unknown console command. Use circuit help to view all commands.", LogLevel.Error);
                    break;
            }
        }

        private static void Help()
        {
            Logger.Log("circuit start - Starts the run", LogLevel.Info);
            Logger.Log("circuit ff - Fast forwards the run timer", LogLevel.Info);
            Logger.Log("circuit event - Commands for managing the run's events", LogLevel.Info);
            Logger.Log("circuit help - Shows this message", LogLevel.Info);
        }

        private static void Start()
        {
            bool started = ModEntry.Instance.TryStart(out string? error);
            if (!started)
            {
                Logger.Log(error!, LogLevel.Error);
                return;
            }

            Logger.Log("Run started.", LogLevel.Info);
        }

        private static void FastForward(string[] args)
        {
            if (ModEntry.Instance.EventManager is null)
            {
                Logger.Log("Cannot fast forward, there is no active Event Manager.", LogLevel.Error);
                return;
            }
            else if (!ModEntry.Instance.EventManager.RunTimer.IsStarted)
            {
                Logger.Log("Cannot fast forward, the Event Manager is not started.", LogLevel.Error);
                return;
            }
            else if (args.Length == 0)
            {
                Logger.Log("Usage: circuit ff <amount> [seconds/minutes/hours]", LogLevel.Error);
                return;
            }

            bool parsed = int.TryParse(args[0], out int amount);
            if (!parsed)
            {
                Logger.Log("Cannot fast forward, the time specified is not a valid integer.", LogLevel.Error);
                return;
            }

            string unit = "seconds";
            if (args.Length > 1)
            {
                unit = args[1];
                if (unit.StartsWith("minute"))
                    amount *= 60;
                else if (unit.StartsWith("hour"))
                    amount *= 60 * 60;
            }

            ModEntry.Instance.EventManager.RunTimer.Tick(amount);

            if (EventManager.AnyEventIsActive())
            {
                EventBase currentEvent = EventManager.GetCurrentEvent()!;
                currentEvent.SecondsRemaining -= amount;
            }

            Logger.Log($"Fast forwarded {args[0]:n0} {unit}.", LogLevel.Info);
        }

        private static void EventGroup(string[] args)
        {
            if (args.Length == 0)
            {
                Logger.Log("Usage: circuit event <start/end/list/schedule>", LogLevel.Error);
                return;
            }

            string[] subcommandArgs = args.Skip(1).ToArray();
            switch (args[0])
            {
                case "start":
                    EventStart(subcommandArgs);
                    break;
                case "end":
                    EventEnd();
                    break;
                case "list":
                    EventList();
                    break;
                case "schedule":
                    EventSchedule();
                    break;
                default:
                    Logger.Log("Usage: circuit event <start/end/list>", LogLevel.Error);
                    break;
            }
        }

        private static void EventStart(string[] args)
        {
            if (ModEntry.Instance.EventManager is null)
            {
                Logger.Log("There is no active Event Manager.", LogLevel.Error);
                return;
            }
            else if (args.Length == 0)
            {
                Logger.Log("Usage: circuit event start <event name>", LogLevel.Error);
                return;
            }

            bool found = Enum.TryParse(args[0], out EventType eventType);
            if (!found)
            {
                Logger.Log($"Invalid event type '{args[0]}'", LogLevel.Error);
                return;
            }

            if (EventManager.AnyEventIsActive())
                ModEntry.Instance.EventManager.EndEvent(EventManager.GetCurrentEvent()!);

            ModEntry.Instance.EventManager.StartEvent(EventManager.GetEventInstanceFromType(eventType));
        }

        private static void EventEnd()
        {
            if (EventManager.AnyEventIsActive() && ModEntry.Instance.EventManager is not null)
            {
                EventBase currentEvent = EventManager.GetCurrentEvent()!;
                currentEvent.SecondsRemaining = 0;

                Logger.Log($"Ended event '{currentEvent.EventType}'.", LogLevel.Info);
            }
            else
                Logger.Log("Cannot end event, no event is active.", LogLevel.Error);
        }

        private static void EventList()
        {
            var values = Enum.GetValues<EventType>();

            Logger.Log("Available event types:", LogLevel.Info);
            foreach (EventType value in values)
                Logger.Log($"   - {value}", LogLevel.Info);
        }

        private static void EventSchedule()
        {
            if (ModEntry.Instance.EventManager is null)
            {
                Logger.Log("There is no active Event Manager.", LogLevel.Error);
                return;
            }

            var schedule = ModEntry.Instance.EventManager.EventSchedule;

            Logger.Log("Event schedule:", LogLevel.Info);
            foreach (string line in schedule.Select(e => $"{e.Event.GetDisplayName()}, {RunTimerMenu.GetHHMMSS(e.StartingSeconds)} (warning at {RunTimerMenu.GetHHMMSS(e.StartingSeconds + ModEntry.Instance.EventManager.EventWarningSeconds)})"))
                Logger.Log($"   {line}", LogLevel.Info);
        }
    }
}
