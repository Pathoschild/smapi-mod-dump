/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.StardustCore.Compatibility.SpaceCore;
using Omegasis.StardustCore.Events.Preconditions;
using Omegasis.StardustCore.Events.Preconditions.NPCSpecific;
using Omegasis.StardustCore.Events.Preconditions.PlayerSpecific;
using Omegasis.StardustCore.Events.Preconditions.TimeSpecific;
using StardewValley;

namespace Omegasis.StardustCore.Events
{
    public class EventManager
    {
        /// <summary>
        /// The list of events that this event manager is holding.
        /// </summary>
        public Dictionary<string, EventHelper> events;

        /// <summary>
        /// Event logic that occurs when the specialized command appears.
        /// </summary>
        public Dictionary<string, Action<EventManager,string>> customEventLogic;


        public Dictionary<string, ConcurrentEventInformation> concurrentEventActions;

        public Dictionary<Farmer, HashSet<EventHelper>> seenEvents;

        public Dictionary<string, Func<string[], EventPrecondition>> eventPreconditionParsingMethods;

        public EventManager()
        {
            this.events = new Dictionary<string, EventHelper>();
            this.customEventLogic = new Dictionary<string, Action<EventManager,string>>();
            this.concurrentEventActions = new Dictionary<string, ConcurrentEventInformation>();
            this.seenEvents = new Dictionary<Farmer, HashSet<EventHelper>>();
            this.eventPreconditionParsingMethods = new Dictionary<string, Func<string[], EventPrecondition>>();

            //ToDO Add a way to register event precondition parsing methods and return the preconditions back to the passed in event helpers.

            this.customEventLogic.Add("Omegasis.EventFramework.AddObjectToPlayersInventory", ExtraEventActions.addObjectToPlayerInventory);
            this.customEventLogic.Add("Omegasis.EventFramework.AddInJunimoActor", ExtraEventActions.AddInJumimoActorForEvent);
            this.customEventLogic.Add("Omegasis.EventFramework.FlipJunimoActor", ExtraEventActions.FlipJunimoActor);
            this.customEventLogic.Add("Omegasis.EventFramework.SetUpAdvanceJunimoMovement", ExtraEventActions.SetUpAdvanceJunimoMovement);
            this.customEventLogic.Add("Omegasis.EventFramework.FinishAdvanceJunimoMovement", ExtraEventActions.FinishAdvanceJunimoMovement);
            this.customEventLogic.Add("Omegasis.EventFramework.AddInJunimoAdvanceMove", ExtraEventActions.AddInJunimoAdvanceMove);
            this.customEventLogic.Add("Omegasis.EventFramework.RemoveJunimoAdvanceMove", ExtraEventActions.RemoveAdvanceJunimoMovement);

            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.AddObjectToPlayersInventory", ExtraEventActions.addObjectToPlayerInventory);
            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.AddInJunimoActor", ExtraEventActions.AddInJumimoActorForEvent);
            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.FlipJunimoActor", ExtraEventActions.FlipJunimoActor);
            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.SetUpAdvanceJunimoMovement", ExtraEventActions.SetUpAdvanceJunimoMovement);
            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.FinishAdvanceJunimoMovement", ExtraEventActions.FinishAdvanceJunimoMovement);
            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.AddInJunimoAdvanceMove", ExtraEventActions.AddInJunimoAdvanceMove);
            SpaceCoreAPIUtil.RegisterCustomEventCommand("Omegasis.EventFramework.RemoveJunimoAdvanceMove", ExtraEventActions.RemoveAdvanceJunimoMovement);

            this.eventPreconditionParsingMethods.Add(DatingNPCEventPrecondition.EventPreconditionId, PreconditionParsingMethods.ParseDatingNpcEventPrecondition);
            this.eventPreconditionParsingMethods.Add(CanReadJunimoEventPrecondition.EventPreconditionId, PreconditionParsingMethods.ParseCanReadJunimoEventPrecondition);
            this.eventPreconditionParsingMethods.Add(CommunityCenterCompletedEventPreconditon.EventPreconditionId, PreconditionParsingMethods.ParseCommunityCenterCompletedPrecondition);
            this.eventPreconditionParsingMethods.Add(IsJojaMemberEventPrecondition.EventPreconditionId, PreconditionParsingMethods.ParseIsJojaMemeberPrecondition);
            this.eventPreconditionParsingMethods.Add(DayOfWeekPrecondition.EventPreconditionId, PreconditionParsingMethods.ParseDayOfWeekPrecondition);
            this.eventPreconditionParsingMethods.Add(TimeOfDayPrecondition.EventPreconditionId, PreconditionParsingMethods.ParseTimeOfDayPrecondition);
            this.eventPreconditionParsingMethods.Add(GameLocationPrecondition.EventPreconditionId, PreconditionParsingMethods.ParseGameLocationPrecondition);

        }

