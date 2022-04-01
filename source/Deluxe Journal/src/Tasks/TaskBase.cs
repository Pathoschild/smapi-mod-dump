/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Newtonsoft.Json;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework;

using static DeluxeJournal.Tasks.ITask;

namespace DeluxeJournal.Tasks
{
    /// <summary>Task base class. All tasks should derive from this.</summary>
    public abstract class TaskBase : ITask
    {
        private readonly string _id;

        protected bool _complete;
        protected bool _viewed;
        private int _index;

        public string ID => _id;

        public string Name { get; set; }

        public Period RenewPeriod { get; set; }

        public WorldDate RenewDate { get; set; }

        public virtual string TargetDisplayName { get; set; }

        public virtual string TargetName { get; set; }

        public virtual int TargetIndex { get; set; }

        public virtual int Variant { get; set; }

        public virtual int Count { get; set; }

        public virtual int MaxCount { get; set; }

        public virtual int BasePrice { get; set; }

        [JsonProperty(Order = -3)]
        public virtual bool Active { get; set; }

        [JsonProperty(Order = -2)]
        public virtual bool Complete
        {
            get
            {
                return _complete;
            }

            set
            {
                _complete = value;
                _viewed = !value;

                if (!value && Count >= MaxCount)
                {
                    Count = 0;
                }
            }
        }

        protected TaskBase(string id) : this(id, string.Empty)
        {
        }

        protected TaskBase(string id, string name)
        {
            _id = id;
            _complete = false;
            _viewed = true;
            _index = 0;

            Name = name;
            Active = true;
            RenewPeriod = Period.Never;
            RenewDate = new WorldDate(1, "spring", 1);
            TargetDisplayName = string.Empty;
            TargetName = string.Empty;
            TargetIndex = -1;
            Variant = 0;
            Count = 0;
            MaxCount = 1;
            BasePrice = 0;
        }

        private static int TotalDaysInYear(WorldDate date)
        {
            return date.SeasonIndex * 28 + date.DayOfMonth;
        }

        /// <summary>Is this task ready to receive update events?</summary>
        public bool CanUpdate()
        {
            return Active && !Complete;
        }

        /// <summary>Mark as completed and trigger audio/visual cues.</summary>
        public virtual void MarkAsCompleted()
        {
            if (!Complete)
            {
                Complete = true;
                Game1.playSound("jingle1");

                if (DeluxeJournalMod.Instance?.Config is Config config && config.EnableVisualTaskCompleteIndicator)
                {
                    Game1.dayTimeMoneyBox.PingQuestLog();
                }
            }
        }

        /// <summary>Increment progress count.</summary>
        /// <param name="amount">Amount to increment by. Effectively equivalent to: Count = Math.Min(MaxCount, Count + amount).</param>
        /// <param name="markAsCompleted">Mark as completed if Count >= MaxCount.</param>
        protected void IncrementCount(int amount = 1, bool markAsCompleted = true)
        {
            Count += amount;

            if (Count >= MaxCount)
            {
                Count = MaxCount;

                if (markAsCompleted)
                {
                    MarkAsCompleted();
                }
            }
        }

        public virtual int DaysRemaining()
        {
            return RenewPeriod switch
            {
                Period.Weekly => (((RenewDate.DayOfMonth - Game1.dayOfMonth) % 7) + 7) % 7,
                Period.Monthly => (((RenewDate.DayOfMonth - Game1.dayOfMonth) % 28) + 28) % 28,
                Period.Annually => (((TotalDaysInYear(RenewDate) - TotalDaysInYear(Game1.Date)) % 112) + 112) % 112,
                _ => 0,
            };
        }

        public virtual int GetPrice()
        {
            return BasePrice;
        }

        public int GetSortingIndex()
        {
            return _index;
        }

        public void SetSortingIndex(int index)
        {
            _index = index;
        }

        public virtual void MarkAsViewed()
        {
            _viewed = true;
        }

        public virtual bool HasBeenViewed()
        {
            return _viewed;
        }

        public virtual bool ShouldShowProgress()
        {
            return false;
        }

        public virtual void EventSubscribe(ITaskEvents events)
        {
        }

        public virtual void EventUnsubscribe(ITaskEvents events)
        {
        }

        public int CompareTo(ITask? other)
        {
            if (other == null)
            {
                return 1;
            }
            else if (Active && other.Active)
            {
                return (Complete == other.Complete) ? _index - other.GetSortingIndex() : Complete.CompareTo(other.Complete);
            }
            else if (!Active && !other.Active)
            {
                return DaysRemaining() - other.DaysRemaining();
            }
            else
            {
                return other.Active.CompareTo(Active);
            }
        }
    }
}
