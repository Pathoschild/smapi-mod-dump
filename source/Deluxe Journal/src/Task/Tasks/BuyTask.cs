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
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class BuyTask : ItemTaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = ItemIdsConstraint)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            [TaskParameter(TaskParameterNames.Quality, TaskParameterTag.Quality, Parent = TaskParameterNames.Item, Constraints = Constraint.GE0)]
            public int Quality { get; set; } = 0;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item;

            public override bool EnableSmartIconCount => true;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is BuyTask buyTask)
                {
                    ItemIds = buyTask.ItemIds;
                    Count = buyTask.MaxCount;
                    Quality = buyTask.Quality;
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new BuyTask(name, ItemIds, Count, Quality) : null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public BuyTask() : base(TaskTypes.Buy)
        {
        }

        public BuyTask(string name, IList<string> itemIds, int count, int quality)
            : base(TaskTypes.Buy, name, itemIds, count, quality)
        {
            BasePrice = BuyPrice(ItemRegistry.Create(itemIds.First()));
            Validate();
        }

        public override void Validate()
        {
            base.Validate();

            if (BaseItemIds.Count > 0 && IngredientItemId != null
                && FlavoredItemHelper.CreateFlavoredItem(BaseItemIds.First(), IngredientItemId) is Item flavoredItem)
            {
                BasePrice = flavoredItem.salePrice();
            }
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.SalablePurchased += OnSalablePurchased;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.SalablePurchased -= OnSalablePurchased;
        }

        private void OnSalablePurchased(object? sender, SalableEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Salable is Item item && CheckItemMatch(item))
            {
                IncrementCount(e.Amount);
            }
        }
    }
}
