/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

using Constraint = DeluxeJournal.Tasks.TaskParameter.Constraint;

namespace DeluxeJournal.Framework.Tasks
{
    internal class SellTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter("item")]
            public Item? Item { get; set; }

            [TaskParameter("count", Tag = "count", Constraints = Constraint.GT0)]
            public int Count { get; set; } = 1;

            public override Item? SmartIconItem()
            {
                return Item;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                Item = new LocalizedObjects(translation).GetItem(task.TargetDisplayName);
                Count = task.MaxCount;
            }

            public override ITask? Create(string name)
            {
                return Item != null ? new SellTask(name, Item, Count) : null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public SellTask() : base(TaskTypes.Sell)
        {
        }

        public SellTask(string name, Item item, int count) : base(TaskTypes.Sell, name)
        {
            TargetDisplayName = item.DisplayName;
            TargetIndex = item.ParentSheetIndex;
            MaxCount = count;

            if (item is SObject obj)
            {
                BasePrice = obj.sellToStorePrice();
            }
            else
            {
                BasePrice = item.salePrice();
            }
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override int GetPrice()
        {
            return -base.GetPrice() * (MaxCount - Count);
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ModEvents.Display.MenuChanged += OnMenuChanged;
            events.ModEvents.GameLoop.DayEnding += OnDayEnding;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ModEvents.Display.MenuChanged -= OnMenuChanged;
            events.ModEvents.GameLoop.DayEnding -= OnDayEnding;
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            foreach (Item item in Game1.getFarm().getShippingBin(Game1.player))
            {
                OnSell(item);
            }
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (CanUpdate() && e.NewMenu is ShopMenu shop)
            {
                ShopHelper.AttachQuietSellCallback(shop, OnSell);
            }
        }

        private bool OnSell(ISalable salable)
        {
            if (salable is Item item && TargetIndex == item.ParentSheetIndex)
            {
                IncrementCount(item.Stack);
            }

            return false;
        }
    }
}
