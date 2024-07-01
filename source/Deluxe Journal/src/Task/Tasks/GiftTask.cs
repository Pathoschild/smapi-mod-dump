/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using DeluxeJournal.Events;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class GiftTask : ItemTaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Item, TaskParameterTag.ItemList, Constraints = OptionalObjectIdsConstraint)]
            public IList<string>? ItemIds { get; set; }

            [TaskParameter(TaskParameterNames.NPC, TaskParameterTag.NpcName)]
            public string? NpcName { get; set; }

            [TaskParameter(TaskParameterNames.Quality, TaskParameterTag.Quality, Parent = TaskParameterNames.Item, InputType = TaskParameterInputType.DropDown, Constraints = Constraint.GE0)]
            public int Quality { get; set; } = 0;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item | SmartIconFlags.Npc;

            protected override void InitializeInternal(ITask task)
            {
                if (task is GiftTask giftTask)
                {
                    NpcName = giftTask.NpcName;
                    ItemIds = giftTask.ItemIds;
                    Quality = giftTask.Quality;
                }
            }

            protected override ITask? CreateInternal(string name)
            {
                return NpcName != null && (ItemIds == null || ItemIds.Count > 0)
                    ? new GiftTask(name, NpcName, ItemIds ?? new List<string>(), Quality)
                    : null;
            }
        }

        /// <summary>The internal name of the target NPC.</summary>
        public string NpcName { get; set; }

        /// <summary>Serialization constructor.</summary>
        public GiftTask() : base(TaskTypes.Gift)
        {
            NpcName = string.Empty;
        }

        public GiftTask(string name, string npcName, IList<string> itemIds, int quality)
            : base(TaskTypes.Gift, name, itemIds, 1, quality)
        {
            NpcName = npcName;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ItemGifted += OnItemGifted;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ItemGifted -= OnItemGifted;
        }

        private void OnItemGifted(object? sender, GiftEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && NpcName == e.NPC.Name && (BaseItemIds.Count == 0 || CheckItemMatch(e.Item)))
            {
                MarkAsCompleted();
            }
        }
    }
}
