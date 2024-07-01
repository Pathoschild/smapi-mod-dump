/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewValley;
using StardewValley.GameData.Tools;
using DeluxeJournal.Events;
using DeluxeJournal.Framework;
using DeluxeJournal.Framework.Events;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.ITask;

namespace DeluxeJournal.Task
{
    /// <summary>Task base class. All tasks should derive from this.</summary>
    public abstract class TaskBase : ITask
    {
        private readonly string _id;

        private bool _active;
        private bool _complete;
        private bool _viewed;
        private int _count;
        private int _index;

        public string ID => _id;

        public string Name { get; set; }

        public long OwnerUniqueMultiplayerID { get; set; }

        public Period RenewPeriod { get; set; }

        public WorldDate RenewDate { get; set; }

        public int RenewCustomInterval { get; set; }

        public virtual int Count
        {
            get => _count;

            set
            {
                if (_count != value)
                {
                    int oldCount = _count;
                    _count = value;
                    NotifyStatusChanged(Active, Complete, oldCount);
                }
            }
        }

        public virtual int MaxCount { get; set; }

        public virtual int BasePrice { get; set; }

        public virtual int ColorIndex { get; set; }

        public virtual int Group { get; set; }

        public virtual int GroupColorIndex { get; set; }

        public virtual bool Active
        {
            get => _active;

            set
            {
                if (_active != value)
                {
                    bool oldActive = _active;
                    _active = value;

                    if (!value && RenewPeriod == Period.Custom)
                    {
                        WorldDate renewDate = WorldDate.Now();
                        renewDate.TotalDays += RenewCustomInterval;
                        RenewDate = renewDate;
                    }

                    NotifyStatusChanged(oldActive, Complete, Count);
                }
            }
        }

        public virtual bool Complete
        {
            get => _complete;

            set
            {
                if (_complete != value)
                {
                    bool oldComplete = _complete;
                    int count = Count;
                    _complete = value;
                    _viewed = !value;

                    if (!value && count >= MaxCount)
                    {
                        Count = 0;
                    }

                    NotifyStatusChanged(Active, oldComplete, count);
                }
            }
        }

        public virtual bool IsHeader => false;

        protected TaskBase(string id) : this(id, string.Empty)
        {
        }

        protected TaskBase(string id, string name)
        {
            _id = id;
            _active = true;
            _complete = false;
            _viewed = true;
            _index = 0;

            Name = name;
            OwnerUniqueMultiplayerID = Game1.player?.UniqueMultiplayerID ?? 0;
            RenewPeriod = Period.Never;
            RenewDate = new WorldDate(1, Season.Spring, 1);
            RenewCustomInterval = 1;
            MaxCount = 1;
            GroupColorIndex = -1;
        }

        /// <summary>Helper method to get the buy/sale price of an item.</summary>
        /// <param name="item">Item to be bought.</param>
        /// <returns>The buy/sale price or 0 if it could not be resolved.</returns>
        protected static int BuyPrice(Item item)
        {
            if (item is Tool tool && tool.UpgradeLevel > 0 && tool.GetToolData() is ToolData toolData)
            {
                if (ToolHelper.IsToolBaseUpgradeLevel(toolData) && ToolHelper.GetToolUpgradeForPlayer(toolData, Game1.player) is Tool upgradeTool)
                {
                    tool = upgradeTool;
                }

                return ToolHelper.PriceForToolUpgradeLevel(tool.UpgradeLevel);
            }

            int price = ShopHelper.GetItemSalePrice(item);
            return price < 0 ? Math.Max(item.salePrice(), 0) : price;
        }

        /// <summary>Helper method to get the sell price of an item.</summary>
        /// <param name="item">Item to be sold.</param>
        /// <returns>The sell price or 0 if one could not be resolved.</returns>
        protected static int SellPrice(Item item)
        {
            return Math.Max(item.sellToStorePrice(), 0);
        }

        private static int TotalDaysInYear(WorldDate date)
        {
            return date.SeasonIndex * 28 + date.DayOfMonth;
        }

        public virtual ITask Copy()
        {
            ITask copy = (ITask)MemberwiseClone();
            copy.RenewDate = new WorldDate(copy.RenewDate);
            return copy;
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

                if (DeluxeJournalMod.Config is Config config && config.EnableVisualTaskCompleteIndicator)
                {
                    Game1.dayTimeMoneyBox.PingQuestLog();
                }
            }
        }

        /// <summary>Increment progress count and optionally mark as completed if <c>MaxCount</c> is met.</summary>
        /// <param name="amount">Amount to increment <c>Count</c> by.</param>
        /// <param name="markAsCompleted">Mark as completed if <c>Count >= MaxCount</c>.</param>
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

        /// <summary>Notify event subscribers that the status of this task has changed.</summary>
        protected void NotifyStatusChanged(bool oldActive, bool oldComplete, int oldCount)
        {
            if (DeluxeJournalMod.EventManager is EventManager events)
            {
                events.TaskStatusChanged.Raise(this, new(this, oldActive, oldComplete, oldCount, Active, Complete, Count));
            }
        }

        public virtual int DaysRemaining()
        {
            return RenewPeriod switch
            {
                Period.Weekly => (((RenewDate.DayOfMonth - Game1.dayOfMonth) % 7) + 7) % 7,
                Period.Monthly => (((RenewDate.DayOfMonth - Game1.dayOfMonth) % 28) + 28) % 28,
                Period.Annually => (((TotalDaysInYear(RenewDate) - TotalDaysInYear(Game1.Date)) % 112) + 112) % 112,
                Period.Custom => RenewDate.TotalDays - Game1.Date.TotalDays,
                _ => 0,
            };
        }

        public virtual int GetPrice()
        {
            return BasePrice * (MaxCount - Count);
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

        public virtual bool ShouldShowCustomStatus()
        {
            return false;
        }

        public virtual string GetCustomStatusKey()
        {
            return string.Empty;
        }

        public bool IsTaskOwner(Farmer farmer)
        {
            return OwnerUniqueMultiplayerID == farmer.UniqueMultiplayerID;
        }

        public bool IsTaskOwner(long umid)
        {
            return OwnerUniqueMultiplayerID == umid;
        }

        public virtual void EventSubscribe(ITaskEvents events)
        {
        }

        public virtual void EventUnsubscribe(ITaskEvents events)
        {
        }

        public virtual void Validate()
        {
        }

        public int CompareTo(ITask? other)
        {
            if (other == null)
            {
                return 1;
            }
            else if (Group != other.Group)
            {
                return Group - other.Group;
            }
            else if (Active && other.Active)
            {
                return (Complete == other.Complete) ? GetSortingIndex() - other.GetSortingIndex() : Complete.CompareTo(other.Complete);
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
