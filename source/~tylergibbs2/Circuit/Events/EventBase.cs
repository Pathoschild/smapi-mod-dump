/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Netcode;
using StardewValley;

namespace Circuit
{
    public abstract class EventBase : INetObject<NetFields>
    {
        public EventType EventType => eventType.Value;

        public NetFields NetFields { get; } = new NetFields();

        public virtual int Duration { get; } = 600;

        public virtual int SecondsRemaining { get; set; }

        public virtual bool ContinueUntilSleep { get; set; } = false;

        private readonly NetEnum<EventType> eventType = new();

        public EventBase(EventType eventType)
        {
            InitNetFields();

            SecondsRemaining = Duration;
            this.eventType.Value = eventType;
        }

        protected virtual void InitNetFields()
        {
            NetFields.AddFields(eventType);
        }

        public abstract string GetDisplayName();

        public abstract string GetChatWarningMessage();

        public abstract string GetChatStartMessage();

        public abstract string GetDescription();

        public virtual void StartEvent() { }

        public virtual void EndEvent() { }

        public virtual void OnDayStarted() { }

        public virtual void OnPlayerWarped(GameLocation oldLocation, GameLocation newLocation) { }

        public virtual void OnObjectObtained(SObject obj) { }
    }
}
