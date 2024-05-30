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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Tools;
using DeluxeJournal.Events;
using DeluxeJournal.Util;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class BlacksmithTask : TaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.Tool, TaskParameterTag.ItemList, Constraints = Constraint.Upgradable | Constraint.NotEmpty)]
            public IList<string>? ItemIds { get; set; }

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Item;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is BlacksmithTask blacksmithTask)
                {
                    ItemIds = new[] { blacksmithTask.ItemId };
                }
            }

            public override ITask? Create(string name)
            {
                return ItemIds != null && ItemIds.Count > 0 ? new BlacksmithTask(name, ItemIds.First()) : null;
            }
        }

        public const int StageDeliver = 0;

        public const int StageWaiting = 1;

        public const int StageClaimed = 2;

        /// <summary>The qualified item ID of the upgraded tool (i.e. the item to be collected from the blacksmith).</summary>
        public string ItemId { get; set; }

        /// <summary>The tool type. Equivalent to the class name of the tool.</summary>
        public string ToolType { get; set; } = "GenericTool";

        /// <summary>The target upgrade level of the tool or 0 to allow all upgrades within the correct tool type.</summary>
        public int UpgradeLevel { get; set; } = 0;

        /// <summary>Whether the tool actually a trash can.</summary>
        public bool IsTrashCan { get; set; } = false;

        [JsonIgnore]
        private int TrashCanUpgradeLevel { get; set; } = 0;

        /// <summary>Serialization constructor.</summary>
        public BlacksmithTask() : base(TaskTypes.Blacksmith)
        {
            ItemId = string.Empty;
        }

        public BlacksmithTask(string name, string itemId) : base(TaskTypes.Blacksmith, name)
        {
            ItemId = itemId;
            MaxCount = StageClaimed;

            if (ItemRegistry.GetData(itemId)?.RawData is ToolData data)
            {
                ToolType = data.ClassName;
                IsTrashCan = ToolHelper.IsTrashCan(data);

                if (ToolHelper.IsToolBaseUpgradeLevel(data) && ToolHelper.GetToolUpgradeForPlayer(data, Game1.player) is Tool tool)
                {
                    UpgradeLevel = tool.UpgradeLevel;
                }
                else
                {
                    UpgradeLevel = data.UpgradeLevel;
                }
            }

            Validate();
        }

        public override void Validate()
        {
            if (CanUpdate())
            {
                Count = Game1.player.toolBeingUpgraded.Value is Tool tool && IsTargetTool(tool) ? StageWaiting : StageDeliver;
            }
        }

        public override bool ShouldShowCustomStatus()
        {
            return !Complete;
        }

        public override string GetCustomStatusKey()
        {
            if (Count == StageDeliver)
            {
                return "ui.tasks.status.deliver";
            }
            else if (Game1.player.daysLeftForToolUpgrade.Value > 0)
            {
                return "ui.tasks.status.upgrading";
            }
            else
            {
                return "ui.tasks.status.ready";
            }
        }

        public override int GetPrice()
        {
            return Count > StageDeliver || ItemId == ItemRegistry.type_tool + "Pan" ? 0 : ToolHelper.PriceForToolUpgradeLevel(UpgradeLevel);
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.SalablePurchased += OnSalablePurchased;

            if (IsTrashCan)
            {
                events.ModEvents.GameLoop.UpdateTicked += OnUpdateTicked;
            }
            else
            {
                events.ModEvents.Player.InventoryChanged += OnInventoryChanged;
            }
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.SalablePurchased -= OnSalablePurchased;
            events.ModEvents.GameLoop.UpdateTicked -= OnUpdateTicked;
            events.ModEvents.Player.InventoryChanged -= OnInventoryChanged;
        }

        private void OnSalablePurchased(object? sender, SalableEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && Count == StageDeliver)
            {
                if (e.Salable is Tool tool && IsTargetTool(tool))
                {
                    Count = StageWaiting;
                }
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (CanUpdate())
            {
                int trashCanLevel = Game1.player.trashCanLevel;

                if (TrashCanUpgradeLevel == 0)
                {
                    TrashCanUpgradeLevel = UpgradeLevel == 0 ? Math.Min(trashCanLevel + 1, Tool.iridium) : UpgradeLevel;
                }

                if (TrashCanUpgradeLevel <= trashCanLevel)
                {
                    Count = StageClaimed;
                    MarkAsCompleted();
                }
            }
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && e.Player.toolBeingUpgraded.Value == null && Count == StageWaiting)
            {
                foreach (Item item in e.Added)
                {
                    if (item is Tool tool && IsTargetTool(tool))
                    {
                        Count = StageClaimed;
                        MarkAsCompleted();
                        break;
                    }
                }
            }
        }

        private bool IsTargetTool(Tool tool)
        {
            return tool.GetToolData() is ToolData data &&
                (UpgradeLevel == 0 || UpgradeLevel == data.UpgradeLevel) &&
                ToolType == data.ClassName;
        }
    }
}
