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
using StardewValley.Tools;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;
using DeluxeJournal.Util;

namespace DeluxeJournal.Framework.Tasks
{
    internal class BlacksmithTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            private Item? _item = null;
            private int _stages = 2;

            [TaskParameter("tool")]
            public Item? Item
            {
                get
                {
                    return _item;
                }

                set
                {
                    _item = null;
                    _stages = 2;

                    if (value is Tool tool && (tool is Axe || tool is Hoe || tool is WateringCan || tool is Pickaxe))
                    {
                        int localUpgradeLevel = ToolHelper.GetLocalToolUpgradeLevel(tool.GetType());

                        if (tool.UpgradeLevel == 0)
                        {
                            if (localUpgradeLevel < Tool.iridium)
                            {
                                tool.UpgradeLevel = localUpgradeLevel + 1;
                                _item = tool;
                            }
                        }
                        else
                        {
                            _stages = Math.Max(0, tool.UpgradeLevel - localUpgradeLevel) + 1;
                            _item = tool;
                        }
                    }
                }
            }

            public override Item? SmartIconItem()
            {
                return Item;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                Item = new LocalizedObjects(translation).GetItem(task.TargetDisplayName);
            }

            public override ITask? Create(string name)
            {
                if (Item is Tool tool)
                {
                    return new BlacksmithTask(name, tool, _stages);
                }

                return null;
            }
        }

        /// <summary>Serialization constructor.</summary>
        public BlacksmithTask() : base(TaskTypes.Blacksmith)
        {
        }

        public BlacksmithTask(string name, Tool tool, int stages = 2) : base(TaskTypes.Blacksmith, name)
        {
            TargetDisplayName = tool.DisplayName;
            TargetName = tool.BaseName;
            Variant = tool.UpgradeLevel;
            MaxCount = stages;

            UpdateStage(Game1.player, Enumerable.Empty<Item>());
        }

        public override bool ShouldShowProgress()
        {
            return true;
        }

        public override int GetPrice()
        {
            return Count > 0 ? 0 : ToolHelper.PriceForToolUpgradeLevel(Variant);
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ModEvents.Player.InventoryChanged += OnInventoryChanged;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ModEvents.Player.InventoryChanged -= OnInventoryChanged;
        }

        private void UpdateStage(Farmer player, IEnumerable<Item> added)
        {
            Tool? upgraded = player.toolBeingUpgraded.Value;

            if (Count < MaxCount - 1 && upgraded != null && upgraded.BaseName == TargetName)
            {
                Count = MaxCount - (Variant - upgraded.UpgradeLevel) - 1;
            }
            else if (Count == MaxCount - 1)
            {
                foreach (Item item in added)
                {
                    if (item is Tool tool && tool.BaseName == TargetName)
                    {
                        Count = MaxCount;
                        MarkAsCompleted();
                        break;
                    }
                }
            }
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (CanUpdate() && e.IsLocalPlayer)
            {
                UpdateStage(e.Player, e.Added);
            }
        }
    }
}
