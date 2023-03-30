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
using System.Collections.Generic;
using System.Linq;
using Circuit.Events;
using Circuit.UI;
using Circuit.VirtualProperties;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Circuit
{
    public enum EventType
    {
        BountifulHarvest,
        Overslept,
        WaterShortage,
        ResourceRush,
        Nauseous,
        MoodSwings,
        NaturesWrath,
        PoorService,
        StaminaDrain,
        //RepairedServices,
        ChaoticScheduling
    }

    public class EventScheduleItem
    {
        public EventScheduleItem(EventBase evt, int startingSeconds)
        {
            Event = evt;
            StartingSeconds = startingSeconds;
        }

        public EventBase Event { get; }

        public int StartingSeconds { get; }

        public bool HasWarned { get; set; } = false;
    }

    public class EventManager
    {
        internal readonly int EventWarningSeconds = 300;

        internal readonly int EventCooldownSeconds = 600;

        internal readonly int EventStartDelaySeconds = 1500;

        internal Queue<EventScheduleItem> EventSchedule { get; } = new();

        private Random Random { get; }

        public RunTimerMenu RunTimer { get; }

        public EventManager(int runDurationSeconds, Random? random = null)
        {
            random ??= Game1.random;
            Random = random;

            RunTimer = new(runDurationSeconds);

            BuildSchedule(runDurationSeconds);
        }

        public void BindEvents(IModEvents events)
        {
            events.GameLoop.DayStarted += OnDayStarted;
            events.GameLoop.OneSecondUpdateTicked += OnOneSecondUpdateTicked;

            events.Player.Warped += OnPlayerWarped;

            Game1.onScreenMenus.Add(RunTimer);

            Logger.Log($"EM: SMAPI events bound", LogLevel.Debug);
        }

        public void UnbindEvents(IModEvents events)
        {
            events.GameLoop.DayStarted -= OnDayStarted;
            events.GameLoop.OneSecondUpdateTicked -= OnOneSecondUpdateTicked;

            events.Player.Warped -= OnPlayerWarped;

            Game1.onScreenMenus.Remove(RunTimer);

            Logger.Log($"EM: SMAPI events unbound", LogLevel.Debug);
        }

        public void BuildSchedule(int runDurationSeconds)
        {
            runDurationSeconds -= EventStartDelaySeconds;

            while (runDurationSeconds > 0)
            {
                EventBase evt = GetEventInstanceFromType(GetRandomUniqueEventType(EventSchedule));
                EventSchedule.Enqueue(new(evt, runDurationSeconds));

                runDurationSeconds -= EventCooldownSeconds + evt.Duration;
            }
        }

        public static EventBase? GetCurrentEvent()
        {
            var currentEvent = Game1.player.team.get_FarmerTeamCurrentEvent();
            if (currentEvent is null || currentEvent.Value is null)
                return null;

            return currentEvent.Value;
        }

        public static bool AnyEventIsActive()
        {
            var currentEvent = Game1.player.team.get_FarmerTeamCurrentEvent();
            return currentEvent is not null && currentEvent.Value is not null;
        }

        public static bool EventIsActive(EventType eventType)
        {
            var currentEvent = Game1.player.team.get_FarmerTeamCurrentEvent();
            if (currentEvent is null || currentEvent.Value is null)
                return false;

            return currentEvent.Value.EventType == eventType;
        }

        public static EventBase GetEventInstanceFromType(EventType eventType)
        {
            return eventType switch
            {
                EventType.BountifulHarvest => new BountifulHarvest(eventType),
                EventType.Overslept => new Overslept(eventType),
                EventType.WaterShortage => new WaterShortage(eventType),
                EventType.ResourceRush => new ResourceRush(eventType),
                EventType.Nauseous => new Nauseous(eventType),
                EventType.MoodSwings => new MoodSwings(eventType),
                EventType.NaturesWrath => new NaturesWrath(eventType),
                EventType.ChaoticScheduling => new ChaoticScheduling(eventType),
                EventType.PoorService => new PoorService(eventType),
                EventType.StaminaDrain => new StaminaDrain(eventType),
                //EventType.RepairedServices => new RepairedServices(eventType),
                _ => throw new NotImplementedException("invalid event type")
            };
        }

        private EventType GetRandomUniqueEventType(IEnumerable<EventScheduleItem> existingSchedule)
        {
            var allEvents = Enum.GetValues<EventType>().ToList();
            allEvents.RemoveAll(e => existingSchedule.Any(s => s.Event.EventType == e));
            if (allEvents.Count == 0)
                throw new Exception("no more events");

            return allEvents[Random.Next(allEvents.Count)];
        }

        public void StartEvent(EventBase evt)
        {
            Game1.player.team.get_FarmerTeamCurrentEvent().Value = evt;

            Game1.chatBox.addInfoMessage($"[{evt.GetDisplayName()}] {evt.GetChatStartMessage()}");

            evt.StartEvent();
        }

        public void EndEvent(EventBase evt)
        {
            evt.EndEvent();

            Game1.chatBox.addMessage($"{evt.GetDisplayName()} has ended.", Color.Red);

            Game1.player.team.get_FarmerTeamCurrentEvent().Value = null!;
        }

        public void OnOneSecondUpdateTicked(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!RunTimer.IsStarted || (RunTimer.SecondsRemaining <= 0 && !AnyEventIsActive()))
                return;

            if (RunTimer.SecondsRemaining <= 0 && AnyEventIsActive())
            {
                EndEvent(GetCurrentEvent()!);
                return;
            }

            RunTimer.Tick();

            if (AnyEventIsActive())
            {
                EventBase currentEvent = GetCurrentEvent()!;
                currentEvent.SecondsRemaining--;

                if (currentEvent.SecondsRemaining <= 0 && !currentEvent.ContinueUntilSleep)
                    EndEvent(currentEvent);
            }
            else
            {
                if (EventSchedule.Count == 0)
                    return;

                EventScheduleItem NextEvent = EventSchedule.Peek();

                if (!NextEvent.HasWarned)
                {
                    int warningSeconds = NextEvent.StartingSeconds + EventWarningSeconds;
                    if (RunTimer.SecondsRemaining <= warningSeconds)
                    {
                        Game1.chatBox.addInfoMessage(NextEvent.Event.GetChatWarningMessage());
                        NextEvent.HasWarned = true;
                    }
                }
                else if (NextEvent.HasWarned && RunTimer.SecondsRemaining <= NextEvent.StartingSeconds)
                {
                    NextEvent = EventSchedule.Dequeue();
                    StartEvent(NextEvent.Event);
                }
            }
        }

        public void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent is null)
                return;

            if (currentEvent.SecondsRemaining <= 0 && currentEvent.ContinueUntilSleep)
                EndEvent(currentEvent);

            currentEvent.OnDayStarted();
        }

        public void OnPlayerWarped(object? sender, WarpedEventArgs e)
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent is null)
                return;

            currentEvent.OnPlayerWarped(e.OldLocation, e.NewLocation);
        }

        public void OnItemObtained(Item item)
        {
            if (item is SObject obj && !obj.bigCraftable.Value)
            {
                var currentEvent = GetCurrentEvent();
                if (currentEvent is null)
                    return;

                currentEvent.OnObjectObtained(obj);
            }
        }
    }
}