        /// <summary>
        /// Adds an event to the dictionary of events.
        /// </summary>
        /// <param name="Event"></param>
        public void addEvent(EventHelper Event)
        {
            this.events.Add(Event.eventStringId, Event);
        }

        /// <summary>
        /// Adds in custom code that happens when the game's current event sees the given command name.
        /// </summary>
        /// <param name="CommandName"></param>
        /// <param name="Function"></param>
        public void addCustomEventLogic(string CommandName,Action<EventManager,string> Function)
        {
            this.customEventLogic.Add(CommandName, Function);
        }

        public void addConcurrentEvent(ConcurrentEventInformation EventInfo)
        {
            this.concurrentEventActions.Add(EventInfo.id, EventInfo);
        }

        /// <summary>
        /// Hooked into the game's update tick.
        /// </summary>
        public virtual void update()
        {
            if (Game1.eventUp == false)
            {
                if (this.concurrentEventActions.Count > 0)
                {
                    this.concurrentEventActions.Clear();
                }
                return;
            }
            string commandName = this.getGameCurrentEventCommandStringName();
            //HappyBirthday.ModMonitor.Log("Current event command name is: " + commandName, StardewModdingAPI.LogLevel.Info);

            List<string> _eventGC = new List<string>();
            foreach (KeyValuePair<string,ConcurrentEventInformation> eventInfo in this.concurrentEventActions)
            {
                if (eventInfo.Value.finished)
                {
                    _eventGC.Add(eventInfo.Key);
                }
                eventInfo.Value.invokeIfNotFinished();
            }
            foreach(string garbage in _eventGC)
            {
                this.concurrentEventActions.Remove(garbage);
            }

            if (string.IsNullOrEmpty(commandName)) return;
            if (this.customEventLogic.ContainsKey(commandName)){
                this.customEventLogic[commandName].Invoke(this,this.getGameCurrentEventCommandString());
            }
        }

        public virtual void finishConcurrentEvent(string Key)
        {
            if (this.concurrentEventActions.ContainsKey(Key))
            {
                this.concurrentEventActions[Key].finish();
            }
        }

        /// <summary>
        /// Gets the event from this list of events.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public EventHelper getEvent(string Name)
        {
            if (this.events.ContainsKey(Name))
            {
                return this.events[Name];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Starts the event with the given id if possible.
        /// </summary>
        /// <param name="EventName"></param>
        public virtual void startEventAtLocationIfPossible(string EventName)
        {
            if (this.events.ContainsKey(EventName))
            {
                if (this.seenEvents.ContainsKey(Game1.player)){
                    if (this.seenEvents[Game1.player].Contains(this.events[EventName]))
                    {
                        return;
                    }
                }
                if (Game1.eventUp == true)
                {
                    if (this.events[EventName].getEventID() == Game1.CurrentEvent.id)
                    {
                        this.concurrentEventActions.Clear(); //Clean all old parallel actions before starting a new event.
                        bool started2=this.events[EventName].startEventAtLocationifPossible();
                        if (started2)
                        {
                            if (this.seenEvents.ContainsKey(Game1.player))
                            {
                                this.seenEvents[Game1.player].Add(this.events[EventName]);
                            }
                            else
                            {
                                this.seenEvents.Add(Game1.player, new HashSet<EventHelper>() { this.events[EventName] });
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                this.concurrentEventActions.Clear(); //Clean all old parallel actions before starting a new event.
                bool started=this.events[EventName].startEventAtLocationifPossible();
                if (started)
                {
                    if(this.seenEvents.ContainsKey(Game1.player)){
                        this.seenEvents[Game1.player].Add(this.events[EventName]);
                    }
                    else
                    {
                        this.seenEvents.Add(Game1.player,new HashSet<EventHelper>() { this.events[EventName] });
                    }
                }
            }
        }

        public virtual void startEventsAtLocationIfPossible()
        {
            foreach(string eventKey in this.events.Keys)
            {
                this.startEventAtLocationIfPossible(eventKey);
            }
        }

        /// <summary>
        /// Clears the event from the farmer.
        /// </summary>
        /// <param name="EventName"></param>
        public void clearEventFromFarmer(string EventName)
        {

            this.events.TryGetValue(EventName, out EventHelper e);
            if (e == null) return;
            if (this.seenEvents.ContainsKey(Game1.player))
            {
                this.seenEvents[Game1.player].Remove(e);
            }
            //Game1.player.eventsSeen.Remove(e.getEventID());
        }

        /// <summary>
        /// Gets the current command and all of it's data.
        /// </summary>
        /// <returns></returns>
        public virtual string getGameCurrentEventCommandString()
        {
            if (Game1.CurrentEvent != null)
            {
                return Game1.CurrentEvent.currentCommandString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the name of the action for the current command in the string of event commands.
        /// </summary>
        /// <returns></returns>
        public virtual string getGameCurrentEventCommandStringName()
        {
            if (Game1.CurrentEvent != null)
            {
                return Game1.CurrentEvent.currentCommandStringName();
            }
            else
            {
                return "";
            }
        }
    }
}
